using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGraphNode : MonoBehaviour
{
    public Image image;
    public TMP_Text text;

    private GraphNode node;

    public void Reset() //!@#1 ?
    {
        SetColor(node.CanVisit ? Color.white : Color.gray);
        SetText($"ID: {node}\nWeight: {node.weight}");
    }

    public void SetNode(GraphNode node) //!@#2? 어디서 호출하고 받아오는거지
    {
        this.node = node;
    }

    public void SetColor(Color color) //!@#3? 어디서 호출하고 받아오는거지
    {
        image.color = color;
    }

    public void SetText(string text) //!@#4? 어디서 호출하고 받아오는거지
    {
        this.text.text = text;
    }


}
