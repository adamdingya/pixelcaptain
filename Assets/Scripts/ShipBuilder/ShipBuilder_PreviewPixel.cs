using UnityEngine;
using System.Collections;

public class ShipBuilder_PreviewPixel : MonoBehaviour
{
    /// <summary>
    /// Initialise & store data for the preview pixel.
    /// Calculate the relevant sprite when a tool is selected, showing the correct variant.
    /// </summary>

    //Scene References
    Game_Manager game;
    Input_Manager input;
    ShipBuilder_Manager shipBuilder;
    SpriteRenderer spriteRenderer;

    //Special tool sprites, not defined by a ship pixel type.
    public Sprite eraserSprite;

    //Current hovered coordinate, and the corresponding pixel.
    public Vector2 coordinates;
    public ShipBuilder_PixelBehavior hoveredPixel; //Equals 'null' if no pixel is present.
    public ShipBuilder_PixelBehavior hoveredPixelPrev; //Equals 'null' if no pixel was present.

    //The coordinate offset of the preview pixel -makes placing under your finger easier.
    public Vector2 coordinatesOffset;

    //Index of the sprite variant.
    public int spriteVariantIndex;

    public string sortingLayer
    {
        get { return spriteRenderer.sortingLayerName; }
        set { spriteRenderer.sortingLayerName = value; }
    }

    //Boolean to control the preview pixel's visibility.
    public bool visible
    {
        get { return spriteRenderer.enabled; }
        set { spriteRenderer.enabled = value; }
    }

    //Initialise the preview pixel.
    public void Init()
    {
        //Get scene references.
        game = Game_Manager.instance;
        input = game.input;
        shipBuilder = game.shipBuilderManager; 
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "PreviewPixel";

        //Any pixel currently at the same position as the preview.
        hoveredPixel = null;
    }

    //Get the preview pixel's sprite based on builder tool.
    public void CalculateSprite()
    {
        //Work out the sprite.
        if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.ArmourPixel)
            spriteRenderer.sprite = game.sprArmour[spriteVariantIndex];
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.EnginePixel)
            spriteRenderer.sprite = game.sprEngine[spriteVariantIndex];
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.HardpointPixel)
            spriteRenderer.sprite = game.sprHardpoint[spriteVariantIndex];
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.None)
            spriteRenderer.sprite = null;
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.PixelEraser)
            spriteRenderer.sprite = eraserSprite;
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.PowerPixel)
            spriteRenderer.sprite = game.sprPower[spriteVariantIndex];
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.ScrapPixel)
            spriteRenderer.sprite = game.sprScrap[spriteVariantIndex];
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.Turret)
            spriteRenderer.sprite = game.sprTurret[spriteVariantIndex];
        else if (shipBuilder.shipBuilderTool == ShipBuilder_Manager.ShipBuilderTools.CorePixel)
            spriteRenderer.sprite = game.sprCore[spriteVariantIndex];
    }
}
