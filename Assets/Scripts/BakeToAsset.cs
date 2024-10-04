using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;



public class NavigationMapBuilder
{
    HashSet<(int, int)> map = new();

    public void Set(int x, int y)
    {
        map.Add((x, y));
    }

    public void Unset(int x, int y)
    {
        map.Remove((x, y));
    }


    public void Clear()
    {
        map.Clear();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var p in map)
        {
            Debug.Log(p);
            builder.Append(p.Item1).Append(",").Append(p.Item2).Append("\n");
        }
        return builder.ToString();
    }

}

[ExecuteInEditMode]
public class BakeToAsset : MonoBehaviour
{
    // Start is called before the first frame update
    public bool bake = false;
    public Tilemap[] maps;
    public Tilemap decorationMap;
    public Sprite[] walkableSprites = new Sprite[0];
    public string filename = "walkable";

    NavigationMapBuilder walkableMap = new();


    void Save()
    {
        var path = Path.Join(Application.dataPath, $"{filename}.navmap");
        File.WriteAllText(path, walkableMap.ToString());
        Debug.Log("Saved Map to: " + path);
    }

    void Bake()
    {
        walkableMap.Clear();
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
                        walkableMap.Set(x, y);
                    }
                    else
                    {
                        walkableMap.Unset(x, y);
                    }
                }
            }

        }
        Save();

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
