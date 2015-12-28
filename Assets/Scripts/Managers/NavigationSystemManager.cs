using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavigationSystemManager : MonoBehaviour {

	private Button engageButton;

	// Use this for initialization
	void Start () {
		GameManager.instance.setGameState (GameManager.GameState.NavigationSystem);
		
		engageButton = GameObject.Find ("Engage").GetComponent<Button> ();
		engageButton.onClick.AddListener (() => GameManager.instance.loadScene("TacticalOptions"));
	}
}
