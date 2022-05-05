using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject grass;
    public GameObject mud;
    public GameObject asphalt;
    public GameObject ice;
    public GameObject default_tile;

    public List<List<Tile_Type>> tile_type = new List<List<Tile_Type>>();

    public enum Tile_Type
    {
        GRASS,
        MUD,
        ASPHALT,
        ICE,
        DEFAULT
    };

    // Start is called before the first frame update
    void Start()
    {
        Texture2D texture = Resources.Load<Texture2D>("map");

        for (int row = 0; row < texture.height; ++row)
        {
            tile_type.Add(new List<Tile_Type>());

            for (int col = 0; col < texture.width; ++col)
            {
                Color pixel = texture.GetPixel(col, row);

                GameObject spawned = null;

                // Mud
                if (pixel.r == 1.0f && pixel.g == 0 && pixel.b == 0)
                {
                    spawned = GameObject.Instantiate(mud);
                    tile_type[row].Add(Tile_Type.MUD);
                }
                // Grass
                else if (pixel.r == 0.0f && pixel.g == 1.0f && pixel.b == 0)
                {
                    spawned = GameObject.Instantiate(grass);
                    tile_type[row].Add(Tile_Type.GRASS);
                }
                // Asphalt
                else if (pixel.r == 0 && pixel.g == 0 && pixel.b == 0)
                {
                    spawned = GameObject.Instantiate(asphalt);
                    tile_type[row].Add(Tile_Type.ASPHALT);
                }
                // Ice
                else if (pixel.r == 0.0f && pixel.g == 1 && pixel.b == 1.0f)
                {
                    spawned = GameObject.Instantiate(ice);
                    tile_type[row].Add(Tile_Type.ICE);
                }
                else
                {
                    spawned = GameObject.Instantiate(default_tile);
                    tile_type[row].Add(Tile_Type.DEFAULT);
                }

                spawned.transform.position = new Vector3(col + 0.5f, row + 0.5f, 0.0f);
            }
        }
    }

    public Tile_Type GetSpeedForLocation(Vector3 position)
    {
        int row = Mathf.FloorToInt(position.y);
        int col = Mathf.FloorToInt(position.x);

        print(tile_type[row][col]);

        return tile_type[row][col];
    }

}
