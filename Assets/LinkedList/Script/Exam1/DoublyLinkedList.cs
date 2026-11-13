using System;
using System.Collections;
using System.Collections.Generic;

public class DoublyLinkedList<T> : IEnumerable<T>
{
    private DoublyLinkedListNode<T> head;
    // [1] 왜? 넥스트 선언한거 받아오기
    private DoublyLinkedListNode<T> tail;
    // [2] 왜? 프리비어스 선언한거 받아오기

    public int Count { get; private set; }

    public DoublyLinkedListNode<T> First => head;
    public DoublyLinkedListNode<T> Last => tail;

    // ── 앞으로 열거 ──
    public IEnumerator<T> GetEnumerator()
    {
        DoublyLinkedListNode<T> current = head;
        // [3] 잠깐 그냥 겟이뉴머면 넥스트네?
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
            // [4] 왜?
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // ── 뒤로 열거 ──
    public IEnumerable<T> Backward()
    {
        var current = tail;
        // [5] 뒤로가는거면 Previous
        while (current != null)
        {
            yield return current.Value;
            current = current.Previous;
            // [6] 왜?
        }
    }

    // ── 맨 앞에 추가 ──
    public DoublyLinkedListNode<T> AddFirst(T value)
    {
        var node = new DoublyLinkedListNode<T>(value);
        node.List = this;
        // [7] 왜?

        if (head == null)
        // [8] 왜?
        {
            head = node;
            tail = node;
        }
        else
        {
            node.Next = head;
            // [9] 왜?
            head.Previous = node;
            // [10] 왜?
            head = node;
        }

        Count++;
        return node;
    }

    // ── 맨 뒤에 추가 ──
    public DoublyLinkedListNode<T> AddLast(T value)
    {
        var node = new DoublyLinkedListNode<T>(value);
        node.List = this;

        if (tail == null)
        {
            head = node;
            tail = node;
        }
        else
        {
            node.Previous = tail; //새로 들어온 로드가 테일노드
            // [11] 왜?
            tail.Next = node;
            // [12] 왜?
            tail = node;
            // [13] 왜?
        }

        Count++;
        return node;
    }

    //public void Remove(DoublyLinkedListNode<T> node)
    //{
    //    ValidateNode(node);

    //    if(node.Previous == null)
    //    {
    //        head = node.Next;
    //    }

    //    if(node == tail)
    //    {

    //    }
    //}










}