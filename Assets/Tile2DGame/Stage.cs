using UnityEngine;

public class Stage : MonoBehaviour
{

    public GameObject tilePrefab;
    public GameObject[] tileObjs;

    public int mapWidth = 20;

    public int mapHeight = 20;

    public Vector2 tileSize = new Vector2(16, 16);

    public Sprite[] islandSprites;

    private Map map;
    public Map Map => map;

    [ContextMenu("Reset Stage")]

    public void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        CreateGrid();
    }

    private void CreateGrid()
    {
        if(tileObjs != null)
        {
            foreach(var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapWidth * mapHeight];

        Vector3 pos = Vector3.zero;

        float startX = pos.x;

        for(int i = 0; i < mapHeight; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.position = pos;
                pos.x += tileSize.x;

                newGo.name = $"Tile {tileId} ({i}, {j})";
                tileObjs[tileId] = newGo;

                DecoreateTile(tileId);
            }
            pos.x = startX;
            pos.y -= tileSize.y;
        }
    }

    public void DecoreateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if(tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite= null;
        }
    }

}
