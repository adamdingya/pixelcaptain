﻿using UnityEngine;
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

    //Reference to the preview pixel.
    public ShipBuilder_PreviewPixel previewPixel;

    //Building & saving pixel arrays.
    public ShipBuilder_PixelBehavior[] pixels;

    //Core pixel coordinates.
    public Vector2 coreCoordinates;
    public int coreSpriteVariant;
    public ShipBuilder_PixelBehavior corePixel;

    //Last placed or selected turret.
    public ShipBuilder_TurretBehavior turretEditTarget;

    //Amount of pixels used in the current build.
    public int usedScrapPixelsCount;
    public int usedArmourPixelsCount;
    public int usedEnginePixelsCount;
    public int usedPowerPixelsCount;
    public int usedHardpointPixelsCount;
    public int usedWeaponPixelsCount;

    //The user input coordinates, positions, and window visibility checks.
    public Vector2 inputCoordinate;
    public Vector2 inputCoordinate_previewOffset;
    public bool inputIsInBuildWindow;
    public bool inputCoordinateIsVisibleInBuildWindow; //Check the offset window, so the player can place pixel right up against the edge despite any preview offsets.

    //Pixel placement is allowed.
    public bool pixelPlacement;

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
        coreSpriteVariant = Random.Range(0, game.sprCore.Length);
        corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, game.sprCore[coreSpriteVariant]);
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
        //Placement preview offset (to account for the user's finger getting in the way)
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

        //Get coordinates.
        inputCoordinate = PositionToCoordinates(game.input.inputPosition);
        inputCoordinate_previewOffset = inputCoordinate + previewPixel.coordinatesOffset;

        //Check if the  user input position is within the build window.
        inputIsInBuildWindow = false;
        if (camera.viewBounds_worldSpace.Contains(input.inputPosition))
            inputIsInBuildWindow = true;

        Vector2 offsetInputPosition = CoordinatesToPosition(inputCoordinate_previewOffset);

        //Check if the previewer's offset position is within the build window (calculated in all directions with offsets of 0.5, to account for the visible pixel edge, rather than it's centre).
        inputCoordinateIsVisibleInBuildWindow = false;
        if ((offsetInputPosition.x + 0.5f) > camera.viewBounds_worldSpace.x && (offsetInputPosition.x - 0.5f) < (camera.viewBounds_worldSpace.x + camera.viewBounds_worldSpace.width)
            && (offsetInputPosition.y + 0.5f) > camera.viewBounds_worldSpace.y && (offsetInputPosition.y - 0.5f) < (camera.viewBounds_worldSpace.y + camera.viewBounds_worldSpace.height))
        {
            inputCoordinateIsVisibleInBuildWindow = true;
        }

        //CAMERA MOVEMENT

        //If user touches two fingers in the build window...
        if (inputIsInBuildWindow && (input.stateChange == Input_Manager.StateChange.NoneToTwo || input.stateChange == Input_Manager.StateChange.OneToTwo))
            camera.UserMovementEnabled = true; //Let the camera be controlled.

        //If user stops a two-finger touch anywhere...
        if (input.stateChange == Input_Manager.StateChange.TwoToNone || input.stateChange == Input_Manager.StateChange.TwoToOne)
            camera.UserMovementEnabled = false; //Stop the camera from being controlled.

        // TOUCH BEGIN

        //If the user begins a single touch in the build window...
        if (inputIsInBuildWindow && input.stateChange == Input_Manager.StateChange.NoneToOne)
        {


            //If no tool is active...
            if (tools.currentTool == Tools.Tool.None)
            {
                Vector2 touchedPixelCoordinates = PositionToCoordinates(game.input.inputPosition);
                int touchedPixelIndex = (int)(touchedPixelCoordinates.y * game.shipArraySqrRootLength + touchedPixelCoordinates.x);
                ShipBuilder_PixelBehavior touchedPixel = pixels[touchedPixelIndex];

                //If the user has touched a pixel...
                if (touchedPixel != null)
                {
                    //If the user has touched down on a turret...
                    if (touchedPixel.turret != null)
                    {
                        turretEditTarget = touchedPixel.turret;
                        tools.ChangeTool(Tools.Tool.TurretEditor);
                    }

                    //If the user has touched down on the core pixel...
                    if (touchedPixel.type == Pixel.Type.Core)
                    {
                        corePixel.Destroy();
                        tools.ChangeTool(Tools.Tool.CoreMover);

                        //Pixel placement can commence.
                        pixelPlacement = true;
                    }
                }
            }
            else
            {
                //Pixel placement can commence.
                pixelPlacement = true;
            }
        }

        //TOUCH CANCEL

        //If the user cancels a single touch, or ends it outside the build window...
        if ((!inputCoordinateIsVisibleInBuildWindow && input.stateChange == Input_Manager.StateChange.OneToNone) || input.stateChange == Input_Manager.StateChange.OneToTwo)
        {

            //If in the core movement tool, exit it here.
            if (tools.currentTool == Tools.Tool.CoreMover)
            {
                corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, previewPixel.spriteRenderer.sprite);
                tools.ChangeTool(Tools.Tool.None);
            }

            //If in the core movement tool, exit it here.
            if (tools.currentTool == Tools.Tool.TurretEditor)
            {

                tools.ChangeTool(Tools.Tool.None);
            }

            pixelPlacement = false;
        }

        //TOUCH END

        //If the user ends a single touch within the build window...
        if (inputCoordinateIsVisibleInBuildWindow && input.stateChange == Input_Manager.StateChange.OneToNone)
        {

            if (tools.currentTool != Tools.Tool.None)
            {
                //If in the core movement tool, exit it here.
                if (tools.currentTool == Tools.Tool.CoreMover)
                {
                    coreCoordinates = previewPixel.coordinates;
                    corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, previewPixel.spriteRenderer.sprite);
                    tools.ChangeTool(Tools.Tool.None);
                }
                //If in the core movement tool, exit it here.
                else if (tools.currentTool == Tools.Tool.TurretEditor)
                {

                    tools.ChangeTool(Tools.Tool.None);
                }
                else
                {
                    if (pixelPlacement)
                        PlacePixel();
                }

            }

            pixelPlacement = false;
        }


        // PREVIEW PIXEL PLACEMENT

        if (pixelPlacement)
        {
            //Placement preview sprite rendering.
            previewPixel.sortingLayer = "PreviewPixel";
            if (tools.currentTool == Tools.Tool.ArmourPlacer)
                previewPixel.spriteRenderer.sprite = game.sprArmour[Random.Range(0, game.sprArmour.Length)];
            if (tools.currentTool == Tools.Tool.CoreMover)
            {
                previewPixel.sortingLayer = "CorePixel";
                previewPixel.spriteRenderer.sprite = game.sprCore[Random.Range(0, game.sprCore.Length)];
            }
            if (tools.currentTool == Tools.Tool.EnginePlacer)
                previewPixel.spriteRenderer.sprite = game.sprEngine[Random.Range(0, game.sprEngine.Length)];
            if (tools.currentTool == Tools.Tool.Eraser)
            {
                previewPixel.sortingLayer = "Eraser";
                previewPixel.spriteRenderer.sprite = game.sprEraser;
            }
            if (tools.currentTool == Tools.Tool.HardpointPlacer)
                previewPixel.spriteRenderer.sprite = game.sprHardpoint[Random.Range(0, game.sprHardpoint.Length)];
            if (tools.currentTool == Tools.Tool.PowerPlacer)
                previewPixel.spriteRenderer.sprite = game.sprPower[Random.Range(0, game.sprPower.Length)];
            if (tools.currentTool == Tools.Tool.ScrapPlacer)
                previewPixel.spriteRenderer.sprite = game.sprScrap[Random.Range(0, game.sprScrap.Length)];
            if (tools.currentTool == Tools.Tool.TurretPlacer)
            {
                previewPixel.sortingLayer = "PreviewTurret";
                previewPixel.spriteRenderer.sprite = game.sprTurrets[tools.currentTurretType_index];
            }

            previewPixel.visible = true;


            //Placement preview position.
            previewPixel.coordinates = inputCoordinate_previewOffset;

            if (previewPixel.coordinates.x > (game.shipArraySqrRootLength - 1))
                previewPixel.coordinates.x = (game.shipArraySqrRootLength - 1);
            if (previewPixel.coordinates.x < 0)
                previewPixel.coordinates.x = 0;
            if (previewPixel.coordinates.y > (game.shipArraySqrRootLength - 1))
                previewPixel.coordinates.y = (game.shipArraySqrRootLength - 1);
            if (previewPixel.coordinates.y < 0)
                previewPixel.coordinates.y = 0;

            previewPixel.transform.position = CoordinatesToPosition(previewPixel.coordinates);

            //Placement preview selection...
            previewPixel.selectedPixelPrev = previewPixel.selectedPixel;
            previewPixel.selectedPixel = pixels[(int)((previewPixel.coordinates.y * game.shipArraySqrRootLength) + previewPixel.coordinates.x)];

            //Placement selection hiding.
            if (previewPixel.selectedPixel != null && previewPixel.selectedPixel.type == Pixel.Type.Core)
                previewPixel.visible = false;
            else
                previewPixel.visible = true;

        }
        else
        {
            previewPixel.spriteRenderer.sprite = null;
            previewPixel.visible = false;

            //Show any pixels hidden by the previewer.
            if (previewPixel.selectedPixel != null)
                previewPixel.selectedPixel.visible = true;

            if (previewPixel.selectedPixel != null && previewPixel.selectedPixel.turret != null)
                previewPixel.selectedPixel.turret.visible = true;
        }

        //Show any pixels hidden by the previewer when they are no longer selected.
        if (previewPixel.selectedPixelPrev != previewPixel.selectedPixel)
        {
            if (previewPixel.selectedPixelPrev != null)
            {
                previewPixel.selectedPixelPrev.visible = true;

                if (previewPixel.selectedPixelPrev.turret != null)
                    previewPixel.selectedPixelPrev.turret.visible = true;

            }
        }

        //Hide pixels underneath the preview.
        if (previewPixel.visible == true && tools.currentTool != Tools.Tool.Eraser)
        {
            if (tools.currentTool != Tools.Tool.TurretPlacer)
            {
                if (previewPixel.selectedPixel != null)
                {
                    previewPixel.selectedPixel.visible = false;
                    if (previewPixel.selectedPixel.turret != null)
                        previewPixel.selectedPixel.turret.visible = false;
                }
                else
                {
                    if (previewPixel.selectedPixel != null && previewPixel.selectedPixel.turret != null)
                        previewPixel.selectedPixel.turret.visible = false;
                }
            }
            else
            {
                if (previewPixel.selectedPixel != null)
                {
                    previewPixel.selectedPixel.visible = true;

                    if (previewPixel.selectedPixel.turret != null)
                        previewPixel.selectedPixel.turret.visible = true;

                }
            }
        }

        // TURRET EDITING

        if (tools.currentTool == Tools.Tool.TurretEditor)
        {
            Vector2 turretToTouch = (input.inputPosition - (Vector2)turretEditTarget.transform.position);
            //Acute angle.
            if (input.inputPosition.x <= turretEditTarget.transform.position.x)
                turretEditTarget.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.Angle(new Vector2(0f, 1f), turretToTouch.normalized));
            else
                turretEditTarget.transform.rotation = Quaternion.Euler(0f, 0f, 360f - Vector2.Angle(new Vector2(0f, 1f), turretToTouch.normalized));
        }
    }

    //Place a pixel.
    void PlacePixel()
    {
        if (tools.currentTool == Tools.Tool.ArmourPlacer && PlaythroughData.armourPixels > usedArmourPixelsCount)
            BuildPixel(Pixel.Type.Armour, previewPixel.coordinates, previewPixel.spriteRenderer.sprite);
        else if (tools.currentTool == Tools.Tool.EnginePlacer && PlaythroughData.enginePixels > usedEnginePixelsCount)
            BuildPixel(Pixel.Type.Engine, previewPixel.coordinates, previewPixel.spriteRenderer.sprite);
        else if (tools.currentTool == Tools.Tool.HardpointPlacer && PlaythroughData.hardpointPixels > usedHardpointPixelsCount)
            BuildPixel(Pixel.Type.Hardpoint, previewPixel.coordinates, previewPixel.spriteRenderer.sprite);
        else if (tools.currentTool == Tools.Tool.Eraser)
            UnBuildPixel(previewPixel.selectedPixel);
        else if (tools.currentTool == Tools.Tool.PowerPlacer && PlaythroughData.powerPixels > usedPowerPixelsCount)
            BuildPixel(Pixel.Type.Power, previewPixel.coordinates, previewPixel.spriteRenderer.sprite);
        else if (tools.currentTool == Tools.Tool.ScrapPlacer && PlaythroughData.scrapPixels > usedScrapPixelsCount)
            BuildPixel(Pixel.Type.Scrap, previewPixel.coordinates, previewPixel.spriteRenderer.sprite);

        else if (tools.currentTool == Tools.Tool.TurretPlacer)
        {
            if (previewPixel.selectedPixel != null && previewPixel.selectedPixel.type == Pixel.Type.Hardpoint)
            {
                turretEditTarget = BuildTurret(tools.currentTurretType, previewPixel.selectedPixel, previewPixel.spriteVariantIndex);
            }
        }
        else if (tools.currentTool == Tools.Tool.CoreMover)
        {
            //Unbuild the core pixel, rebuild it elsewhere.
            UnBuildPixel(corePixel);
            corePixel = BuildPixel(Pixel.Type.Core, previewPixel.coordinates, previewPixel.spriteRenderer.sprite);
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
    public ShipBuilder_PixelBehavior BuildPixel(Pixel.Type _type, Vector2 _coordinates, Sprite _sprite)
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
        pixel.Init(index, _type, _coordinates, _sprite);

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

    //Clear the grid and reset the core pixel (called by UI).
    public void ToolSelect_ClearGrid()
    {
        if (input.state == Input_Manager.State.None)
        {
            ClearGrid();
            coreCoordinates = new Vector2(game.shipArraySqrRootLength * 0.5f - 1f, game.shipArraySqrRootLength * 0.5f - 1f); //Calculate centre coordinates.
            corePixel = BuildPixel(Pixel.Type.Core, coreCoordinates, game.sprCore[coreSpriteVariant]);
        }
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

        ClearGrid();

        //Iterate through the pixels
        for (int index = 0; index < PlaythroughData.savedPixels.Length; index++)
        {
            CompressedPixelData savedPixel = PlaythroughData.savedPixels[index];
            if (savedPixel != null)
            {
                //Generate pixel based on data.
                Sprite loadedSprite = null;
                if (savedPixel.pixelType == Pixel.Type.Armour)
                    loadedSprite = game.sprArmour[savedPixel.spriteVariantIndex];
                if (savedPixel.pixelType == Pixel.Type.Core)
                    loadedSprite = game.sprCore[savedPixel.spriteVariantIndex];
                if (savedPixel.pixelType == Pixel.Type.Engine)
                    loadedSprite = game.sprEngine[savedPixel.spriteVariantIndex];
                if (savedPixel.pixelType == Pixel.Type.Hardpoint)
                    loadedSprite = game.sprHardpoint[savedPixel.spriteVariantIndex];
                if (savedPixel.pixelType == Pixel.Type.Power)
                    loadedSprite = game.sprPower[savedPixel.spriteVariantIndex];
                if (savedPixel.pixelType == Pixel.Type.Scrap)
                    loadedSprite = game.sprScrap[savedPixel.spriteVariantIndex];

                ShipBuilder_PixelBehavior pixel = BuildPixel(savedPixel.pixelType, savedPixel.coordinates, loadedSprite);

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

    void ClearGrid()
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
                    userInterface.button_selected.enabled = false;
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
                    userInterface.button_selected.enabled = false;
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
