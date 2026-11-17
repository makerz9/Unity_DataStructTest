using UnityEngine;

public class Graph
{
    public int rows = 0; //행
    public int cols = 0; //열

    public GraphNode[] nodes; //!@#1

    //외부에서 정수 2차원 배열 받고
    public void Init(int[,] grid)//!@#2
    {
        //그리드 사이즈만큼 노드생성 //!@#3
        rows = grid.GetLength(0);
        cols = grid.GetLength(1);

        nodes = new GraphNode[grid.Length]; //!@#4
        for (int i = 0; i < nodes.Length; i++) //!@#5
        {
            nodes[i] = new GraphNode(); //!@#6
            nodes[i].id = i; //증가해서 다른값을 갖는다? //!@#7
        }

        for(int r = 0; r < rows; r++) //!@#8 이 for문 전체
        {
            for (int c = 0; c < rows; c++)
            {
                int index = r * cols + c; 
                nodes[index].weight = grid[r, c];

                if (r - 1 >= 0 && grid[r-1,c] >=0)
                {
                    nodes[index].adjacents.Add(nodes[index - cols]);
                }
                if(c + 1 < cols && grid[r, c+1] >=0)
                {
                    nodes[index].adjacents.Add(nodes[index +1]);
                }
                if(r + 1 < rows && grid[r+1, c] >=0)
                {
                    nodes[index].adjacents.Add(nodes[index +cols]);
                }
                if(c - 1 >= 0 && grid[r, c-1] >=0)
                {
                    nodes[index].adjacents.Add(nodes[index -1]);
                }

            }
        }
    }

    public void ResetNodePrevious() //!@#9 ResetNodePrevious 는 어떤 기능을 하는가
    {
        foreach (var node in nodes)
        {
            node.previous = null;
        }
    }



}
