﻿using UnityEngine;
using System.Collections;

public class ArmourPixelProperties : MonoBehaviour, IPixelProperties {
	
	public float mass;
	public float health;
	
	public void Init(){
		mass = DefaultValues.DEFAULT_ARMOUR_MASS;
		health = DefaultValues.DEFAULT_ARMOUR_HEALTH;
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
