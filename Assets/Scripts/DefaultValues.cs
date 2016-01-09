using UnityEngine;
using System.Collections;

public class DefaultValues : MonoBehaviour {



    public static string DEFAULT_SHIP_NAME = "My Ship";

    public static int DEFAULT_SCRAP_PIXEL_COUNT = 999;
    public static int DEFAULT_ARMOUR_PIXEL_COUNT = 999;
    public static int DEFAULT_POWER_PIXEL_COUNT = 999;
    public static int DEFAULT_HARDPOINT_PIXEL_COUNT = 999;
    public static int DEFAULT_ENGINE_PIXEL_COUNT = 999;



    // Armour square attributes
    public static float DEFAULT_ARMOUR_MASS = 20f;
	public static float DEFAULT_ARMOUR_HEALTH = 200f;

	// Scrap square attributes
	public static float DEFAULT_SCRAP_MASS = 5f;
	public static float DEFAULT_SCRAP_HEALTH = 60f;

	// Engine square attributes
	public static float DEFAULT_ENGINE_MASS = 200f;
	public static float DEFAULT_ENGINE_HEALTH = 90f;
	public static float DEFAULT_ENGINE_COMSUMPTION = 300f;
	public static float DEFAULT_ENGINE_THRUST = 100f;
	
	// Bridge square attributes
	public static float DEFAULT_BRIDGE_MASS = 40f;
	public static float DEFAULT_BRIDGE_HEALTH = 30f;
	public static float DEFAULT_BRIDGE_COMSUMPTION = 70f;

	// Pivot square attributes
	public static float DEFAULT_HARDPOINT_MASS = 10f;
	public static float DEFAULT_HARDPOINT_HEALTH = 70f;
	public static float DEFAULT_HARDPOINT_COMSUMPTION = 25f;

	// Power square attributes
	public static float DEFAULT_POWER_MASS = 120f;
	public static float DEFAULT_POWER_HEALTH = 50f;
	public static float DEFAULT_POWER_CAPACITY = 650f;

	// Turret square attributes
	public static float DEFAULT_TURRET_MASS = 150f;
	public static float DEFAULT_TURRET_HEALTH = -1f;
	public static float DEFAULT_TURRET_COMSUMPTION = 150f;
	public static int DEFAULT_TURRET_BARREL_COUNT = 1;
	public static float DEFAULT_TURRET_BULLET_SPEED = 20f;
	public static float DEFAULT_TURRET_RATE_OF_FIRE = 1f;
	public static float DEFAULT_TURRET_RANGE = 100f;
	public static float DEFAULT_TURRET_DAMAGE = 35f;
	public static float DEFAULT_TURRET_ROTATION_COEFFICIENT = 5f;
	public static float DEFAULT_TURRET_ACCURACY = 0.8f;
	
	// Shield square attributes
	public static float DEFAULT_SHIELD_MASS = 170f;
	public static float DEFAULT_SHIELD_HEALTH = 90f;
	public static float DEFAULT_SHIELD_COMSUMPTION = 200f;
	public static int DEFAULT_SHIELD_HORIZONTAL_COVERAGE = 3;
	public static int DEFAULT_SHIELD_VERTICAL_COVERAGE = 3;
	public static float DEFAULT_SHIELD_STRENGTH = 500f;
	public static float DEFAULT_SHIELD_REGENERATION_RATE = 2f;

	// Astroid atrributes
	public static int DEFAULT_ASTEROID_COUNT = 10;
	public static float DEFAULT_ASTEROID_HEALTH = 100f;
	public static float DEFAULT_ASTEROID_DAMAGE = 10f;
	public static float DEFAULT_ASTEROID_ROTATION_COEFFICIENT_MIN = 10f;
	public static float DEFAULT_ASTEROID_ROTATION_COEFFICIENT_MAX = 25f;
	public static float DEFAULT_ASTEROID_FORCE_X_MIN = 25f;
	public static float DEFAULT_ASTEROID_FORCE_X_MAX = 75f;
	public static float DEFAULT_ASTEROID_FORCE_Y_MIN = 25f;
	public static float DEFUALT_ASTEROID_FORCE_Y_MAX = 75f;


	public static float DEFAULT_BULLET_SELF_DESTRUCTION_TIME = 10f;
	public static int DEFAULT_MAX_CONCURRENT_ENEMY_COUNT = 3;
	public static Vector3 DEFAULT_PLAYER_SPAWNING_POINT = new Vector3 (-16f, -13f, -1f);
	public static Vector3[] DEFAULT_ENEMY_SPAWNING_POINT = new Vector3[]{new Vector3(16f, 13f, -1f), new Vector3(10f, 13f, -1f), new Vector3(16f, 6f, -1f)};	

}
