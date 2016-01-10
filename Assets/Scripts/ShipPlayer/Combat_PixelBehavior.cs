using UnityEngine;
using System.Collections;

public class Combat_PixelBehavior : MonoBehaviour {

    Combat_Manager combatManager;
    public Pixel.Type pixelType;

    public Turret.Type turretType;
    public Combat_TurretBehavior turret;

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

	public void init(Combat_Manager combatManager, Turret.Type _turretType)
    {
		this.combatManager = combatManager;
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

    public void GetSurroundingPixels()
    {
        //Below Left
        GetAdjacentPixel(-1, -1);

        //Below
        GetAdjacentPixel(0, -1);

        //Below Right
        GetAdjacentPixel(1, -1);

        //Left
        GetAdjacentPixel(-1, 0);

        //Right
        GetAdjacentPixel(1, 0);

        //Above Left
        GetAdjacentPixel(-1, 1);

        //Above
        GetAdjacentPixel(0, 1);

        //Above Right
        GetAdjacentPixel(1, 1);
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
