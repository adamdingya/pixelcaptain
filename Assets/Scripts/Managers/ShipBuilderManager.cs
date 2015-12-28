using UnityEngine;
using System.Collections;

public class ShipBuilderManager : MonoBehaviour
{
    //Scene References
    public ShipBuilderGrid builderGrid;
    public ShipBuilderUI UI;
    public Camera cam;

    //Camera Data
    float camOrthoSize;
    public float camDepth = -20.0f;
    public float borderPadding = 1f;

    //Toolbox Cursor
    public Texture2D selectedHighlight;

	void Start(){
		GameManager.instance.setGameState (GameManager.GameState.ShipBuilder);

		cam = GameObject.Find("Camera").GetComponent<Camera>();
		this.init();
	}

    //Initialise
    public void init()
    {
        camOrthoSize = 15.0f + borderPadding;
        cam.orthographicSize = camOrthoSize;

        float aspectRatio = 16.0f / 9.0f;
        float aspectRatioInv = 9.0f / 16.0f;

        cam.transform.position = new Vector3(aspectRatio * camOrthoSize - (aspectRatioInv * borderPadding * 2.0f), 15.0f, camDepth);

        builderGrid = GameObject.Find("BuilderGrid").GetComponent<ShipBuilderGrid>();
        UI = GameObject.Find("ShipBuilderUI").GetComponent<ShipBuilderUI>();

        UI.init(this);
        builderGrid.init(this);
    }

	void Update(){
		if (GameManager.instance.gameState == GameManager.GameState.ShipBuilder)
			builderGrid.OnUpdate();   
	}

    //Converters
    public Pixel.Type ToolToPixelType(ShipBuilderUI.Tool tool)
    {
        if (tool == ShipBuilderUI.Tool.Scrap)
            return Pixel.Type.Scrap;
        else if (tool == ShipBuilderUI.Tool.Armour)
            return Pixel.Type.Armour;
        else if(tool == ShipBuilderUI.Tool.Engine)
            return Pixel.Type.Engine;
        else if(tool == ShipBuilderUI.Tool.Power)
            return Pixel.Type.Power;
        else if(tool == ShipBuilderUI.Tool.Hardpoint)
            return Pixel.Type.Hardpoint;

        return Pixel.Type.Scrap;
    }

    public Sprite PixelTypeToSprite(Pixel.Type type)
    {
        if (type == Pixel.Type.Scrap)
            return GameManager.instance.spritePixelScrap;
        else if(type == Pixel.Type.Armour)
			return GameManager.instance.spritePixelArmour;
        else if(type == Pixel.Type.Engine)
			return GameManager.instance.spritePixelEngine;
        else if(type == Pixel.Type.Power)
			return GameManager.instance.spritePixelPower;
        else if(type == Pixel.Type.Hardpoint)
			return GameManager.instance.spritePixelHardpoint;

		return GameManager.instance.spritePixelScrap;
    }

	//Write the ship pixels to an array.
	public void saveShip()
	{
		ShipBuilderPixel[] pixels = builderGrid.pixelsArray;
		SavedPixel[] savedPixels = new SavedPixel[builderGrid.pixelsArrayLength * builderGrid.pixelsArrayLength];
		
		for (int i = 0; i < pixels.Length - 1; i++) {
			if (pixels [i] != null) {
				ShipBuilderPixel pixel = pixels [i];
				SavedPixel savedPixel = new SavedPixel ();
				
				if (pixel.type == Pixel.Type.Armour)
					savedPixel.pixelType = Pixel.Type.Armour;
				if (pixel.type == Pixel.Type.Engine)
					savedPixel.pixelType = Pixel.Type.Engine;
				
				if (pixel.type == Pixel.Type.Power)
					savedPixel.pixelType = Pixel.Type.Power;
				if (pixel.type == Pixel.Type.Scrap)
					savedPixel.pixelType = Pixel.Type.Scrap;
				
				if (pixel.type == Pixel.Type.Hardpoint) {
					savedPixel.pixelType = Pixel.Type.Hardpoint;
					if (pixel.turret != null) {
						if (pixel.turret.type == ShipBuilderTurret.Type.Small)
							savedPixel.turretType = Turret.Type.Small;
					}
				}
				
				savedPixel.coordinates = pixel.coordinates;				
				savedPixels [i] = savedPixel;
				
				if (savedPixel.pixelType == Pixel.Type.Hardpoint)
					Debug.Log ("Exported HardPoint Pixel (Turret: " + savedPixel.turretType + ") at position " + i + ".");
				else
					Debug.Log ("Exported " + savedPixel.pixelType + "Pixel at position " + i + ".");
			}
		}

		GameManager.instance.exportShip (savedPixels);
		Debug.Log ("Succesfully exported ship to GameManager!");

		GameManager.instance.loadScene("NavigationSystem");
	}

}
