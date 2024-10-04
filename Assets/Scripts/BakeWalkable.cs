using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class BakeWalkable : MonoBehaviour
{
    // Start is called before the first frame update
    public bool bake = false;
    public Tilemap[] maps;
    public Tilemap decorationMap;
    [Header("Walkable")]
    public TileBase walkableTile;
    public Tilemap walkableMap;
    public Sprite[] walkableSprites = new Sprite[0];
    void Bake()
    {
        walkableMap.ClearAllTiles();
        var set = new HashSet<Sprite>(walkableSprites);
        foreach (Tilemap map in maps)
        {
            var position = new Vector3Int(0, 0, 0);
            for (int x = map.cellBounds.min.x; x <= map.cellBounds.max.x; x++)
            {
                position.x = x;
                for (int y = map.cellBounds.min.y; y <= map.cellBounds.max.y; y++)
                {
                    position.y = y;
                    var tile = map.GetSprite(position);
                    var decoration = decorationMap.GetSprite(position);
                    if (tile == null || decoration != null) continue;
                    if (set.Contains(tile))
                    {
                        walkableMap.SetTile(position, walkableTile);
                    }
                    else
                    {
                        walkableMap.SetTile(position, null);
                    }
                }
            }

        }

    }
    void Update()
    {
        if (bake)
        {
            Bake();
            bake = false;
        }
    }
}
