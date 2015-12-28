using UnityEngine;
using System.Collections;

public class TurretSquare : MonoBehaviour {

	public float mass;
	public float health;
	public float comsumption;
	public int barrelCount;
	public float bulletSpeed;
	public float rateOfFire;
	public float range;
	public float damage;
	public float rotationCoefficient;
	public float accuracy;

	public void init(){
		mass = DefaultValues.DEFAULT_TURRET_MASS;
		health = DefaultValues.DEFAULT_TURRET_HEALTH;
		comsumption = DefaultValues.DEFAULT_TURRET_COMSUMPTION;
		barrelCount = DefaultValues.DEFAULT_TURRET_BARREL_COUNT;
		bulletSpeed = DefaultValues.DEFAULT_TURRET_BULLET_SPEED;
		rateOfFire = DefaultValues.DEFAULT_TURRET_RATE_OF_FIRE;
		range = DefaultValues.DEFAULT_TURRET_RANGE;
		damage = DefaultValues.DEFAULT_TURRET_DAMAGE;
		rotationCoefficient = DefaultValues.DEFAULT_TURRET_ROTATION_COEFFICIENT;
		accuracy = DefaultValues.DEFAULT_TURRET_ACCURACY;
	}

	public void init(float mass, float health, float comsumption, int barrelCount, float bulletSpeed, float rateOfFire, float range, float damage, float rotationCoefficient, float accuracy) {
		this.mass = mass;
		this.health = health;
		this.comsumption = comsumption;
		this.barrelCount = barrelCount;
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
		return comsumption;
	}

	public void setBarrelCount(int barrelCount){
		this.barrelCount = barrelCount;
	}
	
	public int getBarrelCount(){
		return barrelCount;
	}

	public void setBulletSpeed(float bulletSpeed){
		this.bulletSpeed = bulletSpeed;
	}
	
	public float getBulletSpeed(){
		return bulletSpeed;
	}

	public void setRateOfFire(float rateOfFire){
		this.rateOfFire = rateOfFire;
	}
	
	public float getRateOfFire(){
		return rateOfFire;
	}

	public void setRange(float range){
		this.range = range;
	}
	
	public float getRange(){
		return range;
	}

	public void setDamage(float damage){
		this.damage = damage;
	}
	
	public float getDamage(){
		return damage;
	}

	public void setRotationCoefficient(float rotationCoefficient){
		this.rotationCoefficient = rotationCoefficient;
	}
	
	public float getRotationCoefficient(){
		return rotationCoefficient;
	}

	public void setAccuracy(float accuracy){
		this.accuracy = accuracy;
	}
	
	public float getAccuracy(){
		return accuracy;
	}
}
