using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour {

	private string UUID;
	private string leftUUID;
	private string rightUUID;
	private string upUUID;
	private string downUUID;
	private float mass; // Mass of the square, a bigger mass corresponds to slower acceleration and less control		
	private float health; // Square is destroyed after health reaches 0
	
	public Square(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID){
		this.UUID = System.Guid.NewGuid().ToString();
		this.leftUUID = leftUUID;
		this.rightUUID = rightUUID;
		this.upUUID = upUUID;
		this.downUUID = downUUID;
		this.mass = mass;
		this.health = health;
	}

	public float getMass(){
		return mass;
	}
	
	public void setMass(float mass){
		this.mass = mass;
	}
	
	public float getHealth(){
		return health;
	}
	
	public void setHealth(float health){
		this.health = health;
	}
	
	public string getUUID(){
		return UUID;
	}
	
	public string getLeftUUID(){
		return leftUUID;
	}
	
	public void setLeftUUID(string leftUUID){
		this.leftUUID = leftUUID;
	}
	
	public string getRightUUID(){
		return rightUUID;
	}
	
	public void setRightUUID(string rightUUID){
		this.rightUUID = rightUUID;
	}
	
	public string getUpUUID(){
		return upUUID;
	}
	
	public void setUpUUID(string upUUID){
		this.upUUID = upUUID;
	}
	
	public string getDownUUID(){
		return downUUID;
	}
	
	public void setDownUUID(string downUUID){
		this.downUUID = downUUID;
	}
}
