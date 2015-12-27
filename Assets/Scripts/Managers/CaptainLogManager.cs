using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CaptainLogManager : MonoBehaviour {

	private Button backButton;

	void Start () {
		GameManager.instance.setGameState (GameManager.GameState.CaptainLog);

		backButton = GameObject.Find ("Back").GetComponent<Button> ();
		backButton.onClick.AddListener (() => GameManager.instance.loadScene("MainMenu"));
	}
}
