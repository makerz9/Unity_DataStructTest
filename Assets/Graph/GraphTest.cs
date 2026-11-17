using System.Collections.Generic;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
    public enum Algorithm
    {
        DFS,
        BFS,
        DFSRecursive,
        PathFindingBFS,
        Dijkstra,
        AStar
    }

    public Algorithm algorithm;

    public int startIndex;
    public int endIndex;

    public UIGraphNode nodePrefab;
    public List<UIGraphNode> uiNodes;


    public Transform uiNoodRoot;

    private Graph graph;

    private void Start()
    {
        //int[,] map = new int[5, 5]; !@#1 왜 ;를 넣으라고 로그가 떴지?
        int[,] map = new int[5, 5] // 이차원배열 고정생성
        {
            { 1, -1, 1, 1, 1},
            { 2, -1, 3, 1, 1},
            { 1, -1, 8, 5, 1},
            { 3, -1, 1, 3, 1},
            { 1, 1, 1, 1, 1},
        };
        graph = new Graph(); //!@#2
        graph.Init(map); //!@#3
        InitUiNode(graph); //!@#4
    }

    private void InitUiNode(Graph graph) //!@#5
    {
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNoodRoot);
            uiNode.SetNode(node);
            uiNode.Reset();

            uiNodes.Add(uiNode);
        }
    }

    private void ResetUiNodes() //!@#6
    {
        foreach (var uiNode in uiNodes)
        {
            uiNode.Reset();
        }
    }

    [ContextMenu("Search")] //!@# ??
    public void Search()
    {
        if(graph == null)
        {
            return;
        }

        var search = new GraphSearch();
        search.Init(graph);

        switch(algorithm)
        {
            case Algorithm.DFS:
                search.DFS(graph.nodes[startIndex]);
                break;
            case Algorithm.BFS:
                search.BFS(graph.nodes[startIndex]);
                break;
            case Algorithm.DFSRecursive:
                search.DFSRecursive(graph.nodes[startIndex]);
                break;
            case Algorithm.PathFindingBFS:
                search.PathFindingBFS(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
            case Algorithm.Dijkstra:
                search.Dijkstra(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
            case Algorithm.AStar:
                search.AStar(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
        }

        ResetUiNodes();

        for(int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            float t = search.path.Count > 1 ? (float)i / (search.path.Count - 1) : 0f;
            var color = Color.Lerp(Color.red, Color.green, t) ;
            uiNodes[node.id].SetColor(color);
            uiNodes[node.id].SetText($"ID: {node.id}\nWeight: {node.weight}\n방문: {i}");
        }
    }
}
