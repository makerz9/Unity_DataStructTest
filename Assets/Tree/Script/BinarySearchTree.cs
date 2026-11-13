using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Tilemaps.TilemapRenderer;



//TKey 트리 키값이라 크기구분 가능해야함
public class BinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : IComparable<TKey>
{
    //where 사용 일반화 제약주기
    protected TreeNode<TKey, TValue> root;

    //생성자
    public BinarySearchTree()
    {
        root = null;
    }

    public TValue this[TKey key]
    { 
        get
        {
            if(TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"키 {key}를 찾을 수 없습니다.");
            }
        }
        set
        {
            root = AddOrUpdate(root, key, value);
        }
    }

    public ICollection<TKey> Keys => InOrderTraversal().Select(kvp => kvp.Key).ToList(); //linq연산자를?

    public ICollection<TValue> Values => InOrderTraversal().Select(kvp => kvp.Value).ToList();

    //public int Count => CountNodes(root);
    //public virtual int CountNodes(TreeNode<TKey, TValue> node)
    //{
    //    if (node == null)
    //        return 0;
    //    ///////////////////////////////////////////////////////////////////////return 1 = + CountNodes(node.Left) + CountNodes(node.Right);
    //}

    public bool IsReadOnly => false;

    public int Count => throw new NotImplementedException();

    public virtual void Add(TKey key, TValue value)
    {
        

    }

    protected virtual TreeNode<TKey, TValue> Add(
        TreeNode<TKey, TValue> node,
        TKey key,
        TValue value
        )
    {
        if(node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);
        if(compare < 0)
        {
            node.Left = Add(node.Left, key, value);
        }
        else if (compare > 0)
        {
            node.Right = Add(node.Right, key, value);
        }
        else
        {
            throw new ArgumentException($"키 {key}는 이미 존재합니다.");
        }
        return node;

    }

    public virtual void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }




    protected virtual TreeNode<TKey, TValue> AddOrUpdate(
    TreeNode<TKey, TValue> node,
    TKey key,
    TValue value
    )
    {
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            node.Left = Add(node.Left, key, value);
        }
        else if (compare > 0)
        {
            node.Right = Add(node.Right, key, value);
        }
        else
        {
            node.Value = value;
        }
        UpdateHeight(node);
        return node;

    }

    protected virtual void UpdateHeight(TreeNode<TKey, TValue> node)
    {
        node.Height = Mathf.Max(Height(node.Left), Height(node.Right)) + 1;
    }


    protected virtual int Height(TreeNode<TKey, TValue> node)
    {
        return node == null ? 0 : node.Height;
    }




    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    public bool Contains(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public bool ContainsKey(TKey key)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if(array == null)
            throw new ArgumentException(nameof(array));
        if(arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));  
        if(array.Length - arrayIndex < 0)



        foreach (var kvp in this)
        {
            array[arrayIndex++] = kvp;
        }



    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        //throw new System.NotImplementedException();
        //return InOrderTraversal(root);
        return InOrderTraversal().GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        //throw new System.NotImplementedException();
        if (!ContainsKey(key))
            return false;

        root = Remove(root, key);
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    protected virtual TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        if(node == null)
        {
            return node;
        }

        int compare = key.CompareTo(node.Key);
        if(compare < 0)
        {
            node.Left = Remove(node.Left, key);
        }
        else if (compare > 0)
        {
            node.Right = Remove(node.Right, key);
        }
        else
        {
            if(node.Left == null)
            {
                return node.Right;
            }
            if(node.Right == null)
            {
                return node.Left;
            }

            var minNode = FindMin(node.Right);
            node.Key = minNode.Key;
            node.Value = minNode.Value;

            node.Right = Remove(node.Right, minNode.Key);

        }

        UpdateHeight(node);
        return node;
    }

    protected virtual TreeNode<TKey, TValue> FindMin(TreeNode<TKey, TValue> node)
    {
        while(node.Left != null)
        {
            node = node.Left;
        }
        return node;

    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return TryGetValue(root, key, out value);
    }

    protected virtual bool TryGetValue(TreeNode<TKey, TValue> node, TKey key, out TValue value)
    {
        if(node == null)
        {
            value = default;
            return false;
        }

        int compare = key.CompareTo(node.Key);
        if(compare == 0)
        {
            value = node.Value;
            return true;
        }
        else if(compare <0)
        {
            return TryGetValue(node.Left, key, out value);

        }
        else
        {
            return TryGetValue(node.Left, key, out value);
        }

    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    private void PreOrder(TreeNode<TKey, TValue> node, List<KeyValuePair<TKey, TValue>> result)
    {
        if (node == null) return;
        result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
        PreOrder(node.Left, result);
        PreOrder(node.Right, result);
    }

    //PreOrder	전위	나 → 왼 → 오
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            foreach (var kvp in PreOrderTraversal(node.Left))
            {
                yield return kvp;
            }
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            foreach (var kvp in PreOrderTraversal(node.Right))
            {
                yield return kvp;
            }
        }
    }


    private void InOrder(TreeNode<TKey, TValue> node, List<KeyValuePair<TKey, TValue>> result)
    {
        ////전위 - 나 왼 오
        //if (node == null) return;
        //PreOrder(node.Left, result);
        //result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
        //PreOrder(node.Right, result);

        return InOrderTraversal(root);
    }


    //InOrder	중위	왼 → 나 → 오
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(TreeNode<TKey, TValue>node) //이거는 매개변수 기능 없다고도 하심
    {
        //var result = new List<KeyValuePair<TKey, TValue>>();
        //InOrder(root, result);
        //return result;
        if(node != null)
        {
            foreach (var kvp in InOrderTraversal(node.Left))
            {
                yield return kvp;
            }
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            foreach (var kvp in InOrderTraversal(node.Right))
            {
                yield return kvp;
            }
        }


    }


    //PostOrder 후위  왼 → 오 → 나
    private void PostOrder(TreeNode<TKey, TValue> node, List<KeyValuePair<TKey, TValue>> result)
    {
        //전위 - 나 왼 오
        if (node == null) return;
        
        PreOrder(node.Left, result);
        PreOrder(node.Right, result);
        result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
    }


    //후위 - 왼 오 나
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal()
    {
        var result = new List<KeyValuePair<TKey, TValue>>();
        PostOrder(root, result);
        return result;
    }

    //??? 계층구조?
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal(
        TreeNode<TKey, TValue> node)
    {
        //throw new NotImplementedException();
        if(node == null)
        {
            yield break;
        }

        var queue = new Queue<TreeNode<TKey, TValue>>();
        queue.Enqueue(node);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);

            if(current.Left != null)
            {
                queue.Enqueue(current.Left);
            }
            if(current.Right != null)
            {
                queue.Enqueue(current.Right);
            }
        }
    }

}
