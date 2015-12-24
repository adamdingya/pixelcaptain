using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	private Vector3 playerSpawningPosition = DefaultValues.DEFAULT_PLAYER_SPAWNING_POINT;
	private int numberOfEnemiesToSpawn = DefaultValues.DEFAULT_MAX_CONCURRENT_ENEMY_COUNT;
	private Vector3[] enemySpawningPositions = DefaultValues.DEFAULT_ENEMY_SPAWNING_POINT;
	private ArrayList enemies = new ArrayList ();

	public GameObject player;
	public GameObject enemy;
	
	void Start () {
		// import player's ship from the ship builder
		// player = Instantiate(player, playerSpawningPosition, this.transform.rotation) as GameObject;
		

		// import enemy's ship
		foreach (Vector3 enemySpawningPosition in enemySpawningPositions) {
			enemies.Add(Instantiate (enemy, enemySpawningPosition, this.transform.rotation));
		}
	}

	void Update () {
	
	}
}
