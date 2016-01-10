using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu_Manager : MonoBehaviour {

	private Button startButton;
	private Button captainLogButton;
	private Button settingsButton;
	
	public void Init ()
    {
		startButton = GameObject.Find ("Start").GetComponent<Button> ();
		startButton.onClick.AddListener (() => Game_Manager.instance.loadScene("Builder"));

		startButton = GameObject.Find ("CaptainLog").GetComponent<Button> ();
		startButton.onClick.AddListener (() => Game_Manager.instance.loadScene("CaptainsLog"));

		startButton = GameObject.Find ("Settings").GetComponent<Button> ();
		startButton.onClick.AddListener (() => Game_Manager.instance.loadScene("Settings"));
	}

    public void OnUpdate()
    {

    }
}
