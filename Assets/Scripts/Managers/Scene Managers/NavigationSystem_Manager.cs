using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NavigationSystem_Manager : MonoBehaviour {

	private Button engageButton;

	// Use this for initialization
	public void Init()
    {
		engageButton = GameObject.Find ("Engage").GetComponent<Button> ();
		engageButton.onClick.AddListener (() => Game_Manager.instance.loadScene("TacticalOptions"));
	}

    public void OnUpdate()
    {

    }
}
