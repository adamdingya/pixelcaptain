using UnityEngine;
using System.Collections;

public class EnginePixelProperties : MonoBehaviour, IPixelProperties {

	public float mass;
	public float health;
	public float comsumption;
	public float thrust;
	
	public void Init(){
		mass = DefaultValues.DEFAULT_ENGINE_MASS;
		health = DefaultValues.DEFAULT_ENGINE_HEALTH;
		comsumption = DefaultValues.DEFAULT_ENGINE_COMSUMPTION;
		thrust = DefaultValues.DEFAULT_ENGINE_THRUST;
	}
	
	public void init(float mass, float health, float comsumption, float thrust){
		this.mass = mass;
		this.health = health;
		this.comsumption = comsumption;
		this.thrust = thrust;
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
	
	public void setComsumption(float comsumption){
		this.comsumption = comsumption;
	}
	
	public float getComsumption(){
		return comsumption;
	}

	public void setThrust(float thrust){
		this.thrust = thrust;
	}

	public float getThrust(){
		return thrust;
	}
}
