using UnityEngine;
using System.Collections;

public class Game_Manager : MonoBehaviour
{

    /// <summary>
    /// Manage the overall game state & behavior.
    /// This is the only class which actually calls Awake() & Update().
    /// This is to control the order of all other Init() & OnUpdate() methods.
    /// This is the only persistant object.
    /// </summary>

    public static Game_Manager instance;

    //General references.
    public Input_Manager input;
    public CameraBehavior camera;

    //Current game state
    public enum GameState { MainMenu, CaptainsLog, Settings, Death, NavigationSystem, Reward, ShipBuilder, Combat, TacticalOptions };
    public GameState state;

    //Stat managers.
    public MainMenu_Manager mainMenu;
    public CaptainsLog_Manager captainsLog;
    public Settings_Manager settings;
    public NavigationSystem_Manager navigationSystem;
    public ShipBuilder_Manager shipBuilder;
    public Combat_Manager combat;
    public TacticalOptions_Manager tacticalOptions;

    //Define the one dimensional length of the ship array.
    public int shipArraySqrRootLength = DefaultValues.DEFAULT_SHIP_ARRAY_SQR_ROOT_LENGTH;

    //Platform specific behavior, set to 'false' if not on OSX/Windows.
    public static bool NON_MOBILE_PLATFORM = true;

    public bool UNITY_REMOTE_MODE = false;

    //Define the game's sprites. Probably should implement a static resource loader for these...
    public Sprite[] sprTurrets;
    public Sprite[] sprScrap;
    public Sprite[] sprArmour;
    public Sprite[] sprEngine;
    public Sprite[] sprPower;
    public Sprite[] sprCore;
    public Sprite[] sprHardpoint;
    public Sprite[] sprHardpointDisabled;

    public Sprite sprEraser;

    public Sprite sprExhaustRegion;

    //Global Awake() method. Calls all others to maintain order.
    public void Awake()
    {
        //Allocated the ship save space.
        PlaythroughData.savedPixels = new CompressedPixelData[shipArraySqrRootLength * shipArraySqrRootLength];

        //Singleton pattern.
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        //Determine the OS, set boolean accordingly.
        if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer)
            NON_MOBILE_PLATFORM = false;

        //If you're using Unity Remote, the OS will be non-mobile. However, you want mobile behavior as you control the game via the app.
        if (UNITY_REMOTE_MODE)
            NON_MOBILE_PLATFORM = false;

        //Generic initialisers (objects present in every scene).
        input = GameObject.Find("InputManager").GetComponent<Input_Manager>();
        camera = GameObject.Find("Camera").GetComponent<CameraBehavior>();

        //General Awake() methods.
        input.Init();
        camera.Init();
    }

    //Global Update() method. Calls all others to maintain order.
    void Update()
    {
        bool hadToFindNewCamera = false;
        bool hadToFindNewInput = false;

        //Generic finders (objects present in every scene) - Adam, if you wanna use your 'loader' system, re-create it here :)
        if (camera == null)
        {
            hadToFindNewCamera = true;
            camera = GameObject.Find("Camera").GetComponent<CameraBehavior>();
        }

        if (input == null)
        {
            hadToFindNewInput = true;
            input = GameObject.Find("InputManager").GetComponent<Input_Manager>();
        }

        //Generic Initialisers (objects present in every scene).
        if (hadToFindNewInput)
            input.Init();
        if (hadToFindNewCamera)
            camera.Init();

        input.GetInput();


        camera.OnUpdate();


        //Scene-specific initialisers & updaters.
        if (state == GameState.MainMenu && Application.loadedLevelName == "MainMenu")
        {
            if (mainMenu == null)
            {
                mainMenu = GameObject.Find("MainMenuManager").GetComponent<MainMenu_Manager>();
                mainMenu.Init();
            }
            mainMenu.OnUpdate();
        }

        if (state == GameState.CaptainsLog && Application.loadedLevelName == "CaptainsLog")
        {
            if (captainsLog == null)
            {
                captainsLog = GameObject.Find("CaptainsLogManager").GetComponent<CaptainsLog_Manager>();
                captainsLog.Init();
            }
            captainsLog.OnUpdate();
        }

        if (state == GameState.Settings && Application.loadedLevelName == "Settings")
        {
            if (settings == null)
            {
                settings = GameObject.Find("SettingsManager").GetComponent<Settings_Manager>();
                settings.Init();
            }
            settings.OnUpdate();
        }

        if (state == GameState.NavigationSystem && Application.loadedLevelName == "NavigationSystem")
        {
            if (navigationSystem == null)
            {
                navigationSystem = GameObject.Find("NavigationSystemManager").GetComponent<NavigationSystem_Manager>();
                navigationSystem.Init();
            }
            navigationSystem.OnUpdate();
        }


        if (state == GameState.ShipBuilder && Application.loadedLevelName == "Builder")
        {
            if (shipBuilder == null)
            {
                shipBuilder = GameObject.Find("ShipBuilderManager").GetComponent<ShipBuilder_Manager>();
                shipBuilder.Init();
            }
            shipBuilder.OnUpdate();
        }

        if (state == GameState.Combat && Application.loadedLevelName == "Combat")
        {
            if (combat == null)
            {
                combat = GameObject.Find("CombatManager").GetComponent<Combat_Manager>();
                combat.Init();
            }
            combat.OnUpdate();
        }

        if (state == GameState.TacticalOptions && Application.loadedLevelName == "TacticalOptions")
        {
            if (tacticalOptions == null)
            {
                tacticalOptions = GameObject.Find("TacticalOptionsManager").GetComponent<TacticalOptions_Manager>();
                tacticalOptions.Init();
            }
            tacticalOptions.OnUpdate();
        }
    }

    public void loadScene(string scene)
    {

        if (scene == "MainMenu")
            state = GameState.MainMenu;
        else if (scene == "CaptainsLog")
            state = GameState.CaptainsLog;
        else if (scene == "Settings")
            state = GameState.Settings;
        else if (scene == "Death")
            state = GameState.Death;
        else if (scene == "NavigationSystem")
            state = GameState.NavigationSystem;
        else if (scene == "Reward")
            state = GameState.Reward;
        else if (scene == "Builder")
            state = GameState.ShipBuilder;
        else if (scene == "Combat")
            state = GameState.Combat;
        else if (scene == "TacticalOptions")
            state = GameState.TacticalOptions;

        Application.LoadLevel(scene);
    }

}
