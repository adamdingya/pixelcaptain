using UnityEngine;
using System.Collections;

// Last edited : Mat Hill 21/12/2015

public class ShipBuilderPreviewPixel : MonoBehaviour
{
    /// <summary>
    /// This class defines the preview object behavior.
    /// The preveiw object shows the potentiol pixel placement on the grid.
    /// It holds methods which allow the preview objectto be changed, hidden and shown.
    /// </summary>

    SpriteRenderer spriteRenderer;

    public bool hidden;

    //Initialise
    public void Init()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "PreviewPixel";
    }

    //Change Sprite
    public void UpdateSprite(Sprite _sprite)
    {
        spriteRenderer.sprite = _sprite;
    }

    //Visibility
    public void UpdateVisibility(bool _show)
    {
        //Show the pixel & its turret.
        if (_show)
        {
            hidden = false;
            spriteRenderer.enabled = true;
        }
        //Hide the pixel & its turret.
        else
        {
            hidden = true;
            spriteRenderer.enabled = false;
        }

    }
}
