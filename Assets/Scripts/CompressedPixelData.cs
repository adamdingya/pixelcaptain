using UnityEngine;
using System.Collections;

public class CompressedPixelData
{

    /// <summary>
    /// Defines properties to describe any pixel.
    /// Used to save/load ship data between scenes.
    /// </summary>

    public Pixel.Type pixelType;
    public Turret.Type turretType;
    public Vector2 coordinates;
    public int spriteVariantIndex;
	public float turretFacingAngle;
	public float turretMountRange;

}