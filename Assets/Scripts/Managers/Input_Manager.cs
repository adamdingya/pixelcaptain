using UnityEngine;
using System.Collections;

public class Input_Manager : MonoBehaviour
{

    /// <summary>
    /// Manage multi-platform inputs.
    /// Holds data about touch positions, two-finger spreading, two-finger dragging e.t.c.
    /// </summary>

    //General references.
    Game_Manager game;
    CameraBehavior camera;

    //Define possible input states & store previous.
    public enum State { None, One, Two };
    public State state;
    public State statePrev;

    public Vector2 inputPosition; //The selected position in world-space (Averaged for two touches).
    public Vector2 inputPositionScreen; //The selected position in screen-space (Averaged for two touches).

    public Vector2 inputDrag; //The amount of two-finger dragging.
    public float inputSpread; //The amount of two-finger spreading.

    float touchAngle; //The current angle between finger movements.
    public float spreadAngleThreshold; //The angle between finger movements that defines spreading from dragging.

    //Custom monitoring of touch position deltas (Overcomes an issue I found with built-in touch[0].deltaPosition).
    int firstTouchID;
    int firstTouchIDPrev;
    Vector2 firstTouchPos;
    Vector2 firstTouchPosPrev;
    Vector2 firstTouchPosDelta;
    int secondTouchID;
    int secondTouchIDPrev;
    Vector2 secondTouchPos;
    Vector2 secondTouchPosPrev;
    Vector2 secondTouchPosDelta;

    //Initialise the input manager, assigning references.
    public void Init()
    {
        game = Game_Manager.instance;
        spreadAngleThreshold = DefaultValues.DEFAULT_SPREAD_ANGLE_THRESHOLD;
    }

