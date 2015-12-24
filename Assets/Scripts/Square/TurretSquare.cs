using UnityEngine;
using System.Collections;

public class TurretSquare : Square {

	private float comsumption;
	private int barrel;
	private float bulletSpeed;
	private float rateOfFire;
	private float range;
	private float damage;
	private float rotationCoefficient;
	private float accuracy;

	public TurretSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID) {
		
	}

	public TurretSquare(float mass, float health, string leftUUID, string rightUUID, string upUUID, string downUUID, float comsumption, int barrel, float bulletSpeed, float rateOfFire, float range, float damage, float rotationCoefficient, float accuracy) : base (mass, health, leftUUID, rightUUID, upUUID, downUUID) {
		this.comsumption = comsumption;
		this.barrel = barrel;
		this.bulletSpeed = bulletSpeed;
		this.rateOfFire = rateOfFire;
		this.range = range;
		this.damage = damage;
		this.rotationCoefficient = rotationCoefficient;
		this.accuracy = accuracy;
	}
	
	public void setComsumption(float comsumption){
		this.comsumption = comsumption;
	}
	
	public float getComsumption(){
		if (comsumption == 0f) {
			return DefaultValues.DEFAULT_TURRET_COMSUMPTION;
		}
		return comsumption;
	}

	public void setBarrel(int barrel){
		this.barrel = barrel;
	}
	
	public int getBarrel(){
		if (barrel == 0) {
			return DefaultValues.DEFAULT_TURRET_BARREL;
		}
		return barrel;
	}

	public void setBulletSpeed(float bulletSpeed){
		this.bulletSpeed = bulletSpeed;
	}
	
	public float getBulletSpeed(){
		if (bulletSpeed == 0f) {
			return DefaultValues.DEFAULT_TURRET_BULLET_SPEED;
		}
		return bulletSpeed;
	}

	public void setRateOfFire(float rateOfFire){
		this.rateOfFire = rateOfFire;
	}
	
	public float getRateOfFire(){
		if (rateOfFire == 0f) {
			return DefaultValues.DEFAULT_TURRET_RATE_OF_FIRE;
		}
		return rateOfFire;
	}

	public void setRange(float range){
		this.range = range;
	}
	
	public float getRange(){
		if (range == 0f) {
			return DefaultValues.DEFAULT_TURRET_RANGE;
		}
		return range;
	}

	public void setDamage(float damage){
		this.damage = damage;
	}
	
	public float getDamage(){
		if (damage == 0f) {
			return DefaultValues.DEFAULT_TURRET_DAMAGE;
		}
		return damage;
	}

	public void setRotationCoefficient(float rotationCoefficient){
		this.rotationCoefficient = rotationCoefficient;
	}
	
	public float getRotationCoefficient(){
		if (rotationCoefficient == 0f) {
			return DefaultValues.DEFAULT_TURRET_ROTATION_COEFFICIENT;
		}
		return rotationCoefficient;
	}

	public void setAccuracy(float accuracy){
		this.accuracy = accuracy;
	}
	
	public float getAccuracy(){
		if (accuracy == 0f) {
			return DefaultValues.DEFAULT_TURRET_ACCURACY;
		}
		return accuracy;
	}
}
