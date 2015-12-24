using UnityEngine;
using System.Collections;

public class ShieldSquare : Square {

	// This is future concept, currently not used at this stage

	private float comsumption;
	private int horizontalCoverage;
	private int verticalCoverage;
	private float strength;
	private float regenerationRate;
	
	public ShieldSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID) {
		
	}
	
	public ShieldSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID, float comsumption, int horizontalCoverage, int verticalCoverage, float strength, float regenerationRate) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID) {
		this.comsumption = comsumption;
		this.horizontalCoverage = horizontalCoverage;
		this.verticalCoverage = verticalCoverage;
		this.strength = strength;
		this.regenerationRate = regenerationRate;
	}

	public void setComsumption(float comsumption){
		this.comsumption = comsumption;
	}
	
	public float getComsumption(){
		if (comsumption == 0f) {
			return DefaultValues.DEFAULT_SHIELD_COMSUMPTION;
		}
		return comsumption;
	}
	
	public void setHorizontalCoverage(int horizontalCoverage){
		this.horizontalCoverage = horizontalCoverage;
	}
	
	public float getHorizontalCoverage(){
		if (horizontalCoverage == 0f) {
			return DefaultValues.DEFAULT_SHIELD_HORIZONTAL_COVERAGE;
		}
		return horizontalCoverage;
	}
	
	public void setVerticalCoverage(int verticalCoverage){
		this.verticalCoverage = verticalCoverage;
	}
	
	public float getVerticalCoverage(){
		if (verticalCoverage == 0f) {
			return DefaultValues.DEFAULT_SHIELD_VERTICAL_COVERAGE;
		}
		return verticalCoverage;
	}
	
	public void setStrength(float strength){
		this.strength = strength;
	}
	
	public float getStrength(){
		if (strength == 0f) {
			return DefaultValues.DEFAULT_SHIELD_STRENGTH;
		}
		return strength;
	}
	
	public void setregerationRate(float regenerationRate){
		this.regenerationRate = regenerationRate;
	}
	
	public float getRegenerationRate(){
		if (regenerationRate == 0f) {
			return DefaultValues.DEFAULT_SHIELD_REGENERATION_RATE;
		}
		return regenerationRate;
	}
}
