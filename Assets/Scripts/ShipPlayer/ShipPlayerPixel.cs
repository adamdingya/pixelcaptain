﻿using UnityEngine;
using System.Collections;

public class ShipPlayerPixel : MonoBehaviour {
	
    ShipPlayerManager shipPlayerManager;
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

    public void init(ShipPlayerManager shipPlayerManager, Turret.Type _turretType)
    {
        this.shipPlayerManager = shipPlayerManager;
        turretType = _turretType;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        if(pixelType == Pixel.Type.Hardpoint)
            spriteRenderer.sprite = GameManager.instance.spritePixelHardpoint;
        else if (pixelType == Pixel.Type.Armour)
			spriteRenderer.sprite = GameManager.instance.spritePixelArmour;
        else if (pixelType == Pixel.Type.Engine)
			spriteRenderer.sprite = GameManager.instance.spritePixelEngine;
        else if (pixelType == Pixel.Type.Power)
			spriteRenderer.sprite = GameManager.instance.spritePixelPower;
        else if (pixelType == Pixel.Type.Scrap)
			spriteRenderer.sprite = GameManager.instance.spritePixelScrap;
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
        int offsetIndex = index + (int)offsetVector.x + ((int)offsetVector.y * GameManager.instance.gridSize);

        if (offsetVector == new Vector2(-1, -1))
        {
            adjacentPixel_below_left = shipPlayerManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(0, -1))
        {
			adjacentPixel_below = shipPlayerManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(1, -1))
        {
			adjacentPixel_below_right = shipPlayerManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(-1, 0))
        {
			adjacentPixel_left = shipPlayerManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(1, 0))
        {
			adjacentPixel_right = shipPlayerManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(-1, 1))
        {
			adjacentPixel_above_left = shipPlayerManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(0, 1))
        {
			adjacentPixel_above = shipPlayerManager.shipPixels[offsetIndex];
        }
        else if (offsetVector == new Vector2(1, 1))
        {
			adjacentPixel_above_right = shipPlayerManager.shipPixels[offsetIndex];
        }

    }


}
