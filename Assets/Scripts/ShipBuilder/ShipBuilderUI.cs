using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Last edited : Mat Hill 20/12/2015

public class ShipBuilderUI : MonoBehaviour
{

    /// <summary>
    /// This class defines the Ship Builder UI (Not using Unity's UI API).
    /// </summary>

    GameManager game;
    ShipBuilderManager shipBuilder; //Reference the ship builder manager.
    InputManager input;
    Camera cam;


    public GameObject scrapButton;
    public GameObject armourButton;
    public GameObject engineButton;
    public GameObject powerButton;
    public GameObject hardpointButton;
    public GameObject eraserButton;
    public GameObject turretButton;


    Canvas canvas;
    public GameObject toolCursor; //White frame around selected tool.

    public enum Tool { Scrap, Armour, Engine, Power, Hardpoint, Eraser, Turret };
    public Tool tool = Tool.Scrap;

    public ShipBuilderTurret.Type turretType;

    public float toolPixelSize = 8f;

    public void Init(GameManager _game)
    {
        game = _game;
        shipBuilder = game.shipBuilder;
        input = game.input;
        cam = game.cam;
        canvas = GetComponent<Canvas>();

        SelectTool_Scrap();
    }

    public void SelectTool_Scrap()
    {
        UpdateSelection(Tool.Scrap, scrapButton.transform.position);
    }
    public void SelectTool_Armour()
    {
        UpdateSelection(Tool.Armour, armourButton.transform.position);
    }
    public void SelectTool_Hardpoint()
    {
        UpdateSelection(Tool.Hardpoint, hardpointButton.transform.position);
    }
    public void SelectTool_Power()
    {
        UpdateSelection(Tool.Power, powerButton.transform.position);
    }
    public void SelectTool_Turret()
    {
        UpdateSelection(Tool.Turret, turretButton.transform.position);
    }
    public void SelectTool_Engine()
    {
        UpdateSelection(Tool.Engine, engineButton.transform.position);
    }
    public void SelectTool_Eraser()
    {
        UpdateSelection(Tool.Eraser, eraserButton.transform.position);
    }
    public void SelectTool_TestShip()
    {
        game.SaveShip();
    }

    public void UpdateSelection(Tool _tool, Vector3 _cursorPos)
    {
        tool = _tool;
        toolCursor.transform.position = _cursorPos;
        game.sound.PlaySFX(SoundManager.GameSFX.SelectTool);
    }
}
