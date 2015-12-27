using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;

    public bool SOUND_ENABLED;

    public enum GameSFX {SelectTool, PlacePixel, PanZoomBegin, PanZoomEnd};
    public GameSFX gameSFX;

    public AudioClip selectToolSFX;
    public AudioClip placePixelSFX;
    public AudioClip panZoomBeginSFX;
    public AudioClip panZoomEndSFX;

	void Start(){
		if (instance == null) {			
			instance = this;	
		} else if (instance != this) {			
			Destroy (gameObject);    
		}
		DontDestroyOnLoad(gameObject);
	}

    public void PlaySFX(GameSFX _gameSFX)
    {
        if (SOUND_ENABLED)
        {
            if (_gameSFX == GameSFX.PanZoomBegin)
                AudioSource.PlayClipAtPoint(panZoomBeginSFX, GameManager.instance.GetComponent<Camera>().transform.position);
            else if (_gameSFX == GameSFX.PanZoomEnd)
				AudioSource.PlayClipAtPoint(panZoomBeginSFX, GameManager.instance.GetComponent<Camera>().transform.position);
            else if (_gameSFX == GameSFX.PlacePixel)
				AudioSource.PlayClipAtPoint(placePixelSFX, GameManager.instance.GetComponent<Camera>().transform.position);
            else if (_gameSFX == GameSFX.SelectTool)
				AudioSource.PlayClipAtPoint(selectToolSFX, GameManager.instance.GetComponent<Camera>().transform.position);
        }       
    }
}
