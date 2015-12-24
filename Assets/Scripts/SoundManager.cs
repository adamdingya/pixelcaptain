using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    GameManager game;

    public bool SOUND_ENABLED;

    public enum GameSFX {SelectTool, PlacePixel, PanZoomBegin, PanZoomEnd};
    public GameSFX gameSFX;

    public AudioClip selectToolSFX;
    public AudioClip placePixelSFX;
    public AudioClip panZoomBeginSFX;
    public AudioClip panZoomEndSFX;

    public void Init(GameManager _game)
    {
        game = _game;
    }

    public void PlaySFX(GameSFX _gameSFX)
    {
        if (SOUND_ENABLED)
        {
            if (_gameSFX == GameSFX.PanZoomBegin)
                AudioSource.PlayClipAtPoint(panZoomBeginSFX, game.cam.transform.position);
            else if (_gameSFX == GameSFX.PanZoomEnd)
                AudioSource.PlayClipAtPoint(panZoomBeginSFX, game.cam.transform.position);
            else if (_gameSFX == GameSFX.PlacePixel)
                AudioSource.PlayClipAtPoint(placePixelSFX, game.cam.transform.position);
            else if (_gameSFX == GameSFX.SelectTool)
                AudioSource.PlayClipAtPoint(selectToolSFX, game.cam.transform.position);
        }
        
    }

}
