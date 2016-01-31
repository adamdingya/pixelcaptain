using UnityEngine;
using System.Collections;

public class BulletPixel : MonoBehaviour {

	public float damage;

	public void Init(float turretDamage){
		damage = turretDamage;
	}
}
