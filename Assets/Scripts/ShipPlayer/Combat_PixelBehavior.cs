using UnityEngine;
using System.Collections;

public class Combat_PixelBehavior : MonoBehaviour {


    Game_Manager game;
    Combat_Manager combatManager;
    public Pixel.Type pixelType;

    public Turret.Type turretType;
	public Combat_TurretBehavior turretBehaviour;

    SpriteRenderer spriteRenderer;

    public Combat_PixelBehavior adjacentPixel_below_left;
    public Combat_PixelBehavior adjacentPixel_below;
    public Combat_PixelBehavior adjacentPixel_below_right;
    public Combat_PixelBehavior adjacentPixel_left;
    public Combat_PixelBehavior adjacentPixel_right;
    public Combat_PixelBehavior adjacentPixel_above_left;
    public Combat_PixelBehavior adjacentPixel_above;
    public Combat_PixelBehavior adjacentPixel_above_right;

    public Vector2 coordinates;
    public int index;

	public void init(Turret.Type _turretType)
    {
        game = Game_Manager.instance;
        combatManager = game.combat;
        turretType = _turretType;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "GridPixel";
        if(pixelType == Pixel.Type.Hardpoint)
            spriteRenderer.sprite = Game_Manager.instance.sprHardpoint[0];
        else if (pixelType == Pixel.Type.Armour)
			spriteRenderer.sprite = Game_Manager.instance.sprArmour[0];
        else if (pixelType == Pixel.Type.Engine)
			spriteRenderer.sprite = Game_Manager.instance.sprEngine[0];
        else if (pixelType == Pixel.Type.Power)
			spriteRenderer.sprite = Game_Manager.instance.sprPower[0];
        else if (pixelType == Pixel.Type.Scrap)
			spriteRenderer.sprite = Game_Manager.instance.sprScrap[0];
        else if (pixelType == Pixel.Type.Core)
            spriteRenderer.sprite = Game_Manager.instance.sprCore[0];
    }

    //Create mutual references with surrounding pixels.
    public void GetSurroundingPixels(Combat_PixelBehavior[] _pixels)
    {
        Combat_PixelBehavior[] surroundingPixels = new Combat_PixelBehavior[8]; ;
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
            adjacentPixel_below_left = surroundingPixels[0];

        if (coordinates.y != 0)
            adjacentPixel_below = surroundingPixels[1];

        if (coordinates.x != (game.shipArraySqrRootLength - 1) && coordinates.y != 0)
            adjacentPixel_below_right = surroundingPixels[2];

        if (coordinates.x != 0)
            adjacentPixel_left = surroundingPixels[3];

        if (coordinates.x != (game.shipArraySqrRootLength - 1))
            adjacentPixel_right = surroundingPixels[4];

        if (coordinates.x != 0 && coordinates.y != (game.shipArraySqrRootLength - 1))
            adjacentPixel_above_left = surroundingPixels[5];

        if (coordinates.y != (game.shipArraySqrRootLength - 1))
            adjacentPixel_above = surroundingPixels[6];

        if (coordinates.x != (game.shipArraySqrRootLength - 1) && coordinates.y != (game.shipArraySqrRootLength - 1))
            adjacentPixel_above_right = surroundingPixels[7];



        //Mutually assign references of self to surrounding pixels, saves them having to do their own searches!
        if (adjacentPixel_below_left != null)
            adjacentPixel_below_left.adjacentPixel_above_right = this;

        if (adjacentPixel_below != null)
            adjacentPixel_below.adjacentPixel_above = this;

        if (adjacentPixel_below_right != null)
            adjacentPixel_below_right.adjacentPixel_above_left = this;

        if (adjacentPixel_left != null)
            adjacentPixel_left.adjacentPixel_right = this;

        if (adjacentPixel_right != null)
            adjacentPixel_right.adjacentPixel_left = this;

        if (adjacentPixel_above_left != null)
            adjacentPixel_above_left.adjacentPixel_below_right = this;

        if (adjacentPixel_above != null)
            adjacentPixel_above.adjacentPixel_below = this;

        if (adjacentPixel_above_right != null)
            adjacentPixel_above_right.adjacentPixel_below_left = this;



        //POSITIONAL SPRITE SCHANGES (EDGES E.T.C) GO HERE!!
        /// ***




    }

    public void GetAdjacentPixel( int _xOffset, int _yOffset)
    {
        Vector2 offsetVector = new Vector2(_xOffset, _yOffset);
        int offsetIndex = index + (int)offsetVector.x + ((int)offsetVector.y * Game_Manager.instance.shipArraySqrRootLength);

        if (offsetVector == new Vector2(-1, -1))
        {
			adjacentPixel_below_left = combatManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(0, -1))
        {
			adjacentPixel_below = combatManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(1, -1))
        {
			adjacentPixel_below_right = combatManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(-1, 0))
        {
			adjacentPixel_left = combatManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(1, 0))
        {
			adjacentPixel_right = combatManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(-1, 1))
        {
			adjacentPixel_above_left = combatManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(0, 1))
        {
			adjacentPixel_above = combatManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(1, 1))
        {
			adjacentPixel_above_right = combatManager.shipPixels[offsetIndex];
        }

    }
}
