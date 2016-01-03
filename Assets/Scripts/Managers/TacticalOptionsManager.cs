using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TacticalOptionsManager : MonoBehaviour {

	private Button fightButton;

	// Use this for initialization
	void Start () {
		GameManager.instance.setGameState (GameManager.GameState.TacticalOptions);
		
		fightButton = GameObject.Find ("Fight").GetComponent<Button> ();
		fightButton.onClick.AddListener (() => GameManager.instance.loadScene("Combat"));
	}
}
