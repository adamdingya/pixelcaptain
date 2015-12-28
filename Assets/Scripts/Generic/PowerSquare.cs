using UnityEngine;
using System.Collections;

public class PowerSquare : Square {

	private float capacity;

	public PowerSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID) {
		
	}

	public PowerSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID, float capacity) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID) {
		this.capacity = capacity;
	}
	
	public void setCapacity(float capacity){
		this.capacity = capacity;
	}
	
	public float getCapacity(){
		if(capacity==0f){
			return DefaultValues.DEFAULT_POWER_CAPACITY;
		}
		return capacity;
	}
}
