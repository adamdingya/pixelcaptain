using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.loadedLevelName.Equals ("Combat")) {
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			if(player!=null && enemies.Length==0){
				Application.LoadLevel("Reward");
				Debug.Log("Player has won!");
			}
			if(player==null && enemies.Length>0){
				Application.LoadLevel("Navigation_System");
				Debug.Log("Enemy has won!");
			}
		}
	}
	
	public void LoadLevel(string name) {
		Debug.Log("Loading level: "+name);
		Application.LoadLevel(name);
	}
	
	public void LoadNextLevel(){
		Application.LoadLevel(Application.loadedLevel+1);
	}

}
