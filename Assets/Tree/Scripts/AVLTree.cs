using System;
using UnityEngine;

public class AVLTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    // ① 삽입: BST처럼 넣고, 돌아오면서 Balance
    protected override TreeNode<TKey, TValue> Add
        (TreeNode < TKey, TValue> node, TKey tkey, TValue tvalue)
    {
        if(node == null)
            return new TreeNode<TKey, TValue>(tkey, tvalue); // 빈 자리 → 새 노드

        int cmp = tkey.CompareTo (tkey);

        if (cmp < 0)
            node.Left = Add(node.Left, tkey, tvalue); // 작으면 왼쪽
        else if (cmp > 0)
            node.Right = Add(node.Right, tkey, tvalue); // 크면 오른쪽
        else
            node.Value = tvalue; // 같으면 값 덮어쓰기

        return Balance(node); // ★ BST와의 유일한 차이: 돌아오면서 균형 검사
    }

    //  ② 높이
    protected int Height(TreeNode<TKey, TValue> node)
    {
        if (node == null) return 0;
        return 1 + Math.Max(Height(node.Left), Height(node.Right));
    }

    // ③ 균형 인수
    protected int BalanceFactor(TreeNode<TKey , TValue> node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);

    }

    //④ 균형 맞추기: 4가지 케이스
    protected TreeNode<TKey, TValue> Balance(TreeNode<TKey, TValue> node)
    {
        int balanceFactor = BalanceFactor(node);

        if(balanceFactor > 1)                   // 왼쪽이 무거움
        {
            if (BalanceFactor(node.Left) < 0)   // LR: 꺾여 있으면 먼저 펴기
                node.Left = RotateLeft(node.Left);
            return RotateRight(node);           // LL
        }

        if(balanceFactor < -1)                      // 오른쪽이 무거움
        {
            if(BalanceFactor(node.Right) > 0)
                node.Right = RotateRight(node.Right); // RL: 꺾여 있으면 먼저 펴기
            return RotateLeft(node);                // RR
        }

        return node;                                // 균형 OK
    }

    //⑥ 왼쪽 회전 (Right와 좌우만 반대)
    protected TreeNode<TKey, TValue> RotateLeft(TreeNode<TKey, TValue> node)
    {

        var rightChild = node.Right;
        var leftSubTreeOfRightChild = rightChild.Left;

        rightChild.Left = node;
        node.Right = leftSubTreeOfRightChild;
        UpdateHeight(node);
        UpdateHeight(rightChild);

        return rightChild;

        #region
        ////L의 오른쪽 자식(b)을 따로 보관
        //TreeNode<TKey, TValue> newRoot = node.Right.Left; // L (새 루트) / 왼쪽 자식이 새 루트
        //TreeNode<TKey, TValue> b = newRoot.Left; // L의 오른쪽 자식(b)을 따로 보관 / 왼쪽 자식이 새 루트

        ////L의 오른쪽 = P    
        //newRoot.Left = node;

        ////P의 왼쪽 = b --> //p의 왼쪽이 b일수가 없는데 혹시 이미 바꾼건가?
        //// 새 루트의 오른쪽 짐을 넘겨받음
        //node.Right = b;

        //UpdateHeight(node);       // P 먼저 (이제 자식)
        //UpdateHeight(newRoot);    // L 나중 (이제 부모)

        ////P와 L의 높이를 다시 계산 (자식 먼저, 그다음 부모)


        ////L을 새 루트로 반환
        //// 원래 노드는 오른쪽 아래로
        //return node;
        #endregion

    }

    protected TreeNode<TKey, TValue> RotateRight(TreeNode<TKey, TValue> node)
    {
        var leftChild = node.Left;
        var rightSubTreeOfRightChild = leftChild.Left;

        leftChild.Right = node;
        node.Left = rightSubTreeOfRightChild;
        UpdateHeight(node);
        UpdateHeight(leftChild);

        return leftChild;

        #region
        ////L의 오른쪽 자식(b)을 따로 보관
        //TreeNode<TKey, TValue> newRoot = node.Left.Right; // L (새 루트) / 왼쪽 자식이 새 루트
        //TreeNode<TKey, TValue> b = newRoot.Right; // L의 오른쪽 자식(b)을 따로 보관 / 왼쪽 자식이 새 루트

        ////L의 오른쪽 = P    
        //newRoot.Right = node;

        ////P의 왼쪽 = b --> //p의 왼쪽이 b일수가 없는데 혹시 이미 바꾼건가?
        //// 새 루트의 오른쪽 짐을 넘겨받음
        //node.Left = b;

        //UpdateHeight(node);       // P 먼저 (이제 자식)
        //UpdateHeight(newRoot);    // L 나중 (이제 부모)

        ////P와 L의 높이를 다시 계산 (자식 먼저, 그다음 부모)


        ////L을 새 루트로 반환
        //// 원래 노드는 오른쪽 아래로
        //return node;                                
        #endregion
    }



    protected TreeNode<TKey, TValue> RotateLefty(TreeNode<TKey, TValue> node)
    {
        int balanceFactor = BalanceFactor(node);
        if(balanceFactor > 1)
        {
            if(BalanceFactor(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
            }
            return RotateRight(node);
        }
        else if (balanceFactor < -1)
        {
            if (BalanceFactor(node.Right) < 0)
            {
                node.Right = RotateRight(node.Right);
            }
            return RotateLeft(node);
        }
        return node;

        
    }


}

