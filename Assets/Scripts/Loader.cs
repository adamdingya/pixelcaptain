using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

	public GameManager gameManager;
	public SoundManager soundManager;
	public InputManager inputManager;

	void Awake () 
	{
		if (GameManager.instance == null) {
			Instantiate(gameManager);
		}
		if (SoundManager.instance == null) {
			Instantiate (soundManager);
		}
		if (InputManager.instance == null) {
			Instantiate (inputManager);
		}
	}
}
