
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;



public class CircularDoublyLinkedList<T> : IEnumerable<T>
//그러네 CircularDoublyLinkedList 스크립트인데 왜 CircularDoublyLinkedList를 참조하지
{
    private CircularDoublyLinkedListNode<T> head;
    // [1] 왜? 넥스트 선언한거 받아오기

    // - [ ]  `tail` 필드를 없앤다.  진짜 없애???
    //private CircularDoublyLinkedListNode<T> tail;
    // [2] 왜? 프리비어스 선언한거 받아오기

    public int Count { get; private set; }

    public CircularDoublyLinkedListNode<T> First => head;
    public CircularDoublyLinkedListNode<T> Last => head?.prev;


    private void InsertBefore(CircularDoublyLinkedListNode<T> target, CircularDoublyLinkedListNode<T> newNode)
    {
        newNode.prev = target.prev;
        target.prev.next = newNode;
        newNode.next = target;

        target.prev = newNode;
    }

    //public CircularDoublyLinkedListNode<T> AddBefore(CircularDoublyLinkedListNode<T> node, T value)
    //{
    //    validataNode(node);

    //    if(node == head)
    //    {

    //    }
    //}


    // ── 앞으로 열거 ──
    public IEnumerator<T> GetEnumerator()
    {
        CircularDoublyLinkedListNode<T> start = head;
        var current = start;
        // [3] 잠깐 그냥 겟이뉴머면 넥스트네?
        while (current != null)
        {
            yield return current.Value;
            current = current.next;
            if (current == start) break;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // ── 뒤로 열거 ──
    public IEnumerable<T> Backward()
    {
        //var current = tail;
        //tail을 지웠으니 그에 상응하는 값을 넣어야하나?
        var start = head?.prev;
        var current = start;
        // [5] 뒤로가는거면 Previous
        while (current != null)
        {
            yield return current.Value;
            current = current.prev;
            if (current == start) break;
        }
    }

    // ── 맨 앞에 추가 ──
    public CircularDoublyLinkedListNode<T> AddFirst(T value)
    {
        var node = new CircularDoublyLinkedListNode<T>(value);
        node.List = this;
        // [7] 왜?

        if (head == null)
        // [8] 왜?
        {
            node.next = node;
            node.prev = node;
            head = node;
            //tail = node; 안쓰니까 지워
        }
        else
        {
            node.prev = head?.prev;
            head.prev.next = node;

            node.next = head;
            // [9] 왜?
            head.prev = node;
            // [10] 왜?
            head = node;
        }

        Count++;
        return node;
    }

    // ── 맨 뒤에 추가 ──
    public CircularDoublyLinkedListNode<T> AddLast(T value)
    {
        var node = new CircularDoublyLinkedListNode<T>(value);
        node.List = this;

        //if (head?.prev == null)
        if (head == null)
        {
            node.next = node;
            node.prev = node;

            head = node;
            //tail = node;
        }
        else
        {
            InsertBefore(head, node);
        }

        Count++;
        return node;
    }


    public CircularDoublyLinkedListNode<T> AddAfter(CircularDoublyLinkedListNode<T> node, T value)
    {
        ValidataNode(node);

        var newNode = new CircularDoublyLinkedListNode<T>(value);
        newNode.List = this;

        Count++;
        return newNode;

        //var newNode = new CircularDoublyLinkedListNode<T>(value);
    }

    public CircularDoublyLinkedListNode<T> AddBefore(CircularDoublyLinkedListNode<T> node, T value)
    {
        ValidataNode(node);

        var newNode = new CircularDoublyLinkedListNode<T>(value);
        newNode.List = this;

        if(node == head)
        {
            head = newNode;
        }
        Count++;
        return newNode;
    }



    public void Remove(CircularDoublyLinkedListNode<T> node)
    {
        ValidataNode(node);

        if(Count == 1)
        {
            head = null;
        }
        else
        {
            node.prev.next = node.next;
            node.prev.prev = node.prev;

            if(node == head)
            {
                head = node.next;
            }
        }

        node.next = null;
        node.prev = null;
        node.List = null;


        Count--;
    }

    private void ValidataNode(CircularDoublyLinkedListNode<T> node)
    {
        throw new NotImplementedException();
    }

    public CircularDoublyLinkedListNode<T> Find(T value)
    {
        var comparer = EqualityComparer<T>.Default;
        //CircularDoublyLinkedListNode<T> current = head;

        var start = head;
        var current = start;
        while (current != null)
        {
            if (comparer.Equals(current.Value, value))
            {
                return current;
            }
            current = current.next; //여기도 대문자 안되서
            if (current == start) break;
        }
        return null;
    }


    public void Clear()
    {
        var start = head;
        var current = start;

        for (var i = 0; i < Count; i++)
        {
            //var next = current.Next;
            var next = current.next;//대문자 쓰면 안됨
            current.next = null; //Next하니까 Doubly로 뜨네 
            current.prev = null;
            current.List = null;
            current = next;
        }


        head = null;
        Count = 0;
    }







}
