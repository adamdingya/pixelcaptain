using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CaptainsLog_Manager : MonoBehaviour {

	private Button backButton;

	public void Init ()
    {
		backButton = GameObject.Find ("Back").GetComponent<Button> ();
		backButton.onClick.AddListener (() => Game_Manager.instance.loadScene("MainMenu"));
	}

    public void OnUpdate()
    {

    }
}
