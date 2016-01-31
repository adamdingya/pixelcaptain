using UnityEngine;
using System.Collections;

public class PlaythroughData : MonoBehaviour
{

    public static CompressedPixelData[] savedPixels; //Current ship for loading/saving.

    public static string shipName = DefaultValues.DEFAULT_SHIP_NAME;
    public static int scrapPixels = DefaultValues.DEFAULT_SCRAP_PIXEL_COUNT;
    public static int armourPixels = DefaultValues.DEFAULT_ARMOUR_PIXEL_COUNT;
    public static int hardpointPixels = DefaultValues.DEFAULT_HARDPOINT_PIXEL_COUNT;
    public static int powerPixels = DefaultValues.DEFAULT_POWER_PIXEL_COUNT;
    public static int enginePixels = DefaultValues.DEFAULT_ENGINE_PIXEL_COUNT;
    public static int weaponPixels = DefaultValues.DEFAULT_WEAPON_PIXEL_COUNT;

}
