using UnityEngine;
using System.Collections;

public class PowerPixelProperties : MonoBehaviour, IPixelProperties {

	public float mass;
	public float health;
	public float capacity;
	
	public void Init(){
		mass = DefaultValues.DEFAULT_POWER_MASS;
		health = DefaultValues.DEFAULT_POWER_HEALTH;
		capacity = DefaultValues.DEFAULT_POWER_CAPACITY;
	}

	public void init(float mass, float health, float capacity){
		this.mass = mass;
		this.health = health;
		this.capacity = capacity;
	}

	public void setMass(float mass){
		this.mass = mass;
	}

	public float getMass(){
		return mass;
	}

	public void setHealth(float health){
		this.health = health;
	}

	public float getHealth(){
		return health;
	}

	public void setCapacity(float capacity){
		this.capacity = capacity;
	}
	
	public float getCapacity(){
		return capacity;
	}
}
