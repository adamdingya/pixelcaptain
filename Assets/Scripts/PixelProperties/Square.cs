using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour {

	private float mass; // Mass of the square, a bigger mass corresponds to slower acceleration and less control		
	private float health; // Square is destroyed after health reaches 0
	
	public Square(float mass, float health){
		this.mass = mass;
		this.health = health;
	}

	public float getMass(){
		return mass;
	}
	
	public void setMass(float mass){
		this.mass = mass;
	}
	
	public float getHealth(){
		return health;
	}
	
	public void setHealth(float health){
		this.health = health;
	}
}
