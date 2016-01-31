using UnityEngine;
using System.Collections;

public class ShipBuilder_TurretBehavior : MonoBehaviour
{

    /// <summary>
    /// Turret component for turrets created in the Ship Builder.
    /// Holds type data, a reference to the Hardpoint mount.
    /// </summary>

    //General references.
    Game_Manager game;
    ShipBuilder_Manager builder;


    //Store the turret types.
    public Turret.Type type;

    //Sprite renderer.
    public SpriteRenderer spriteRenderer;
    public Sprite sprite
    {
        //Update the spriteRenderer's sprite when this is changed.
        get { return this.spriteRenderer.sprite; }
        set { this.spriteRenderer.sprite = value; }
    }
    public float spriteAlpha;
    public Vector3 spriteRGB;

    public bool visible
    {
        get { return this.spriteRenderer.enabled; }
        set { this.spriteRenderer.enabled = value; }
    }

    //Reference to the Hardpoint mount pixel.
    public ShipBuilder_PixelBehavior mountPixel;

    //User-defined z-rotation which the turret sweeps around.
	public float turretFacingAngle;

    public bool coreConnection;

    //Is this turret currently being edited?
    public bool editing;

    //Sweep Animations
    float animationAngleShift = 0;
    bool turnRight = true;

    //Firing angle sprite renderer
    public GameObject turretAngleTemplate;
    public SpriteRenderer turretAngle_spriteRenderer;

    //Initialise created turret.
    public void Init(Turret.Type _type, ShipBuilder_PixelBehavior _mountPixel, int spriteVariantIndex)
    {
        game = Game_Manager.instance;
        builder = game.shipBuilder;
        mountPixel = _mountPixel;
        mountPixel.turret = this; //Set mount pixel's turret reference to this.
        transform.position = _mountPixel.transform.position;
        transform.name = _type + " Turret at " + _mountPixel.coordinates;

        type = _type;

        spriteAlpha = 1f;
        spriteRGB = new Vector3(1f, 1f, 1f);

        spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turret";

        //Set Turret angle sprite
        turretAngleTemplate = new GameObject();
        turretAngleTemplate.transform.parent = transform;
        turretAngleTemplate.transform.name = transform.name + "'s turret angle template";
        turretAngleTemplate.transform.position = transform.position;
        turretAngle_spriteRenderer = turretAngleTemplate.AddComponent<SpriteRenderer>();
        turretAngle_spriteRenderer.color = new Color(1f, 1f, 1f, DefaultValues.DEFAULT_TURRET_ANGLE_TEMPLATE_ALPHA);
        turretAngle_spriteRenderer.sortingLayerName = "ExhaustRegion";
       

        sprite = game.sprTurrets[(int)_type - 1];

        //Type specific sprite assignment.
        if (_type == Turret.Type.Small)
            builder.usedWeaponPixelsCount += DefaultValues.DEFAULT_TURRET_SMALL_COST;
        else if (_type == Turret.Type.Medium)
            builder.usedWeaponPixelsCount += DefaultValues.DEFAULT_TURRET_MEDIUM_COST;
        else if (_type == Turret.Type.Large)
            builder.usedWeaponPixelsCount += DefaultValues.DEFAULT_TURRET_LARGE_COST;
        else if (_type == Turret.Type.Laser)
            builder.usedWeaponPixelsCount += DefaultValues.DEFAULT_TURRET_LASER_COST;

    }

    //Destroy this turret, recycle pixels.
    public void Destroy()
    {
        //Recycle the pixel count.
        if (type == Turret.Type.Small)
            builder.usedWeaponPixelsCount -= DefaultValues.DEFAULT_TURRET_SMALL_COST;
        if (type == Turret.Type.Medium)
            builder.usedWeaponPixelsCount -= DefaultValues.DEFAULT_TURRET_MEDIUM_COST;
        if (type == Turret.Type.Large)
            builder.usedWeaponPixelsCount -= DefaultValues.DEFAULT_TURRET_LARGE_COST;
        if (type == Turret.Type.Laser)
            builder.usedWeaponPixelsCount -= DefaultValues.DEFAULT_TURRET_LASER_COST;

        GameObject.Destroy(this.gameObject);
    }

    public void SwitchCoreConnection(bool _bool)
    {
        coreConnection = _bool;
        if (!coreConnection)
            spriteRGB = new Vector3(DefaultValues.DEFAULT_NO_CORE_CONNECTION_TINT.r, DefaultValues.DEFAULT_NO_CORE_CONNECTION_TINT.g, DefaultValues.DEFAULT_NO_CORE_CONNECTION_TINT.b); //Tint
        else
            spriteRGB = new Vector3(1f, 1f, 1f); //White
    }

    
    public void OnUpdate()
    {
        //Set Turret angle template sprite depending on mount angle
		if (mountPixel.turretMountRange == DefaultValues.DEFAULT_TURRET_MOUNT_RANGE)
            turretAngle_spriteRenderer.sprite = game.sprTurretAngleTemplate[0];
		if (mountPixel.turretMountRange == DefaultValues.DEFAULT_TURRET_MOUNT_RANGE * 2)
            turretAngle_spriteRenderer.sprite = game.sprTurretAngleTemplate[1];
		if (mountPixel.turretMountRange == DefaultValues.DEFAULT_TURRET_MOUNT_RANGE * 3)
            turretAngle_spriteRenderer.sprite = game.sprTurretAngleTemplate[2];
		if (mountPixel.turretMountRange == DefaultValues.DEFAULT_TURRET_MOUNT_RANGE * 4)
            turretAngle_spriteRenderer.sprite = game.sprTurretAngleTemplate[3];

        //Sets turret to sweep in builder
        if (editing)
            animationAngleShift = 0;
              
        if (turnRight == true && animationAngleShift < mountPixel.turretMountRange * 0.5f)
        {
            animationAngleShift += DefaultValues.DEFAULT_TURRET_SWEEP_PREVIEW_SPEED;
        }
        else
        {
            turnRight = false;
            animationAngleShift -= DefaultValues.DEFAULT_TURRET_SWEEP_PREVIEW_SPEED;
        }
        if (animationAngleShift < -mountPixel.turretMountRange * 0.5f)
            turnRight = true;

        

        //Set rotation
		transform.rotation = Quaternion.Euler(0f, 0f, turretFacingAngle + animationAngleShift);
		turretAngleTemplate.transform.rotation = Quaternion.Euler(0f, 0f, turretFacingAngle);

        //Set colour
        spriteRenderer.color = new Color (spriteRGB.x, spriteRGB.y, spriteRGB.z, spriteAlpha);
    }



}
