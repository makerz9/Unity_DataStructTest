using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HashTableBucketView : MonoBehaviour
{
    public Image backGroundImage;
    public TMP_Text indexText;
    public TMP_Text contentText;

    private int bucketIndex;

    private static readonly Color EmptyColor = Color.grey;
    private static readonly Color OccupiedColor = Color.green;
    private static readonly Color DeletedColor = Color.orange;

    public void SetIndex(int index)
    {
        bucketIndex = index;
        indexText.text = $"I: {bucketIndex}";
    }

    public void SetEmpty()
    {
        backGroundImage.color = EmptyColor;
        contentText.text = string.Empty;

    }

    public void SetOccupied(string key, string value)
    {
        backGroundImage.color = OccupiedColor;
        contentText.text = $"K: {key}\t\tV:{value}";

    }

    public void SetDeleted()
    {
        backGroundImage.color = DeletedColor;
        contentText.text = "(deleted)";

    }

    public void SetChainedItem(List<string> items)
    {
        if(items == null || items.Count ==0)
        {
            SetEmpty();
            return;
        }

        backGroundImage.color = OccupiedColor;
        contentText.text = string.Join(" ->", items);
    }




}
