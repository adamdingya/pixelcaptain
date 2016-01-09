using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraBehavior : MonoBehaviour
{

    /// <summary>
    /// Class describing generalised camera constrained pan & zoom behvaior.
    /// </summary>

    //General references.
    public Game_Manager game;
    public Camera camera;

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

    //Scene boundaries in world-space.
    public Vector2 sceneBounds_BL;
    public Vector2 sceneBounds_TR = new Vector2(100f, 100f);

    //Control zoom and pan from another script (allows for creation of two-touch areas that don't initiate camera movement.
    public bool canZoomOrPan = true;

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

    //Initialise the camera behvaior, assigning in references.
    public void Init()
    {
        canZoomOrPan = false;

        game = Game_Manager.instance;
        camera = GetComponent<Camera>();

        UICanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        UICanvasScaler = UICanvas.GetComponent<CanvasScaler>();
        screenDimensions = UICanvasScaler.referenceResolution;
        aspectRatio = screenDimensions.x / screenDimensions.y;

        CalculateViewBounds();
    }

    //Update the camera.
    public void OnUpdate()
    {

        //Choose the centre of zooming.
        if (zoomFocusObj == null)
            zoomFocus = game.input.inputPosition;
        else
            zoomFocus = zoomFocusObj.position;

        //Update zoom based on input & conditions.
        float zoomIncr;
        if (canZoomOrPan)
            zoomIncr =  game.input.inputSpread * zoom * zoomSensitivity;
        else
            zoomIncr = 0f;

        zoom += zoomIncr; //Increment zoom.

        //Calculate the constraining edges of the view.
        CalculateViewBounds();
        
        //Calculate zoomMax.
        if (viewWindowDimensions_worldSpace.y < viewWindowDimensions_worldSpace.x)
            zoomMax = (sceneBounds_TR.x - sceneBounds_BL.x) / (aspectRatio * 2f * viewWindowDimensions_normalised.x);
        else
            zoomMax = (sceneBounds_TR.y - sceneBounds_BL.y) / (2f * viewWindowDimensions_normalised.y);

        if (zoom > zoomMax)
            zoom = zoomMax;

        //Update pan based on input
        Vector2 draggedPanIncr = game.input.inputDrag * zoom * panSensitivity;
        Vector2 zoomFocusPanIncr = (zoomFocus - (Vector2)transform.position) * (game.input.inputSpread * zoomSensitivity);

        Vector2 panIncr;

        if (canZoomOrPan)
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
        if (viewBounds_BL.x < sceneBounds_BL.x)
            pan.x = viewBounds_BL_toPan.x; 
        if (viewBounds_BL.y < sceneBounds_BL.y)
            pan.y = viewBounds_BL_toPan.y;

        //Pan so that the top right corners line up.
        if (pan.x - viewBounds_TR_toPan.x > sceneBounds_TR.x)
            pan.x = sceneBounds_TR.x + viewBounds_TR_toPan.x;

        if (pan.y - viewBounds_TR_toPan.y > sceneBounds_TR.y)
            pan.y = sceneBounds_TR.y + viewBounds_TR_toPan.y;

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

    //Calculate the view boundaries in world-space.
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
            viewWindowDimensions_normalised.x = viewBounds.rect.width * ((100 / screenDimensions.x)) * 0.01f;
            viewWindowDimensions_normalised.y = viewBounds.rect.height * ((100 / screenDimensions.y)) * 0.01f;

            viewWindowDimensions_worldSpace.x = zoom * aspectRatio * 2f * viewWindowDimensions_normalised.x;
            viewWindowDimensions_worldSpace.y = zoom * 2f * viewWindowDimensions_normalised.y;
        }
        else
        {
            //If there isn't, just use the camera edges.
            viewBounds_BL.x = ScreenToWorldPosition(Vector2.zero / screenScale.x).x;
            viewBounds_BL.y = ScreenToWorldPosition(Vector2.zero / screenScale.y).y;
            viewBounds_TR.x = ScreenToWorldPosition(screenDimensions / screenScale.x).x;
            viewBounds_TR.y = ScreenToWorldPosition(screenDimensions / screenScale.y).y;

            //Set the view dimensions in worldspace as a function of zoom.
            viewWindowDimensions_normalised = new Vector2(1f, 1f);

            viewWindowDimensions_worldSpace.x = zoom * aspectRatio * 2f;
            viewWindowDimensions_worldSpace.y = zoom * 2f;
        }
    }    

}
