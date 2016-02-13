using UnityEngine;
using System.Collections;

public class Combat_Manager : MonoBehaviour
{
    Game_Manager game;
    CameraBehavior camera;

    public Combat_PixelBehavior[] shipPixels; //Store all pixelsArray as an array.
	public int asteroidCount;

	public void Init()
    {
        game = Game_Manager.instance;
        camera = game.camera;
        camera.zoom = 100f;
        camera.pan = DefaultValues.DEFAULT_SHIP_SPAWN_POSITION;
        camera.UserMovementEnabled = true;
        shipPixels = new Combat_PixelBehavior[Game_Manager.instance.shipArraySqrRootLength * Game_Manager.instance.shipArraySqrRootLength];
		loadPlayer (PlaythroughData.savedPixels);

		loadEnemies ();

		asteroidCount = DefaultValues.DEFAULT_ASTEROID_COUNT;
		loadAsteroids ();

        
	}

    public void OnUpdate()
    {

    }

    public void GoToBuilder()
    {
        game.savedShip = true;
        game.loadScene("Builder");
    }

    void loadPlayer(CompressedPixelData[] savedPixels)
    {
        Vector3 averagePosition = Vector3.zero;
        int averagePositionCount = 0;
        for (int i = 0; i < savedPixels.Length - 1; i++)
        {
            CompressedPixelData pixel = savedPixels[i];
            if (savedPixels[i] != null)
            {
                CompressedPixelData savedPixel = savedPixels[i];
                Combat_PixelBehavior buildPixel = new GameObject().AddComponent<Combat_PixelBehavior>();
                buildPixel.transform.name = "Pixel_x" + savedPixel.coordinates.x + "y" + savedPixel.coordinates.y;
                buildPixel.pixelType = savedPixel.pixelType;
                buildPixel.turretType = savedPixel.turretType;
                buildPixel.transform.position = savedPixel.coordinates + new Vector2(0.5f, 0.5f);
                buildPixel.coordinates = savedPixel.coordinates;
                buildPixel.index = i;
				buildPixel.init(savedPixel.turretType);
                //Hardpoint & turret.
                if (buildPixel.pixelType == Pixel.Type.Hardpoint)
                {
                    if (buildPixel.turretType != Turret.Type.None)
                    {
                        Combat_TurretBehavior turretBehaviour;
						turretBehaviour = new GameObject().AddComponent<Combat_TurretBehavior>();
						turretBehaviour.turretType = buildPixel.turretType;
						turretBehaviour.transform.position = buildPixel.transform.position;
						turretBehaviour.transform.parent = buildPixel.transform;
						turretBehaviour.transform.localRotation = Quaternion.Euler(0f, 0f, savedPixel.turretFacingAngle);
						turretBehaviour.turretFacingAngle = savedPixel.turretFacingAngle;
						turretBehaviour.turretMountSpan = savedPixel.turretMountRange;

						turretBehaviour.Init(buildPixel);
						buildPixel.turretBehaviour = turretBehaviour;
                    }
                }

                shipPixels[i] = buildPixel;
                averagePositionCount++;
                averagePosition += buildPixel.transform.position;
            }
        }

        averagePosition /= averagePositionCount;
        GameObject shipParent = new GameObject();
        shipParent.transform.position = averagePosition;
        shipParent.transform.name = "ShipParent";
		shipParent.tag = "Player";

        //SetParents & Get surrounding pixels
        for (int i = 0; i < shipPixels.Length - 1; i++)
        {
            Combat_PixelBehavior pixel = shipPixels[i];
            if (pixel != null)
            {
                pixel.transform.parent = shipParent.transform;
                pixel.GetSurroundingPixels(shipPixels);
            }
        }

        shipParent.transform.position = DefaultValues.DEFAULT_SHIP_SPAWN_POSITION;
		shipParent.layer = DefaultValues.DEFAULT_FRIENDLY_LAYER;

		// Adding controller to the ship
		shipParent.AddComponent<ShipController> ();

		foreach(Transform child in shipParent.transform){
			// Adding controller to each pixel
			addController(child);
			// Adding rigidbody and collider to each pixel under parent
			addRigidBodyAndCollider(child);
			// Adding properties such as mass and health to each gameobject under parent
			addProperties(child);
			// Adding layer to each child game object under ship parent
			addLayer(child);
		}
    }

