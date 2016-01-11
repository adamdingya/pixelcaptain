using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipBuilder_Manager : MonoBehaviour
{
    /// <summary>
    /// 
    /// Manage the ship builder UI, interactions & states.
    /// 
    /// The Tool system allows for 'states' which are selected one-at-a-time and altered with single touches (Mainly single-touch entering, and single-touch releasing).
    /// This allows two-touch zoom/pan interaction within any state. Starting a single-touch, then transitioning to two will cancel the single-touch actions whilst keeping state.
    /// 
    /// </summary>

    //General references
    Game_Manager game;
    CameraBehavior camera;
    Input_Manager input;

    //Ship builder manager sub-classes.
    public UserInterface userInterface;
    public Tools tools;

    //Bool to fine-tune the state which allows for pixel placement.
    public bool pixelPlacement = false;

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
    public void Init()
    {
        //Get General references.
        game = Game_Manager.instance;
        camera = game.camera;
        input = game.input;

        //Instanciate & initialise ship-builder sub-managers.
        userInterface = new UserInterface();
        tools = new Tools();
        userInterface.Init(game);
        tools.Init(game);

        //Initiliase the ship pixels array.
        pixels = new ShipBuilder_PixelBehavior[game.shipArraySqrRootLength * game.shipArraySqrRootLength];
        coreCoordinates = new Vector2(game.shipArraySqrRootLength * 0.5f - 1f, game.shipArraySqrRootLength * 0.5f - 1f); //Calculate centre coordinates.
        corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, coreSpriteVariant);
        SaveShip(); //Save the initial ship with the default core pixel;

        //Adjust the camera based on ship-builder requirements.
        camera.zoom = DefaultValues.DEFAULT_INITIAL_CAMERA_ZOOM;
        camera.pan = (Vector2)corePixel.transform.position + new Vector2((camera.viewBounds_TR.x - camera.viewBounds_BL.x) * 0.5f, 0f);

        camera.sceneDimensions = new Vector2(game.shipArraySqrRootLength, game.shipArraySqrRootLength); //This sets the scene boundary to the edge of the builder grid.

        pixelPlacement = false;

        previewPixel.Init();
        tools.ChangeTurretType(DefaultValues.DEFAULT_INITIAL_TURRET_TYPE);

        userInterface.UpdatePixelCounters();

    }

    public void OnUpdate()
    {
        if (camera.viewBounds_worldSpace.Contains(game.input.inputPosition))
        {
            if (game.input.state == Input_Manager.State.Two && (game.input.statePrev == Input_Manager.State.None || game.input.statePrev == Input_Manager.State.One))
                camera.UserMovementEnabled = true;
        }

        //If there were two touches but aren't now, cancel zoom/pan.
        if (game.input.state != Input_Manager.State.Two && game.input.statePrev == Input_Manager.State.Two)
            camera.UserMovementEnabled = false;


        /*
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

                if ((pixels[touchedPixelIndex] != null && pixels[touchedPixelIndex].type == Pixel.Type.Core) && currentTool == Tool.None)
                    currentTool = Tool.CoreMover;

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
        if (previewPixel.visible == true && currentTool != Tool.None)
        {
            //Doesn't apply for turrets, they sit on top anyway. 
            if (currentTool != Tool.TurretPlacer && currentTool != Tool.Eraser)
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
            else if (currentTool == Tool.TurretPlacer)
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
        }*/
    }

    //Preview a pixel placement.
    void PreviewPixel()
    {

        if ((tools.currentTool == Tools.Tool.TurretPlacer || tools.currentTool == Tools.Tool.Eraser))
            previewPixel.sortingLayer = "PreviewTurret";
        else
        {
            if (tools.currentTool != Tools.Tool.CoreMover)
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
        if (tools.currentTool == Tools.Tool.ArmourPlacer && PlaythroughData.armourPixels > usedArmourPixelsCount)
            BuildPixel(Pixel.Type.Armour, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (tools.currentTool == Tools.Tool.EnginePlacer && PlaythroughData.enginePixels > usedEnginePixelsCount)
            BuildPixel(Pixel.Type.Engine, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (tools.currentTool == Tools.Tool.HardpointPlacer && PlaythroughData.hardpointPixels > usedHardpointPixelsCount)
            BuildPixel(Pixel.Type.Hardpoint, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (tools.currentTool == Tools.Tool.Eraser)
            UnBuildPixel(previewPixel.hoveredPixel);
        else if (tools.currentTool == Tools.Tool.PowerPlacer && PlaythroughData.powerPixels > usedPowerPixelsCount)
            BuildPixel(Pixel.Type.Power, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (tools.currentTool == Tools.Tool.ScrapPlacer && PlaythroughData.scrapPixels > usedScrapPixelsCount)
            BuildPixel(Pixel.Type.Scrap, previewPixel.coordinates, previewPixel.spriteVariantIndex);
        else if (tools.currentTool == Tools.Tool.TurretPlacer)
        {
            if (previewPixel.hoveredPixel != null && previewPixel.hoveredPixel.type == Pixel.Type.Hardpoint)
            {
                BuildTurret(tools.currentTurretType, previewPixel.hoveredPixel, previewPixel.spriteVariantIndex);
            }
        }
        else if (tools.currentTool == Tools.Tool.CoreMover)
        {
            //Unbuild the core pixel, rebuild it elsewhere.
            UnBuildPixel(corePixel);
            corePixel = BuildPixel(Pixel.Type.Core, previewPixel.coordinates, coreSpriteVariant);
            coreCoordinates = corePixel.coordinates;

            tools.currentTool = Tools.Tool.CoreMover; //Reset the tool to none now its placed.
        }
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
        userInterface.UpdatePixelCounters();
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
                if (tools.currentTool == Tools.Tool.CoreMover)
                    _pixel.Destroy();
                else
                    return; //Cant delete the core pixel by erasing/replacing!
            }
            _pixel.Destroy();

            userInterface.UpdatePixelCounters();
        }
    }

    public ShipBuilder_TurretBehavior BuildTurret(Turret.Type _type, ShipBuilder_PixelBehavior _mountPixel, int _spriteVariantIndex)
    {
        //If there is a pixel at the build position...
        if (_mountPixel.turret != null)
            _mountPixel.turret.Destroy();

        GameObject turretObj = new GameObject();
        turretObj.transform.position = _mountPixel.transform.position + new Vector3(0.5f, 0.5f, 0f);

        ShipBuilder_TurretBehavior turret = turretObj.AddComponent<ShipBuilder_TurretBehavior>();
        turret.Init(_type, _mountPixel, _spriteVariantIndex);

        userInterface.UpdatePixelCounters();

        return turret;
    }

    /*
    BUTTON EVENT METHODS
    These are always checked for Input_Manager.State.None as they should only be pressable when no fingers are touching the screen.
    (The function is called before the input manager registers the touch, hence the 'None' being the relevent state).
    */

    public void ToolSelect_scrapPlacer()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.ScrapPlacer);
    }

    public void ToolSelect_powerPlacer()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.PowerPlacer);
    }

    public void ToolSelect_hardpointPlacer()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.HardpointPlacer);
    }

    public void ToolSelect_eraser()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.Eraser);
    }

    public void ToolSelect_enginePlacer()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.EnginePlacer);
    }

    public void ToolSelect_corePlacer()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.CoreMover);
    }

    public void ToolSelect_armourPlacer()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.ArmourPlacer);
    }

    public void ToolSelect_turretPlacer()
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTool(Tools.Tool.TurretPlacer);
    }

    public void ToolSelect_TurretTypeChange(int _increment)
    {
        if (input.state == Input_Manager.State.None)
            tools.ChangeTurretType(_increment);
    }

    public void ToolSelect_SaveShip()
    {
        if (input.state == Input_Manager.State.None)
            SaveShip();
    }

    public void ToolSelect_LoadShip()
    {
        if (input.state == Input_Manager.State.None)
            LoadShip();
    }

    public void ToolSelect_TestShip()
    {
        if (input.state == Input_Manager.State.None)
            TestShip();
    }

    public void ToolSelect_ClearGrid()
    {
        if (input.state == Input_Manager.State.None)
            ClearGrid();
    }

    public void SaveShip()
    {
        //Iterate through the pixels
        for (int index = 0; index < pixels.Length; index++)
        {
            //Clear the saved array as you we go through it.
            PlaythroughData.savedPixels[index] = null;

            //Get the current saving pixel.
            ShipBuilder_PixelBehavior pixel = pixels[index];

            //If it is an actual pixel, write it to the array.
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


                PlaythroughData.savedPixels[index] = savedPixel;

            }
        }
    }

    public void LoadShip()
    {

        ClearBuilderGrid();

        //Iterate through the pixels
        for (int index = 0; index < PlaythroughData.savedPixels.Length; index++)
        {
            CompressedPixelData savedPixel = PlaythroughData.savedPixels[index];
            if (savedPixel != null)
            {
                //Generate pixel based on data.
                ShipBuilder_PixelBehavior pixel = BuildPixel(savedPixel.pixelType, savedPixel.coordinates, savedPixel.spriteVariantIndex);

                //Generate turret based on data.
                if (savedPixel.turretType != Turret.Type.None)
                    BuildTurret(savedPixel.turretType, pixel, 0);

                pixels[index] = pixel;

                //Set the corePixel.
                if (pixel.type == Pixel.Type.Core)
                {
                    corePixel = pixels[index];
                    coreCoordinates = corePixel.coordinates;
                }
            }
        }
    }

    public void TestShip()
    {
        SaveShip();
        game.loadScene("Combat");
    }

    //Clear the grid and reset the core pixel (called by UI).
    public void ClearGrid()
    {
        ClearBuilderGrid();
        coreCoordinates = new Vector2(game.shipArraySqrRootLength * 0.5f - 1f, game.shipArraySqrRootLength * 0.5f - 1f); //Calculate centre coordinates.
        corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, coreSpriteVariant);
    }

    //Clear the grid.
    void ClearBuilderGrid()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            ShipBuilder_PixelBehavior currentPixel = pixels[i];
            if (currentPixel != null)
            {
                //Destroy core pixels.
                if (currentPixel.type == Pixel.Type.Core)
                    currentPixel.Destroy();

                UnBuildPixel(currentPixel);
            }
        }
        tools.ChangeTool(Tools.Tool.None);
    }

    //Tool Attributes & Methods.
    [System.Serializable]
    public class Tools
    {
        //Scene references.
        Game_Manager game;
        Input_Manager input;
        ShipBuilder_Manager shipBuilder;
        UserInterface userInterface;

        //Ship Builder tools. Only one can be selected at a time.
        public enum Tool { None, CoreMover, ScrapPlacer, ArmourPlacer, HardpointPlacer, TurretPlacer, TurretEditor, Eraser, PowerPlacer, EnginePlacer };
        public Tool currentTool;

        //Turret type selector.
        public Turret.Type currentTurretType;
        public int currentTurretType_index;

        //Intialise.
        public void Init(Game_Manager _game)
        {
            //Scene references.
            game = _game;
            input = game.input;
            shipBuilder = game.shipBuilder;
            userInterface = shipBuilder.userInterface;

            //Tool & Turret initial selections.
            ChangeTool(Tools.Tool.None);
            ChangeTurretType(DefaultValues.DEFAULT_INITIAL_TURRET_TYPE);
        }

        //Change the tool enumarator. This is called from UI button event triggers.
        public void ChangeTool(Tool _targetTool)
        {
            //Display the selected button highlighter by default.
            userInterface.button_selected.enabled = true;

            switch (_targetTool)
            {

                //Armour pixel placement tool.
                case Tool.ArmourPlacer:
                    if (currentTool != Tool.ArmourPlacer && input.state == Input_Manager.State.None)
                    {
                        currentTool = Tool.ArmourPlacer;
                        userInterface.button_selected.transform.position = userInterface.button_armourPlacer.transform.position;
                    }
                    else
                        ChangeTool(Tool.None);
                    break;

                //Core pixel movement tool.
                case Tool.CoreMover:
                    currentTool = Tool.CoreMover;
                    break;

                //Engine pixel placement tool.
                case Tool.EnginePlacer:
                    if (currentTool != Tool.EnginePlacer && input.state == Input_Manager.State.None)
                    {
                        currentTool = Tool.EnginePlacer;
                        userInterface.button_selected.transform.position = userInterface.button_enginePlacer.transform.position;
                    }
                    else
                        ChangeTool(Tool.None);
                    break;

                //Engine pixel placement tool.
                case Tool.HardpointPlacer:
                    if (currentTool != Tool.HardpointPlacer && input.state == Input_Manager.State.None)
                    {
                        currentTool = Tool.HardpointPlacer;
                        userInterface.button_selected.transform.position = userInterface.button_hardpointPlacer.transform.position;
                    }
                    else
                        ChangeTool(Tool.None);
                    break;

                //Reset to no tool.
                case Tool.None:
                    currentTool = Tool.None;
                    userInterface.button_selected.enabled = false;
                    break;

                //Eraser tool.
                case Tool.Eraser:
                    if (currentTool != Tool.Eraser && input.state == Input_Manager.State.None)
                    {
                        currentTool = Tool.Eraser;
                        userInterface.button_selected.transform.position = userInterface.button_eraser.transform.position;
                    }
                    else
                        ChangeTool(Tool.None);
                    break;

                //Power pixel placement tool.
                case Tool.PowerPlacer:
                    if (currentTool != Tool.PowerPlacer && input.state == Input_Manager.State.None)
                    {
                        currentTool = Tool.PowerPlacer;
                        userInterface.button_selected.transform.position = userInterface.button_powerPlacer.transform.position;
                    }
                    else
                        ChangeTool(Tool.None);
                    break;

                //Scrap pixel placement tool.
                case Tool.ScrapPlacer:
                    if (currentTool != Tool.ScrapPlacer && input.state == Input_Manager.State.None)
                    {
                        currentTool = Tool.ScrapPlacer;
                        userInterface.button_selected.transform.position = userInterface.button_scrapPlacer.transform.position;
                    }
                    else
                        ChangeTool(Tool.None);
                    break;

                //Turret placement tool.
                case Tool.TurretPlacer:
                    if (currentTool != Tool.TurretPlacer && input.state == Input_Manager.State.None)
                    {
                        currentTool = Tool.TurretPlacer;
                        userInterface.button_selected.transform.position = userInterface.button_turretPlacer.transform.position;
                    }
                    else
                        ChangeTool(Tool.None);
                    break;

                //Turret editing tool.
                case Tool.TurretEditor:
                    currentTool = Tool.TurretEditor;
                    break;

            }
        }

        //Change the turret type selector by incrementing with UI arrows. This is called from UI button event triggers.
        public void ChangeTurretType(int increment)
        {
            currentTurretType_index += increment;

            if (currentTurretType_index < 0)
                currentTurretType_index = game.sprTurrets.Length - 1;
            if (currentTurretType_index == game.sprTurrets.Length)
                currentTurretType_index = 0;

            userInterface.image_turretType.sprite = game.sprTurrets[currentTurretType_index];

            if (currentTurretType_index == DefaultValues.DEFAULT_TURRET_TYPE_SMALL_INDEX)
            {
                currentTurretType = Turret.Type.Small;
                userInterface.text_turretCost.text = (DefaultValues.DEFAULT_TURRET_SMALL_COST).ToString();
            }
            if (currentTurretType_index == DefaultValues.DEFAULT_TURRET_TYPE_MEDIUM_INDEX)
            {
                currentTurretType = Turret.Type.Medium;
                userInterface.text_turretCost.text = (DefaultValues.DEFAULT_TURRET_MEDIUM_COST).ToString();
            }
            if (currentTurretType_index == DefaultValues.DEFAULT_TURRET_TYPE_LARGE_INDEX)
            {
                currentTurretType = Turret.Type.Large;
                userInterface.text_turretCost.text = (DefaultValues.DEFAULT_TURRET_LARGE_COST).ToString();
            }
        }

        //Change the turret type selector by incrementing with UI arrows.
        public void ChangeTurretType(Turret.Type _targetTurretType)
        {
            currentTurretType = _targetTurretType;

            userInterface.image_turretType.sprite = game.sprTurrets[currentTurretType_index];

            if (currentTurretType == Turret.Type.Small)
            {
                currentTurretType_index = DefaultValues.DEFAULT_TURRET_TYPE_SMALL_INDEX;
                userInterface.text_turretCost.text = (DefaultValues.DEFAULT_TURRET_SMALL_COST).ToString();
            }
            if (currentTurretType == Turret.Type.Medium)
            {
                currentTurretType_index = DefaultValues.DEFAULT_TURRET_TYPE_MEDIUM_INDEX;
                userInterface.text_turretCost.text = (DefaultValues.DEFAULT_TURRET_MEDIUM_COST).ToString();
            }
            if (currentTurretType == Turret.Type.Large)
            {
                currentTurretType_index = DefaultValues.DEFAULT_TURRET_TYPE_LARGE_INDEX;
                userInterface.text_turretCost.text = (DefaultValues.DEFAULT_TURRET_LARGE_COST).ToString();
            }
        }
    }

    //UI Attributes & Methods.
    [System.Serializable]
    public class UserInterface
    {
        //Scene references.
        Game_Manager game;
        Input_Manager input;
        ShipBuilder_Manager shipBuilder;

        //Ship (re-)naming.
        Text text_shipName;
        TouchScreenKeyboard keyboard;

        //White crosshair which highlights the currently selected tool.
        public Image button_selected;

        //References to the UI.
        public GameObject button_scrapPlacer;
        public Text resourceCounter_scrapPixels;

        public GameObject button_armourPlacer;
        public Text resourceCounter_armourPixels;

        public GameObject button_enginePlacer;
        public Text resourceCounter_enginePixels;

        public GameObject button_powerPlacer;
        public Text resourceCounter_powerPixels;

        public GameObject button_hardpointPlacer;
        public Text resourceCounter_hardpointPixels;

        public GameObject button_turretPlacer;
        public Text resourceCounter_turretPixels;

        public Image image_turretType;
        public Text text_turretCost;

        public GameObject button_eraser;

        public RectTransform builderGridWindowUI; //Screen space build grid window.
        public Vector4 builderGridWindow;

        //Intialise.
        public void Init(Game_Manager _game)
        {
            //Scene references.
            game = _game;
            input = game.input;
            shipBuilder = game.shipBuilder;

            text_shipName = GameObject.Find("Text_shipName").GetComponent<Text>();
            text_shipName.text = PlaythroughData.shipName;

            //Get the highlighter object from the scene.
            button_selected = GameObject.Find("Button_selected").GetComponent<Image>();

            //Get button objects from the scene.
            button_scrapPlacer = GameObject.Find("Button_scrapPlacer");
            resourceCounter_scrapPixels = GameObject.Find("Text_scrapCounter").GetComponent<Text>();

            button_armourPlacer = GameObject.Find("Button_armourPlacer");
            resourceCounter_armourPixels = GameObject.Find("Text_armourCounter").GetComponent<Text>();

            button_enginePlacer = GameObject.Find("Button_enginePlacer");
            resourceCounter_enginePixels = GameObject.Find("Text_engineCounter").GetComponent<Text>();

            button_powerPlacer = GameObject.Find("Button_powerPlacer");
            resourceCounter_powerPixels = GameObject.Find("Text_powerCounter").GetComponent<Text>();

            button_hardpointPlacer = GameObject.Find("Button_hardpointPlacer");
            resourceCounter_hardpointPixels = GameObject.Find("Text_hardpointCounter").GetComponent<Text>();

            button_turretPlacer = GameObject.Find("Button_turretPlacer");
            resourceCounter_turretPixels = GameObject.Find("Text_turretCounter").GetComponent<Text>();

            image_turretType = GameObject.Find("Image_turretType").GetComponent<Image>();
            text_turretCost = GameObject.Find("Text_turretCost").GetComponent<Text>();

            button_eraser = GameObject.Find("Button_eraser");
        }

        //Sync the UI pixel amounts to the correct values.
        public void UpdatePixelCounters()
        {
            resourceCounter_scrapPixels.text = (PlaythroughData.scrapPixels - shipBuilder.usedScrapPixelsCount).ToString();
            resourceCounter_armourPixels.text = (PlaythroughData.armourPixels - shipBuilder.usedArmourPixelsCount).ToString();
            resourceCounter_enginePixels.text = (PlaythroughData.enginePixels - shipBuilder.usedEnginePixelsCount).ToString();
            resourceCounter_powerPixels.text = (PlaythroughData.powerPixels - shipBuilder.usedPowerPixelsCount).ToString();
            resourceCounter_hardpointPixels.text = (PlaythroughData.hardpointPixels - shipBuilder.usedHardpointPixelsCount).ToString();
            resourceCounter_turretPixels.text = (PlaythroughData.weaponPixels - shipBuilder.usedWeaponPixelsCount).ToString();
        }

        //Ship name.
        public void ChangeShipName()
        {
            if (!Game_Manager.NON_MOBILE_PLATFORM)
                keyboard = TouchScreenKeyboard.Open(text_shipName.text, TouchScreenKeyboardType.Default);
        }
        void OnGUI()
        {
            if (keyboard != null)
            {
                PlaythroughData.shipName = keyboard.text;
                text_shipName.text = PlaythroughData.shipName;
            }
        }

    }
}
