using UnityEngine;
using System.Collections;

// Last edited : Mat Hill 21/12/2015

public class ShipBuilderGrid : MonoBehaviour
{

    /// <summary>
    /// This class stores the pixel grid data, and manages movement and editing.
    /// </summary>

    GameManager game;
    InputManager input;
    ShipBuilderManager shipBuilder;

    //States
    public enum State { PanZoom, Placing, Inactive }; //Define local interaction types.
    public State state = State.Inactive;
    public State statePrev = State.Inactive;

    //Panning and Zooming
    public float zoomFactor = 1f; //The grid zoom (scale) factor.
    public float zoomMax = 5f;
    float zoomSpeed = 0.1f;
    public Vector2 panVector = Vector2.zero;

    //Pixel Grid
    public ShipBuilderPixel[] pixelsArray; //Store all pixelsArray as an array.
    public int pixelsArrayLength; //Length of the bulder grid starting from 1.

    //Grid Window Region (World Space)
    public GameObject gridWindowEditor;
    public Vector2 gridWindowPosition;
    public Vector2 gridWindowDimensions;
    public Rect gridWindowRegion;

    //Block Placing Previewing
    public bool touchingGrid = false;
    public Vector2 currentCoordinates = Vector2.zero;
    public ShipBuilderPixel currentPixel = null;
    public ShipBuilderPreviewPixel preview;
    Vector2 hoverOffset;

    #region Converters
    //Get the coordinate containing a world-space position (cheaper than collision detecting the mouse).
    public Vector2 PositionToCoordinate(Vector2 position)
    {
        Vector2 coordinate;
        coordinate.x = (position.x / 1f) - (position.x % 1f);
        coordinate.y = (position.y / 1f) - (position.y % 1f);

        if (coordinate.x > (pixelsArrayLength - 1))
            coordinate.x = (pixelsArrayLength - 1);
        if (coordinate.x < 0)
            coordinate.x = 0;
        if (coordinate.y > (pixelsArrayLength - 1))
            coordinate.y = (pixelsArrayLength - 1);
        if (coordinate.y < 0)
            coordinate.y = 0;

        return coordinate;
    }

    //Return a position on the grid for any given coordinate
    public Vector2 CoordinateToPosition(Vector2 coordinate)
    {
        Vector2 position;

        position = coordinate + new Vector2(0.5f, 0.5f);
        return position * zoomFactor + new Vector2(transform.position.x, transform.position.y);
    }

    //Return a world space position relative to the grid
    public Vector2 RelativePosition(Vector2 position)
    {
        Vector2 relativePos = position;
        relativePos -= new Vector2(transform.position.x, transform.position.y);
        relativePos *= 1 / zoomFactor;

        return relativePos;
    }
    public Vector2 RelativePosition(Vector3 position)
    {
        Vector2 relativePos = new Vector2(position.x, position.y);
        relativePos -= new Vector2(transform.position.x, transform.position.y);
        relativePos *= 1 / zoomFactor;

        return relativePos;
    }

    //Return the array index value of any given coordinate
    public int CoordinateToIndex(Vector2 coordinate)
    {
        return (int)((coordinate.y * pixelsArrayLength) + coordinate.x);
    }
    #endregion

