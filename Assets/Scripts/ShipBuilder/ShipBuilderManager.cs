using UnityEngine;
using System.Collections;

public class ShipBuilderManager : MonoBehaviour
{
    //Game references
    public GameManager game;
    public InputManager input;

    //Scene References
    public ShipBuilderGrid builderGrid;
    public ShipBuilderUI UI;
    public Camera cam;

    //Camera Data
    float camOrthoSize;
    public float camDepth = -20.0f;
    public float borderPadding = 1f;

    //Toolbox Cursor
    public Texture2D selectedHighlight;

    //Initialise
    public void Init(GameManager _game)
    {
        game = _game;
        input = game.input;

        camOrthoSize = 15.0f + borderPadding;
        cam.orthographicSize = camOrthoSize;

        float aspectRatio = 16.0f / 9.0f;
        float aspectRatioInv = 9.0f / 16.0f;

        cam.transform.position = new Vector3(aspectRatio * camOrthoSize - (aspectRatioInv * borderPadding * 2.0f), 15.0f, camDepth);

        builderGrid = GameObject.Find("BuilderGrid").GetComponent<ShipBuilderGrid>();
        UI = GameObject.Find("ShipBuilderUI").GetComponent<ShipBuilderUI>();

        UI.Init(game);
        builderGrid.Init(game);
    }

    //Update
    public void OnUpdate()
    {
        //Update the Ship Builder objects.
        builderGrid.OnUpdate();
    }

    //Converters
    public Pixel.Type ToolToPixelType(ShipBuilderUI.Tool tool)
    {
        if (tool == ShipBuilderUI.Tool.Scrap)
            return Pixel.Type.Scrap;
        else if (tool == ShipBuilderUI.Tool.Armour)
            return Pixel.Type.Armour;
        else if(tool == ShipBuilderUI.Tool.Engine)
            return Pixel.Type.Engine;
        else if(tool == ShipBuilderUI.Tool.Power)
            return Pixel.Type.Power;
        else if(tool == ShipBuilderUI.Tool.Hardpoint)
            return Pixel.Type.Hardpoint;

        return Pixel.Type.Scrap;
    }

    public Sprite PixelTypeToSprite(Pixel.Type type)
    {
        if (type == Pixel.Type.Scrap)
            return game.spritePixelScrap;
        else if(type == Pixel.Type.Armour)
            return game.spritePixelArmour;
        else if(type == Pixel.Type.Engine)
            return game.spritePixelEngine;
        else if(type == Pixel.Type.Power)
            return game.spritePixelPower;
        else if(type == Pixel.Type.Hardpoint)
            return game.spritePixelHardpoint;

        return game.spritePixelScrap;
    }

}
