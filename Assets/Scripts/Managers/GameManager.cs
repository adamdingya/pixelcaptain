using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    //General references.
    public static GameManager instance = null; //Singleton.
    public InputManager input;
    public CameraBehavior camera;

    //ShipBuilder state references.
    public ShipBuilder_Manager builder;

    //Current game state
    public enum GameState { MainMenu, CaptainLog, Settings, Death, NavigationSystem, Reward, ShipBuilder, Combat, TacticalOptions };
    public GameState state;

    //Define the one dimensional length of the ship array.
    public int shipArraySqrRootLength = 30;

    //Platform specific behavior, set to 'false' if not on OSX/Windows.
    public bool NON_MOBILE_PLATFORM;

    //Define the game's sprites.
    public Sprite[] sprTurret;
    public Sprite[] sprScrap;
    public Sprite[] sprArmour;
    public Sprite[] sprEngine;
    public Sprite[] sprPower;
    public Sprite[] sprCore;
    public Sprite[] sprHardpoint;

    //ShipBuilder State Data
    public CompressedPixelData[] savedPixels; //Store all pixelsArray as an array.

    // Current playthrough data.
    public static string shipName = "My Space Ship";
    public static int scrapPixels = 999;
    public static int armourPixels = 999;
    public static int hardpointPixels = 999;
    public static int powerPixels = 999;
    public static int enginePixels = 999;


    void Awake () {	
        	
        //Singleton pattern.
		if (instance == null)	
			instance = this;	
		else if (instance != this)			
			Destroy (gameObject);

		DontDestroyOnLoad(gameObject);

        //Determine the OS, set boolean accordingly.
        NON_MOBILE_PLATFORM = true;
        if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer)
            NON_MOBILE_PLATFORM = false;

    }

    void Update()
    {
        bool generateCamera = false;
        bool generateInput = false;

        if (camera == null)
        {
            generateCamera = true;
            camera = new GameObject().AddComponent<CameraBehavior>();
            camera.gameObject.AddComponent<Camera>();
            camera.gameObject.transform.name = "Camera";
            
        }

        if (input == null)
        {
            generateInput = true;
            input = new GameObject().AddComponent<InputManager>();
            input.gameObject.transform.name = "InputManager";
        }

        if (generateCamera)
            camera.Init();
        if (generateInput)
            input.Init();

        
        if (!NON_MOBILE_PLATFORM)
            input.GetInput();
        else
            input.GetInputPC();
        camera.OnUpdate();


        if (state == GameState.ShipBuilder)
        {
            camera.viewBounds = GameObject.Find("GridWindow").GetComponent<RectTransform>();
            if (builder == null)
            {
                builder = GameObject.Find("SHIP_BUILDER").GetComponent<ShipBuilder_Manager>();
                builder.Init();
            }
            builder.OnUpdate();
        }





    }

	public void loadScene (string scene)
    {
		Application.LoadLevel (scene);
	}

	public void setGameState (GameState _state)
    {
		this.state = _state;
	}

	public void exportShip (CompressedPixelData[] _savedPixels)
    {
		this.savedPixels = _savedPixels;
	}
}
