using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {
	
	public float health;
	public float damage;
	public float rotationCoefficient;
	public Vector2 force;

	public void init(){
		health = DefaultValues.DEFAULT_ASTEROID_HEALTH;
		damage = DefaultValues.DEFAULT_ASTEROID_DAMAGE;

		calculateRotationCoefficient ();
		calculateForce ();
	}

	private void calculateRotationCoefficient(){
		float rotationCoefficientMin = DefaultValues.DEFAULT_ASTEROID_ROTATION_COEFFICIENT_MIN;
		float rotationCoefficientMax = DefaultValues.DEFAULT_ASTEROID_ROTATION_COEFFICIENT_MAX;
		rotationCoefficient = Random.Range (rotationCoefficientMin, rotationCoefficientMax);
		if (Random.Range (0, 2) == 0) {
			rotationCoefficient = -rotationCoefficient;
		}
	}

	private void calculateForce(){
		float forceXMin = DefaultValues.DEFAULT_ASTEROID_FORCE_X_MIN;
		float forceXMax = DefaultValues.DEFAULT_ASTEROID_FORCE_X_MAX;
		float forceYMin = DefaultValues.DEFAULT_ASTEROID_FORCE_Y_MIN;
		float forceYMax = DefaultValues.DEFUALT_ASTEROID_FORCE_Y_MAX;

		float forceX = Random.Range (forceXMin, forceXMax);
		float forceY = Random.Range (forceYMin, forceYMax);

		switch (Random.Range (0, 4)) {
		case 0:
			forceX = -forceX;
			break;
		case 1:
			forceY = -forceY;
			break;
		case 2:
			forceX = -forceX;
			forceY = -forceY;	
			break;
		case 3:
			// Do nothing
			break;
		}

		force = new Vector2 (forceX, forceY);
	}

	public void setHealth(float health){
		this.health = health;
	}
	
	public float getHealth(){
		return health;
	}

	public void setRotationCoefficient(float rotationCoefficient){
		this.rotationCoefficient = rotationCoefficient;
	}

	public float getRotationCoefficient(){
		return rotationCoefficient;
	}

	public void setForce(Vector2 force){
		this.force = force;
	}

	public Vector2 getForce(){
		return force;
	}

	public void setDamage(float damage){
		this.damage = damage;
	}

	public float getDamage(){
		return damage;
	}
}
