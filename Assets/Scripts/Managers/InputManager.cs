using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
	public static InputManager instance = null;

    Camera cam;

    public enum State { None, One, Two };
    public State state;
    public State statePrev;

    public bool touch1Begin = false;
    public bool touch2Begin = false;

    public bool touch1End = false;
    public bool touch2End = false;

    public Vector2 touch1Pos;
    public Vector2 touch1PosPrev;
    public Vector2 touch1DeltaPos;
    public int touch1ID;
    public int touch1IDPrev;

    public Vector2 touch2Pos;
    public Vector2 touch2PosPrev;
    public Vector2 touch2DeltaPos;
    public int touch2ID;
    public int touch2IDPrev;

    public float touch12Angle;

    public Touch[] touches;

	void Start(){
		if (instance == null) {			
			instance = this;	
		} else if (instance != this) {			
			Destroy (gameObject);    
		}
		DontDestroyOnLoad(gameObject);

		init ();
	}

	void Update(){
		if (!GameManager.instance.PC_MODE) {
			getInput (); //Get the user input.
		} else {
			getInputPC (); //Get the user input.
		}
	}

    private void init()
    {
        cam = GameManager.instance.cam;

        touch1Pos = cam.ScreenToWorldPoint(Input.mousePosition);
        touch1PosPrev = touch1Pos;
        touch1DeltaPos = Vector2.zero;

        touch2Pos = touch1Pos;
        touch2PosPrev = touch1Pos;
        touch2DeltaPos = Vector2.zero;
    }

    private void getInputPC()
    {
        //If LBM
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            if (Input.GetMouseButton(0))
            {
                statePrev = state;
                state = State.One;
            }
            else if(Input.GetMouseButton(1))
            {
                statePrev = state;
                state = State.Two;
            }


            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                touch1Begin = true;
                touch1Pos = cam.ScreenToWorldPoint(Input.mousePosition);
                touch1PosPrev = touch1Pos;
            }
            else
            {
                touch1Begin = false;
                touch1PosPrev = touch1Pos;
                touch1Pos = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            touch1DeltaPos = touch1Pos - touch1PosPrev;
        }
        else
        {
            statePrev = state;
            state = State.None;
            touch1DeltaPos = Vector2.zero;
        }
    }

    private void getInput()
    {
        Touch[] touches = Input.touches;

        int fingerCount = 0;
        foreach (Touch touch in touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;
        }

        //One touch
        if (fingerCount == 1)
        {
            Touch t = touches[0];

            statePrev = state;
            state = State.One;

            if (t.phase == TouchPhase.Began)
            {
                touch1Begin = true;
                touch1ID = t.fingerId;
                touch1IDPrev = t.fingerId;
                touch1Pos = cam.ScreenToWorldPoint(t.position);
                touch1PosPrev = touch1Pos;
            }
            else
            {
                touch1Begin = false;
                touch1IDPrev = touch1ID;
                touch1ID = t.fingerId;
                touch1PosPrev = touch1Pos;
                touch1Pos = cam.ScreenToWorldPoint(t.position);
            }

            touch1DeltaPos = touch1Pos - touch1PosPrev;
        }
        //Two touches
        else if (fingerCount == 2)
        {
            Touch t1 = touches[0];
            Touch t2 = touches[1];

            statePrev = state;
            state = State.Two;

            //Finger 1
            if (t1.phase == TouchPhase.Began)
            {
                touch1Begin = true;
                touch1ID = t1.fingerId;
                touch1IDPrev = t1.fingerId;
                touch1Pos = cam.ScreenToWorldPoint(t1.position);
                touch1PosPrev = touch1Pos;
            }
            else
            {
                touch1Begin = false;
                touch1IDPrev = touch1ID;
                touch1ID = t1.fingerId;
                touch1PosPrev = touch1Pos;
                touch1Pos = cam.ScreenToWorldPoint(t1.position);
            }

            if ((touch1ID == touch1IDPrev) && (state == statePrev))
                touch1DeltaPos = touch1Pos - touch1PosPrev;

            //Finger 2
            if (t2.phase == TouchPhase.Began)
            {
                touch2Begin = true;
                touch2ID = t2.fingerId;
                touch2IDPrev = t2.fingerId;
                touch2Pos = cam.ScreenToWorldPoint(t2.position);
                touch2PosPrev = touch2Pos;
            }
            else
            {
                touch2Begin = false;
                touch2IDPrev = touch2ID;
                touch2ID = t2.fingerId;
                touch2PosPrev = touch2Pos;
                touch2Pos = cam.ScreenToWorldPoint(t2.position);
            }

            if ((touch1ID == touch1IDPrev) && (touch2ID == touch2IDPrev) && (state == statePrev))
                touch2DeltaPos = touch2Pos - touch2PosPrev;

            //Calculate the angle between finger movements
            if (touch1DeltaPos.sqrMagnitude > 0f && touch2DeltaPos.sqrMagnitude > 0f)
                touch12Angle = Mathf.Abs(Vector2.Angle(touch1DeltaPos, touch2DeltaPos));
            else
                touch12Angle = 0f;
        }
        //No Touches
        else
        {
            statePrev = state;
            state = State.None;

            touch1DeltaPos = Vector2.zero;
            touch2DeltaPos = Vector2.zero;

            touch12Angle = 0f;
        }
    }
}
