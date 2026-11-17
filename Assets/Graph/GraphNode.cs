using System.Collections.Generic;
using UnityEngine;


public class GraphNode
{

    public int id;

    public int weight = 1;

    //!@#0 무방향성 그래프? -> 잘 짝지어줘야함?
    public List<GraphNode> adjacents = new List<GraphNode>();

    public GraphNode previous = null; //!@#1 previous?


    public bool CanVisit => adjacents.Count > 0 && weight >= 0; //!@#2 뭐지이거




}
