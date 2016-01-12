using UnityEngine;
using System.Collections;

public class ShipBuilder_PixelBehavior : MonoBehaviour
{

    //General references.
    Game_Manager game;
    ShipBuilder_Manager builder;

    //Instance pixel type.
    public Pixel.Type type;

    public bool coreConnection;

    //Instance turret type (If it has one mounted).
    public ShipBuilder_TurretBehavior turret;

    //Can a turret be mounted? (Default true for all hardpoints, but gets altered by their proximity to another turret).
    public bool canHaveTurret;

    //Range of the mounted turret's sweep, for doubling up hardpoints.
    public float turretMountRange;

    //Array position.
    public Vector2 coordinates;
    public int index;

    //Sprite rendering.
    public SpriteRenderer spriteRenderer;
    public Sprite sprite
    {
        //Update the spriteRenderer's sprite when this is changed.
        get { return this.spriteRenderer.sprite; }
        set { this.spriteRenderer.sprite = value; }
    }

    public int spriteVariantIndex; //Sprite variant array index.

    public bool visible
    {
        get { return this.spriteRenderer.enabled; }
        set { this.spriteRenderer.enabled = value; }
    }

    //Surrounding pixels.
    public ShipBuilder_PixelBehavior pixel_belowLeft;
    public ShipBuilder_PixelBehavior pixel_below;
    public ShipBuilder_PixelBehavior pixel_belowRight;
    public ShipBuilder_PixelBehavior pixel_left;
    public ShipBuilder_PixelBehavior pixel_right;
    public ShipBuilder_PixelBehavior pixel_aboveLeft;
    public ShipBuilder_PixelBehavior pixel_above;
    public ShipBuilder_PixelBehavior pixel_aboveRight;


    //Exhaust region display
    public GameObject exhaustRegion;
    public SpriteRenderer exhaustRegion_spriteRenderer;

    //Initialise the pixel.
    public void Init(int _index, Pixel.Type _type, Vector2 _coordinates, Sprite _sprite)
    {
        game = Game_Manager.instance;
        builder = game.shipBuilder;
        type = _type;
        index = _index;
        coordinates = _coordinates;

        transform.position = builder.CoordinatesToPosition(_coordinates);
        transform.name = _type + " Pixel at " + _coordinates;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        coreConnection = false;

        //Get the surrounding pixels.
        GetSurroundingPixels(builder.pixels);

        //Manage sprite sorting layer (core pixel overlaps).
        if (type != Pixel.Type.Core)
            spriteRenderer.sortingLayerName = "GridPixel";
        else
            spriteRenderer.sortingLayerName = "CorePixel";

        sprite = _sprite; //Set the sprite to the passed value (Passing maintains consistency between what was previewed and what is placed).

        canHaveTurret = false; //Set the default turret holding ability.
        turretMountRange = DefaultValues.DEFAULT_TURRET_ANGLE_RANGE;

        //Set sprite and increment counters.
        if (_type == Pixel.Type.Armour)
            builder.usedArmourPixelsCount++;
        if (_type == Pixel.Type.Engine)
        {
            builder.usedEnginePixelsCount++;
            exhaustRegion = new GameObject();
            exhaustRegion.transform.parent = transform;
            exhaustRegion.transform.name = transform.name + "'s exhaust region";
            exhaustRegion.transform.position = transform.position + new Vector3(0f, -1f, 0f);
            exhaustRegion_spriteRenderer = exhaustRegion.AddComponent<SpriteRenderer>();
            exhaustRegion_spriteRenderer.sprite = game.sprExhaustRegion;
            exhaustRegion_spriteRenderer.color = new Color(1f, 1f, 1f, DefaultValues.DEFAULT_EXHAUST_REGION_ALPHA);
            exhaustRegion_spriteRenderer.sortingLayerName = "ExhaustRegion";
        }
        if (_type == Pixel.Type.Hardpoint)
        {
            canHaveTurret = true; //Hardpoint pixels can have turrets (this ability may get disabled later by the builder manager).
            builder.usedHardpointPixelsCount++;
        }
        if (_type == Pixel.Type.Power)
            builder.usedPowerPixelsCount++;
        if (_type == Pixel.Type.Scrap)
            builder.usedScrapPixelsCount++;
        else if (_type == Pixel.Type.Core)
            sprite = game.sprCore[builder.coreSpriteVariant];

        //Assign self to the pixel array.
        builder.pixels[index] = this;

    }

    public void SwitchCoreConnection(bool _bool)
    {
        coreConnection = _bool;
        if (!coreConnection)
            spriteRenderer.color = DefaultValues.DEFAULT_NO_CORE_CONNECTION_TINT;
        else
            spriteRenderer.color = Color.white;

        if (turret != null)
            turret.SwitchCoreConnection(coreConnection);

    }

