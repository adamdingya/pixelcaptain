using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    /// <summary>
    /// Manage the overall game state & behavior.
    /// This is the only class which actually calls Awake() & Update().
    /// This is to control the order of all other Init() & OnUpdate() methods.
    /// This is the only persistant object.
    /// </summary>

    //General references.
    public InputManager input;
    public CameraBehavior camera;
    public CompressedPixelData[] savedPixels; //Current ship for loading/saving.

    //ShipBuilder state references.
    public ShipBuilder_Ship ship;
    public ShipBuilder_Manager builder;

    //Current game state
    public enum GameState { MainMenu, ShipBuilder, Combat };
    public GameState state;

    //Define the one dimensional length of the ship array.
    public int shipArraySqrRootLength = 30;

    //Platform specific behavior, set to 'false' if not on OSX/Windows.
    public bool PC;

    //Define the game's sprites.
    public Sprite[] sprTurret;
    public Sprite[] sprScrap;
    public Sprite[] sprArmour;
    public Sprite[] sprEngine;
    public Sprite[] sprPower;
    public Sprite[] sprCore;
    public Sprite[] sprHardpoint;

    //Global Awake() method. Calls all others to maintain order.
    public void Awake()
    {
        //Make this object persist across scenes.
        //DontDestroyOnLoad(this.gameObject);

        //Determine the OS, set boolean accordingly.
        if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer)
            PC = false;

        //General Awake() methods.
        input.Init(this);
        camera.Init(this);

        //ShipBuilder state Awake() methods.
        if (state == GameState.ShipBuilder)
        {
            ship.Init(this);
            builder.Init(this);
        }
    }

    //Global Update() method. Calls all others to maintain order.
    void Update()
    {
        //Update input specific to platform.
        if (!PC)
            input.GetInput();
        else
            input.GetInputPC();

        //Update camera
        camera.OnUpdate();

        //ShipBuilder specific updates.
        if (state == GameState.ShipBuilder)
        {
            builder.OnUpdate();
        }
    }

}
