using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	private SpaceShip spaceShip;
	private float rotationCoefficient;
	private float currentSpeed;
	private float acceleration;
	private float maxSpeed;
	private GameObject player;

	// Use this for initialization
	void Start () {
		spaceShip = new SpaceShip ();
		rotationCoefficient = 0.5f * spaceShip.getRotationCoefficient();
		acceleration = 0.5f * spaceShip.getAcceleration ();
		maxSpeed = 0.5f * spaceShip.getMaxSpeed();

		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	void OnTriggerEnter2D(Collider2D collider2D){
		string collider2DTag = collider2D.gameObject.tag;
		if (collider2DTag.Equals ("Bullet")) {
			Debug.Log("Bullet hits enemy, destroy enemy ship!");
			Destroy (gameObject);
			Debug.Log("Bullet hits enemy, destroy bullet!");
			Destroy (collider2D.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision2D){
		string collision2DTag = collision2D.gameObject.tag;
		if (collision2DTag.Equals ("Player")) {
			Debug.Log("Player crashes onto enemy, destroy enemy ship!");
			Destroy (gameObject);	
		}
	}
}
