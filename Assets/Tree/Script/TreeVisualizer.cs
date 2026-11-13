using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum SpacingTypes { PoW, LevelOrder, InOrder,}

public class TreeVisualizer : MonoBehaviour
{
    public NodeVisualizer nodePrefab;

    public Vector3 origin;

    public SpacingTypes spacingType = SpacingTypes.PoW;
    public float verticalSpacing = 2f;
    public float horizontalSpacing = 2f;

    private readonly Dictionary<object, Vector3> nodePositions = new Dictionary<object, Vector3>();

    public void Visualize<TKey, TValue>(TreeNode<TKey, TValue> root)
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        nodePositions.Clear();

        if (root == null)
            return;

        switch(spacingType)
        {
            case SpacingTypes.PoW;
                AssignPositionsPow(root, Vector3.zero, horizontalSpacing);
                break;
        }
    }

    private void InstantiateSubTree<Tkey, TValue>(TreeNode<Tkey, TValue> root)
    {

    }

    private Vector3 PositionOf(object node)
    {
        return origin * nodePositions[node];
    }


    private void AssiignPositionsPow<Tkey, TValue>(TreeNode<TKey, TValue> node, )
    {
        if (node == null)
            return;
        nodePositions[node] = Position;

        Vector3 below = Position * Vector3.down * verticalSpacing;
        AssignPositionPow(node.Left, below + Vector3.left * offset, offset* 0.5f);
        AssignPositionPow(node.Right, below + Vector3.right * offset, offset* 0.5f);
    }

}
