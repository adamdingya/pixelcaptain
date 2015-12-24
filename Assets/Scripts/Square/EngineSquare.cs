using UnityEngine;
using System.Collections;

public class EngineSquare : Square {

	private float comsumption;
	private float thrust;
	
	public EngineSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID){

	}

	public EngineSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID, float comsumption, float thrust) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID){
		this.comsumption = comsumption;
		this.thrust = thrust;
	}

	public void setComsumption(float comsumption){
		this.comsumption = comsumption;
	}

	public float getComsumption(){
		if (comsumption == 0f) {
			return DefaultValues.DEFAULT_ENGINE_COMSUMPTION;
		}
		return comsumption;
	}

	public void setThrust(float thrust){
		this.thrust = thrust;
	}

	public float getThrust(){
		if(thrust==0f){
			return DefaultValues.DEFAULT_ENGINE_THRUST;
		}
		return thrust;
	}
}
