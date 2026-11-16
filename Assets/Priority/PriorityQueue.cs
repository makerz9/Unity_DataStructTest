using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<TElement, TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap;
    private readonly IComparer<TPriority> comparer;

    public int Count => heap.Count;

    public PriorityQueue()
    {
        //(int v1, string v2) value = (0, "1");
        //value.Item1 = 10;
        //value.Item2 = "가나다";

        //value.v1 = 20;
        //value.v2 = "ABC";

        heap = new List<(TElement Element, TPriority Priority)>();
        comparer = Comparer<TPriority>.Default;

    }

    // 요구사항 2: 비교자를 받는 오버로드
    public PriorityQueue(IComparer<TPriority> comparer) //!@# comparer가 비교자?
    {
        heap = new List<(TElement Element, TPriority Priority)>();
        this.comparer = comparer;

        //this.comparer = comparer ?? Comparer<TPriority>.Default; // null이면 기본값 !@#1 ㅅㅂ 이건 또 뭔문법이야 아얘첨모는데 ㄱ시발
        //!@#2 this.comparer 매개변수도 comparer 원래도 comparer 있어서? 근데 왜 자기 comparer 인걸 명시하려고하는거지

    }

    public PriorityQueue(IEnumerable<(TElement TElement, TPriority priority)> items)
        :this(items, null)
    {


        //
        //heap = new List<(TElement,  TPriority Priority)>();
        //foreach (var item in items)
        //{
        //    Enqueue(item.Element, item.Priority);
        //}

        heap = new List<(TElement, TPriority)>(items);
        for(int i = heap.Count / 2 - 1; i >= 0; --i)
        {
            Heap
        }
    }




    public void Clear()
    {
        heap.Clear();
    }

    public TElement Peek() //!@#3 뭐라는거야 이건
    {
        if(heap.Count == 0)
        {
            throw new InvalidOperationException("큐가 비어 있습니다.");
        }
        return heap[0].Element;
    }

    private void Swap(int i, int j) //!@$4??? 왜 스왑함 갑자기?
    {
        var temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }

    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority)); //!@#5 바깥은 Add( ) 호출, 안쪽은 (원소, 우선순위)를 한 묶음으로 만드는 괄호예요
        //!@#6 그대로 옮긴 거예요. heap.Count - 1 = 신입이 앉은 맨 끝 자리 번호를 넘겨줘요
        HeapifyUp(heap.Count - 1);

    }


    public TElement Dequeue()
    {
        if(heap.Count == 0)
        {
            throw new InvalidOperationException("큐가 비어 있습니다.");
        }

        TElement result = heap[0].Element;//!@#7 사장 이름 적어두기. 곧 덮어쓰니까 미리 저장 -> !@#7-1 아 맨 처음이 루트지 루트 = 사장이라 생각하면?
        
        int lastIndex = heap.Count - 1;
        //!@#8 ??????????? 아니 Enququ는 HeapifyUp(heap.Count - 1); 이렇게했는데 왜
        //둘 다 "맨 끝 자리 번호"라는 뜻이에요. Enqueue는 신입 자리, Dequeue는 사장 자리로 옮길 사람 자리로 써요

        heap[0] = heap[lastIndex];//!@#9 끝 사람을 사장 자리에 앉히기

        heap.RemoveAt(lastIndex);//!@#10 끝 자리 지우기

        if (heap.Count > 0) HeapifyDown(0); //!@#11 마지막 1명을 뺀 거면 남은 사람이 없으니 내려보낼 게 없어요
        return result;//!@#12 적어둔 사장 반환 -> !@#12-1 어디로?
        //!@# 진짜 존나비직관적이네 뭐 
    }



    //// 요구사항 3: 비교는 필드 comparer만 사용 → 주입된 기준이 자동 반영
    ///!@#13 주입 이거 그냥 말만 거창하고 기존기능 함수로 떼어낸다음 그냥 함수호출하라는거 아님 결국?
    private void HeapifyUp(int index) //!@#14 뭐지 int index 이거 -> "몇 번 자리부터 승진을 시작할지"예요
    {
        
        while (index > 0)//!@#15 0번(사장)이 되면 더 올라갈 데가 없으니 멈추기
        {
            int parentIndex = (index - 1) / 2; //!@#16 위에서 본 상사 번호 공식

            //!@#17 "내가 상사보다 우선이 아니면" → 멈춤(break)
            if (comparer.Compare(heap[index].Priority, heap[parentIndex].Priority) >= 0)
            {
                break;
            }
            Swap(index, parentIndex);//!@#18 나랑 상사랑 자리 바꾸기예요.

            index = parentIndex + 1;//!@#19 바꿨으니 이제 내 자리는 상사가 있던 자리
        }
    }

    //!@#20 이건 그냥..모르겠다 ㅈㅈ
    public void HeapifyDown(int index)
    {


        //lastIndex = heap.Count - 1;
        int lastIndex = heap.Count - 1; //!@# 아니 왜 또 Up이랑 아얘 다르냐고
        //int index = 0; ..!@#21 이제 HeapifyDown(0)처럼 매개변수로 0을 넘겨줘서 필요 없어요
        while (true)
        {
            int left = 2 * index + 1;         // 왼쪽 +1
            int right = 2 * index + 2;        // 오른쪽 +2
            int best = index;

            if (left <= lastIndex && comparer.Compare(heap[left].Priority, heap[best].Priority) < 0)
                best = left;
            if (right <= lastIndex && comparer.Compare(heap[right].Priority, heap[best].Priority) < 0)
                best = right;

            if (best == index) break;
            Swap(index, best);
            index = best;
        }

    }

}