    //Calculate the input data for this frame.
    public void GetInput()
    {
        //Store touches to an array and count relevent ones with 'touchCount'.
        Touch[] touches = Input.touches;

        int touchCount = 0;
        foreach (Touch touch in touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                touchCount++;
        }

        //If there are no touches or more than two, cancel and reset.
        if (touchCount == 0 || touchCount > 2)
        {
            statePrev = state;
            state = State.None;
            inputDrag = Vector2.zero;
            inputSpread = 0f;
            touchAngle = 0f;
        }

        //If there is one touch...
        if (touchCount == 1)
        {
            //If the previous amount of touches was different...
            if (state != State.One)
            {
                //Sample the current frame data, ensure that the previous frame data is the same (avoids jumping in deltas calculated from 'current - previous').
                firstTouchID = touches[0].fingerId;
                firstTouchIDPrev = firstTouchID;
                firstTouchPos = touches[0].position;
                firstTouchPosPrev = firstTouchPos;
            }
            //Else if this is a continuation of one touch...
            else
            {
                //Sample the previous frame data, then set the current.
                firstTouchIDPrev = firstTouchID;
                firstTouchID = touches[0].fingerId;
                firstTouchPosPrev = firstTouchPos;
                firstTouchPos = touches[0].position;
            }

            //Calculate the movement delta, but check that the induvidual fingers are still the same/haven't been swapped.
            if (firstTouchID == firstTouchIDPrev && state == statePrev)
                firstTouchPosDelta = firstTouchPos - firstTouchPosPrev;
            else
                firstTouchPosDelta = Vector2.zero;

            //Set the state & input position.
            statePrev = state;
            state = State.One;
            inputPositionScreen = firstTouchPos;
            inputPosition = game.camera.ScreenToWorldPosition(inputPositionScreen);

            //Reset anything from two touch states.
            secondTouchPos = Vector2.zero;
            secondTouchPosPrev = Vector2.zero;
            secondTouchPosDelta = Vector2.zero;
            inputDrag = Vector2.zero;
            inputSpread = 0f;
        }

        //If there are two touches...
        if (touchCount == 2)
        {
            //If the previous amount of unique touches was different...
            if (touches[0].phase == TouchPhase.Began || state != State.Two)
            {
                //Sample the current frame data, ensure that the previous frame data is the same (avoids jumping in deltas calculated from 'current - previous').
                firstTouchID = touches[0].fingerId;
                firstTouchIDPrev = firstTouchID;
                firstTouchPos = touches[0].position;
                firstTouchPosPrev = firstTouchPos;
            }
            //Else if this is a continuation the same two touches...
            else
            {
                //Sample the previous frame data, then set the current.
                firstTouchIDPrev = firstTouchID;
                firstTouchID = touches[0].fingerId;
                firstTouchPosPrev = firstTouchPos;
                firstTouchPos = touches[0].position;
            }

            //Calculate the movement delta, but check that the induvidual fingers are still the same/haven't been swapped.
            if (firstTouchID == firstTouchIDPrev && state == statePrev)
                firstTouchPosDelta = firstTouchPos - firstTouchPosPrev;
            else
                firstTouchPosDelta = Vector2.zero;

            //If the previous amount of unique touches was different...
            if (touches[1].phase == TouchPhase.Began || state != State.Two)
            {
                //Sample the current frame data, ensure that the previous frame data is the same (avoids jumping in deltas calculated from 'current - previous').
                secondTouchID = touches[1].fingerId;
                secondTouchIDPrev = secondTouchID;
                secondTouchPos = touches[1].position;
                secondTouchPosPrev = secondTouchPos;
            }
            //Else if this is a continuation the same two touches...
            else
            {
                //Sample the previous frame data, then set the current.
                secondTouchIDPrev = secondTouchID;
                secondTouchID = touches[1].fingerId;
                secondTouchPosPrev = secondTouchPos;
                secondTouchPos = touches[1].position;
            }

            //Calculate the movement delta, but check that the induvidual fingers are still the same/haven't been swapped.
            if (secondTouchID == secondTouchIDPrev && state == statePrev && state != State.None)
                secondTouchPosDelta = secondTouchPos - secondTouchPosPrev;
            else
                secondTouchPosDelta = Vector2.zero;

            //Set the state & input position.
            statePrev = state;
            state = State.Two;
            inputPositionScreen = (firstTouchPos + secondTouchPos) / 2.0f;
            inputPosition = (game.camera.ScreenToWorldPosition(firstTouchPos) + game.camera.ScreenToWorldPosition(secondTouchPos)) / 2.0f;

            //Calculate the current angle between finger movements.
            if (firstTouchPosDelta.sqrMagnitude > 0f && secondTouchPosDelta.sqrMagnitude > 0f)
                    touchAngle = Mathf.Abs(Vector2.Angle(firstTouchPosDelta, secondTouchPosDelta));
                else
                    touchAngle = 0f;

            //Switch between spread/drag based on the finger-spreading angle.
            if (touchAngle > spreadAngleThreshold)
            {
                float touchSpreadDistancePrevious = ((firstTouchPos - firstTouchPosDelta) - (secondTouchPos - secondTouchPosDelta)).magnitude;
                float touchSpreadDistance = (firstTouchPos - secondTouchPos).magnitude;
                inputSpread = (touchSpreadDistancePrevious - touchSpreadDistance) * 0.5f * (1/(float)Screen.height);
                inputDrag = Vector2.zero;
            }
            else
            {
                inputDrag = firstTouchPosDelta + secondTouchPosDelta / 2.0f;
                inputDrag = new Vector2((inputDrag.x / (float)Screen.width) * game.camera.aspectRatio, inputDrag.y / (float)Screen.height);
                inputSpread = 0f;
            }
        }
    }


    //Non-mobile OS input (for testing).
    public Vector2 mousePos;
    public Vector2 mousePosPrev;
    public Vector2 mousePosDelta;
    public void GetInputPC()
    {
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            statePrev = state;
            state = State.None;
        }
        else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            statePrev = state;
            state = State.One;
        }
        else if (!Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            statePrev = state;
            state = State.Two;
        }

        mousePosPrev = mousePos;
        mousePos = Input.mousePosition;

        if (state == statePrev && state != State.None)
            mousePosDelta = mousePos - mousePosPrev;
        else
            mousePosDelta = Vector2.zero;

        if (state == State.Two)
        {
            inputDrag = mousePosDelta;
            inputDrag = new Vector2((inputDrag.x / (float)Screen.width) * game.camera.aspectRatio, inputDrag.y / (float)Screen.height) * 2f;
            inputSpread = -Input.mouseScrollDelta.y * 0.04f;
        }
        else
        {
            inputDrag = Vector2.zero;
            inputSpread = 0f;
        }

        inputPositionScreen = mousePos;
        inputPosition = game.camera.ScreenToWorldPosition(inputPositionScreen);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(inputPosition, 1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(inputPositionScreen, 0.8f);
    }
    
}
