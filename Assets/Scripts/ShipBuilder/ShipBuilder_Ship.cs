using UnityEngine;
using System.Collections;

public class ShipBuilder_Ship : MonoBehaviour
{
    /// <summary>
    /// Manage the 'ship' whilst building.
    /// Separate from the builder manager, this stores the array and builds/destroys from the grid.
    /// </summary>

    //Scene references.
    ShipBuilder_Manager builder;

    //Building & saving pixel arrays.
    public ShipBuilder_PixelBehavior[] pixels;

    //Core pixel coordinates.
    public Vector2 coreCoordinates;
    public int coreSpriteVariant = 0; //There is only one core pixel, so just choose it's sprite at the start for now.
    public ShipBuilder_PixelBehavior corePixel;

    //Amount of pixels used in the current build.
    public int usedScrapPixelsCount;
    public int usedArmourPixelsCount;
    public int usedEnginePixelsCount;
    public int usedPowerPixelsCount;
    public int usedHardpointPixelsCount;

    //Initialise a new pixel array, place a new core pixel.
    public void Init()
    {
        builder = GameManager.instance.builder;
        pixels = new ShipBuilder_PixelBehavior[GameManager.instance.shipArraySqrRootLength * GameManager.instance.shipArraySqrRootLength];
        coreCoordinates = new Vector2(GameManager.instance.shipArraySqrRootLength * 0.5f - 1f, GameManager.instance.shipArraySqrRootLength * 0.5f - 1f);
    }

    //Build a pixel in the ship builder.
    public ShipBuilder_PixelBehavior BuildPixel(Pixel.Type _type, Vector2 _coordinates, int _spriteVariantIndex)
    {
        //Calculate the array index position from the passed in coordinates.
        int index = (int)(_coordinates.y * GameManager.instance.shipArraySqrRootLength + _coordinates.x);

        //If there is a pixel at the build position...
        if (pixels[index] != null)
        {
            //If there isn't a core pixel at the position...
            if (pixels[index].type != Pixel.Type.Core)
            {
                UnBuildPixel(pixels[index]); //Recycle the current pixel.
            }
            else //If there is a core pixel at the position...
            {
                return null; //Don't build anything (Return early to prevent the remaining method code frome executing).
            }
        }

        //Create the pixel gameobject.
        GameObject pixelObj = new GameObject();

        //Assign the pixel behvaior script.
        ShipBuilder_PixelBehavior pixel = pixelObj.AddComponent<ShipBuilder_PixelBehavior>();
        pixel.Init(GameManager.instance, index, _type, _coordinates, _spriteVariantIndex);

        //Update the pixel counter UI.
        builder.UpdatePixelCounters();
        return pixel;
    }

    //Destroy a pixel in the ship builder.
    public void UnBuildPixel(ShipBuilder_PixelBehavior _pixel)
    {
        if (_pixel != null)
        {
            if (_pixel.type == Pixel.Type.Core)
                return; //Cant delete the core pixel!

            _pixel.Destroy();

            builder.UpdatePixelCounters();
        }
    }

    public void UnBuildCore()
    {
        corePixel.Destroy();
    }

    public ShipBuilder_TurretBehavior BuildTurret(Turret.Type _type, ShipBuilder_PixelBehavior _mountPixel, int _spriteVariantIndex)
    {
        //If there is a pixel at the build position...
        if (_mountPixel.turret != null)
            UnBuildTurret(_mountPixel.turret); //Recycle the current pixel.

            GameObject turretObj = new GameObject();
        turretObj.transform.position = _mountPixel.transform.position + new Vector3(0.5f, 0.5f, 0f);

        ShipBuilder_TurretBehavior turret = turretObj.AddComponent<ShipBuilder_TurretBehavior>();
        turret.Init(GameManager.instance, _type, _mountPixel, _spriteVariantIndex);

        return turret;
    }

    public void UnBuildTurret(ShipBuilder_TurretBehavior _turret)
    {
        _turret.Destroy();
    }

    public void SaveShip()
    {
        GameManager.instance.savedPixels = new CompressedPixelData[GameManager.instance.shipArraySqrRootLength * GameManager.instance.shipArraySqrRootLength];

        //Iterate through the pixels
        for (int index = 0; index < pixels.Length; index++)
        {
            ShipBuilder_PixelBehavior pixel = pixels[index];

            if (pixel != null)
            {
                //Transfer pixel data.
                CompressedPixelData savedPixel = new CompressedPixelData();
                savedPixel.pixelType = pixel.type;
                savedPixel.coordinates = pixel.coordinates;
                savedPixel.spriteVariantIndex = pixel.spriteVariantIndex;

                //Transfer turret data.
                if (pixel.turret != null)
                    savedPixel.turretType = pixel.turret.type;
                else
                    savedPixel.turretType = Turret.Type.None;

                GameManager.instance.savedPixels[index] = savedPixel;
            }
        }
    }

    public void LoadShip()
    {
        pixels = new ShipBuilder_PixelBehavior[GameManager.instance.shipArraySqrRootLength * GameManager.instance.shipArraySqrRootLength];

        //Iterate through the pixels
        for (int index = 0; index < GameManager.instance.savedPixels.Length; index++)
        {
            CompressedPixelData savedPixel = GameManager.instance.savedPixels[index];
            if (savedPixel != null)
            {
                //Generate pixel based on data.
                ShipBuilder_PixelBehavior pixel = BuildPixel(savedPixel.pixelType, savedPixel.coordinates, savedPixel.spriteVariantIndex);

                //Generate turret based on data.
                if (savedPixel.turretType != Turret.Type.None)
                    BuildTurret(savedPixel.turretType, pixel, 0);

                pixels[index] = pixel;
            }
        }
    }
}
