using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float moveSpeed = 5f;
    private Tile currentTile;
    private Stage stage;

    private Animator animator;

    private bool isMoving;


    private void Start()
    {
        var mapObject = GameObject.FindWithTag("Map");
        stage = mapObject.GetComponent<Stage>();

        currentTile = stage.Map.startTile;
        transform.position = stage.GetTilePos(currentTile.id);
        stage.OnTileVisted(currentTile.id);

        animator = GetComponent<Animator>();
        animator.speed = 0f;

    }


    void Update()
    {

        if (isMoving || currentTile == null)
            return;

        int direction = -1;
        //if(Input.GetKeyDown(KeyCode.W))
        //{
        //    direction = (int)Sides.Top;
        //}
        //else if (Input.GetKeyDown(KeyCode.A))
        //{
        //    direction = (int)Sides.Left;
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //{
        //    direction = (int)Sides.Bottom;
        //}
        //else if (Input.GetKeyDown(KeyCode.D))
        //{
        //    direction = (int)Sides.Right;
        //}

        if (direction < 0)  
            return;

        var next = currentTile.adjacents[direction];
        if (next == null || !next.CanMove)
            return;

        StartCoroutine(CoMoveOneStep(next));

        //transform.position = stage.GetTilePos(next.id);
        //currentTile = next;
        //stage.OnTileVisted(currentTile.id);
    }

    private IEnumerator CoMoveOneStep(Tile next)
    {
        isMoving = true;
        animator.speed = 1f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = stage.GetTilePos(next.id);
        float moveDuration = Vector3.Distance(startPos, targetPos) / moveSpeed; //선형보간 거속시..?
        var elapsed = 0f;

        while(elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration; //!@# t가 0에서 1까지 등속도로..?
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        transform.position = stage.GetTilePos(next.id);
        currentTile = next;
        animator.speed = 0f;
        stage.OnTileVisted(currentTile.id);
        isMoving = false;
    }

}
