using UnityEngine;
using System.Collections;

// Last edited : Mat Hill 21/12/2015

public class ShipBuilderPixel : MonoBehaviour
{
    /// <summary>
    /// This class defines the pixels that appear on the build grid.
    /// It holds information about the pixel's type and position.
    /// It holds type specific information and a turret reference.
    /// It contains methods for deleting, hiding/showing & managing a turret.
    /// </summary>

    public ShipBuilderManager shipBuilder; //Reference to the manager.
    public SpriteRenderer spriteRenderer; //Reference to this pixel's sprite renderer component.

    public Vector2 coordinates; //Coordinates of this pixel.
    public bool hidden; //Pixel visibility (For hiding whilst previewing a new pixel placement).

    //Use an enumerator to describe the pixel type.
    public Pixel.Type type;

    //Define the ability to host a weapon and hold a reference to it.
    public bool canHaveTurret = false; //Type dependent.
    public ShipBuilderTurret turret = null;//Turret reference.

    //Initialise
    public void InitPixel(ShipBuilderManager _shipBuilderManager, Vector2 _coordinates, Pixel.Type _type)
    {
        //Pass in arguments.
        shipBuilder = _shipBuilderManager;
        coordinates = _coordinates;
        type = _type;

        shipBuilder.builderGrid.pixelsArray.SetValue(this, shipBuilder.builderGrid.CoordinateToIndex(coordinates)); //Store this pixel in the builder grid array.

        transform.parent = shipBuilder.builderGrid.transform; //Make this a child of the grid transform.
        transform.name = "Pixel_x" + coordinates.x + "y" + coordinates.y; //Name appropriately.
        transform.position = shipBuilder.builderGrid.CoordinateToPosition(coordinates); //Set the position based on coordinates.
        transform.localScale = new Vector3(1f, 1f, 1f); //Reset the scale.

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "GridPixel";

        //Get type specific data.
        if (type == Pixel.Type.Scrap)
        {
            spriteRenderer.sprite = shipBuilder.game.spritePixelScrap;
        }
        else if (type == Pixel.Type.Armour)
        {
            spriteRenderer.sprite = shipBuilder.game.spritePixelArmour;
        }
        else if (type == Pixel.Type.Power)
        {
            spriteRenderer.sprite = shipBuilder.game.spritePixelPower;
        }
        else if (type == Pixel.Type.Engine)
        {
            spriteRenderer.sprite = shipBuilder.game.spritePixelEngine;
        }
        else if (type == Pixel.Type.Hardpoint)
        {
            canHaveTurret = true;
            spriteRenderer.sprite = shipBuilder.game.spritePixelHardpoint;
        }
    }

    //Deletion Behavior
    public void Delete()
    {
        GameObject.Destroy(this.gameObject);
    }

    //Visibility
    public void UpdateVisibility(bool _show)
    {
        //Show the pixel & its turret.
        if (_show)
        {
            spriteRenderer.enabled = true;
            if (turret != null)
                turret.UpdateVisibility(_show);
        }
        //Hide the pixel & its turret.
        else
        {
            spriteRenderer.enabled = false;
            if (turret != null)
                turret.UpdateVisibility(_show);
        }
        
    }

    //Add a Turret
    public void AddTurret()
    {
        GameObject turretObj = new GameObject();
        turret = turretObj.AddComponent<ShipBuilderTurret>();
        turret.Init(shipBuilder.UI.turretType, this);
    }

    //Delete the Turret
    public void DeleteTurret()
    {
        turret.Delete();
    }

    //Update Every Frame
    public void OnUpdate()
    {

    }

}




