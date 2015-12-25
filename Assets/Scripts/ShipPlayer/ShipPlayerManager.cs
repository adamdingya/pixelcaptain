using UnityEngine;
using System.Collections;

public class ShipPlayerManager : MonoBehaviour
{

    GameManager game;
    InputManager input;
    SoundManager sound;

    public ShipPlayerPixel[] shipPixels; //Store all pixelsArray as an array.
    GameObject shipParent;

    public void Init(GameManager _game)
    {
        game = _game;
        input = game.input;
        sound = game.sound;

        shipPixels = new ShipPlayerPixel[game.gridSize * game.gridSize];
    }

    public void LoadShip(SavedPixel[] savedPixels)
    {
        Vector3 averagePosition = Vector3.zero;
        int averagePositionCount = 0;
        for (int i = 0; i < savedPixels.Length - 1; i++)
        {
            SavedPixel pixel = savedPixels[i];
            if (savedPixels[i] != null)
            {
                SavedPixel savedPixel = savedPixels[i];
                ShipPlayerPixel buildPixel = new GameObject().AddComponent<ShipPlayerPixel>();
                buildPixel.transform.name = "Pixel_x" + savedPixel.coordinates.x + "y" + savedPixel.coordinates.y;
                buildPixel.pixelType = savedPixel.pixelType;
                buildPixel.turretType = savedPixel.turretType;
                buildPixel.transform.position = savedPixel.coordinates + new Vector2(0.5f, 0.5f);
                buildPixel.coordinates = savedPixel.coordinates;
                buildPixel.index = i;
                buildPixel.Init(game, savedPixel.turretType);
                //Hardpoint & turret.
                if (buildPixel.pixelType == Pixel.Type.Hardpoint)
                {
                    if (buildPixel.turretType != Turret.Type.None)
                    {
                        ShipPlayerTurret turret;
                        turret = new GameObject().AddComponent<ShipPlayerTurret>();
                        turret.type = buildPixel.turretType;
                        turret.transform.position = buildPixel.transform.position;
                        turret.transform.parent = buildPixel.transform;
                        turret.Init(game, buildPixel);
                        buildPixel.turret = turret;
                    }
                }

                shipPixels[i] = buildPixel;
                averagePositionCount++;
                averagePosition += buildPixel.transform.position;

                if (pixel.pixelType == Pixel.Type.Hardpoint)
                    Debug.Log("Imported HardPoint Pixel (Turret: " + pixel.turretType + ") at position " + i + ".");
                else
                    Debug.Log("Imported " + pixel.pixelType + "Pixel at position " + i + ".");
            }
        }

        averagePosition /= averagePositionCount;
        shipParent = new GameObject();
        shipParent.transform.position = averagePosition;
        shipParent.transform.name = "ShipParent";

        //SetParents & Get surrounding pixels
        for (int i = 0; i < shipPixels.Length - 1; i++)
        {
            ShipPlayerPixel pixel = shipPixels[i];
            if (pixel != null)
            {
                pixel.transform.parent = shipParent.transform;
                pixel.GetSurroundingPixels();
            }
        }

        shipParent.transform.position = Vector3.zero;

		shipParent.AddComponent<ShipPlayerController> ();
		//TODO 


        Debug.Log("Succesfully Imported Ship!");
    }

}
