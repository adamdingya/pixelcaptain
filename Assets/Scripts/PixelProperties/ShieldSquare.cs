using UnityEngine;
using System.Collections;

public class ShieldSquare : MonoBehaviour {

	// This is future concept, currently not used at this stage
	public float mass;
	public float health;
	public float comsumption;
	public int horizontalCoverage;
	public int verticalCoverage;
	public float strength;
	public float regenerationRate;
	
	public void init(){
		mass = DefaultValues.DEFAULT_SHIELD_MASS;
		health = DefaultValues.DEFAULT_SHIELD_HEALTH;
		comsumption = DefaultValues.DEFAULT_SHIELD_COMSUMPTION;
		horizontalCoverage = DefaultValues.DEFAULT_SHIELD_HORIZONTAL_COVERAGE;
		verticalCoverage = DefaultValues.DEFAULT_SHIELD_VERTICAL_COVERAGE;
		strength = DefaultValues.DEFAULT_SHIELD_STRENGTH;
		regenerationRate = DefaultValues.DEFAULT_SHIELD_REGENERATION_RATE;
	}
	
	public void init(float mass, float health, float comsumption, int horizontalCoverage, int verticalCoverage, float strength, float regenerationRate){
		this.mass = mass;
		this.health = health;
		this.comsumption = comsumption;
		this.horizontalCoverage = horizontalCoverage;
		this.verticalCoverage = verticalCoverage;
		this.strength = strength;
		this.regenerationRate = regenerationRate;
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

	public void setHorizontalCoverage(int horizontalCoverage){
		this.horizontalCoverage = horizontalCoverage;
	}

	public int getHorizontalCoverage(){
		return horizontalCoverage;
	}

	public void setVerticalCoverage(int verticalCoverage){
		this.verticalCoverage = verticalCoverage;
	}

	public int getVerticalCoverage(){
		return verticalCoverage;
	}

	public void setStrength(float strength){
		this.strength = strength;
	}

	public float getStrength(){
		return strength;
	}

	public void setRegenerationRate(float regenerationRate){
		this.regenerationRate = regenerationRate;
	}

	public float getRegenerationRate(){
		return regenerationRate;
	}
}
