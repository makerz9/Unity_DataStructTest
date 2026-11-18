using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Tile currentTile;
    private Stage stage;
    private Animator animator;
    private bool isMoving;

    private Tile pendingTarget;

    private void Start()
    {
        var mapObject = GameObject.FindWithTag("Map"); // 씬에서 "Map" 태그가 붙은 오브젝트를 찾는다
        stage = mapObject.GetComponent<Stage>(); // !@#1 그 오브젝트에 붙어 있는 Stage 스크립트를 가져온다
                                                 // Stage 안의 함수들을 쓰려고)

        currentTile = stage.Map.startTile; // 시작할때는 현재칸 = 시작지점
        transform.position = stage.GetTilePos(currentTile.id); // 지금 위치  / 칸 번호(id)를 주면 위치를 알려주는 기능
        stage.OnTileVisted(currentTile.id); // !@#3 Stage에게 이 칸 밟음 전달

        animator = GetComponent<Animator>();
        animator.speed = 0f;
    }

    void Update()
    {
            //return;
        if (Input.GetMouseButtonDown(0))
        {
            var targetTileId = stage.ScreenPosTileId(Input.mousePosition);
            var target = stage.Map.tiles[targetTileId];
            if (!target.CanMove)
                return;

            pendingTarget = target;
            if(!isMoving)
            StartCoroutine(CoMovePlayer());

            #region
            //// 이동 중이면 클릭 무시
            //if (isMoving || currentTile == null)
            //{
            //    StopAllCoroutines();
            //    stage.OnTileVisted(currentTile.id); // !@#20 이 칸 밟았어" 알리기
            //}

            //// 1. 클릭 위치 → 목표 타일
            //int targetId = stage.ScreenPosTileId(Input.mousePosition); //!@#4 마우스 클릭위치 → 몇 번 칸인지id 구하기 (GetTilePos의 반대)
            //Tile target = stage.Map.tiles[targetId]; //!@#5 칸 번호 → 실제 Tile 꺼내기 (tiles 배열에서 그 번호 칸)

            //// 2. 현재 칸 → 목표 칸 A* 경로
            //List<Tile> path = stage.Map.PathFinding(currentTile, target);  // !@#5 A*로 "지금 칸 → 클릭한 칸" 가장 싼 길 구하기
            //                                                               //      결과: 밟고 갈 칸들의 목록 !@#5-1 A*로 설정한적이 없는데 어떻게 설정된거지

            //// 3. 갈 수 없거나 제자리 클릭이면 무시
            //if (path == null || path.Count < 2)  // !@#6 길이 없거나(null), 제자리 클릭이면(칸 1개) 그만
            //    return; //리턴되서 이동이 실행되지 않음

            //// 4. 경로 따라 이동 시작
            //StartCoroutine(CoMovePath(path));
            #endregion
        }
    }

    private IEnumerator CoMovePlayer()
    {
        isMoving = true;
        animator.speed = 1f;

        List<Tile> path = null;
        Tile pathTarget = null;
        int pathIndex = 0;

        while (pendingTarget != currentTile)
        {
            if(pathTarget != pendingTarget)
            {
                path = stage.Map.PathFinding(currentTile, pendingTarget);
                pathTarget = pendingTarget;
                pathIndex = 1;
            }

            if(path == null || pathIndex  >= path.Count)
            {
                break;
            }

            yield return CoMoveOneStep(path[pathIndex++]);
        }


        animator.speed = 0f;
        isMoving = false;

    }


    // ── 4. 한 칸씩 걷기 ──
    private IEnumerator CoMovePath(List<Tile> path)  // !@#7길 목록을 받아서 처음부터 끝까지 걷는 함수
    {
        isMoving = true;
        animator.speed = 1f;

        // path[0]은 지금 서 있는 칸
        for (int i = 1; i < path.Count; ++i)  // !@#8 목록의 첫 칸(0)은 지금 서 있는 칸이라 건너뛰고, 다음 칸부터
        {
            yield return StartCoroutine(CoMoveOneStep(path[i]));  // !@#9 이동 다할때까지 기다리기 -> 다음 칸
        }

        animator.speed = 0f;
        isMoving = false;
    }

    private IEnumerator CoMoveOneStep(Tile next)  // ── 5. 한 칸 이동 ──
    {
        Vector3 startPos = transform.position; // !@#10 시작지점 = 현재 위치
        Vector3 targetPos = stage.GetTilePos(next.id); // !@#11 !@#2와 같은 함수, 다른 칸
                                                       // !@#2 = 현재 칸의 화면 위치, 여기 = 다음 칸의 화면 위치 !@#11-2 그래서 이게 뭐하는건데

        float moveDuration = Vector3.Distance(startPos, targetPos) / moveSpeed; // !@#12  걸리는 시간 = 거리 ÷ 속도 (거속시)
        float elapsed = 0f;

        while (elapsed < moveDuration)  // !@#13 정해진 시간이 다 지날 때까지 반복 
        {
            elapsed += Time.deltaTime; //보간 떄문인가?  // !@#14 흐른 시간에 이번 프레임 시간 더한거
            float t = elapsed / moveDuration;  // !@#15
            transform.position = Vector3.Lerp(startPos, targetPos, t);  // !@#16 출발과 도착 사이 t만큼 지점으로 이동 ← 이게 보간
            yield return null;  // !@#17 이동 여기까지하고 반복
        }

        transform.position = targetPos; // !@#18 정확히 도착 위치에 딱 맞추기
        currentTile = next; // !@#19 이제 내가 서 있는 칸 = 방금 도착한 칸
        stage.OnTileVisted(currentTile.id); // !@#20 이 칸 밟았어" 알리기
    }
}