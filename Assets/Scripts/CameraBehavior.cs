using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraBehavior : MonoBehaviour
{

    /// <summary>
    /// Class describing generalised camera constrained pan & zoom behvaior.
    /// 
    /// Camera always assumes that the scene starts with (0, 0) in the bottom-left corner, and a top-right corner at sceneDimensions.
    /// </summary>

    //General references.
    public Game_Manager game;
    public Camera camera;
    public Input_Manager input;

    //Activate or deactivate zoom & pan behavior.
    public bool UserMovementEnabled = true;

    //UI references.
    public Canvas UICanvas;
    private CanvasScaler UICanvasScaler;

    //Account for different 16:9 resolutions.
    public Vector2 screenScale
    {
        get { return new Vector2(UICanvasScaler.referenceResolution.x / (float)Screen.width, UICanvasScaler.referenceResolution.y / (float)Screen.height); } 
    }

    //View boundaries in screen-space (If unassigned, assume edges of camera view).
    public RectTransform viewBounds;

    //View boundaries in world-space.
    public Vector2 viewBounds_TR;
    public Vector2 viewBounds_BL;
    public Vector2 viewBounds_BL_toPan;
    public Vector2 viewBounds_TR_toPan;

    //Variables used for camera calculations.
    Vector2 viewWindowDimensions_normalised;
    Vector2 viewWindowDimensions_worldSpace;

    public Rect viewBounds_worldSpace; //Final calculated Rect at end of update. (for accurate window positions).

    //Scene dimensions in world-space.
    public Vector2 sceneDimensions = DefaultValues.DEFAULT_SCENE_DIMENSIONS;

    //Zoom parameters (zoom is equivilant to half the world space height of the camera view).
    public float zoom = 5f;
    public float zoomSensitivity = 2f;
    public float zoomMax;

    public Vector2 zoomFocus; //Zoom relative to this position (used for pinch-to-zoom e.t.c).
    public Transform zoomFocusObj; //Object transform for zoom to focus on.

    //Pan parameters.
    public Vector2 pan;
    public float panSensitivity = 1f;

    //Desired reference dimensions (independant of device scaling)
    public Vector2 screenDimensions;
    public float aspectRatio;

    //Initialise the camera behvaior.
    public void Init()
    {
        //Get general references.
        game = Game_Manager.instance;
        input = game.input;
        camera = GetComponent<Camera>();

        //Default behavior is that user can't control the camera movement.
        UserMovementEnabled = false;

        //Get the UI Canvas & Scaler.
        UICanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        UICanvasScaler = UICanvas.GetComponent<CanvasScaler>();
        UICanvasScaler.referenceResolution = DefaultValues.DEFAULT_TARGET_RESOLUTION;

        //Calculate the aspect ratio
        aspectRatio = DefaultValues.DEFAULT_TARGET_RESOLUTION.x / DefaultValues.DEFAULT_TARGET_RESOLUTION.y;

        //Calculate the initial view boundaires (in-case any other initialisers rely on its values).
        CalculateViewBounds();
    }

    //Update the camera.
    public void OnUpdate()
    {

        //Choose the centre of zooming.
        if (zoomFocusObj == null)
            zoomFocus = input.inputPosition;
        else
            zoomFocus = zoomFocusObj.position;

        //Update zoom based on input & conditions.
        float zoomIncr;
        if (UserMovementEnabled)
            zoomIncr =  input.inputSpread * zoom * zoomSensitivity;
        else
            zoomIncr = 0f;

        zoom += zoomIncr; //Increment zoom.

        //Calculate the constraining edges of the view, to calculate the maximum zoom.
        CalculateViewBounds();
        
        //Calculate zoomMax.
        if (viewWindowDimensions_worldSpace.y < viewWindowDimensions_worldSpace.x)
            zoomMax = (sceneDimensions.x - 0f) / (aspectRatio * 2f * viewWindowDimensions_normalised.x);
        else
            zoomMax = (sceneDimensions.y - 0f) / (2f * viewWindowDimensions_normalised.y);

        if (zoom > zoomMax)
            zoom = zoomMax;

        //Update pan based on input
        Vector2 draggedPanIncr = input.inputDrag * zoom * panSensitivity;
        Vector2 zoomFocusPanIncr = (zoomFocus - (Vector2)transform.position) * (input.inputSpread * zoomSensitivity);

        Vector2 panIncr;

        if (UserMovementEnabled)
        {
            if (zoom != zoomMax)
                panIncr = draggedPanIncr + zoomFocusPanIncr;
            else
                panIncr = draggedPanIncr;
        }
        else
            panIncr = Vector2.zero;

        pan -= panIncr; //Increment pan.

        //Calculate the constraining edges of the view, this time to constrain zoom & pan.
        CalculateViewBounds();

        //Get the worldspace vectors to describe pan in terms of the view boundary corners.
        viewBounds_BL_toPan = pan - viewBounds_BL;
        viewBounds_TR_toPan = pan - viewBounds_TR;

        //Pan so that the bottom left corners line up.
        if (viewBounds_BL.x < 0f)
            pan.x = viewBounds_BL_toPan.x; 
        if (viewBounds_BL.y < 0f)
            pan.y = viewBounds_BL_toPan.y;

        //Pan so that the top right corners line up.
        if (pan.x - viewBounds_TR_toPan.x > sceneDimensions.x)
            pan.x = sceneDimensions.x + viewBounds_TR_toPan.x;

        if (pan.y - viewBounds_TR_toPan.y > sceneDimensions.y)
            pan.y = sceneDimensions.y + viewBounds_TR_toPan.y;

        //Update the transforms accordingly.
        camera.orthographicSize = zoom;
        transform.position = new Vector3(pan.x, pan.y, transform.position.z);

        //Calculate after final zoom & pan values have been set. Set a rect to contain final bounds, for use in other scripts.
        CalculateViewBounds();
        viewBounds_worldSpace = new Rect(viewBounds_BL.x, viewBounds_BL.y, (viewBounds_TR.x - viewBounds_BL.x), (viewBounds_TR.y - viewBounds_BL.y));
    }

    //Custom screen-to-world function, allowing for calculation before updating camera transforms (otherwise output is off by one frame, and the view fights through corners)
    public Vector2 ScreenToWorldPosition(Vector2 position)
    {
        Vector2 normalisedPosition;

        normalisedPosition.x = (100f / (float)Screen.width) * position.x;
        normalisedPosition.y = (100f / (float)Screen.height) * position.y;
        normalisedPosition /= 100f;

        Vector2 worldScreenDimensions = new Vector2(aspectRatio * zoom * 2f, zoom * 2f);

        Vector2 positionRelativetoViewOrigin = new Vector2(worldScreenDimensions.x * normalisedPosition.x, worldScreenDimensions.y * normalisedPosition.y);

        return positionRelativetoViewOrigin + (pan - (worldScreenDimensions * 0.5f));

    }

    /*Calculate the world-space view boundary positions.
    (view boundaries are constrained by a defined viewBounds RectTransform.
    'Null' viewBounds will default these boundaries to those of the camera's edges.*/
    public void CalculateViewBounds()
    {
        //Define the constraining edges of the view.
        if (viewBounds != null)
        {
            //If there is a defined viewBounds RectTransform.
            viewBounds_BL.x = ScreenToWorldPosition(viewBounds.anchoredPosition / screenScale.x).x;
            viewBounds_BL.y = ScreenToWorldPosition(viewBounds.anchoredPosition / screenScale.y).y;
            viewBounds_TR.x = ScreenToWorldPosition((viewBounds.anchoredPosition + new Vector2(viewBounds.rect.width, viewBounds.rect.height)) / screenScale.x).x;
            viewBounds_TR.y = ScreenToWorldPosition((viewBounds.anchoredPosition + new Vector2(viewBounds.rect.width, viewBounds.rect.height)) / screenScale.y).y;

            //Calculate the view dimensions in worldspace as a function of zoom (Required for calculating the maximum zoom).
            viewWindowDimensions_normalised.x = viewBounds.rect.width * ((100 / DefaultValues.DEFAULT_TARGET_RESOLUTION.x)) * 0.01f;
            viewWindowDimensions_normalised.y = viewBounds.rect.height * ((100 / DefaultValues.DEFAULT_TARGET_RESOLUTION.y)) * 0.01f;

            viewWindowDimensions_worldSpace.x = zoom * aspectRatio * 2f * viewWindowDimensions_normalised.x;
            viewWindowDimensions_worldSpace.y = zoom * 2f * viewWindowDimensions_normalised.y;
        }
        else
        {
            //If there isn't, just use the camera edges.
            viewBounds_BL.x = ScreenToWorldPosition(Vector2.zero / screenScale.x).x;
            viewBounds_BL.y = ScreenToWorldPosition(Vector2.zero / screenScale.y).y;
            viewBounds_TR.x = ScreenToWorldPosition(DefaultValues.DEFAULT_TARGET_RESOLUTION / screenScale.x).x;
            viewBounds_TR.y = ScreenToWorldPosition(DefaultValues.DEFAULT_TARGET_RESOLUTION / screenScale.y).y;

            //Set the view dimensions in worldspace as a function of zoom.
            viewWindowDimensions_normalised = new Vector2(1f, 1f);
            viewWindowDimensions_worldSpace.x = zoom * aspectRatio * 2f;
            viewWindowDimensions_worldSpace.y = zoom * 2f;
        }

    }    

}
