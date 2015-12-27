using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {

	private Button backButton;
	
	void Start () {
		backButton = GameObject.Find ("Back").GetComponent<Button> ();
		backButton.onClick.AddListener (() => GameManager.instance.loadScene("MainMenu"));
	}
}