    public void Init(GameManager _game)
    {
        game = _game;
        pixelsArrayLength = game.gridSize;
        shipBuilder = game.shipBuilder; //Pass in the ship builder manager.
        input = shipBuilder.game.input;

        //Setup the grid window area based on the temporary gameobject.
        gridWindowEditor = GameObject.Find("GridWindowEditor");
        gridWindowPosition.x = gridWindowEditor.transform.position.x;
        gridWindowPosition.y = gridWindowEditor.transform.position.y;
        gridWindowDimensions.x = gridWindowEditor.transform.localScale.x;
        gridWindowDimensions.y = gridWindowEditor.transform.localScale.y;
        gridWindowRegion = new Rect(gridWindowPosition.x, gridWindowPosition.y, gridWindowDimensions.x, gridWindowDimensions.y);
        gridWindowEditor.transform.FindChild("GridWindowFrame").transform.parent = shipBuilder.transform;
        GameObject.Destroy(gridWindowEditor);

        //Pixel grid array and area.
        pixelsArray = new ShipBuilderPixel[pixelsArrayLength * pixelsArrayLength];

        //Pan the grid so it is centered.
        Vector2 initPan;
        initPan.x = (0.5f * -pixelsArrayLength * zoomFactor) + (gridWindowDimensions.x * 0.5f);
        initPan.y = (0.5f * -pixelsArrayLength * zoomFactor) + (gridWindowDimensions.y * 0.5f);
        panVector = initPan;

        //Initialise the placement previewer.
        preview = GameObject.Find("Preview").GetComponent<ShipBuilderPreviewPixel>();
        preview.Init();
    }

    public void OnUpdate()
    {
        
        touchingGrid = (new Rect(0f, 0f, gridWindowDimensions.x, gridWindowDimensions.y).Contains(input.touch1Pos));

        //Sample the state before updating
        statePrev = state;

        //Enter No Touches
        if (input.state == InputManager.State.None && input.statePrev != InputManager.State.None)
            state = State.Inactive;

        //Enter One Touch
        if (input.state == InputManager.State.One && input.statePrev != InputManager.State.One)
        {
            //If going from two touches to one...
            if (input.statePrev == InputManager.State.Two)
                state = State.Inactive;
            else
            {
                //Can't enter the placing state unless you touch within the grid window.
                state = State.Placing;
            }
        }

        //Enter Two Touches
        if (input.state == InputManager.State.Two && input.statePrev != InputManager.State.Two)
        {
            if (!game.PC_MODE)
            {
                if ((input.statePrev == InputManager.State.Two) || (gridWindowRegion.Contains(input.touch1Pos) && (gridWindowRegion.Contains(input.touch2Pos))))
                    state = State.PanZoom;
            }
            else
            {
                if ((input.statePrev == InputManager.State.Two) || (gridWindowRegion.Contains(input.touch1Pos)))
                    state = State.PanZoom;
            }
            
        }

        //Exit any touches...
        if (input.state == InputManager.State.None && (input.state != input.statePrev))
        {
            state = State.Inactive;

            //If a pixel is selected & player releases single touch whilst in Placing mode...
            if (input.statePrev == InputManager.State.One && statePrev == State.Placing)
            {
                if (gridWindowRegion.Contains(input.touch1Pos + hoverOffset))
                    Place();
            }
        }

        //Reset the previously selected pixel incase it was hidden.
        if (currentPixel != null)
            currentPixel.UpdateVisibility(true);

        preview.UpdateVisibility(false); //Hide the preview incase it was shown perviously.
        if (state == State.Placing)
            Preview(); //Get the new hovered pixel, update accordingly.

        //Pan & Zoom State
        if (state == State.PanZoom)
        {
            if (!game.PC_MODE)
                PanZoom();
            else
                PanZoomPC();
        }
            
        //Stop the grid from being panned away from origin corner.
        if (panVector.x > 0f)
            panVector.x = 0f;
        if (panVector.y > 0f)
            panVector.y = 0f;

        //Stop the grid from being panned away from maximum corner.
        if (Mathf.Abs(panVector.x) >  (zoomFactor * pixelsArrayLength - gridWindowDimensions.x))
            panVector.x = - (zoomFactor * pixelsArrayLength - gridWindowDimensions.x);
        if (Mathf.Abs(panVector.y) > (zoomFactor * pixelsArrayLength - gridWindowDimensions.y))
            panVector.y = - (zoomFactor * pixelsArrayLength - gridWindowDimensions.y);

        transform.position = new Vector3(panVector.x, panVector.y, 0f);
        transform.localScale = new Vector3(zoomFactor, zoomFactor, 0f);
    }

