using UnityEngine;
using TMPro;

public class NodeVisualizer : MonoBehaviour
{
    public SpriteRenderer nodeRender;
    public TextMeshPro nodeText;
    public LineRenderer LineRenderer;
    public LineRenderer RightRenderer;

    public void SetNode<Tkey, TValue>(TreeNode<Tkey, TValue>node)
    {
        nodeText.text = $"K: {node.Key}\n{node.Value}\nH: {node.Height}";
    }

    public void SetEdge(LineRenderer line, Vector3 target)
    {
        Vector3 back = Vector3.forward * 0.1f;
        line.positionCount = 2;
        line.SetPosition(0, transform.position + back);
        line.SetPosition(1, target + back);
    }

}
