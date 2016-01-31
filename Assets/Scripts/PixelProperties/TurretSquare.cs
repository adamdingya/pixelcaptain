using UnityEngine;
using System.Collections;

public class TurretSquare : MonoBehaviour {

	public float mass;
	public float comsumption;
	public int barrelCount;
	public float bulletSpeed;
	public float cooldown;
	public float range;
	public float damage;
	public float rotationCoefficient;
	public float accuracy;

	public void Init(Turret.Type type){
		mass = DefaultValues.DEFAULT_TURRET_MASS;
		comsumption = DefaultValues.DEFAULT_TURRET_COMSUMPTION;
		barrelCount = DefaultValues.DEFAULT_TURRET_BARREL_COUNT;
		bulletSpeed = DefaultValues.DEFAULT_TURRET_BULLET_SPEED;
		cooldown = DefaultValues.DEFAULT_TURRET_COOLDOWN;
		damage = DefaultValues.DEFAULT_TURRET_DAMAGE;
		rotationCoefficient = DefaultValues.DEFAULT_TURRET_ROTATION_COEFFICIENT;
		accuracy = DefaultValues.DEFAULT_TURRET_ACCURACY;
		switch (type) {
		case Turret.Type.Small:
			range = DefaultValues.DEFAULT_TURRET_SMALL_FIRING_RANGE;
			break;
		case Turret.Type.Medium:
			range = DefaultValues.DEFAULT_TURRET_MEDIUM_FIRING_RANGE;
			break;
		case Turret.Type.Large:
			range = DefaultValues.DEFAULT_TURRET_LARGE_FIRING_RANGE;
			break;
		case Turret.Type.Laser:
			range = DefaultValues.DEFAULT_TURRET_LASER_FIRING_RANGE;
			break;
		}
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

	public void setCooldown(float cooldown){
		this.cooldown = cooldown;
	}
	
	public float getCooldown(){
		return cooldown;
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