    //Pan and zoom the grid view.
    void PanZoom()
    {
        Vector2 touchPosAverage = Vector2.Lerp(input.touch1Pos, input.touch2Pos, 0.5f);
        Vector2 touchDeltaPosAverage = Vector2.Lerp(input.touch1DeltaPos, input.touch2DeltaPos, 0.5f);

        //Pan
        panVector += (RelativePosition(touchPosAverage) - RelativePosition(touchPosAverage - touchDeltaPosAverage)) * zoomFactor;

        //Zoom
        float zoomIncr = 0f;
        if (input.touch12Angle > 10f)

        {
            float touchDeltaMagPrev = (input.touch1PosPrev - input.touch2PosPrev).magnitude;
            float touchDeltaMag = (input.touch1Pos - input.touch2Pos).magnitude;
            zoomIncr = -(touchDeltaMagPrev - touchDeltaMag) * 0.5f;
        }

        zoomIncr = zoomIncr * zoomFactor * zoomSpeed;

        if (Mathf.Abs(zoomIncr) > 0)
        {
            float zoomMin = (float)Mathf.Min(gridWindowDimensions.x, gridWindowDimensions.y) / (float)pixelsArrayLength;

            if (zoomFactor + zoomIncr < zoomMin)
            {
                float zoomFactorPrev = zoomFactor;
                zoomFactor = zoomMin;
                panVector -= (zoomFactor - zoomFactorPrev) * RelativePosition(touchPosAverage);
            }
            else if (zoomFactor + zoomIncr > zoomMax)
            {
                float zoomFactorPrev = zoomFactor;
                zoomFactor = zoomMax;
                panVector -= (zoomFactor - zoomFactorPrev) * RelativePosition(touchPosAverage);
            }
            else
            {
                float zoomFactorPrev = zoomFactor;
                zoomFactor += zoomIncr;
                panVector -= (zoomFactor - zoomFactorPrev) * RelativePosition(touchPosAverage);
            }
        }
    }

    void PanZoomPC()
    {
        //Pan
        panVector += (RelativePosition(input.touch1Pos) - RelativePosition(input.touch1Pos - input.touch1DeltaPos)) * zoomFactor;

        //Zoom
        float zoomIncr = 0f;
        if (Input.mouseScrollDelta.y != 0)
        {
            zoomIncr = Mathf.Sign(Input.mouseScrollDelta.y);
        }

        zoomIncr = zoomIncr * (zoomSpeed * 5);

        if (Mathf.Abs(zoomIncr) > 0)
        {
            float zoomMin = (float)Mathf.Min(gridWindowDimensions.x, gridWindowDimensions.y) / (float)pixelsArrayLength;

            if (zoomFactor + zoomIncr < zoomMin)
            {
                float zoomFactorPrev = zoomFactor;
                zoomFactor = zoomMin;
                panVector -= (zoomFactor - zoomFactorPrev) * RelativePosition(input.touch1Pos);
            }
            else if (zoomFactor + zoomIncr > zoomMax)
            {
                float zoomFactorPrev = zoomFactor;
                zoomFactor = zoomMax;
                panVector -= (zoomFactor - zoomFactorPrev) * RelativePosition(input.touch1Pos);
            }
            else
            {
                float zoomFactorPrev = zoomFactor;
                zoomFactor += zoomIncr;
                panVector -= (zoomFactor - zoomFactorPrev) * RelativePosition(input.touch1Pos);
            }
        }
    }

