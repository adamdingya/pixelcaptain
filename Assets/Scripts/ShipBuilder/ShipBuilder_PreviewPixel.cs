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

    public SpriteRenderer spriteRenderer;
    //Special tool sprites, not defined by a ship pixel type.
    public Sprite eraserSprite;

    //Current hovered coordinate, and the corresponding pixel.
    public Vector2 coordinates;
    public ShipBuilder_PixelBehavior selectedPixel; //Equals 'null' if no pixel is present.
    public ShipBuilder_PixelBehavior selectedPixelPrev; //Equals 'null' if no pixel was present.

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
        shipBuilder = game.shipBuilder; 
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "PreviewPixel";

        //Any pixel currently at the same position as the preview.
        selectedPixel = null;
    }

}
