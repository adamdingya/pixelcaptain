using UnityEngine;
using System.Collections;

public class BridgePixelProperties : MonoBehaviour, IPixelProperties {

	public float mass;
	public float health;
	public float comsumption;
	
	public void Init(){
		mass = DefaultValues.DEFAULT_BRIDGE_MASS;
		health = DefaultValues.DEFAULT_BRIDGE_HEALTH;
		comsumption = DefaultValues.DEFAULT_BRIDGE_COMSUMPTION;
	}
	
	public void init(float mass, float health, float comsumption){
		this.mass = mass;
		this.health = health;
		this.comsumption = comsumption;
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
}