    //Preview the tool's placement.
    void Preview()
    {
        //Work out a suitable offset for placing the pixel under the user's finger (aids accessibility with fine placements).
        if (zoomFactor > 1.4f)
            hoverOffset = new Vector2(-zoomFactor, zoomFactor);
        else
            hoverOffset = new Vector2(2.0f * -zoomFactor, 2.0f * zoomFactor);

        //Calculate the coordinates currently selected by the user.
        currentCoordinates = PositionToCoordinate(RelativePosition(input.touch1Pos + hoverOffset));
        //Calculate the pixel currently selected by the user.
        currentPixel = pixelsArray[CoordinateToIndex(currentCoordinates)];

        //If there is a pixel at the placement preview position, hide it.
        if (currentPixel != null)
        {
            //If the user isn't placing turrets...
            if (shipBuilder.UI.tool != ShipBuilderUI.Tool.Turret)
            {
                currentPixel.UpdateVisibility(false); //Hide the pixel.
            }
            //If the user is placing turrets...
            else
            {
                //If there is a turret at the placement preview position, hide it.
                if (currentPixel.turret != null)
                    currentPixel.turret.UpdateVisibility(false); //Hide the turret.
            }
        }


        //Set the preview pixel's sprite to depict the potentiol result...
        if (shipBuilder.UI.tool != ShipBuilderUI.Tool.Turret)
        {
            if (shipBuilder.UI.tool != ShipBuilderUI.Tool.Eraser)
                preview.UpdateSprite(shipBuilder.PixelTypeToSprite(shipBuilder.ToolToPixelType(shipBuilder.UI.tool))); //Show selected tool type.
            else
                preview.UpdateSprite(game.spritePixelEraser);
        }
        else
            preview.UpdateSprite(game.spriteTurret[Random.Range(0, game.spriteTurret.Length - 1)]); //Show selected turret type.

        // Snap the preview pixel's position to the selected pixel's position
        preview.transform.position = CoordinateToPosition(currentCoordinates);

        if (gridWindowRegion.Contains(input.touch1Pos + hoverOffset))
        {
            preview.UpdateVisibility(true); //Show the preview for this frame.
           
        }
    }

    //Place the pixel or turret.
    void Place()
    {
        game.sound.PlaySFX(SoundManager.GameSFX.PlacePixel);

        //If there is a pixel in positon already...
        if (currentPixel != null)
        {
            //If the user isn't placing turrets...
            if (shipBuilder.UI.tool != ShipBuilderUI.Tool.Turret)
            {
                //Delete the selected pixel.
                currentPixel.Delete();
                //If the user intends to replace the deleted pixel, create a new one...
                if (shipBuilder.UI.tool != ShipBuilderUI.Tool.Eraser)
                {
                    GameObject pixelObj = new GameObject();
                    ShipBuilderPixel pixel = pixelObj.AddComponent<ShipBuilderPixel>();
                    pixel.InitPixel(shipBuilder, currentCoordinates, shipBuilder.ToolToPixelType(shipBuilder.UI.tool)); //Initialise the pixel, assigning a reference to this script, and setting its cooridantes;
                }
            }
            //If the user is placing turrets...
            else
            {
                if (currentPixel.type == Pixel.Type.Hardpoint)
                {
                    //Delete the selected turret.
                    if (currentPixel.turret != null)
                        currentPixel.DeleteTurret();
                    //Create a new one.
                    currentPixel.AddTurret();
                    game.sound.PlaySFX(SoundManager.GameSFX.PlacePixel);
                }
            }    
        }
        //If there isn't a pixel in positon already, make a new one...
        else
        {
            //If a pixel Placing tool is active...
            if (shipBuilder.UI.tool != ShipBuilderUI.Tool.Eraser && shipBuilder.UI.tool != ShipBuilderUI.Tool.Turret)
            {
                GameObject pixelObj = new GameObject();
                ShipBuilderPixel pixel = pixelObj.AddComponent<ShipBuilderPixel>();
                pixel.InitPixel(shipBuilder, currentCoordinates, shipBuilder.ToolToPixelType(shipBuilder.UI.tool)); //Initialise the pixel, assigning a reference to this script, and setting its cooridantes;
            }
        }
    }
    
}