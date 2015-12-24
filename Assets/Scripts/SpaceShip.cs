using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceShip {

	private List<Square> components = new List<Square>();
	private List<Square> bridgeSquares = new List<Square>();
	private List<Square> armousrSquares = new List<Square>();
	private List<Square> engineSquares = new List<Square>();
	private List<Square> weaponSquares = new List<Square>();

	private float currentSpeed;
	private float acceleration;
	private float maxSpeed;
	private float rotationCoefficient;

	// Creates a dummy space ship
	public SpaceShip(){
		// TODO write some dummy squares for the spaceship

		calculateMaxSpeed ();
		calculateAcceleration ();
		calculateRotationCoefficient ();
	}
	
	public SpaceShip(List<Square> bridgeSquares, List<Square> armousrSquares, List<Square> engineSquares, List<Square> weaponSquares){
		this.bridgeSquares = bridgeSquares;
		this.armousrSquares = armousrSquares;
		this.engineSquares = engineSquares;
		this.weaponSquares = weaponSquares;

		combineSquaresToComponents();
		calculateMaxSpeed ();
		calculateAcceleration ();
		calculateRotationCoefficient ();
	}

	private void combineSquaresToComponents(){
		components.AddRange (bridgeSquares);
		components.AddRange (armousrSquares);
		components.AddRange (engineSquares);
		components.AddRange (weaponSquares);
	}

	private void calculateMaxSpeed(){
		//TODO write an equation to find max speed involving total engineSquares' capacity and total mass
		maxSpeed = 5f; // Dummy
	}

	private void calculateAcceleration(){
		//TODO write an equation to find acceleration involving total engineSquares' capacity and total mass
		acceleration = 1f; // Dummy
	}

	private void calculateRotationCoefficient(){
		// TODO write an equation to find the rotation coefficient based on the placement of the engine squares relative to the centre of the ship
		rotationCoefficient = 300f; // Dummy
	}

	public float getMaxSpeed(){
		return maxSpeed;
	}

	public float getAcceleration(){
		return acceleration;
	}

	public float getRotationCoefficient(){
		return rotationCoefficient;
	}

	public float getCurrentSpeed(){
		return currentSpeed;
	}

	public void changeCurrentSpeed(float amount){
		currentSpeed += amount;
	}
}
