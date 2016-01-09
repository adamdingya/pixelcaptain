using UnityEngine;
using System.Collections;

public class ShipBuilder_TurretBehavior : MonoBehaviour
{

    /// <summary>
    /// Turret component for turrets created in the Ship Builder.
    /// Holds type data, a reference to the Hardpoint mount.
    /// </summary>

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

    public bool visible
    {
        get { return this.spriteRenderer.enabled; }
        set { this.spriteRenderer.enabled = value; }
    }

    //Reference to the Hardpoint mount pixel.
    public ShipBuilder_PixelBehavior mountPixel;

    //Initialise created turret.
    public void Init(Game_Manager game, Turret.Type _type, ShipBuilder_PixelBehavior _mountPixel, int spriteVariantIndex)
    {
        _mountPixel.turret = this; //Set mount pixel's turret reference to this.
        transform.position = _mountPixel.transform.position;
        transform.name = _type + " Turret at " + _mountPixel.coordinates;

        type = _type;

        spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turret";
        
        //Type specific sprite assignment.
        if (_type == Turret.Type.Normal)
        {
            sprite = game.sprTurret[spriteVariantIndex];
        }

    }

    //Destroy this turret, recycle pixels.
    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }

}
