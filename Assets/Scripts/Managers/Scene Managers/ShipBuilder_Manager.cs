using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipBuilder_Manager : MonoBehaviour
{
    /// <summary>
    /// Manage the ship builder UI, interactions & states.
    /// </summary>
    
    //General references
    Game_Manager game;
    CameraBehavior camera;

    //Ship renaming UI & functionality (the name is stored in the shipBuild.
    Text shipNameText;
    TouchScreenKeyboard keyboard;

    //Reference to the tool button highlighter frame.
    public Image toolButtonHighlighter;

    //Bool to fine-tune the state which allows for pixel placement.
    public bool pixelPlacement = false;
    
    //References to the UI buttons.
    public GameObject scrapPixelButton;
    public GameObject armourPixelButton;
    public GameObject enginePixelButton;
    public GameObject powerPixelButton;
    public GameObject hardpointPixelButton;
    public GameObject pixelEraserButton;
    public GameObject turretPixelButton;

    public Text scrapPixelCounter;
    public Text armourPixelCounter;
    public Text enginePixelCounter;
    public Text powerPixelCounter;
    public Text hardpointPixelCounter;

    public Text turretPixelCounter;

    public Text turretPixelCost;
    public Image turretTypePreview;

    public RectTransform builderGridWindowUI; //Screen space build grid window.
    public Vector4 builderGridWindow;

    //Only one tool can be selected at a time.
    public enum ShipBuilderTools { None, CorePixel, ScrapPixel, ArmourPixel, HardpointPixel, Turret, PixelEraser, PowerPixel, EnginePixel };
    public ShipBuilderTools shipBuilderTool;

    //Currently selected turret type.
    public Turret.Type shipBuilderTurretType = Turret.Type.Small;
    public int shipBuilderTurretTypeIndex = 0;

    //Reference to the preview pixel.
    public ShipBuilder_PreviewPixel previewPixel;


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
    public int usedWeaponPixelsCount;


    //Initialise a new grid.
    public void Init ()
    {
        game = Game_Manager.instance;
        camera = game.camera;

        shipNameText = GameObject.Find("ShipNameText").GetComponent<Text>();
        shipNameText.text = Game_Manager.shipName;

        ChangeTool("None");

        //Initiliase the ship pixels array.
        pixels = new ShipBuilder_PixelBehavior[game.shipArraySqrRootLength * game.shipArraySqrRootLength];
        coreCoordinates = new Vector2(game.shipArraySqrRootLength * 0.5f - 1f, game.shipArraySqrRootLength * 0.5f - 1f); //Calculate centre coordinates.

        corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, coreSpriteVariant);
        camera.pan = (Vector2)corePixel.transform.position + new Vector2((camera.viewBounds_TR.x - camera.viewBounds_BL.x) * 0.5f, 0f);

        camera.sceneBounds_BL = Vector2.zero;
        camera.sceneBounds_TR = new Vector2(game.shipArraySqrRootLength, game.shipArraySqrRootLength);
        camera.canZoomOrPan = false;
        pixelPlacement = false;

        previewPixel.Init();
        ChangeTurretType(0);

        UpdatePixelCounters();

    }

    public void OnUpdate()
    {

        //If the touch is in the grid window.
        if (camera.viewBounds_worldSpace.Contains(game.input.inputPosition))
        {
            if (game.input.state == Input_Manager.State.Two && (game.input.statePrev == Input_Manager.State.None || game.input.statePrev == Input_Manager.State.One))
                camera.canZoomOrPan = true;

            //If the player begins a touch within the grid window, initiate pixel placement.
            if (game.input.state == Input_Manager.State.One && game.input.statePrev == Input_Manager.State.None)
            {
                Vector2 touchedPixelCoordinates = PositionToCoordinates(game.input.inputPosition);
                int touchedPixelIndex = (int)(touchedPixelCoordinates.y * game.shipArraySqrRootLength + touchedPixelCoordinates.x);

                if ((pixels[touchedPixelIndex] != null && pixels[touchedPixelIndex].type == Pixel.Type.Core) && shipBuilderTool == ShipBuilderTools.None)
                    shipBuilderTool = ShipBuilderTools.CorePixel;

                pixelPlacement = true;
            }
        }

        //If there were two touches but aren't now, cancel zoom/pan.
        if ((game.input.state == Input_Manager.State.None || game.input.state == Input_Manager.State.One) && game.input.statePrev == Input_Manager.State.Two)
        {
            camera.canZoomOrPan = false;
        }


        //If there are two touches, cancel placement previewing.
        if (game.input.state == Input_Manager.State.Two)
            pixelPlacement = false;

        if (pixelPlacement)
            PreviewPixel();
        else
        {
            previewPixel.visible = false;
            corePixel.spriteRenderer.enabled = true; //If the player was moving the core pixel but cancels, be sure to reshow it.
        }


        //Hide any pixels or turrets that are at the previewing position.
        if (previewPixel.visible == true && shipBuilderTool != ShipBuilderTools.None)
        {
            //Doesn't apply for turrets, they sit on top anyway. 
            if (shipBuilderTool != ShipBuilderTools.Turret && shipBuilderTool != ShipBuilderTools.PixelEraser)
            {
                //Hide pixel.
                if (previewPixel.hoveredPixel != null)
                {
                    previewPixel.hoveredPixel.visible = false;
                    //Hide any turret.
                    if (previewPixel.hoveredPixel.turret != null)
                        previewPixel.hoveredPixel.turret.visible = false;
                }

            }
            //If the tool type is turret, then hide the turret but not the hardpoint pixel.
            else if (shipBuilderTool == ShipBuilderTools.Turret)
            {
                if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.turret != null)
                    previewPixel.hoveredPixel.turret.visible = false;
            }
        }
        else
        {
            //If the preview pixel gets hidden, show any hidden grid pixels/turrets.
            if (previewPixel.hoveredPixel != null)
            {
                previewPixel.hoveredPixel.visible = true;
                if (previewPixel.hoveredPixel.turret != null)
                    previewPixel.hoveredPixel.turret.visible = true;
            }
        }

        //If the preview pixel has changed, but there was a hidden pixel there...
        if (previewPixel.hoveredPixelPrev != null && previewPixel.hoveredPixelPrev != previewPixel.hoveredPixel)
        {
            if (previewPixel.hoveredPixelPrev.visible == false)
                previewPixel.hoveredPixelPrev.visible = true;

            if (previewPixel.hoveredPixelPrev.turret != null)
            {
                if (previewPixel.hoveredPixelPrev.turret.visible == false)
                    previewPixel.hoveredPixelPrev.turret.visible = true;
            }
        }
    }

    //Preview a pixel placement.
    void PreviewPixel()
    {

        if ((shipBuilderTool == ShipBuilderTools.Turret || shipBuilderTool == ShipBuilderTools.PixelEraser))
            previewPixel.sortingLayer = "PreviewTurret";
        else
        {
            if (shipBuilderTool != ShipBuilderTools.CorePixel)
                previewPixel.sortingLayer = "PreviewPixel";
            else
            {
                previewPixel.sortingLayer = "CorePixel";
                corePixel.spriteRenderer.enabled = false;
            }
        }

            if (!Game_Manager.NON_MOBILE_PLATFORM)
            {
                //Dynamically adjust the preview pixel's offset relative to the current zoom.
                if (camera.zoom < camera.zoomMax * 0.7f)
                    previewPixel.coordinatesOffset = new Vector2(-2, 0);
                else
                    previewPixel.coordinatesOffset = new Vector2(-4, 0);
            }
            else
            {
                //Remove the offset when using a mouse cursor.
                previewPixel.coordinatesOffset = Vector2.zero;
            }

            previewPixel.coordinates.x = (game.input.inputPosition.x / 1f) - (game.input.inputPosition.x % 1f) + previewPixel.coordinatesOffset.x;
            previewPixel.coordinates.y = (game.input.inputPosition.y / 1f) - (game.input.inputPosition.y % 1f) + previewPixel.coordinatesOffset.y;

            int gridLength = game.shipArraySqrRootLength;

            if (previewPixel.coordinates.x > (gridLength - 1))
                previewPixel.coordinates.x = (gridLength - 1);
            if (previewPixel.coordinates.x < 0)
                previewPixel.coordinates.x = 0;
            if (previewPixel.coordinates.y > (gridLength - 1))
                previewPixel.coordinates.y = (gridLength - 1);
            if (previewPixel.coordinates.y < 0)
                previewPixel.coordinates.y = 0;

            previewPixel.transform.position = previewPixel.coordinates + new Vector2(0.5f, 0.5f);
            previewPixel.hoveredPixelPrev = previewPixel.hoveredPixel;
            previewPixel.hoveredPixel = pixels[(int)((previewPixel.coordinates.y * gridLength) + previewPixel.coordinates.x)];

            if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.type == Pixel.Type.Core)
                previewPixel.visible = false;
            else
                previewPixel.visible = true;

            previewPixel.CalculateSprite();

            //Work out the coordinates of the pixels visible along the edges.
            Vector2 maxVisibleCoordinates = PositionToCoordinates(new Vector2(camera.viewBounds_worldSpace.x + camera.viewBounds_worldSpace.width, camera.viewBounds_worldSpace.y + camera.viewBounds_worldSpace.height));
            Vector2 minVisibleCoordinates = PositionToCoordinates(new Vector2(camera.viewBounds_worldSpace.x, camera.viewBounds_worldSpace.y));

            //Calculate whether or not a pixel can be placed.
            bool canPlacePixel = true;

            if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.type == Pixel.Type.Core)
                canPlacePixel = false; //Cant replace the core pixel!

            //Check if the edge coordinate is visible. (Requires the grid window to be on the left, offset to go left)
            if (maxVisibleCoordinates.x < (game.shipArraySqrRootLength - 1))
            {
                if (!(previewPixel.coordinates.x < maxVisibleCoordinates.x + 1 && previewPixel.coordinates.x > minVisibleCoordinates.x - 1 && previewPixel.coordinates.y < maxVisibleCoordinates.y + 1 && previewPixel.coordinates.y > minVisibleCoordinates.y - 1))
                    canPlacePixel = false; //If the pixel is not visible in the window, disable placement.
            }
            else
            {
                //The edge of the grid doesn't have any further coordinates, so calculate based on the touhc position distance form the view edge.
                if (!(game.input.inputPosition.x < game.shipArraySqrRootLength + Mathf.Abs(previewPixel.coordinatesOffset.x)))
                    canPlacePixel = false; //If the pixel is not visible in the window, disable placement.
            }

            //Hide the hover (preview) pixel if placement isn't going to be possible.
            if (!canPlacePixel)
                previewPixel.visible = false;

            //If release touch...
            if (game.input.statePrev == Input_Manager.State.One && game.input.state == Input_Manager.State.None)
            {
                pixelPlacement = false; //End placement mode.

                if (canPlacePixel)
                    PlacePixel(); //Place pixzel.
            }
    }

    //Place a pixel.
    void PlacePixel()
    {
        if (shipBuilderTool == ShipBuilderTools.ArmourPixel && Game_Manager.armourPixels > usedArmourPixelsCount)
            BuildPixel(Pixel.Type.Armour, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.EnginePixel && Game_Manager.enginePixels > usedEnginePixelsCount)
            BuildPixel(Pixel.Type.Engine, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.HardpointPixel && Game_Manager.hardpointPixels > usedHardpointPixelsCount)
            BuildPixel(Pixel.Type.Hardpoint, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.PixelEraser)
            UnBuildPixel(previewPixel.hoveredPixel);
        else if (shipBuilderTool == ShipBuilderTools.PowerPixel && Game_Manager.powerPixels > usedPowerPixelsCount)
            BuildPixel(Pixel.Type.Power, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.ScrapPixel && Game_Manager.scrapPixels > usedScrapPixelsCount)
            BuildPixel(Pixel.Type.Scrap, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.Turret)
        {
            if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.type == Pixel.Type.Hardpoint)
            {
                BuildTurret(shipBuilderTurretType, previewPixel.hoveredPixel, previewPixel.spriteVariantIndex);

            }
        }
        else if (shipBuilderTool == ShipBuilderTools.CorePixel)
        {
            //Unbuild the core pixel, rebuild it elsewhere.
            UnBuildPixel(corePixel);
            corePixel = BuildPixel(Pixel.Type.Core, previewPixel.coordinates, coreSpriteVariant);
            coreCoordinates = corePixel.coordinates;

            shipBuilderTool = ShipBuilderTools.None; //Reset the tool to none now its placed.
        }
    }

    //Ship name.
    public void ChangeShipName()
    {
        if (!Game_Manager.NON_MOBILE_PLATFORM)
            keyboard = TouchScreenKeyboard.Open(shipNameText.text, TouchScreenKeyboardType.Default);
    }
    void OnGUI()
    {
        if (keyboard != null)
        {
            Game_Manager.shipName = keyboard.text;
            shipNameText.text = Game_Manager.shipName;
        }
    }

    //Change the tool enumarator. This is called from UI button event triggers.
    public void ChangeTool(string tool)
    {
        if (tool == "Armour")
        {
            if (shipBuilderTool != ShipBuilderTools.ArmourPixel && game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.ArmourPixel;
                toolButtonHighlighter.transform.position = armourPixelButton.transform.position;
                toolButtonHighlighter.enabled = true;
            }
            else
                ChangeTool("None");
        }
        else if (tool == "Core")
        {

        }
        else if (tool == "Engine")
        {
            if (shipBuilderTool != ShipBuilderTools.EnginePixel && game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.EnginePixel;
                toolButtonHighlighter.transform.position = enginePixelButton.transform.position;
                toolButtonHighlighter.enabled = true;
            }
            else
                ChangeTool("None");
        }
        else if (tool == "Hardpoint")
        {
            if (shipBuilderTool != ShipBuilderTools.HardpointPixel && game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.HardpointPixel;
                toolButtonHighlighter.transform.position = hardpointPixelButton.transform.position;
                toolButtonHighlighter.enabled = true;
            }
            else
                ChangeTool("None");
        }
        else if (tool == "None")
        {
            if (game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.None;
                toolButtonHighlighter.enabled = false;
            }
        }
        else if (tool == "Eraser")
        {
            if (shipBuilderTool != ShipBuilderTools.PixelEraser && game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.PixelEraser;
                toolButtonHighlighter.transform.position = pixelEraserButton.transform.position;
                toolButtonHighlighter.enabled = true;
            }
            else
                ChangeTool("None");
        }
        else if (tool == "Power")
        {
            if (shipBuilderTool != ShipBuilderTools.PowerPixel && game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.PowerPixel;
                toolButtonHighlighter.transform.position = powerPixelButton.transform.position;
                toolButtonHighlighter.enabled = true;
            }
            else
                ChangeTool("None");
        }
        else if (tool == "Scrap")
        {
            if (shipBuilderTool != ShipBuilderTools.ScrapPixel && game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.ScrapPixel;
                toolButtonHighlighter.transform.position = scrapPixelButton.transform.position;
                toolButtonHighlighter.enabled = true;
            }
            else
                ChangeTool("None");
        }
        else if (tool == "Turret")
        {
            if (shipBuilderTool != ShipBuilderTools.Turret && game.input.state == Input_Manager.State.None)
            {
                shipBuilderTool = ShipBuilderTools.Turret;
                toolButtonHighlighter.transform.position = turretPixelButton.transform.position;
                toolButtonHighlighter.enabled = true;
            }
            else
                ChangeTool("None");
        }
    }

    //Sync the UI pixel amounts to the correct values.
    public void UpdatePixelCounters()
    {
        scrapPixelCounter.text = (Game_Manager.scrapPixels - usedScrapPixelsCount).ToString();
        armourPixelCounter.text = (Game_Manager.armourPixels - usedArmourPixelsCount).ToString();
        enginePixelCounter.text = (Game_Manager.enginePixels - usedEnginePixelsCount).ToString();
        powerPixelCounter.text = (Game_Manager.powerPixels - usedPowerPixelsCount).ToString();
        hardpointPixelCounter.text = (Game_Manager.hardpointPixels - usedHardpointPixelsCount).ToString();
        turretPixelCounter.text = (Game_Manager.weaponPixels - usedWeaponPixelsCount).ToString();
    }

    //Position converters.
    public Vector2 PositionToCoordinates(Vector2 position)
    {
        Vector2 coordinates;
        coordinates.x = (position.x / 1f) - (position.x % 1f);
        coordinates.y = (position.y / 1f) - (position.y % 1f);

        if (coordinates.x > (game.shipArraySqrRootLength - 1))
            coordinates.x = (game.shipArraySqrRootLength - 1);
        if (coordinates.x < 0)
            coordinates.x = 0;
        if (coordinates.y > (game.shipArraySqrRootLength - 1))
            coordinates.y = (game.shipArraySqrRootLength - 1);
        if (coordinates.y < 0)
            coordinates.y = 0;

        return coordinates;
    }

    public Vector2 CoordinatesToPosition(Vector2 coordinates)
    {
        return coordinates + new Vector2(0.5f, 0.5f);
    }

    //Build a pixel in the ship builder.
    public ShipBuilder_PixelBehavior BuildPixel(Pixel.Type _type, Vector2 _coordinates, int _spriteVariantIndex)
    {
        //Calculate the array index position from the passed in coordinates.
        int index = (int)(_coordinates.y * game.shipArraySqrRootLength + _coordinates.x);

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
        pixel.Init(index, _type, _coordinates, _spriteVariantIndex);

        //Update the pixel counter UI.
        UpdatePixelCounters();
        return pixel;
    }

    //Destroy a pixel in the ship builder.
    public void UnBuildPixel(ShipBuilder_PixelBehavior _pixel)
    {
        if (_pixel != null)
        {
            //Make sure the core pixel only gets deleted if its in the code to create a new one (when the player moves it to a new coordinates).
            if (_pixel.type == Pixel.Type.Core)
                {
                if (shipBuilderTool == ShipBuilderTools.CorePixel)
                    _pixel.Destroy();
                else
                    return; //Cant delete the core pixel by erasing/replacing!
                }
            _pixel.Destroy();

            UpdatePixelCounters();
        }
    }

    public ShipBuilder_TurretBehavior BuildTurret(Turret.Type _type, ShipBuilder_PixelBehavior _mountPixel, int _spriteVariantIndex)
    {
        //If there is a pixel at the build position...
        if (_mountPixel.turret != null)
            UnBuildTurret(_mountPixel.turret); //Recycle the current pixel.

        GameObject turretObj = new GameObject();
        turretObj.transform.position = _mountPixel.transform.position + new Vector3(0.5f, 0.5f, 0f);

        ShipBuilder_TurretBehavior turret = turretObj.AddComponent<ShipBuilder_TurretBehavior>();
        turret.Init(_type, _mountPixel, _spriteVariantIndex);

        UpdatePixelCounters();

        return turret;
    }

    public void UnBuildTurret(ShipBuilder_TurretBehavior _turret)
    {
        _turret.Destroy();

        UpdatePixelCounters();
    }

    public void SaveShip()
    {
        game.savedPixels = new CompressedPixelData[game.shipArraySqrRootLength * game.shipArraySqrRootLength];

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
                    savedPixel.turretType = Turret.Type.Small;

                game.savedPixels[index] = savedPixel;
            }
        }
    }

    public void LoadShip()
    {
        pixels = new ShipBuilder_PixelBehavior[game.shipArraySqrRootLength * game.shipArraySqrRootLength];

        //Iterate through the pixels
        for (int index = 0; index < game.savedPixels.Length; index++)
        {
            CompressedPixelData savedPixel = game.savedPixels[index];
            if (savedPixel != null)
            {
                //Generate pixel based on data.
                ShipBuilder_PixelBehavior pixel = BuildPixel(savedPixel.pixelType, savedPixel.coordinates, savedPixel.spriteVariantIndex);

                //Generate turret based on data.
                if (savedPixel.turretType != Turret.Type.Small)
                    BuildTurret(savedPixel.turretType, pixel, 0);

                pixels[index] = pixel;
            }
        }
    }

    public void TestShip()
    {
    }

    public void ChangeTurretType(int increment)
    {
        shipBuilderTurretTypeIndex += increment;

        if (shipBuilderTurretTypeIndex < 0)
            shipBuilderTurretTypeIndex = game.sprTurrets.Length - 1;
        if (shipBuilderTurretTypeIndex == game.sprTurrets.Length)
            shipBuilderTurretTypeIndex = 0;

        turretTypePreview.sprite = game.sprTurrets[shipBuilderTurretTypeIndex];

        if (shipBuilderTurretTypeIndex == 0)
        {
            shipBuilderTurretType = Turret.Type.Small;
            turretPixelCost.text = (DefaultValues.DEFAULT_TURRET_SMALL_COST).ToString();
        }
        if (shipBuilderTurretTypeIndex == 1)
        {
            shipBuilderTurretType = Turret.Type.Medium;
            turretPixelCost.text = (DefaultValues.DEFAULT_TURRET_MEDIUM_COST).ToString();
        }
        if (shipBuilderTurretTypeIndex == 2)
        {
            shipBuilderTurretType = Turret.Type.Large;
            turretPixelCost.text = (DefaultValues.DEFAULT_TURRET_LARGE_COST).ToString();
        }
    }

}
