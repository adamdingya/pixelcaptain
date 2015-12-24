using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public enum GameState {ShipBuilder, ShipTester};
    public GameState gameState;
    public Camera cam;

    public bool PC_MODE = true;

    public InputManager input;

    public SoundManager sound;

    public ShipPlayerManager shipPlayer;

    public int gridSize;

    //ShipBuilder State Data
    public ShipBuilderManager shipBuilder;
    public SavedPixel[] savedPixels; //Store all pixelsArray as an array.

    //Sprites
    public Sprite spritePixelScrap;
    public Sprite spritePixelArmour;
    public Sprite spritePixelEngine;
    public Sprite spritePixelPower;
    public Sprite spritePixelHardpoint;
    public Sprite spritePixelEraser;
    public Sprite[] spriteTurret;

    // Use this for initialization
    void Awake () {

        DontDestroyOnLoad(gameObject);

        //Find the camera & input manager.
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        input = GameObject.Find("InputManager").GetComponent<InputManager>();
        sound = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        input.Init(this);
        sound.Init(this);

        //If in the ship builder...
        if (gameState == GameState.ShipBuilder)
        {
            shipBuilder = GameObject.Find("ShipBuilderManager").GetComponent<ShipBuilderManager>();
            shipBuilder.Init(this);
        }
        else if (gameState == GameState.ShipTester)
        {
            shipPlayer = GameObject.Find("ShipPlayerManager").GetComponent<ShipPlayerManager>();
        }

	}

    // Update is called once per frame
    void Update()
    {
        if (cam == null)
            cam = GameObject.Find("Camera").GetComponent<Camera>();
        if (input == null)
        {
            input = GameObject.Find("InputManager").GetComponent<InputManager>();
            input.Init(this);
        }
        if (sound == null)
        {
            sound = GameObject.Find("SoundManager").GetComponent<SoundManager>();
            sound.Init(this);
        }

        
        //If just loaded the Ship Player...
        if (Application.loadedLevelName == "ShipTest" && gameState != GameState.ShipTester)
        {
            gameState = GameState.ShipTester;
            shipPlayer = GameObject.Find("ShipPlayerManager").GetComponent<ShipPlayerManager>();
            shipPlayer.Init(this);
            shipPlayer.LoadShip(savedPixels);
        }

        //If in the Ship Builder...
        if (gameState == GameState.ShipBuilder)
        {
            shipBuilder.OnUpdate();
        }


        if (!PC_MODE)
            input.GetInput(); //Get the user input.
        else
            input.GetInputPC(); //Get the user input.
    }

    //Write the ship pixels to an array.
    public void SaveShip()
    {
        ShipBuilderPixel[] pixels = shipBuilder.builderGrid.pixelsArray;
        savedPixels = new SavedPixel[shipBuilder.builderGrid.pixelsArrayLength * shipBuilder.builderGrid.pixelsArrayLength];

        for (int i = 0; i < pixels.Length - 1; i++)
        {
            if (pixels[i] != null)
            {
                ShipBuilderPixel pixel = pixels[i];
                SavedPixel savedPixel = new SavedPixel();

                if (pixel.type == Pixel.Type.Armour)
                    savedPixel.pixelType = Pixel.Type.Armour;
                if (pixel.type == Pixel.Type.Engine)
                    savedPixel.pixelType = Pixel.Type.Engine;

                if (pixel.type == Pixel.Type.Power)
                    savedPixel.pixelType = Pixel.Type.Power;
                if (pixel.type == Pixel.Type.Scrap)
                    savedPixel.pixelType = Pixel.Type.Scrap;

                if (pixel.type == Pixel.Type.Hardpoint)
                {
                    savedPixel.pixelType = Pixel.Type.Hardpoint;
                    if (pixel.turret != null)
                    {
                        if (pixel.turret.type == ShipBuilderTurret.Type.Small)
                            savedPixel.turretType = Turret.Type.Small;
                    }
                }

                savedPixel.coordinates = pixel.coordinates;

                savedPixels[i] = savedPixel;

                if (savedPixel.pixelType == Pixel.Type.Hardpoint)
                    Debug.Log("Exported HardPoint Pixel (Turret: " + savedPixel.turretType + ") at position " + i + ".");
                else
                    Debug.Log("Exported " + savedPixel.pixelType + "Pixel at position " + i + ".");
            }
        }
        Debug.Log("Succesfully Exported Ship!");
        Application.LoadLevel("ShipTest"); 
    }
}
