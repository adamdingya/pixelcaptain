using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	private Button startButton;
	private Button captainLogButton;
	private Button settingsButton;
	
	void Start () {
		startButton = GameObject.Find ("Start").GetComponent<Button> ();
		startButton.onClick.AddListener (() => GameManager.instance.loadScene("ShipBuilder"));

		startButton = GameObject.Find ("CaptainLog").GetComponent<Button> ();
		startButton.onClick.AddListener (() => GameManager.instance.loadScene("CaptainLog"));

		startButton = GameObject.Find ("Settings").GetComponent<Button> ();
		startButton.onClick.AddListener (() => GameManager.instance.loadScene("Settings"));
	}
}
