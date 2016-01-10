using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TacticalOptions_Manager : MonoBehaviour {

	private Button fightButton;

	// Use this for initialization
	public void Init ()
    {
		fightButton = GameObject.Find ("Fight").GetComponent<Button> ();
		fightButton.onClick.AddListener (() => Game_Manager.instance.loadScene("Combat"));
	}

    public void OnUpdate()
    {

    }
}
