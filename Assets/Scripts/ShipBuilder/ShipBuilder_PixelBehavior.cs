using UnityEngine;
using System.Collections;

public class ShipBuilder_PixelBehavior : MonoBehaviour
{

    //General references.
    Game_Manager game;
    ShipBuilder_Manager builder;

    //Instance pixel type.
    public Pixel.Type type;

    //Instance turret type (If it has one mounted).
    public ShipBuilder_TurretBehavior turret;

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

    //Initialise the pixel.
    public void Init(int _index, Pixel.Type _type, Vector2 _coordinates, int _spriteVariantIndex)
    {
        game = Game_Manager.instance;
        builder = game.shipBuilder;
        type = _type;
        index = _index;
        coordinates = _coordinates;

        transform.position = builder.CoordinatesToPosition(_coordinates);
        transform.name = _type + " Pixel at " + _coordinates;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        //Get the surrounding pixels.
        GetSurroundingPixels(builder.pixels);

        //Manage sprite sorting layer (core pixel overlaps).
        if (type != Pixel.Type.Core)
            spriteRenderer.sortingLayerName = "GridPixel";
        else
            spriteRenderer.sortingLayerName = "CorePixel";

        //Set sprite and increment counters.
        if (_type == Pixel.Type.Armour)
        {
            sprite = game.sprArmour[_spriteVariantIndex];
            builder.usedArmourPixelsCount++;
        }
        if (_type == Pixel.Type.Engine)
        {
            sprite = game.sprEngine[_spriteVariantIndex];
            builder.usedEnginePixelsCount++;
        }
        if (_type == Pixel.Type.Hardpoint)
        {
            sprite = game.sprHardpoint[_spriteVariantIndex];
            builder.usedHardpointPixelsCount++;
        }
        if (_type == Pixel.Type.Power)
        {
            sprite = game.sprPower[_spriteVariantIndex];
            builder.usedPowerPixelsCount++;
        }
        if (_type == Pixel.Type.Scrap)
        {
            sprite = game.sprScrap[_spriteVariantIndex];
            builder.usedScrapPixelsCount++;
        }
        else if (_type == Pixel.Type.Core)
        {
            sprite = game.sprCore[builder.coreSpriteVariant];
        }

        //Assign self to the pixel array.
        builder.pixels[index] = this;

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
                if (searchIndex >= 0 && searchIndex < (_pixels.Length - 1))
                    surroundingPixels[surroundingPixelsIndex] = _pixels[searchIndex];

                surroundingPixelsIndex++;
            } 
        }

        //Assign search results to variables for easier access.
        pixel_belowLeft = surroundingPixels[0];
        if (pixel_belowLeft != null)
            pixel_belowLeft.pixel_aboveRight = this;
        pixel_below = surroundingPixels[1];
        if (pixel_below != null)
            pixel_below.pixel_above = this;
        pixel_belowRight = surroundingPixels[2];
        if (pixel_belowRight != null)
            pixel_belowRight.pixel_aboveLeft = this;

        pixel_left = surroundingPixels[3];
        if (pixel_left != null)
            pixel_left.pixel_right = this;
        pixel_right = surroundingPixels[4];
        if (pixel_right != null)
            pixel_right.pixel_left = this;

        pixel_aboveLeft = surroundingPixels[5];
        if (pixel_aboveLeft != null)
            pixel_aboveLeft.pixel_belowRight = this;
        pixel_above = surroundingPixels[6];
        if (pixel_above != null)
            pixel_above.pixel_below = this;
        pixel_aboveRight = surroundingPixels[7];
        if (pixel_aboveRight != null)
            pixel_aboveRight.pixel_belowLeft = this;
    }
}
