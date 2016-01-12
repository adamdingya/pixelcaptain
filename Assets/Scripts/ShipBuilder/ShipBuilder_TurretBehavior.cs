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

    //Uder-defined z-rotation which the turret sweeps around.
    public float facingRotationAngle;

    public bool coreConnection;

    //Is this turret currently being edited?
    public bool editing;

    //Sweep Animations
    float animationIncrement;

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
        
        //Type specific sprite assignment.
        if (_type == Turret.Type.Small)
        {
            sprite = game.sprTurrets[0];
            builder.usedWeaponPixelsCount += DefaultValues.DEFAULT_TURRET_SMALL_COST;
        }
        else if (_type == Turret.Type.Medium)
        {
            sprite = game.sprTurrets[1];
            builder.usedWeaponPixelsCount += DefaultValues.DEFAULT_TURRET_MEDIUM_COST;
        }
        else if (_type == Turret.Type.Large)
        {
            sprite = game.sprTurrets[2];
            builder.usedWeaponPixelsCount += DefaultValues.DEFAULT_TURRET_LARGE_COST;
        }

        animationIncrement = 0f;

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
        if (!editing)
            animationIncrement += DefaultValues.DEFAULT_TURRET_SWEEP_PREVIEW_SPEED * (DefaultValues.DEFAULT_TURRET_ANGLE_RANGE / mountPixel.turretMountRange); //Increment the animation proportional to the sweep distance ratio (keeps speed constant).
        else
            animationIncrement = 0f;

        if (animationIncrement > Mathf.PI * 2f)
            animationIncrement -= Mathf.PI * 2f;

        float animationAngleShift = Mathf.Sin(animationIncrement);

        //Set rotation
        transform.rotation = Quaternion.Euler(0f, 0f, facingRotationAngle + (animationAngleShift * (mountPixel.turretMountRange * 0.5f)));

        //Set colour
        spriteRenderer.color = new Color (spriteRGB.x, spriteRGB.y, spriteRGB.z, spriteAlpha);
    }

}
