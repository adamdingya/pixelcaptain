using UnityEngine;
using System.Collections;
using System;

public class TurretController : MonoBehaviour
{	
	private Combat_TurretBehavior combatTurretBehaviour; 
	private TurretSquare turretSquare;
	public float turretAngleLeft;
	public float turretAngleRight;
	public Vector3 shipRotation;
	public float initialFacingAngle;
	public Vector3 targetAngle;
	private bool isFiring;
	
	private float bulletSpeed = DefaultValues.DEFAULT_TURRET_BULLET_SPEED;
	private float bulletSelfDestructionTime = DefaultValues.DEFAULT_BULLET_SELF_DESTRUCTION_TIME;
	private float bulletRateOfFire = DefaultValues.DEFAULT_TURRET_RATE_OF_FIRE;

	// Use this for initialization
	void Start()
	{
		combatTurretBehaviour = gameObject.GetComponent<Combat_TurretBehavior>();
		turretSquare = gameObject.GetComponent<TurretSquare> ();
		initialFacingAngle = combatTurretBehaviour.turretFacingAngle;
		isFiring = false;
	}
	
	// Update is called once per frame
	void Update()
	{
		/*
		if (DefaultValues.turretOn) {		
			if(manualSelection()){	
				//TODO
			} else {		
				ArrayList enemiesWithinTurretMountAngle = locateEnemiesWithinTurretMountAngle();
				if(enemiesWithinTurretMountAngle.Count>0){				
					GameObject nearestEnemy = calculateNearestEnemy(enemiesWithinTurretMountAngle);
					//TODO Rotate turret to track nearest enemy					
					if(enemyInTurretRange()){
						//TODO Fire on tracked enemy
					}	
				}	
			}

		*/
		if (DefaultValues.turretOn) {	

			if(manualSelection()){	
				//TODO
			} else {
				// As ship rotates, the turret facing angle changes as well.
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
						if(!isFiring){
							// invoke repeating should not be triggered on every frame!!!
							InvokeRepeating ("fire", 1f, bulletRateOfFire);
							isFiring = true;
						}
					} else {
						CancelInvoke();
						isFiring = false;
					}
				}
			
			}
		}
		


		


			/*
			// locking the turrent onto the pivot point of the player's ship
			//this.transform.position.Set(player.transform.position.x, player.transform.position.y, this.transform.position.z);
			
			if (enemies.Count != 0) {
				GameObject anEnemy = enemies[0] as GameObject;
				if(anEnemy==null){
					enemies.RemoveAt(0);
				} else {
					Quaternion rotation = Quaternion.LookRotation (anEnemy.transform.position - this.transform.position, this.transform.TransformDirection (Vector3.back));
					this.transform.rotation = new Quaternion (0f, 0f, rotation.z, rotation.w);
					if(!isFiring){
						InvokeRepeating ("fire", 1f, bulletRateOfFire);
						isFiring = true;
					}
				}
			} else {
				CancelInvoke();
				isFiring = false;
			}
		
		*/
		


	}

	private void calculateTurretSpan(){
		shipRotation = transform.root.eulerAngles;
		turretAngleLeft = shipRotation.z + initialFacingAngle + (combatTurretBehaviour.turretMountRange / 2);
		if (turretAngleLeft > 360) {
			turretAngleLeft = turretAngleLeft - 360;
		}
		turretAngleRight = shipRotation.z + combatTurretBehaviour.turretFacingAngle - (combatTurretBehaviour.turretMountRange / 2);
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

		//Filter out the list of enemies within turret span
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
			if(curDistance < distance){
				nearest = enemyWithinTurretSpan;
				distance = curDistance;
			}
		}
		return nearest;
	}

	/*
	private ArrayList withinTurretMountAngle(){
		ArrayList enemiesWithin = new ArrayList ();
		ArrayList enemies = getEnemiesOnMap ();
		foreach (GameObject enemy in enemies){
			double horizontalDistance = Math.Abs (enemy.transform.position.x - player.transform.position.x);
			double verticalDistance = Math.Abs (enemy.transform.position.y - player.transform.position.y);
			float angle = (float)Math.Atan(horizontalDistance / verticalDistance);
			if(angle <= (combatTurretBehaviour.turretFacingAngle/2)){
				enemiesWithin.Add(enemy);
			} 
		}
		return enemiesWithin; 
	}
	*/

	private bool manualSelection(){
		// TODO
		return false;
	}
	
	private void fire()
	{
		GameObject bulletInstance = Instantiate(Resources.Load ("Prefabs/Bullet"), transform.position, this.transform.rotation) as GameObject;
		Rigidbody2D bulletRigidbody = bulletInstance.GetComponent<Rigidbody2D>();
		bulletRigidbody.AddForce(transform.up * bulletSpeed);
		Destroy (bulletInstance, bulletSelfDestructionTime);
	}
}
