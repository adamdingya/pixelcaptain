using UnityEngine;
using System.Collections;

public class PixelController : MonoBehaviour {

	public IPixelProperties pixelProperties;
	public ShipBuilder_Manager shipBuilderManager;

	// Use this for initialization
	void Start () {
		pixelProperties = this.gameObject.GetComponent<IPixelProperties>();
	}
	
	// Update is called once per frame
	void Update () {
		if(pixelProperties.getHealth() <= 0){
			Destroy(this.gameObject);
		}

		// Monitoring connection to the core pixel
		if (!stillConnectedToCore()){
			shipBuilderManager.CoreConnectionSearch();
		}
	}
		
	private bool stillConnectedToCore(){
		bool stillConnected = true;



		return stillConnected;
	}
}
