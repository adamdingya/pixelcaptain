using UnityEngine;
using System.Collections;

public class TestTurret : MonoBehaviour {

	public float cooldown = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(cooldown <= 0){
			fire ();
			cooldown = 3;
		} else {
			cooldown = cooldown - Time.deltaTime;
		}
	}

	private void fire()
	{
		GameObject bulletInstance = Instantiate(Resources.Load ("Prefabs/Bullet"), transform.position, this.transform.rotation) as GameObject;
		
		BulletController bulletController = bulletInstance.AddComponent<BulletController>();
		bulletController.Init(DefaultValues.DEFAULT_FRIENDLY_LAYER, DefaultValues.DEFAULT_TURRET_DAMAGE);
		
		Rigidbody2D bulletRigidbody = bulletInstance.GetComponent<Rigidbody2D>();
		bulletRigidbody.AddForce(transform.up * DefaultValues.DEFAULT_TURRET_BULLET_SPEED);
		
		Destroy (bulletInstance, DefaultValues.DEFAULT_BULLET_SELF_DESTRUCTION_TIME);
	}
}
