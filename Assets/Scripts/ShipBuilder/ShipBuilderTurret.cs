using UnityEngine;
using System.Collections;

// Last edited : Mat Hill 20/12/2015

public class ShipBuilderTurret : MonoBehaviour {

    /// <summary>
    /// This class defines the turrets that appear on the build grid.
    /// It holds information about the turret's type and where its mounted.
    /// It contains a method for deleting the turret.
    /// </summary>

    public ShipBuilderPixel mountPixel; //Reference to the pixel this turret is mounted to.
	public SpriteRenderer spriteRenderer; //Reference to this pixel's sprite renderer component.

    public enum Type {Small}; //Define the discrete types of turrets.
    public Type type;

    //Initialise
    public void Init(Type _type, ShipBuilderPixel _mountPixel)
    {
        //Pass in arguments.
        mountPixel = _mountPixel;
        type = _type;

        //Set parent and reset transforms.
        transform.parent = mountPixel.transform;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.name = _mountPixel.transform.name + ".Turret";

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Turrets";

        //Get type specific data.
        if (type == Type.Small)
        {
			spriteRenderer.sprite = GameManager.instance.spriteTurret[Random.Range(0, GameManager.instance.spriteTurret.Length - 1)];
        }
    }

    //Visibility
    public void UpdateVisibility(bool _show)
    {
        //Show the turret.
        if (_show)
             spriteRenderer.enabled = true;
        //Hide the turret.
        else
            spriteRenderer.enabled = false;

    }

    //Deletion Behavior
    public void Delete()
    {
        GameObject.Destroy(this.gameObject);
    }
}
