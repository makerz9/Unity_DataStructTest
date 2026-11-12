
using System;
using System.Collections;
using System.Collections.Generic;

public class DoublyLinkedList<T> : IEnumerable<T>
{
    private DoublyLinkedListNode<T> head;
    private DoublyLinkedListNode<T> tail;

    public int Count { get; private set; }

    public DoublyLinkedListNode<T> First => head;
    public DoublyLinkedListNode<T> Last => tail;

    public IEnumerator<T> GetEnumerator()
    {
        DoublyLinkedListNode<T> current = head;
        while (current != null) //반대로 다음 칸(오른쪽)이 없을 때까지 이동
        {
            yield return current.Value;
            current = current.Next; //지금 공간에 다음칸으로 이동
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    //모든 연산에서 `Next`와 함께 `Previous`도 올바르게 유지한다.
    //? Previous 선언만 했지 기능 정의가 없는데
    //ㄴ> Previous는 기능이 아니라 "메모 칸"
    // Previous { get; set; } (노드 클래스) = 앞 칸 주소를 적어둘 빈칸
    // head.Previous = node; 같은 줄들 (각 메서드) = 그 빈칸에 실제로 주소를 적는 것
    //모든 연산에서 Previous도 올바르게 유지한다 = AddFirst, AddLast, AddAfter, AddBefore, Remove에서 .Previous = ...를 써

    //- `Backward()`를 추가한다: tail → head 순서로 값을 열거.
    //왜 tail -> head? 앞에서 뒤로가는 정방향은있음
    //근데 양방향(이중연결리스트라) 뒤로가는 역방향도 있어야함
    //그래서 끝 tail부터 앞부분 head까지 가는 Backward 따로만듬
    public IEnumerable<T> Backward() //앞으로는 어디있는데 -> 그냥 겟이뉴머가 Next떄문에 자동이래
    {
        var current = tail;
        while (current != null) //더 왼쪽이 없을때까지 이동
        {
            yield return current.Value;
            current = current.Previous;//지금 공간에 앞 칸(왼쪽)으로 이동
        }
        //!@# 인데 이게 뭐 어쨌다는거지 이게 왜 뒤로가는겨
    }



    public DoublyLinkedListNode<T> AddFirst(T value)
    {
        var node = new DoublyLinkedListNode<T>(value);
        node.List = this;

        //node.Next = head; //!@#2 왜 지워냐했나? -> 결과적으로 [0]이 자기 자신을 가리키는 고리
        //head = node;

        if (head == null)
        {
            head = node;
            tail = node;
        }
        else
        {
            node.Next = head; //새 노드의 다음 = 기존 head
            head.Previous = node; 
            head = node;
        }

        Count++;
        return node;
    }

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
            node.Previous = tail;
            tail.Next = node;
            tail = node;
        }
        Count++;
        return node;
    }

    public DoublyLinkedListNode<T> AddAfter(DoublyLinkedListNode<T> node, T value)
    {
        ValidataNode(node);

        if (node == tail)
        {
            return AddLast(value);
        }

        var newNode = new DoublyLinkedListNode<T>(value);
        newNode.List = this;

        newNode.Previous = node; //추가한 Previous
        newNode.Next = node.Next;
        node.Next.Previous = newNode;   // C가 B를 뒤돌아봄
        node.Next = newNode; // 마지막

        Count++;
        return newNode;
    }

    public DoublyLinkedListNode<T> AddBefore(DoublyLinkedListNode<T> node, T value)
    {
        ValidataNode(node);

        if (node == head)
        {
            return AddFirst(value);
        }

        //`AddBefore`, **O(1) * *로 만든다(순회 금지).
        //var previous = head;
        //while (previous.Next != node)
        //{
        //    previous = previous.Next;
        //}

        var newNode = new DoublyLinkedListNode<T>(value);
        newNode.List = this;

        newNode.Next = node;
        newNode.Previous = node.Previous;
        node.Previous.Next = newNode;
        node.Previous = newNode;

        Count++;
        return newNode;
    }

    public void RemoveFirst()
    {
        if (head == null)
        {
            throw new InvalidOperationException("리스트가 비어있습니다.");
        }
        Remove(head);
    }

    public void RemoveLast()
    {
        //`RemoveLast`를 **O(1)**로 만든다 (순회 금지).
        //!@# 원래 없는데?
        if (tail == null)
        {
            throw new InvalidOperationException("리스트가 비어있습니다.");
        }
        Remove(tail);// ← 여기서 Remove를 부름
        //여기엔 없더라도 여기서 호출되서 순회가일어났나?
    }

    public bool Remove(T value)
    {
        var node = Find(value);
        if (node == null)
        {
            return false;
        }
        Remove(node);
        return true;
    }

    public void Remove(DoublyLinkedListNode<T> node)
    {
        ValidataNode(node);

        //DoublyLinkedListNode<T> previous = null;
        //DoublyLinkedListNode<T> current = head;

        //`Remove(node)`, **O(1)**로 만든다 (순회 금지).
        //while (current != node)
        //{
        //    previous = current;
        //    current = current.Next;
        //}

        if (node.Previous == null)
        {
            head = node.Next;
        }
        else
        {
            node.Previous.Next = node.Next;
        }

        if (node.Next == null)                // 맨 뒤면
            tail = node.Previous;
        else
            node.Next.Previous = node.Previous;   // A ← C

        node.Next = null;
        //`Remove(node)`는 노드의 `Previous`도 `null`로 끊는다.
        //!@# 칸 지우는거랑 null이 뭔상관이지
        //그러니까 지금 지운 칸의 연결을 끊는건가? 앞 끊고 뒤 끊고 리스트도 없애고?
        node.Previous = null;   // 
        node.List = null;

        Count--;
    }

    public DoublyLinkedListNode<T> Find(T value)
    {
        var comparer = EqualityComparer<T>.Default;
        DoublyLinkedListNode<T> current = head;
        while (current != null)
        {
            if (comparer.Equals(current.Value, value))
            {
                return current;
            }
            current = current.Next;
        }
        return null;
    }

    public bool Contains(T value)
    {
        return Find(value) != null;
    }

    public void Clear()
    {
        var current = head;
        while (current != null)
        {
            var next = current.Next;
            current.Next = null;
            //- `Clear`는 노드의 `Previous`도 `null`로 끊는다.
            //연결끊기2
            //아 그러네 Remove는 하나 없애는거라 끊는거고
            //이건 아예 싹 비우는거라 끊는거네
            current.Previous = null;   //  추가
            current.List = null;
            current = next;
        }

        head = tail = null;
        Count = 0;
    }

    private void ValidataNode(DoublyLinkedListNode<T> node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }
        if (node.List != this)
        {
            throw new InvalidOperationException("이 리스트에 속한 노드가 아닙니다.");
        }
    }
}