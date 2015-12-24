using UnityEngine;
using System.Collections;

public class ShipPlayerTurret : MonoBehaviour
{
    GameManager game;

    public Turret.Type type;
    public SpriteRenderer spriteRenderer;
    public ShipPlayerPixel mountPixel;

    public void Init(GameManager _game, ShipPlayerPixel mountPixel)
    {
        game = _game;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turrets";

        if (type == Turret.Type.Small)
        {
            spriteRenderer.sprite = game.spriteTurret[Random.Range(0, game.spriteTurret.Length)];
        }

        transform.name = mountPixel.transform.name + "'s " + type + " Turret";
    }
	
}
