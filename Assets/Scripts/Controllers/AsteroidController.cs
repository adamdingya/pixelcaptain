using UnityEngine;
using System.Collections;
using System;

public class AsteroidController : MonoBehaviour {

	private Asteroid asteroid;

	// Use this for initialization
	void Start () {
		asteroid = this.gameObject.GetComponent<Asteroid> ();
		asteroid.init ();

		Rigidbody2D rigidbody2D = this.gameObject.GetComponent<Rigidbody2D> ();
		rigidbody2D.AddForce (asteroid.getForce());
	}
	
	// Update is called once per frame
	void Update () {
		// Rotating asteroids.
		transform.Rotate (Vector3.forward * asteroid.getRotationCoefficient() * Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D collision){	
		try{
			// Re-applying 1/2 force to the incoming asteroid to keep asteroids moving.
			collision.rigidbody.AddForce (asteroid.getForce() / 2);
		} catch (NullReferenceException e){
			Debug.Log("Asteroids are instantiated on top of each other: " + e);
		}
	
	}
}
