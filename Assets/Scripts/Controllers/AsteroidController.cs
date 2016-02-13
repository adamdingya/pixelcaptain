using UnityEngine;
using System.Collections;
using System;

public class AsteroidController : MonoBehaviour {

	public AsteroidPixelProperties asteroidPixelProperties;
	
	void Start () {
		asteroidPixelProperties = this.gameObject.GetComponent<AsteroidPixelProperties> ();

		Rigidbody2D rigidbody2D = this.gameObject.GetComponent<Rigidbody2D> ();
		rigidbody2D.AddForce (asteroidPixelProperties.getForce());
	}

	void Update () {
		// Rotating asteroids.
		transform.Rotate (Vector3.forward * asteroidPixelProperties.getRotationCoefficient() * Time.deltaTime);
		if(asteroidPixelProperties.health <= 0){
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision){	
		try{
			// Re-applying 1/2 force to the incoming asteroid to keep asteroids moving.
			collision.rigidbody.AddForce (asteroidPixelProperties.getForce() / 2);
		} catch (NullReferenceException e){
			Debug.Log("Asteroids are instantiated on top of each other: " + e);
		}
	
	}
}
