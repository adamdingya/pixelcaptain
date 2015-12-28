using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour
{	
	private GameObject player;
	private ArrayList enemies = new ArrayList();
	private bool isFiring;
		
	private float bulletSpeed = DefaultValues.DEFAULT_TURRET_BULLET_SPEED;
	private float bulletSelfDestructionTime = DefaultValues.DEFAULT_BULLET_SELF_DESTRUCTION_TIME;
	private float bulletRateOfFire = DefaultValues.DEFAULT_TURRET_RATE_OF_FIRE;

	public Rigidbody2D bullet;

	// Use this for initialization
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		enemies.AddRange (GameObject.FindGameObjectsWithTag("Enemy"));
		isFiring = false;
	}
	
	// Update is called once per frame
	void Update()
	{
		// locking the turrent onto the pivot point of the player's ship
		this.transform.position.Set(player.transform.position.x, player.transform.position.y, this.transform.position.z);

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
	}
	
	private void fire()
	{
		Vector3 bulletPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1);
		Rigidbody2D bulletInstance = Instantiate(bullet, bulletPosition, this.transform.rotation) as Rigidbody2D;
		bulletInstance.AddForce(transform.up * bulletSpeed);

		Destroy (bulletInstance.gameObject, bulletSelfDestructionTime);
	}
}
