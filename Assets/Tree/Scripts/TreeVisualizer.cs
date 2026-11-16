using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public enum SpacingTypes
{
    PoW,
    LevelOrder,
    InOrder,
}

public class TreeVisualizer : MonoBehaviour
{
    public NodeVisualizer nodePrefab;

    public SpacingTypes spacingType = SpacingTypes.PoW;
    public float verticalSpacing = 2f;
    public float horizontalSpacing = 2f;

    private readonly Dictionary<object, Vector3> nodePositions = new Dictionary<object, Vector3>();
    private Vector3 origin;

    public void Visualize<TKey, TValue>(TreeNode<TKey, TValue> root)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        nodePositions.Clear();

        if (root == null)
            return;

        switch (spacingType)
        {
            case SpacingTypes.PoW:
                AssignPositionsPow(
                    root,
                    Vector3.zero,
                    horizontalSpacing * Mathf.Pow(2, root.Height - 3)
                );
                break;
            case SpacingTypes.LevelOrder:   
                AssignPositionsLevelOrder(root);
                break;
            case SpacingTypes.InOrder:
                {
                    int xIndex = 0;
                    AssignPositionInorder(root, 0, ref xIndex);
                }
                
                break;

        }

        origin = transform.position - nodePositions[root];
        InstantiateSubTree(root);
    }

    private Vector3 PositionOf(object node)
    {
        return origin + nodePositions[node];
    }

    private void InstantiateSubTree<TKey, TValue>(TreeNode<TKey, TValue> node)
    {
        if (node == null)
            return;

        var view = Instantiate(nodePrefab, PositionOf(node), Quaternion.identity, transform);
        view.SetNode(node);
        if (node.Left != null)
        {
            view.SetEdge(view.leftLineRenderer, PositionOf(node.Left));
        }
        else
        {
            view.leftLineRenderer.enabled = false;
        }

        if (node.Right != null)
        {
            view.SetEdge(view.RightLineRenderer, PositionOf(node.Right));
        }
        else
        {
            view.RightLineRenderer.enabled = false;
        }

        InstantiateSubTree(node.Left);
        InstantiateSubTree(node.Right);
    }

    private void AssignPositionsPow<TKey, TValue>(
        TreeNode<TKey, TValue> node,
        Vector3 position,
        float offset
    )
    {
        Debug.Log(node);
        if (node == null)
            return;
        nodePositions[node] = position;

        Vector3 below = position + Vector3.down * verticalSpacing;
        AssignPositionsPow(node.Left, below + Vector3.left * offset, offset * 0.5f);
        AssignPositionsPow(node.Right, below + Vector3.right * offset, offset * 0.5f);
    }

    private void AssignPositionsLevelOrder<TKey, TValue> (TreeNode<TKey, TValue> root)
    {
        if (root == null)                 //  빈 트리면 그릴 게 없으니 그냥 종료
        {
            Debug.LogWarning("루트가 null이야! 트리에 노드가 안 들어갔어.");
            return;
        }
        var queue = new Queue<TreeNode<TKey, TValue>>();
        queue.Enqueue(root);

        for (int depth = 0; queue.Count > 0; depth++)
        {
            int count = queue.Count;
            for(int i = 0; i < count; i++)
            {
                var nodein = queue.Dequeue();
                float x = (i - (count - 1) * 0.5f) * horizontalSpacing;
                nodePositions[nodein] = new Vector3(x, -depth * verticalSpacing, 0f);

                if(nodein.Left != null)
                {
                    queue.Enqueue(nodein.Left);
                }
                if(nodein.Right != null)
                {
                    queue.Enqueue(nodein.Right);
                }
            }
        }

    }

    //중위순회
    private void AssignPositionInorder<TKey, TValue> (TreeNode<TKey, TValue> root, int depth, ref int xIndex)
    {
        if(root ==null)
            { return; }
        AssignPositionInorder(root.Left, depth + 1, ref xIndex);
        nodePositions[root] = new Vector3 (xIndex * horizontalSpacing, -depth * verticalSpacing, 0f);
        xIndex++;
        AssignPositionInorder(root.Right, depth + 1, ref xIndex);
    }
}
