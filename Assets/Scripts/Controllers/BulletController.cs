using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float bulletDamage;
	public int aimedAtLayer;

	public void Init(int aimedAtLayer, float bulletDamage)
	{
		this.aimedAtLayer = aimedAtLayer;
		this.bulletDamage = bulletDamage;	
	}

	void OnTriggerEnter2D(Collider2D collider){
		if (collider.gameObject.layer == aimedAtLayer){
			IPixelProperties pixelProperties = collider.gameObject.GetComponent<IPixelProperties>();
			if(pixelProperties != null){
				pixelProperties.setHealth(pixelProperties.getHealth() - bulletDamage); 
			} else {
				Debug.LogError("Colliding game object does not implement IPixelProperties");
			}
			Destroy (this.gameObject);
		}
	}
}
