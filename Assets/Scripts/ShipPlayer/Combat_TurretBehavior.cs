using UnityEngine;
using System.Collections;

public class Combat_TurretBehavior : MonoBehaviour
{
    public Turret.Type turretType;
    public SpriteRenderer spriteRenderer;
    public Combat_PixelBehavior mountPixel;

    public void init(Combat_PixelBehavior mountPixel)
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turret";

		if (turretType == Turret.Type.Small)
			spriteRenderer.sprite = Game_Manager.instance.sprTurrets[0];
        if (turretType == Turret.Type.Medium)
            spriteRenderer.sprite = Game_Manager.instance.sprTurrets[1];
        if (turretType == Turret.Type.Large)
            spriteRenderer.sprite = Game_Manager.instance.sprTurrets[2];

        transform.name = mountPixel.transform.name + "'s " + turretType + " Turret";
    }
	
}
