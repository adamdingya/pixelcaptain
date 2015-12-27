using UnityEngine;
using System.Collections;

public class ShipPlayerTurret : MonoBehaviour
{
    public Turret.Type type;
    public SpriteRenderer spriteRenderer;
    public ShipPlayerPixel mountPixel;

    public void init(ShipPlayerPixel mountPixel)
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turrets";

        if (type == Turret.Type.Small)
        {
			spriteRenderer.sprite = GameManager.instance.spriteTurret[Random.Range(0, GameManager.instance.spriteTurret.Length)];
        }

        transform.name = mountPixel.transform.name + "'s " + type + " Turret";
    }
	
}
