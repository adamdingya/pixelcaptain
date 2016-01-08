using UnityEngine;
using System.Collections;

public class ShipPlayerPixel : MonoBehaviour {
	
    CombatManager combatManager;
    public Pixel.Type pixelType;

    public Turret.Type turretType;
    public ShipPlayerTurret turret;

    SpriteRenderer spriteRenderer;

    public ShipPlayerPixel adjacentPixel_below_left;
    public ShipPlayerPixel adjacentPixel_below;
    public ShipPlayerPixel adjacentPixel_below_right;
    public ShipPlayerPixel adjacentPixel_left;
    public ShipPlayerPixel adjacentPixel_right;
    public ShipPlayerPixel adjacentPixel_above_left;
    public ShipPlayerPixel adjacentPixel_above;
    public ShipPlayerPixel adjacentPixel_above_right;

    public Vector2 coordinates;
    public int index;

	public void init(CombatManager combatManager, Turret.Type _turretType)
    {
		this.combatManager = combatManager;
        turretType = _turretType;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        if(pixelType == Pixel.Type.Hardpoint)
            spriteRenderer.sprite = GameManager.instance.sprHardpoint[0];
        else if (pixelType == Pixel.Type.Armour)
			spriteRenderer.sprite = GameManager.instance.sprArmour[0];
        else if (pixelType == Pixel.Type.Engine)
			spriteRenderer.sprite = GameManager.instance.sprEngine[0];
        else if (pixelType == Pixel.Type.Power)
			spriteRenderer.sprite = GameManager.instance.sprPower[0];
        else if (pixelType == Pixel.Type.Scrap)
			spriteRenderer.sprite = GameManager.instance.sprScrap[0];
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
        int offsetIndex = index + (int)offsetVector.x + ((int)offsetVector.y * GameManager.instance.shipArraySqrRootLength);

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
