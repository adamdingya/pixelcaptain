﻿using UnityEngine;
using System.Collections;

public class DefaultValues : MonoBehaviour {

    //Grid length.
    public static int DEFAULT_SHIP_ARRAY_SQR_ROOT_LENGTH = 30;

    //Camera
    public static Vector2 DEFAULT_TARGET_RESOLUTION = new Vector2(1920f, 1080f);
    public static Vector2 DEFAULT_SCENE_DIMENSIONS = new Vector2(100f, 100f);

    //Input.
    public static float DEFAULT_SPREAD_ANGLE_THRESHOLD = 40f; //The angle fingers have to move apart to define spreading rather than dragging.

    //Builder.
    public static float DEFAULT_INITIAL_CAMERA_ZOOM = 5f;

    public static Turret.Type DEFAULT_INITIAL_TURRET_TYPE = Turret.Type.Small; //Current turret type.

    public static int DEFAULT_TURRET_TYPE_SMALL_INDEX = 0; //Small type turret's index position in the sprite array.
    public static int DEFAULT_TURRET_TYPE_MEDIUM_INDEX = 1; //Medium type turret's index position in the sprite array.
    public static int DEFAULT_TURRET_TYPE_LARGE_INDEX = 2; //Large type turret's index position in the sprite array.

    //Combat.
    public static Vector3 DEFAULT_SHIP_SPAWN_POSITION = new Vector3(50f, 50f, 0f);

    //Turret weapon pixel costs;
    public static int DEFAULT_TURRET_SMALL_COST = 3;
    public static int DEFAULT_TURRET_MEDIUM_COST = 6;
    public static int DEFAULT_TURRET_LARGE_COST = 9;

    // Ship data.
    public static string DEFAULT_SHIP_NAME = "My Ship";

    public static int DEFAULT_SCRAP_PIXEL_COUNT = 999;
    public static int DEFAULT_ARMOUR_PIXEL_COUNT = 999;
    public static int DEFAULT_POWER_PIXEL_COUNT = 999;
    public static int DEFAULT_HARDPOINT_PIXEL_COUNT = 999;
    public static int DEFAULT_ENGINE_PIXEL_COUNT = 999;
    public static int DEFAULT_WEAPON_PIXEL_COUNT = 999;



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


    // Hardpoint & turret attributes;
    public static float DEFAULT_TURRET_ANGLE_RANGE = 90;
    public static float DEFAULT_TURRET_ANGLE_RANGE_PLUS1 = 180;
    public static float DEFAULT_TURRET_ANGLE_RANGE_PLUS2 = 270;
    public static float DEFAULT_TURRET_ANGLE_RANGE_PLUS3 = 360;
    public static float DEFAULT_TURRET_SWEEP_PREVIEW_SPEED = 0.1f;




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
