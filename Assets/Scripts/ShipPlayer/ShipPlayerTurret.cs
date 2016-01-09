using UnityEngine;
using System.Collections;

public class ShipPlayerTurret : MonoBehaviour
{
    public Turret.Type turretType;
    public SpriteRenderer spriteRenderer;
    public ShipPlayerPixel mountPixel;

    public void init(ShipPlayerPixel mountPixel)
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turrets";

		if (turretType == Turret.Type.Normal)
        {
			spriteRenderer.sprite = Game_Manager.instance.sprTurret[Random.Range(0, Game_Manager.instance.sprTurret.Length)];
        }

		transform.name = mountPixel.transform.name + "'s " + turretType + " Turret";
    }
	
}
