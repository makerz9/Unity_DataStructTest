using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GraphSearch
{

    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();

    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    //## DFS — 깊이 우선 탐색
    //가능한 한 "깊이" 먼저 내려간 뒤 되돌아옴**

    public void DFS(GraphNode node) // DFS - Stack
    {
        path.Clear();
        graph.ResetNodePrevious();
        var stack = new Stack<GraphNode>();
        var visited = new HashSet<GraphNode>();

        stack.Push(node);
        visited.Add(node);

        while (stack.Count > 0) //여기가 깊이 내려가는 부분인가?
        {
            var current = stack.Pop();
            path.Add(current);

            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                //stack.Push(node); !@# 왜 이러면 outofmemory가?

                stack.Push(adjacent); //!@# 이거랑
                visited.Add(adjacent); //이거 왜 node 적으니까 outofmemory 오류가 난거지?
            }
        }
    }


    //## BFS — 너비 우선 탐색
    //가까운 이웃을 "한 층씩" 훑은 뒤 다음 층으로 이동**
    public void BFS(GraphNode node) // BFS - Queue
    {
        path.Clear();
        graph.ResetNodePrevious();
        var queue = new Queue<GraphNode>();
        var visited = new HashSet<GraphNode>();

        queue.Enqueue(node);
        visited.Add(node);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);

            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                //stack.Push(node); !@# 왜 이러면 outofmemory가?

                queue.Enqueue(adjacent); //!@# 이거랑
                visited.Add(adjacent); //이거 왜 node 적으니까 outofmemory 오류가 난거지?
            }
        }
    }

    public void DFSRecursive(GraphNode node) //DFSR
    {
        path.Clear();
        graph.ResetNodePrevious();
        DFSRecursive(node, new HashSet<GraphNode> ());
    }

    //재귀함수부분
    protected void DFSRecursive(GraphNode node, HashSet<GraphNode> visited)
    {
        path.Add(node);
        visited.Add(node);
        foreach (var adjacent in node.adjacents)
        {
            if(!adjacent.CanVisit || visited.Contains(adjacent))
            {
                continue;
            }
            DFSRecursive(adjacent, visited);
        }
    }

    public bool PathFindingBFS(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var queue = new Queue<GraphNode>();
        var visted = new HashSet<GraphNode>();

        queue.Enqueue(startNode);
        visted.Add(startNode);

        bool success = false;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if(current == endNode)
            {
                success = true;
                break;
            }

            foreach (var adjacent in current.adjacents)
            {
                if(!adjacent.CanVisit || visted.Contains(adjacent))
                {
                    continue;
                }

                adjacent.previous = current;

                visted.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }

        if(!success)
        {
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }

        path.Reverse();
        return true;
    }

    public bool Dijkstra(GraphNode starNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visted = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];
        for(int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[starNode.id] = 0;
        pQueue.Enqueue (starNode, distances[starNode.id]);

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visted.Contains(currentNode))
                continue;
            if(currentNode == endNode)
            {
                success = true;
                break;
            }

            visted.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visted.Contains(adjacent))
                    continue;

                var newDistance = distances[currentNode.id] + adjacent.weight;
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = currentNode;
                    pQueue.Enqueue(adjacent, newDistance);
                }
            }
        }


        if (!success)
        {
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }

        path.Reverse();
        return true;

    }

    //휴리스틱
    private int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }


    public bool AStar(GraphNode starNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visted = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[starNode.id] = 0;
        pQueue.Enqueue(starNode, distances[starNode.id] + Heuristic(starNode, endNode));

        bool success = false;
        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visted.Contains(currentNode))
                continue;
            if (currentNode == endNode)
            {
                success = true;
                break;
            }

            visted.Add(currentNode);
            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visted.Contains(adjacent))
                    continue;

                var newDistance = distances[currentNode.id] + adjacent.weight;
                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = currentNode;
                    pQueue.Enqueue(adjacent, newDistance + Heuristic(starNode, endNode));
                }
            }
        }


        if (!success)
        {
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }

        path.Reverse();
        return true;

    }



}
