using UnityEngine;
using System.Collections;

public class HardpointSquare : MonoBehaviour {
		
	public float mass;
	public float health;
	public float comsumption;
	
	public void init(){
		mass = DefaultValues.DEFAULT_HARDPOINT_MASS;
		health = DefaultValues.DEFAULT_HARDPOINT_HEALTH;
		comsumption = DefaultValues.DEFAULT_HARDPOINT_COMSUMPTION;
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
