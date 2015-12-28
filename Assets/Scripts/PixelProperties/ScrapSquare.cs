using UnityEngine;
using System.Collections;

public class ScrapSquare : MonoBehaviour {
	
	public float mass;
	public float health;
	
	public void init(){
		mass = DefaultValues.DEFAULT_SCRAP_MASS;
		health = DefaultValues.DEFAULT_SCRAP_HEALTH;
	}
	
	public void init(float mass, float health){
		this.mass = mass;
		this.health = health;
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

}
