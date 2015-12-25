using UnityEngine;
using System.Collections;

public class ShipPlayerController : MonoBehaviour {

	private SpaceShip spaceShip;
	private float rotationCoefficient;
	private float currentSpeed;
	private float acceleration;
	private float maxSpeed;

	void Start () {
		spaceShip = new SpaceShip ();
		rotationCoefficient = spaceShip.getRotationCoefficient();
		acceleration = spaceShip.getAcceleration ();
		maxSpeed = spaceShip.getMaxSpeed();
	}

	void Update () {
		if(Input.GetKey(KeyCode.W)){
			if(currentSpeed < maxSpeed){
				spaceShip.changeCurrentSpeed(acceleration);
				currentSpeed+=acceleration;
				transform.position += transform.up * currentSpeed * Time.deltaTime;
			}
		}
		if(Input.GetKey(KeyCode.S)){
			if(currentSpeed > -maxSpeed){
				spaceShip.changeCurrentSpeed(-acceleration);
				currentSpeed-=acceleration;
				transform.position -= transform.up * currentSpeed * Time.deltaTime;
			}
		}		
		transform.position += transform.up * currentSpeed * Time.deltaTime;

		if(Input.GetKey(KeyCode.A)){
			transform.Rotate(-Vector3.back * rotationCoefficient * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.D)){	
			transform.Rotate(Vector3.back * rotationCoefficient * Time.deltaTime);
		}
	}
	
	private float calculateNewVelocity(float currentVelocity, float acceleration, float time){
		float newVelocity = currentVelocity + acceleration * time;
		return newVelocity;
	}
}
