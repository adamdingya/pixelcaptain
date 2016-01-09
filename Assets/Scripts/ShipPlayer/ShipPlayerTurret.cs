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

		if (turretType == Turret.Type.Small)
			spriteRenderer.sprite = Game_Manager.instance.sprTurrets[0];
        if (turretType == Turret.Type.Medium)
            spriteRenderer.sprite = Game_Manager.instance.sprTurrets[1];
        if (turretType == Turret.Type.Large)
            spriteRenderer.sprite = Game_Manager.instance.sprTurrets[2];

        transform.name = mountPixel.transform.name + "'s " + turretType + " Turret";
    }
	
}
