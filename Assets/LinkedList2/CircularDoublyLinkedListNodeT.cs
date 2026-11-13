//using UnityEngine;

//public class CircularDoublyLinkedListNode<T>
//{
//    public T Value { get; set; }
//    public CircularDoublyLinkedListNode<T> List { get; set; }

//    // 리스트 내부 전용 링크 = 마지막 노드의 next가 처음 노드인 원형 연결
//    public CircularDoublyLinkedListNode<T> next;
//    public CircularDoublyLinkedListNode<T> prev;

//    // 사용자가 보는 링크 = 이음새(마지막 → 처음)에서 null을 돌려줘 원형을 숨긴다
//    public CircularDoublyLinkedListNode<T> Next => next == null || next == List.First ? null : next;
//    public CircularDoublyLinkedListNode<T> Previous => prev == null || this == List.First ? null : prev;

//    public CircularDoublyLinkedListNode(T value)
//    {
//        Value = value;
//    }
//}