using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public ShipPlayerPixel[] shipPixels; //Store all pixelsArray as an array.
	public int asteroidCount;

	void Start(){
		GameManager.instance.setGameState (GameManager.GameState.Combat);
	
		shipPixels = new ShipPlayerPixel[GameManager.instance.shipArraySqrRootLength * GameManager.instance.shipArraySqrRootLength];
		loadPlayer (GameManager.instance.savedPixels);

		loadEnemies ();

		asteroidCount = DefaultValues.DEFAULT_ASTEROID_COUNT;
		loadAsteroids ();
	}

	private void loadPlayer(CompressedPixelData[] savedPixels)
    {
        Vector3 averagePosition = Vector3.zero;
        int averagePositionCount = 0;
        for (int i = 0; i < savedPixels.Length - 1; i++)
        {
            CompressedPixelData pixel = savedPixels[i];
            if (savedPixels[i] != null)
            {
                CompressedPixelData savedPixel = savedPixels[i];
                ShipPlayerPixel buildPixel = new GameObject().AddComponent<ShipPlayerPixel>();
                buildPixel.transform.name = "Pixel_x" + savedPixel.coordinates.x + "y" + savedPixel.coordinates.y;
                buildPixel.pixelType = savedPixel.pixelType;
                buildPixel.turretType = savedPixel.turretType;
                buildPixel.transform.position = savedPixel.coordinates + new Vector2(0.5f, 0.5f);
                buildPixel.coordinates = savedPixel.coordinates;
                buildPixel.index = i;
				buildPixel.init(this, savedPixel.turretType);
                //Hardpoint & turret.
                if (buildPixel.pixelType == Pixel.Type.Hardpoint)
                {
                    if (buildPixel.turretType != Turret.Type.None)
                    {
                        ShipPlayerTurret turret;
                        turret = new GameObject().AddComponent<ShipPlayerTurret>();
                        turret.turretType = buildPixel.turretType;
                        turret.transform.position = buildPixel.transform.position;
                        turret.transform.parent = buildPixel.transform;
						turret.init(buildPixel);
                        buildPixel.turret = turret;
                    }
                }

                shipPixels[i] = buildPixel;
                averagePositionCount++;
                averagePosition += buildPixel.transform.position;

                if (pixel.pixelType == Pixel.Type.Hardpoint)
                    Debug.Log("Imported HardPoint Pixel (Turret: " + pixel.turretType + ") at position " + i + ".");
                else
                    Debug.Log("Imported " + pixel.pixelType + "Pixel at position " + i + ".");
            }
        }

        averagePosition /= averagePositionCount;
        GameObject shipParent = new GameObject();
        shipParent.transform.position = averagePosition;
        shipParent.transform.name = "ShipParent";

        //SetParents & Get surrounding pixels
        for (int i = 0; i < shipPixels.Length - 1; i++)
        {
            ShipPlayerPixel pixel = shipPixels[i];
            if (pixel != null)
            {
                pixel.transform.parent = shipParent.transform;
                pixel.GetSurroundingPixels();
            }
        }

        shipParent.transform.position = Vector3.zero;

		// Adding controller to the ship
		shipParent.AddComponent<ShipController> ();

		foreach(Transform child in shipParent.transform){
			// Adding rigidbody and collider to each pixel under parent
			addRigidBodyAndCollider(child);
			// Adding properties such as mass and health to each gameobject under parent
			addProperties(child);
		}

        Debug.Log("Succesfully Loaded Player!");
    }

	private void addRigidBodyAndCollider(Transform child){
		Rigidbody2D childRigidbody = child.gameObject.AddComponent<Rigidbody2D> ();
		childRigidbody.isKinematic = true;
		BoxCollider2D childBoxCollider = child.gameObject.AddComponent<BoxCollider2D> ();
		childBoxCollider.isTrigger = true;
	}

	private void addProperties(Transform child){
		ShipPlayerPixel sps = child.GetComponent<ShipPlayerPixel>();
		switch (sps.pixelType) {
		case Pixel.Type.Power:
			PowerSquare powerSquare = child.gameObject.AddComponent<PowerSquare>();
			powerSquare.init ();
			break;
		case Pixel.Type.Armour:
			ArmourSquare armourSquare = child.gameObject.AddComponent<ArmourSquare>();
			armourSquare.init ();
			break;
		case Pixel.Type.Engine:
			EngineSquare engineSquare = child.gameObject.AddComponent<EngineSquare>();
			engineSquare.init ();
			break;
		case Pixel.Type.Hardpoint:
			HardpointSquare hardpointSquare = child.gameObject.AddComponent<HardpointSquare>();
			hardpointSquare.init ();
			break;
		case Pixel.Type.Scrap:
			ScrapSquare scrapSquare = child.gameObject.AddComponent<ScrapSquare>();
			scrapSquare.init ();
			break;
		}

		foreach(Transform childChild in child.transform){
			ShipPlayerTurret spt = childChild.GetComponent<ShipPlayerTurret> ();
			switch (spt.turretType) {
			case Turret.Type.Normal:
				TurretSquare smallTurretSquare = childChild.gameObject.AddComponent<TurretSquare>();
				smallTurretSquare.init();
				break;
			}
		}
	}

	private void loadAsteroids(){
		Sprite[] asteroidSprites = Resources.LoadAll<Sprite>("Sprites/Asteroids/asteroids");
		for (int i=0; i<asteroidCount; i++) {
			Vector3 screenPosition = Camera.main.ScreenToWorldPoint (new Vector3(Random.Range (0, Screen.width), Random.Range (0, Screen.height), -Camera.main.transform.position.z));
			GameObject asteroid = Instantiate(Resources.Load ("Prefabs/Asteroid"), screenPosition, Quaternion.identity) as GameObject;
			int randomAsteroidSprite = Random.Range(0, asteroidSprites.Length);
			asteroid.GetComponent<SpriteRenderer>().sprite = asteroidSprites[randomAsteroidSprite];
			PolygonCollider2D polygonCollider2D = asteroid.AddComponent<PolygonCollider2D>();
			polygonCollider2D.isTrigger = false;
		}
		Debug.Log("Succesfully Loaded Asteroids!");
	}

	private void loadEnemies(){
		// TODO
		Debug.Log("Succesfully Loaded Enemies!");
	}
}
