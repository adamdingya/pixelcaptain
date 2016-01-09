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
    public CompressedPixelData[] savedPixels; //Current ship for loading/saving.


    //Current game state
    public enum GameState { MainMenu, CaptainsLog, Settings, Death, NavigationSystem, Reward, ShipBuilder, Combat, TacticalOptions };
    public GameState state;

    //Stat managers.
    public MainMenu_Manager mainMenuManager;
    public CaptainsLog_Manager captainsLogManager;
    public Settings_Manager settingsManager;
    public NavigationSystem_Manager navigationSystemManager;
    public ShipBuilder_Manager shipBuilderManager;
    public Combat_Manager combatManager;
    public TacticalOptions_Manager tacticalOptionsManager;

    //Define the one dimensional length of the ship array.
    public int shipArraySqrRootLength = 30;

    //Platform specific behavior, set to 'false' if not on OSX/Windows.
    public static bool NON_MOBILE_PLATFORM = true;

    public bool UNITY_REMOTE_MODE = false;

    //Define the game's sprites.
    public Sprite[] sprTurret;
    public Sprite[] sprScrap;
    public Sprite[] sprArmour;
    public Sprite[] sprEngine;
    public Sprite[] sprPower;
    public Sprite[] sprCore;
    public Sprite[] sprHardpoint;

    public static string shipName;
    public static int scrapPixels;
    public static int armourPixels;
    public static int hardpointPixels;
    public static int powerPixels;
    public static int enginePixels;

    //Global Awake() method. Calls all others to maintain order.
    public void Awake()
    {
        shipName = DefaultValues.DEFAULT_SHIP_NAME;
        scrapPixels = DefaultValues.DEFAULT_SCRAP_PIXEL_COUNT;
        armourPixels = DefaultValues.DEFAULT_ARMOUR_PIXEL_COUNT;
        hardpointPixels = DefaultValues.DEFAULT_HARDPOINT_PIXEL_COUNT;
        powerPixels = DefaultValues.DEFAULT_POWER_PIXEL_COUNT;
        enginePixels = DefaultValues.DEFAULT_ENGINE_PIXEL_COUNT;

        //Singleton pattern.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

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

        print(Game_Manager.NON_MOBILE_PLATFORM);

        //Generic Updaters (objects present in every scene).
        if (!NON_MOBILE_PLATFORM) //Platform specific.
            input.GetInput();
        else
            input.GetInputPC();


        camera.OnUpdate();


        //Scene-specific initialisers & updaters.
        if (state == GameState.MainMenu && Application.loadedLevelName == "MainMenu")
        {
            if (mainMenuManager == null)
            {
                mainMenuManager = GameObject.Find("MainMenuManager").GetComponent<MainMenu_Manager>();
                mainMenuManager.Init();
            }
            mainMenuManager.OnUpdate();
        }

        if (state == GameState.CaptainsLog && Application.loadedLevelName == "CaptainsLog")
        {
            if (captainsLogManager == null)
            {
                captainsLogManager = GameObject.Find("CaptainsLogManager").GetComponent<CaptainsLog_Manager>();
                captainsLogManager.Init();
            }
            captainsLogManager.OnUpdate();
        }

        if (state == GameState.Settings && Application.loadedLevelName == "Settings")
        {
            if (settingsManager == null)
            {
                settingsManager = GameObject.Find("SettingsManager").GetComponent<Settings_Manager>();
                settingsManager.Init();
            }
            settingsManager.OnUpdate();
        }

        if (state == GameState.NavigationSystem && Application.loadedLevelName == "NavigationSystem")
        {
            if (navigationSystemManager == null)
            {
                navigationSystemManager = GameObject.Find("NavigationSystemManager").GetComponent<NavigationSystem_Manager>();
                navigationSystemManager.Init();
            }
            navigationSystemManager.OnUpdate();
        }


        if (state == GameState.ShipBuilder && Application.loadedLevelName == "Builder")
        {
            if (shipBuilderManager == null)
            {
                shipBuilderManager = GameObject.Find("ShipBuilderManager").GetComponent<ShipBuilder_Manager>();
                shipBuilderManager.Init();
            }
            shipBuilderManager.OnUpdate();
        }

        if (state == GameState.Combat && Application.loadedLevelName == "Combat")
        {
            if (combatManager == null)
            {
                combatManager = GameObject.Find("CombatManager").GetComponent<Combat_Manager>();
                combatManager.Init();
            }
            combatManager.OnUpdate();
        }

        if (state == GameState.TacticalOptions && Application.loadedLevelName == "TacticalOptions")
        {
            if (tacticalOptionsManager == null)
            {
                tacticalOptionsManager = GameObject.Find("TacticalOptionsManager").GetComponent<TacticalOptions_Manager>();
                tacticalOptionsManager.Init();
            }
            tacticalOptionsManager.OnUpdate();
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
