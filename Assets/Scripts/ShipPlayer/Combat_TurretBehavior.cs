using UnityEngine;
using System.Collections;

public class Combat_TurretBehavior : MonoBehaviour
{
    public Turret.Type turretType;
    public SpriteRenderer spriteRenderer;
    public Combat_PixelBehavior mountPixel;
	public float turretFacingAngle;
	public float turretMountRange;

    public void Init(Combat_PixelBehavior mountPixel)
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turret";

		if (turretType == Turret.Type.Small)
			spriteRenderer.sprite = Game_Manager.instance.sprTurrets[(int)Turret.Type.Small - 1];
        if (turretType == Turret.Type.Medium)
            spriteRenderer.sprite = Game_Manager.instance.sprTurrets[(int)Turret.Type.Medium - 1];
        if (turretType == Turret.Type.Large)
            spriteRenderer.sprite = Game_Manager.instance.sprTurrets[(int)Turret.Type.Large - 1];
        if (turretType == Turret.Type.Laser)
            spriteRenderer.sprite = Game_Manager.instance.sprTurrets[(int)Turret.Type.Laser - 1];

		this.mountPixel = mountPixel;
        transform.name = mountPixel.transform.name + "'s " + turretType + " Turret";
    }
	
}
