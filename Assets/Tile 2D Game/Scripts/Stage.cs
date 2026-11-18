using UnityEngine;
using UnityEngine.InputSystem;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    public int viewWidth = 5;
    public int viewHeight = 5;

    public Vector2 tileSize = new Vector2(16, 16);

    public Sprite[] islandSprites;
    public Sprite[] fogSprite;

    private bool[] revealed;

    private Map map;

    public Map Map => map;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.3f;
    public float erodeIterations = 2;

    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;

    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;

    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;

    [Range(0f, 0.9f)]
    public float moutainPercent = 0.1f;

    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;

    [Range(0f, 0.9f)]
    public float monsterPercent = 0.1f;


    private void Awake()
    {
        ResetStage();
    }


    [ContextMenu("Reset Stage")]
    //맵 재생성
    public void ResetStage()
    {
        map = new Map(); //새 맵 생성
        bool succeed = false;
        int maxAttemp = 10; //무한루프 방지
        for (int i = 0; i < maxAttemp && !succeed; ++i)
        {
            map.Init(mapHeight, mapWidth);//맵 초기화
            succeed = map.CreateIsland( //성공했다면 succeed에 true 담김
                erodePercent,
                erodeIterations,
                lakePercent,
                treePercent,
                hillPercent,
                moutainPercent,
                townPercent,
                monsterPercent
            );
        }

        if (!succeed)
        {
            Debug.LogError("맵 생성 실패");
            map = null;
            return;
        }

        CreateGrid();
        CreatePlayer();
    }

    public Vector3 firstTilePos;

    public GameObject playerPrefab;
    private Player player;


    private void CreatePlayer() //캐릭터 새로 생성
    {
        if(player != null) //플레이어가 존재한다 = 제거함
        {
            Destroy(player.gameObject);
            player = null;
        }

        //Instantiate(playerPrefab, GetTilePos(map.startTile.id), Quaternion.identity);

        var newGo = Instantiate(playerPrefab); //플레이어프리팹 생성
        player = newGo.GetComponent<Player>(); // 생성한 오브젝트에 붙어 있는 Player 스크립트를 꺼내 player에 보관
    }

    private void CreateGrid()
    {
        if (tileObjs != null) // tileObjs가 존재한다?
        {
            foreach (var tile in tileObjs) 
            {
                Destroy(tile.gameObject); //겹치지 않게 싹 비움
            }
        }
        tileObjs = new GameObject[mapWidth * mapHeight];
        revealed = new bool[mapWidth * mapHeight]; //이게 왜안됨?

        Vector3 pos = firstTilePos;
        float startX = pos.x;
        for (int i = 0; i < mapHeight; ++i)
        {
            for (int j = 0; j < mapWidth; ++j)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.position = pos;
                pos.x += tileSize.x;

                newGo.name = $"Tile {tileId} ({i}, {j})";
                tileObjs[tileId] = newGo;

                DecorateTile(tileId);
                newGo.GetComponent<SpriteRenderer>().enabled = false;
            }
            pos.x = startX;
            pos.y -= tileSize.y;
        }
    }


    public void OnTileVisted(int tileId)
    {
        int centerY = tileId / mapWidth; // !@#1 ???????
        int centerX = tileId % mapWidth; // !@#2 ???????? 이게 아까 그 칸수구하는 그거임?

        // 2. 5×5의 절반
        int halfW = viewWidth / 2;
        int halfH = viewHeight / 2;

        for (int y = centerY - halfH - 1; y <= centerY + halfH + 1; ++y)
        {
            for (int x = centerX - halfW - 1; x <= centerX + halfW + 1; ++x)
            {
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight) //개씨발 Height는 왜 되는데 애미 씨빨
                    continue;

                // 5. 행, 열 → 칸 번호
                int id = y * mapWidth + x;
                var ren = tileObjs[id].GetComponent<SpriteRenderer>();

                // 6. 안쪽 5×5인가?
                bool inside = Mathf.Abs(x - centerX) <= halfW
                           && Mathf.Abs(y - centerY) <= halfH;

                if (inside)
                {
                    revealed[id] = true;    // 봤다고 기억 아니 봤잖아
                    DecorateTile(id);               // 원래 그림으로 되돌리기
                    ren.enabled = true;
                }
                else if (!revealed[id])  // 테두리인데 아직 안 본 칸만
                {
                    ren.sprite = fogSprite[0];
                    ren.enabled = true;
                }
            }
        }
    }





    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }
    }

    public int ScreenPosTileId(Vector3 screenPos, bool clamp = true)
    {
        Camera cam = Camera.main;
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        return WorldPosToTileId(Camera.main.ScreenToWorldPoint(screenPos), clamp);
    }

    public int WorldPosToTileId(Vector3 worldPos, bool clamp = true)
    {
        int x = Mathf.FloorToInt((worldPos.x - firstTilePos.x) / tileSize.x + 0.5f);
        int y = Mathf.FloorToInt((firstTilePos.y - worldPos.y) / tileSize.y + 0.5f);

        if (clamp)
        {
            x = Mathf.Clamp(x, 0, mapWidth - 1);
            y = Mathf.Clamp(y, 0, mapHeight - 1);
        }
        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        return firstTilePos + new Vector3(x * tileSize.x, -y * tileSize.y);
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }
}
