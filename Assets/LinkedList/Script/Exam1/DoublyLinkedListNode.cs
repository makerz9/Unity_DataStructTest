using System;
using System.Collections;
using System.Collections.Generic;

public class DoublyLinkedListNode<T>
// 왜? 무엇이든 담기 위해서 T 자료형 선언 (왜 무엇이든 담으려하지?)
// ㄴ>리스트 코드를 한 번만 짜고 여러 곳에 재사용하려고
// 이게 없으면 내용같아도 타입다르면 타입마다 복사해야해서 미친듯이 불어남

{
    public T Value { get; set; }
    // 왜? 무엇이든 온다 했으니 그 값도 무엇이든 받을수 있게 맞춰줌

    public DoublyLinkedListNode<T> Next { get; set; }
    // 왜? 오른쪽 칸 주소를 적어두는 칸입니다. 실제로 이동하는 건 current = current.Next;처럼 그 칸을 읽어서 따라가는 코드예요.

    public DoublyLinkedListNode<T> Previous { get; set; }
    // 왜? 문제에서 추가하라고 함 + 이중연결리스트는 앞뒤로 이동해야해서 prev까지 필요

    public DoublyLinkedList<T> List { get; set; }
    // 왜? 나는 이 리스트 소속"이라고 명찰에 적어두는 것뿐이에요. 그 명찰을 ValidataNode가 검사

    public DoublyLinkedListNode(T value)
    {
        Value = value;
        // 왜? 잠깐 받은 값을 사라지기 전에 영구 칸에 옮겨 담는 것입니다.
        //왜? new 할 때 받은 값(value)은 생성자가 끝나면 사라지니까, 노드가 계속 들고 있을 칸(Value)에 옮겨 저장
    }
}