using UnityEngine;
using System.Collections;
using System;

public class TurretController : MonoBehaviour
{	
	public Combat_TurretBehavior combatTurretBehaviour; 
	public TurretPixelProperties turretSquare;
	public float turretAngleLeft;
	public float turretAngleRight;
	public Vector3 shipRotation;
	public float initialFacingAngle;
	public Vector3 targetAngle;
	public float cooldown;	
	public float bulletSpeed;
	public float turretCooldown;

	// Use this for initialization
	void Start()
	{
		combatTurretBehaviour = gameObject.GetComponent<Combat_TurretBehavior>();
		turretSquare = gameObject.GetComponent<TurretPixelProperties> ();
		initialFacingAngle = combatTurretBehaviour.turretFacingAngle;
		cooldown = turretSquare.cooldown;
		bulletSpeed = DefaultValues.DEFAULT_TURRET_BULLET_SPEED;
		turretCooldown = DefaultValues.DEFAULT_TURRET_COOLDOWN;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (DefaultValues.turretOn) {	

			if(manualSelection()){	
				//TODO
			} else {
				// If ship rotates, the turret facing angle changes as well.
				calculateTurretSpan();
				// Returns the nearest enemy for the given turret within its span
				GameObject nearestTargetableEnemy = calculateNearestTargetableEnemy();
				if(nearestTargetableEnemy != null){
					// Debug line.
					Debug.DrawLine (transform.position, nearestTargetableEnemy.transform.position, Color.green);

					// Rotating turret to the acquired target.
					Quaternion targetRelativeAngle = Quaternion.LookRotation (nearestTargetableEnemy.transform.position - this.transform.position, this.transform.TransformDirection (Vector3.back));
					targetRelativeAngle.x = 0;
					targetRelativeAngle.y = 0;
					targetAngle = targetRelativeAngle.eulerAngles;
					transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRelativeAngle, Time.deltaTime * 200f);
					
					// Ray casting as turret turns and fire if ray cast hits an gameobject with the hostile layer.
					RaycastHit2D hit = Physics2D.Raycast (transform.position, transform.up, turretSquare.range, 1 << 9);
					if(hit){
						if(cooldown <= 0){
							fire ();
							cooldown = turretSquare.cooldown;
						}
					}
				}		
			}
		}
		if(cooldown > 0){
			cooldown = cooldown - Time.deltaTime;
		}
	}

	private void calculateTurretSpan(){
		shipRotation = transform.root.eulerAngles;
		turretAngleLeft = shipRotation.z + initialFacingAngle + (combatTurretBehaviour.turretMountSpan / 2);
		if (turretAngleLeft > 360) {
			turretAngleLeft = turretAngleLeft - 360;
		}
		turretAngleRight = shipRotation.z + combatTurretBehaviour.turretFacingAngle - (combatTurretBehaviour.turretMountSpan / 2);
		if (turretAngleRight < 0) {
			turretAngleRight = 360 + turretAngleRight;
		}
		if (turretAngleRight > 360){
			turretAngleRight = turretAngleRight - 360;
		}
	}


	private ArrayList getEnemiesOnMap(){
		ArrayList enemies = new ArrayList ();
		enemies.AddRange (GameObject.FindGameObjectsWithTag("Enemy"));
		enemies.AddRange (GameObject.FindGameObjectsWithTag("Asteroid"));
		return enemies;
	}

	private GameObject calculateNearestTargetableEnemy(){
		ArrayList enemies = getEnemiesOnMap ();

		//Filtering out the list of enemies not within turret span.
		ArrayList enemiesWithinTurretSpan = new ArrayList();
		foreach (GameObject enemy in enemies) {
			Quaternion targetRelativeAngle = Quaternion.LookRotation (enemy.transform.position - this.transform.position, this.transform.TransformDirection (Vector3.back));
			targetRelativeAngle.x = 0;
			targetRelativeAngle.y = 0;

			if(turretAngleLeft < turretAngleRight){
				if(turretAngleLeft == 0){
					if(turretAngleRight < targetRelativeAngle.eulerAngles.z){
						enemiesWithinTurretSpan.Add(enemy);
					}
				} else {
					if(turretAngleLeft > targetRelativeAngle.eulerAngles.z || turretAngleRight < targetRelativeAngle.eulerAngles.z){
						enemiesWithinTurretSpan.Add(enemy);
					}
				}
			} 
			if(turretAngleLeft > turretAngleRight){
				if(turretAngleRight < targetRelativeAngle.eulerAngles.z && turretAngleLeft > targetRelativeAngle.eulerAngles.z){
					enemiesWithinTurretSpan.Add(enemy);		
				}
			}
			if(turretAngleLeft == turretAngleRight){
				enemiesWithinTurretSpan.Add(enemy);	
			}
		}

		GameObject nearest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;

		foreach (GameObject enemyWithinTurretSpan in enemiesWithinTurretSpan) {
			Vector3 diff = enemyWithinTurretSpan.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			// Further filtering out enemies that are not within turret range.
			if(curDistance < turretSquare.range){
				if(curDistance < distance){
					nearest = enemyWithinTurretSpan;
					distance = curDistance;
				}
			}
		}
		Debug.Log("Enemy is " + distance + " units away, turret range is " + turretSquare.range);
		return nearest;
	}

	private bool manualSelection(){
		// TODO
		return false;
	}
	
	private void fire()
	{
		GameObject bulletInstance = Instantiate(Resources.Load ("Prefabs/Bullet"), transform.position, this.transform.rotation) as GameObject;

		BulletController bulletController = bulletInstance.GetComponent<BulletController>();
		bulletController.Init(DefaultValues.DEFAULT_HOSTILE_LAYER, turretSquare.damage);

		Rigidbody2D bulletRigidbody = bulletInstance.GetComponent<Rigidbody2D>();
		bulletRigidbody.AddForce(transform.up * bulletSpeed);

		Destroy (bulletInstance, DefaultValues.DEFAULT_BULLET_SELF_DESTRUCTION_TIME);
	}
}
