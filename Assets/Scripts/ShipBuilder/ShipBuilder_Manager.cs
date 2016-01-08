using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipBuilder_Manager : MonoBehaviour
{
    /// <summary>
    /// Manage the ship builder UI, interactions & states.
    /// </summary>
    
    //General references
    ShipBuilder_Ship ship;
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

    //Initialise a new grid.
    public void Init ()
    {
        camera = GameManager.instance.camera;
        ship = GameManager.instance.ship;
        shipNameText = GameObject.Find("ShipNameText").GetComponent<Text>();
        shipNameText.text = GameManager.shipName;

        ToolSelect_None();
        
        ship.corePixel = ship.BuildPixel(Pixel.Type.Core, ship.coreCoordinates, ship.coreSpriteVariant);

        camera.pan = (Vector2)GameManager.instance.ship.corePixel.transform.position + new Vector2((camera.viewBounds_TR.x - camera.viewBounds_BL.x) * 0.5f, 0f);

        pixelPlacement = false;

        previewPixel.Init(GameManager.instance);
    }

    public void OnUpdate()
    {

        //If the touch is in the grid window.
        if (GameManager.instance.camera.viewBounds_worldSpace.Contains(InputManager.instance.inputPosition))
        {
            if (InputManager.instance.state == InputManager.State.Two && (InputManager.instance.statePrev == InputManager.State.None || InputManager.instance.statePrev == InputManager.State.One))
                GameManager.instance.camera.canZoomOrPan = true;

            //If the player begins a touch within the grid window, initiate pixel placement.
            if (InputManager.instance.state == InputManager.State.One && InputManager.instance.statePrev == InputManager.State.None)
            {
                Vector2 touchedPixelCoordinates = PositionToCoordinates(InputManager.instance.inputPosition);
                int touchedPixelIndex = (int)(touchedPixelCoordinates.y * GameManager.instance.shipArraySqrRootLength + touchedPixelCoordinates.x);

                if (ship.pixels[touchedPixelIndex] != null)
                    print(ship.pixels[touchedPixelIndex].type);

                if ((ship.pixels[touchedPixelIndex] != null && ship.pixels[touchedPixelIndex].type == Pixel.Type.Core) && shipBuilderTool == ShipBuilderTools.None)
                    shipBuilderTool = ShipBuilderTools.CorePixel;

                pixelPlacement = true;
            }
        }

        //If there were two touches but aren't now, cancel zoom/pan.
        if ((InputManager.instance.state == InputManager.State.None || InputManager.instance.state == InputManager.State.One) && InputManager.instance.statePrev == InputManager.State.Two)
        {
            GameManager.instance.camera.canZoomOrPan = false;
        }


        //If there are two touches, cancel placement previewing.
        if (InputManager.instance.state == InputManager.State.Two)
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

            previewPixel.coordinates.x = (InputManager.instance.inputPosition.x / 1f) - (InputManager.instance.inputPosition.x % 1f) + previewPixel.coordinatesOffset.x;
            previewPixel.coordinates.y = (InputManager.instance.inputPosition.y / 1f) - (InputManager.instance.inputPosition.y % 1f) + previewPixel.coordinatesOffset.y;

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
            previewPixel.hoveredPixel = ship.pixels[(int)((previewPixel.coordinates.y * gridLength) + previewPixel.coordinates.x)];

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
                if (!(InputManager.instance.inputPosition.x < GameManager.instance.shipArraySqrRootLength + Mathf.Abs(previewPixel.coordinatesOffset.x)))
                    canPlacePixel = false; //If the pixel is not visible in the window, disable placement.
            }

            //Hide the hover (preview) pixel if placement isn't going to be possible.
            if (!canPlacePixel)
                previewPixel.visible = false;

            //If release touch...
            if (InputManager.instance.statePrev == InputManager.State.One && InputManager.instance.state == InputManager.State.None)
            {
                pixelPlacement = false; //End placement mode.

                if (canPlacePixel)
                    PlacePixel(); //Place pixzel.
            }
    }

    //Place a pixel.
    void PlacePixel()
    {
        if (shipBuilderTool == ShipBuilderTools.ArmourPixel && GameManager.armourPixels > ship.usedArmourPixelsCount)
            ship.BuildPixel(Pixel.Type.Armour, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.EnginePixel && GameManager.enginePixels > ship.usedEnginePixelsCount)
            ship.BuildPixel(Pixel.Type.Engine, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.HardpointPixel && GameManager.hardpointPixels > ship.usedHardpointPixelsCount)
            ship.BuildPixel(Pixel.Type.Hardpoint, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.PixelEraser)
            ship.UnBuildPixel(previewPixel.hoveredPixel);
        else if (shipBuilderTool == ShipBuilderTools.PowerPixel && GameManager.powerPixels > ship.usedPowerPixelsCount)
            ship.BuildPixel(Pixel.Type.Power, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.ScrapPixel && GameManager.scrapPixels > ship.usedScrapPixelsCount)
            ship.BuildPixel(Pixel.Type.Scrap, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (shipBuilderTool == ShipBuilderTools.Turret)
        {
            if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.type == Pixel.Type.Hardpoint)
                ship.BuildTurret(Turret.Type.Normal, previewPixel.hoveredPixel, previewPixel.spriteVariantIndex);
        }
        else if (shipBuilderTool == ShipBuilderTools.CorePixel)
        {
            //Unbuild the core pixel, rebuild it elsewhere.
            ship.UnBuildCore();
            ship.corePixel = ship.BuildPixel(Pixel.Type.Core, previewPixel.coordinates, ship.coreSpriteVariant);
            ship.coreCoordinates = ship.corePixel.coordinates;

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
        if (InputManager.instance.state == InputManager.State.None)
        {
            shipBuilderTool = ShipBuilderTools.None;
            toolButtonHighlighter.enabled = false;
        }
    }

    public void ToolSelect_ScrapPixel()
    {
        if (shipBuilderTool != ShipBuilderTools.ScrapPixel && InputManager.instance.state == InputManager.State.None)
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
        if (shipBuilderTool != ShipBuilderTools.ArmourPixel && InputManager.instance.state == InputManager.State.None)
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
        if (shipBuilderTool != ShipBuilderTools.HardpointPixel && InputManager.instance.state == InputManager.State.None)
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
        if (shipBuilderTool != ShipBuilderTools.Turret && InputManager.instance.state == InputManager.State.None)
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
        if (shipBuilderTool != ShipBuilderTools.PixelEraser && InputManager.instance.state == InputManager.State.None)
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
        if (shipBuilderTool != ShipBuilderTools.PowerPixel && InputManager.instance.state == InputManager.State.None)
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
        if (shipBuilderTool != ShipBuilderTools.EnginePixel && InputManager.instance.state == InputManager.State.None)
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
        scrapPixelCounter.text = (GameManager.scrapPixels - ship.usedScrapPixelsCount).ToString();
        armourPixelCounter.text = (GameManager.armourPixels - ship.usedArmourPixelsCount).ToString();
        enginePixelCounter.text = (GameManager.enginePixels - ship.usedEnginePixelsCount).ToString();
        powerPixelCounter.text = (GameManager.powerPixels - ship.usedPowerPixelsCount).ToString();
        hardpointPixelCounter.text = (GameManager.hardpointPixels - ship.usedHardpointPixelsCount).ToString();
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

}