    //Enable or disable a Hardpoint's turret mount ability.
    public void SwitchHardpoint(bool _bool)
    {
        canHaveTurret = _bool;
        if (canHaveTurret)
            sprite = game.sprHardpoint[0];
        else
            sprite = game.sprHardpointDisabled[0];
    }

    //Destroy pixel.
    public void Destroy()
    {
        //Remove own reference from the builder array.
        builder.pixels[index] = null;

        //Remove reference from surrounding links (saves recalculating them after every action).
        if (pixel_belowLeft != null)
            pixel_belowLeft.pixel_aboveRight = null;
        if (pixel_below != null)
            pixel_below.pixel_above = null;
        if (pixel_belowRight != null)
            pixel_belowRight.pixel_aboveLeft = null;
        if (pixel_left != null)
            pixel_left.pixel_right = null;
        if (pixel_right != null)
            pixel_right.pixel_left = null;
        if (pixel_aboveLeft != null)
            pixel_aboveLeft.pixel_belowRight = null;
        if (pixel_above != null)
            pixel_above.pixel_below = null;
        if (pixel_aboveRight != null)
            pixel_aboveRight.pixel_belowLeft = null;

        //Remove any turrets.
        if (turret != null)
            turret.Destroy();

        //Recycle the pixel count.
        if (type == Pixel.Type.Armour)
            builder.usedArmourPixelsCount--;
        if (type == Pixel.Type.Engine)
            builder.usedEnginePixelsCount--;
        if (type == Pixel.Type.Hardpoint)
            builder.usedHardpointPixelsCount--;
        if (type == Pixel.Type.Power)
            builder.usedPowerPixelsCount--;
        if (type == Pixel.Type.Scrap)
            builder.usedScrapPixelsCount--;

        //Remove any exhaust regions.
        if (exhaustRegion != null)
            GameObject.Destroy(exhaustRegion);

        //Destroy.
        GameObject.Destroy(gameObject);

    }

    //Create mutual references with surrounding pixels.
    public void GetSurroundingPixels(ShipBuilder_PixelBehavior[] _pixels)
    {
        ShipBuilder_PixelBehavior[] surroundingPixels = new ShipBuilder_PixelBehavior[8]; ;
        int surroundingPixelsIndex = 0;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                //Calculate the coordinates offset.
                Vector2 searchOffset = new Vector2(x, y);

                //Skip self's position (0,0).
                if (searchOffset == Vector2.zero)
                    continue;

                //Calculate the index of the pixel at the search position.
                int searchIndex = index + (int)searchOffset.x + ((int)searchOffset.y * game.shipArraySqrRootLength);

                //Assign found pixel/null to the array, increment.
                if (searchIndex >= 0 && searchIndex < _pixels.Length)
                    surroundingPixels[surroundingPixelsIndex] = _pixels[searchIndex];

                surroundingPixelsIndex++;
            } 
        }

        //Set surrounding pixels (account for edges!)

        if (coordinates.x != 0 && coordinates.y != 0)
            pixel_belowLeft = surroundingPixels[0];

        if (coordinates.y != 0)
            pixel_below = surroundingPixels[1];

        if (coordinates.x != (game.shipArraySqrRootLength - 1) && coordinates.y != 0)
            pixel_belowRight = surroundingPixels[2];

        if (coordinates.x != 0)
            pixel_left = surroundingPixels[3];

        if (coordinates.x != (game.shipArraySqrRootLength - 1))
            pixel_right = surroundingPixels[4];

        if (coordinates.x != 0 && coordinates.y != (game.shipArraySqrRootLength - 1))
            pixel_aboveLeft = surroundingPixels[5];

        if (coordinates.y != (game.shipArraySqrRootLength - 1))
            pixel_above = surroundingPixels[6];

        if (coordinates.x != (game.shipArraySqrRootLength - 1) && coordinates.y != (game.shipArraySqrRootLength - 1))
            pixel_aboveRight = surroundingPixels[7];


        
        //Mutually assign references of self to surrounding pixels, saves them having to do their own searches!
        if (pixel_belowLeft != null)
            pixel_belowLeft.pixel_aboveRight = this;

        if (pixel_below != null)
            pixel_below.pixel_above = this;

        if (pixel_belowRight != null)
            pixel_belowRight.pixel_aboveLeft = this;

        if (pixel_left != null)
            pixel_left.pixel_right = this;

        if (pixel_right != null)
            pixel_right.pixel_left = this;

        if (pixel_aboveLeft != null)
            pixel_aboveLeft.pixel_belowRight = this;

        if (pixel_above != null)
            pixel_above.pixel_below = this;

        if (pixel_aboveRight != null)
            pixel_aboveRight.pixel_belowLeft = this;
        


        //POSITIONAL SPRITE SCHANGES (EDGES E.T.C) GO HERE!!
        /// ***



    }

    //Method which gets called every frame, for animation purposes (keep this lightweight!).
    public void OnUpdate()
    {
        if (turret != null)
            turret.OnUpdate();
    }


}
