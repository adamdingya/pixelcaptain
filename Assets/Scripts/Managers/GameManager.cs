using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

    public enum GameState {MainMenu, CaptainLog, Settings, Death, NavigationSystem, Reward, ShipBuilder, Combat, TacticalOptions};
    public GameState gameState;
    public Camera cam;

    public bool PC_MODE = true;

    public int gridSize;

    //ShipBuilder State Data
    public SavedPixel[] savedPixels; //Store all pixelsArray as an array.

    //Sprites
    public Sprite spritePixelScrap;
    public Sprite spritePixelArmour;
    public Sprite spritePixelEngine;
    public Sprite spritePixelPower;
    public Sprite spritePixelHardpoint;
    public Sprite spritePixelEraser;
    public Sprite[] spriteTurret;
	
    void Awake () {		
		if (instance == null) {			
			instance = this;	
		} else if (instance != this) {			
			Destroy (gameObject);    
		}
		DontDestroyOnLoad(gameObject);

		/*
		if (gameState == GameState.ShipTester)
        {
            shipPlayer = GameObject.Find("ShipPlayerManager").GetComponent<ShipPlayerManager>();
        }
		*/
	}
	
    void Update()
    {	 
		/*
        if (Application.loadedLevelName == "ShipTest" && gameState != GameState.ShipTester)
        {
            gameState = GameState.ShipTester;
            shipPlayer = GameObject.Find("ShipPlayerManager").GetComponent<ShipPlayerManager>();
            shipPlayer.init();
            shipPlayer.loadShip(savedPixels);
        }
        */
    }

	public void loadScene (string scene){
		Application.LoadLevel (scene);
	}

	public void setGameState (GameState gameState){
		this.gameState = gameState;
	}

	public void exportShip (SavedPixel[] savedPixels){
		this.savedPixels = savedPixels;
	}
}
