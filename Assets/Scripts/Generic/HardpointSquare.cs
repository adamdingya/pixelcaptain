using UnityEngine;
using System.Collections;

public class HardpointSquare : Square {
		
	private float comsumption;

	public HardpointSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID){

	}

	public HardpointSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID, float comsumption) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID){
		this.comsumption = comsumption;
	}

	public void setComsumption(float comsumption){
		this.comsumption = comsumption;
	}

	public float getComsumption(){
		if (comsumption == 0f) {
			return DefaultValues.DEFAULT_HARDPOINT_COMSUMPTION;
		}
		return comsumption;
	}
}