	void addController(Transform child){
		child.gameObject.AddComponent<PixelController>();
		if(child.transform.childCount!=0){
			child.transform.GetChild(0).gameObject.AddComponent<PixelController>();
		} 
	}

	void addLayer(Transform child){
		child.gameObject.layer = DefaultValues.DEFAULT_FRIENDLY_LAYER;
		if(child.transform.childCount!=0){
			child.transform.GetChild(0).gameObject.layer = DefaultValues.DEFAULT_FRIENDLY_LAYER;
		} 
	}

	void addRigidBodyAndCollider(Transform child){
		Rigidbody2D childRigidbody = child.gameObject.AddComponent<Rigidbody2D> ();
		childRigidbody.isKinematic = true;
		BoxCollider2D childBoxCollider = child.gameObject.AddComponent<BoxCollider2D> ();
		childBoxCollider.isTrigger = true;
	}

	void addProperties(Transform child){
		Combat_PixelBehavior sps = child.GetComponent<Combat_PixelBehavior>();
		switch (sps.pixelType) {
		case Pixel.Type.Power:
			IPixelProperties powerPixelProperties = child.gameObject.AddComponent<PowerPixelProperties>();
			powerPixelProperties.Init ();
			break;
		case Pixel.Type.Armour:
			IPixelProperties armourPixelProperties = child.gameObject.AddComponent<ArmourPixelProperties>();
			armourPixelProperties.Init ();
			break;
		case Pixel.Type.Engine:
			IPixelProperties enginePixelProperties = child.gameObject.AddComponent<EnginePixelProperties>();
			enginePixelProperties.Init ();
			break;
		case Pixel.Type.Hardpoint:
			IPixelProperties hardpointPixelProperties = child.gameObject.AddComponent<HardpointPixelProperties>();
			hardpointPixelProperties.Init ();
			if(child.transform.childCount!=0){
				Combat_TurretBehavior turretBehavior = child.transform.GetChild(0).GetComponent<Combat_TurretBehavior> ();	
				TurretPixelProperties turretPixelProperties = child.transform.GetChild(0).gameObject.AddComponent<TurretPixelProperties>();
				turretPixelProperties.Init();
				turretPixelProperties.setTurretType(turretBehavior.turretType);
				// Adding controller to the turret
				child.transform.GetChild(0).gameObject.AddComponent<TurretController> ();	
			}
			break;
		case Pixel.Type.Scrap:
			IPixelProperties scrapPixelProperties = child.gameObject.AddComponent<ScrapPixelProperties>();
			scrapPixelProperties.Init ();
			break;
		}
	}

	void loadAsteroids(){
		Sprite[] asteroidSprites = Resources.LoadAll<Sprite>("Sprites/asteroids");
		for (int i=0; i<asteroidCount; i++) {
			Vector3 position = new Vector3(Random.Range (0, camera.sceneDimensions.x), Random.Range (0, camera.sceneDimensions.y), 0);
			GameObject asteroid = Instantiate(Resources.Load ("Prefabs/Asteroid"), position, Quaternion.identity) as GameObject;

			asteroid.layer = DefaultValues.DEFAULT_HOSTILE_LAYER;

			int randomAsteroidSprite = Random.Range(0, asteroidSprites.Length);
			asteroid.GetComponent<SpriteRenderer>().sprite = asteroidSprites[randomAsteroidSprite];

			PolygonCollider2D polygonCollider2D = asteroid.AddComponent<PolygonCollider2D>();
			polygonCollider2D.isTrigger = false;

			IPixelProperties asteroidPixelProperties = asteroid.AddComponent<AsteroidPixelProperties>();
			asteroidPixelProperties.Init ();
		}
		Debug.Log("Succesfully Loaded Asteroids!");
	}

	void loadEnemies(){
		// TODO
		Debug.Log("Succesfully Loaded Enemies!");
	}
}
