using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipBuilder_Manager : MonoBehaviour
{
    /// <summary>
    /// Manage the ship builder UI, interactions & states.
    /// </summary>
    
    //General references
    GameManager game;
    InputManager input;
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

    public RectTransform builderGridWindowUI; //Screen space build grid window.
    public Vector4 builderGridWindow;

    //Only one tool can be selected at a time.
    public enum ShipBuilderTools { None, CorePixel, ScrapPixel, ArmourPixel, HardpointPixel, Turret, PixelEraser, PowerPixel, EnginePixel };
    public ShipBuilderTools shipBuilderTool;

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


    //Initialise a new grid.
    public void Init ()
    {

        game = GameManager.instance;
        input = GameManager.instance.input;
        camera = GameManager.instance.camera;
       
        shipNameText = GameObject.Find("ShipNameText").GetComponent<Text>();
        shipNameText.text = GameManager.shipName;

        ToolSelect_None();


        pixels = new ShipBuilder_PixelBehavior[GameManager.instance.shipArraySqrRootLength * GameManager.instance.shipArraySqrRootLength];
        coreCoordinates = new Vector2(GameManager.instance.shipArraySqrRootLength * 0.5f - 1f, GameManager.instance.shipArraySqrRootLength * 0.5f - 1f);

        corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, coreSpriteVariant);

        camera.pan = (Vector2)corePixel.transform.position + new Vector2((camera.viewBounds_TR.x - camera.viewBounds_BL.x) * 0.5f, 0f);

        pixelPlacement = false;

        previewPixel.Init(game);
    }

    public void OnUpdate()
    {

        //If the touch is in the grid window.
        if (GameManager.instance.camera.viewBounds_worldSpace.Contains(input.inputPosition))
        {
            if (input.state == InputManager.State.Two && (input.statePrev == InputManager.State.None || input.statePrev == InputManager.State.One))
                GameManager.instance.camera.canZoomOrPan = true;

            //If the player begins a touch within the grid window, initiate pixel placement.
            if (input.state == InputManager.State.One && input.statePrev == InputManager.State.None)
            {
                Vector2 touchedPixelCoordinates = PositionToCoordinates(input.inputPosition);
                int touchedPixelIndex = (int)(touchedPixelCoordinates.y * GameManager.instance.shipArraySqrRootLength + touchedPixelCoordinates.x);

                if ((pixels[touchedPixelIndex] != null && pixels[touchedPixelIndex].type == Pixel.Type.Core) && shipBuilderTool == ShipBuilderTools.None)
                    shipBuilderTool = ShipBuilderTools.CorePixel;

                pixelPlacement = true;
            }
        }

        //If there were two touches but aren't now, cancel zoom/pan.
        if ((input.state == InputManager.State.None || input.state == InputManager.State.One) && input.statePrev == InputManager.State.Two)
        {
            GameManager.instance.camera.canZoomOrPan = false;
        }


        //If there are two touches, cancel placement previewing.
        if (input.state == InputManager.State.Two)
        {
            pixelPlacement = false;
        }

        if (pixelPlacement)
            PreviewPixel();
        else
            previewPixel.visible = false;


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
                previewPixel.sortingLayer = "CorePixel";
        }


            if (!GameManager.instance.NON_MOBILE_PLATFORM)
            {
                //Dynamically adjust the preview pixel's offset relative to the current zoom.
                if (GameManager.instance.camera.zoom < GameManager.instance.camera.zoomMax * 0.7f)
                    previewPixel.coordinatesOffset = new Vector2(-2, 0);
                else
                    previewPixel.coordinatesOffset = new Vector2(-4, 0);
            }
            else
            {
                //Remove the offset when using a mouse cursor.
                previewPixel.coordinatesOffset = Vector2.zero;
            }

            previewPixel.coordinates.x = (input.inputPosition.x / 1f) - (input.inputPosition.x % 1f) + previewPixel.coordinatesOffset.x;
            previewPixel.coordinates.y = (input.inputPosition.y / 1f) - (input.inputPosition.y % 1f) + previewPixel.coordinatesOffset.y;

            int gridLength = GameManager.instance.shipArraySqrRootLength;

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
            Vector2 maxVisibleCoordinates = PositionToCoordinates(new Vector2(GameManager.instance.camera.viewBounds_worldSpace.x + GameManager.instance.camera.viewBounds_worldSpace.width, GameManager.instance.camera.viewBounds_worldSpace.y + GameManager.instance.camera.viewBounds_worldSpace.height));
            Vector2 minVisibleCoordinates = PositionToCoordinates(new Vector2(GameManager.instance.camera.viewBounds_worldSpace.x, GameManager.instance.camera.viewBounds_worldSpace.y));

            //Calculate whether or not a pixel can be placed.
            bool canPlacePixel = true;

            if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.type == Pixel.Type.Core)
                canPlacePixel = false; //Cant replace the core pixel!

            //Check if the edge coordinate is visible. (Requires the grid window to be on the left, offset to go left)
            if (maxVisibleCoordinates.x < (GameManager.instance.shipArraySqrRootLength - 1))
            {
                if (!(previewPixel.coordinates.x < maxVisibleCoordinates.x + 1 && previewPixel.coordinates.x > minVisibleCoordinates.x - 1 && previewPixel.coordinates.y < maxVisibleCoordinates.y + 1 && previewPixel.coordinates.y > minVisibleCoordinates.y - 1))
                    canPlacePixel = false; //If the pixel is not visible in the window, disable placement.
            }
            else
            {
                //The edge of the grid doesn't have any further coordinates, so calculate based on the touhc position distance form the view edge.
                if (!(input.inputPosition.x < GameManager.instance.shipArraySqrRootLength + Mathf.Abs(previewPixel.coordinatesOffset.x)))
                    canPlacePixel = false; //If the pixel is not visible in the window, disable placement.
            }

            //Hide the hover (preview) pixel if placement isn't going to be possible.
            if (!canPlacePixel)
                previewPixel.visible = false;

            //If release touch...
            if (input.statePrev == InputManager.State.One && input.state == InputManager.State.None)
            {
                pixelPlacement = false; //End placement mode.

                if (canPlacePixel)
                    PlacePixel(); //Place pixzel.
            }
    }

    //Place a pixel.
    void PlacePixel()
    {
        if (shipBuilderTool == ShipBuilderTools.ArmourPixel && GameManager.armourPixels > usedArmourPixelsCount)
            BuildPixel(Pixel.Type.Armour, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.EnginePixel && GameManager.enginePixels > usedEnginePixelsCount)
            BuildPixel(Pixel.Type.Engine, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.HardpointPixel && GameManager.hardpointPixels > usedHardpointPixelsCount)
            BuildPixel(Pixel.Type.Hardpoint, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.PixelEraser)
            UnBuildPixel(previewPixel.hoveredPixel);
        else if (shipBuilderTool == ShipBuilderTools.PowerPixel && GameManager.powerPixels > usedPowerPixelsCount)
            BuildPixel(Pixel.Type.Power, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.ScrapPixel && GameManager.scrapPixels > usedScrapPixelsCount)
            BuildPixel(Pixel.Type.Scrap, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.Turret)
        {
            if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.type == Pixel.Type.Hardpoint)
                BuildTurret(Turret.Type.Normal, previewPixel.hoveredPixel, previewPixel.spriteVariantIndex);
        }
        else if (shipBuilderTool == ShipBuilderTools.CorePixel)
        {
            //Unbuild the core pixel, rebuild it elsewhere.
            UnBuildCore();
            corePixel = BuildPixel(Pixel.Type.Core, previewPixel.coordinates, coreSpriteVariant);
            coreCoordinates = corePixel.coordinates;

            shipBuilderTool = ShipBuilderTools.None; //Reset the tool to none now its placed.
        }
    }

    //Ship name.
    public void ChangeShipName()
    {
        if (!GameManager.instance.NON_MOBILE_PLATFORM)
            keyboard = TouchScreenKeyboard.Open(shipNameText.text, TouchScreenKeyboardType.Default);
    }
    void OnGUI()
    {
        if (keyboard != null)
        {
            GameManager.shipName = keyboard.text;
            shipNameText.text = GameManager.shipName;
        }
    }

    //UI Button tap actions.
    public void ToolSelect_None()
    {
        if (GameManager.instance.input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.None;
            toolButtonHighlighter.enabled = false;
        }
    }

    public void ToolSelect_ScrapPixel()
    {
        if (shipBuilderTool != ShipBuilderTools.ScrapPixel && GameManager.instance.input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.ScrapPixel;
            toolButtonHighlighter.transform.position = scrapPixelButton.transform.position;
            toolButtonHighlighter.enabled = true;
        }
        else
            ToolSelect_None();
    }

    public void ToolSelect_ArmourPixel()
    {
        if (shipBuilderTool != ShipBuilderTools.ArmourPixel && input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.ArmourPixel;
            toolButtonHighlighter.transform.position = armourPixelButton.transform.position;
            toolButtonHighlighter.enabled = true;
        }
        else
            ToolSelect_None();
    }

    public void ToolSelect_HardpointPixel()
    {
        if (shipBuilderTool != ShipBuilderTools.HardpointPixel && input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.HardpointPixel;
            toolButtonHighlighter.transform.position = hardpointPixelButton.transform.position;
            toolButtonHighlighter.enabled = true;
        }
        else
            ToolSelect_None();
    }

    public void ToolSelect_Turret()
    {
        if (shipBuilderTool != ShipBuilderTools.Turret && GameManager.instance.input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.Turret;
            toolButtonHighlighter.transform.position = turretPixelButton.transform.position;
            toolButtonHighlighter.enabled = true;
        }
        else
            ToolSelect_None();
    }

    public void ToolSelect_PixelEraser()
    {
        if (shipBuilderTool != ShipBuilderTools.PixelEraser && input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.PixelEraser;
            toolButtonHighlighter.transform.position = pixelEraserButton.transform.position;
            toolButtonHighlighter.enabled = true;
        }
        else
            ToolSelect_None();
    }

    public void ToolSelect_PowerPixel()
    {
        if (shipBuilderTool != ShipBuilderTools.PowerPixel && input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.PowerPixel;
            toolButtonHighlighter.transform.position = powerPixelButton.transform.position;
            toolButtonHighlighter.enabled = true;
        }
        else
            ToolSelect_None();
    }

    public void ToolSelect_EnginePixel()
    {
        if (shipBuilderTool != ShipBuilderTools.EnginePixel && input.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.EnginePixel;
            toolButtonHighlighter.transform.position = enginePixelButton.transform.position;
            toolButtonHighlighter.enabled = true;
        }
        else
            ToolSelect_None();
    }

    public void ButtonPress_TestShip()
    {
    }

    //Sync the UI pixel amounts to the correct values.
    public void UpdatePixelCounters()
    {
        scrapPixelCounter.text = (GameManager.scrapPixels - usedScrapPixelsCount).ToString();
        armourPixelCounter.text = (GameManager.armourPixels - usedArmourPixelsCount).ToString();
        enginePixelCounter.text = (GameManager.enginePixels - usedEnginePixelsCount).ToString();
        powerPixelCounter.text = (GameManager.powerPixels - usedPowerPixelsCount).ToString();
        hardpointPixelCounter.text = (GameManager.hardpointPixels - usedHardpointPixelsCount).ToString();
    }

    //Position converters.
    public Vector2 PositionToCoordinates(Vector2 position)
    {
        Vector2 coordinates;
        coordinates.x = (position.x / 1f) - (position.x % 1f);
        coordinates.y = (position.y / 1f) - (position.y % 1f);

        if (coordinates.x > (GameManager.instance.shipArraySqrRootLength - 1))
            coordinates.x = (GameManager.instance.shipArraySqrRootLength - 1);
        if (coordinates.x < 0)
            coordinates.x = 0;
        if (coordinates.y > (GameManager.instance.shipArraySqrRootLength - 1))
            coordinates.y = (GameManager.instance.shipArraySqrRootLength - 1);
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
        UpdatePixelCounters();
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

            UpdatePixelCounters();
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
