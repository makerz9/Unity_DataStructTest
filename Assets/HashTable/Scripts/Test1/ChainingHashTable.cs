using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 충돌 해결 = 버킷별 LinkedList 체인
public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;

    private LinkedList<KeyValuePair<TKey, TValue>>[] buckets;
    private int size;
    private int count;

    public ChainingHashTable()
    {
        buckets = new LinkedList<KeyValuePair<TKey, TValue>>[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
    }

    private int GetHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        // 부호 비트만 제거해 항상 0 이상. Math.Abs는 int.MinValue에서 음수를 그대로 돌려주므로 쓰지 않는다.
        return (hash & 0x7fffffff) % size;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException();
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if ((double)count / size > LoadFactor)
            {
                Resize();
            }

            int index = GetHash(key);
            // [빈칸 ⑤] 같은 키가 체인에 있으면 값을 바꾸고, 없으면 체인 끝에 추가한다
            //throw new NotImplementedException("빈칸 ⑤");
            //if(같은 키가 체인이 있는경우)
            //{

            //}
            //else
            //{
            //    list.AddLast(20);
            //}
            //KeyValuePair는 값만 바꿀 수 없어서 쪽지를 새로 만들어 끼워

            if (buckets[index] == null)
                buckets[index] = new LinkedList<KeyValuePair<TKey, TValue>>();

            var node = buckets[index].First;
            while (node != null)
            {
                if(node.Value.Key.Equals(key))
                {
                    node.Value = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
                node = node.Next;
            }
            buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            count++;
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            for (int i = 0; i < size; i++)
            {
                if (buckets[i] != null)
                {
                    foreach (var kvp in buckets[i])
                    {
                        keys.Add(kvp.Key);
                    }
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();
            for (int i = 0; i < size; i++)
            {
                if (buckets[i] != null)
                {
                    foreach (var kvp in buckets[i])
                    {
                        values.Add(kvp.Value);
                    }
                }
            }
            return values;
        }
    }

    public int Count => count;

    public int Capacity => size;

    public bool IsReadOnly => false;

    private void Resize()
    {
        var oldBuckets = buckets;
        var oldSize = size;

        size *= 2;
        buckets = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        // [빈칸 ④] count를 0으로 돌리고, 옛 버킷의 모든 항목을 Add로 다시 넣는다
        //throw new NotImplementedException("빈칸 ④");


        //count = 0;
        //foreach (var kvp in oldBuckets)
        //{
        //    //oldBuckets.AddFirst(kvp); 안됨
        //    oldBuckets.AddRange(kvp);
        //}

        count = 0;
        //oldBuckets는 **서랍장(배열)**이라서 foreach로 꺼낸 건 KVP가 아니라 **서랍 하나(LinkedList 또는 null)
        //서랍장 → 서랍 → 쪽지로 두 겹을 돌아야 해요. 그리고 옛 상자가 아니라 새 상자에 넣어
        for (int i = 0; i < oldSize; i++)
        {
            if (oldBuckets[i] == null) continue; // 빈 서랍이면 건너뛰기
            foreach (var kvp in oldBuckets[i])// 여기서 kvp라는 이름을 만듦
                Add(kvp.Key, kvp.Value); // 새 buckets에 넣기
            //!@# kvp가 없다는 뭐 어떻게 연결해야하나
        }



    }

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (ContainsKey(key))
            throw new ArgumentException("키 중복");

        if ((double)count / size > LoadFactor)
        {
            Resize();
        }

        int index = GetHash(key);
        // [빈칸 ①] 버킷이 null이면 새 LinkedList를 만들고, 체인 끝에 추가한 뒤 count를 늘린다
        //throw new NotImplementedException("빈칸 ①");
        if(buckets[index] == null)
        {
            buckets[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value)); // !@#뭘 넣어도 오류남 왜지?
            count++;
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        buckets = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = GetHash(item.Key);
        if (buckets[index] == null)
            return false;

        foreach (var kvp in buckets[index])
        {
            if (kvp.Key.Equals(item.Key) && kvp.Value.Equals(item.Value))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHash(key);
        if (buckets[index] == null)
            return false;

        foreach (var kvp in buckets[index])
        {
            if (kvp.Key.Equals(key))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < count)
            throw new ArgumentException("배열 크기 부족");

        int index = arrayIndex;
        foreach (var kvp in this)
        {
            array[index++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (buckets[i] != null)
            {
                foreach (var kvp in buckets[i])
                {
                    yield return kvp;
                }
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHash(key);
        // [빈칸 ③] 체인을 LinkedListNode로 따라가며 같은 키의 노드를 지운다
        //throw new NotImplementedException("빈칸 ③");

        //foreach 도는 중에 리스트에서 지우면 에러가 나요. 그래서 First부터 Next로 한 칸씩 직접 따라가요.

        // Remove(TKey key)
        if (buckets[index] == null) return false; //만약 버킷 인덱스값이 없다면 false 반환

        var node = buckets[index].First;
        while(node != null)
        {
            if(node.Value.Key.Equals(key))
            {
                buckets[index].Remove(node);
                count--;
                return true;
            }
            node = node.Next;
        }
        return false;
        //!@#1 없는 인덱스는 어떻게추가해야할까
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        // [빈칸 ③] Remove(key)와 같되 값까지 같아야 지운다
        //throw new NotImplementedException("빈칸 ③");
        //var newList = new LinkedList<KeyValuePair<TKey, TValue>>();
        //if (item.Key == Remove.key)
        //{
        //    newList.Remove(item.Key); //뭐 다안되네
        //    return true;
        //}
        //return false;
        int index = GetHash(item.Key); //GetHash? 키로 몇 번 서랍인지 계산해서 index에 저장
        if (buckets[index] == null) return false; //그 서랍이 비어 있으면(null) false

        var node = buckets[index].First; //그 서랍 줄의 **첫 번째 구슬(노드)**부터 시작
        while (node != null )
        {
            if(node.Value.Key.Equals(item.Key)
                && EqualityComparer<TValue>.Default.Equals(node.Value.Value, item.Value))
            {
                buckets[index].Remove(node);
                count--;
                return true;
            }
            node = node.Next;
        }
        return false;
        
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHash(key);
        // [빈칸 ②] 버킷의 체인을 순회해 같은 키를 찾으면 value에 담고 true
        //throw new NotImplementedException("빈칸 ②");
        // 노드가 필요할 때 (삭제·수정하려면 이쪽)

        var newList = new LinkedList<KeyValuePair<TKey, TValue>>();
        //LinkedListNode<int> node = newList.First; //??? 이게 왜안됨?
        //while (node != null)
        //{
        //    Debug.Log(node.Value);
        //    node = node.Next;   // 마지막 노드의 Next는 null
        //}


        //value = default;
        //return false;

        //리스트에 든 게 int가 아니라 KVP라서 노드 타입도 LinkedListNode<KeyValuePair<TKey,TValue>>
        //찾을 곳은 buckets[index]예요.
        //그리고 찾기만 하는 건 foreach로 충분해요. 노드는 지우거나 바꿀 때

        if (buckets[index] != null)
        {
            foreach (var kvp in buckets[index])
            {
                if(kvp.Key.Equals(key))
                {
                    value = kvp.Value;
                    return true;
                }
            }
        }

        value = default;   // 못 찾았으면 기본값
        return false;
    }

    public IEnumerable<BucketView> GetBucketViews()
    {
        for (int i = 0; i < size; i++)
        {
            if (buckets[i] == null || buckets[i].Count == 0)
            {
                yield return new BucketView(i, BucketState.Empty, null, null, null);
                continue;
            }

            // [빈칸 ⑥] 체인의 (Key, Value)를 문자열 튜플 리스트로 모아 Occupied로 돌려준다
            //throw new NotImplementedException("빈칸 ⑥");

            //괄호로 묶은 두 값 = 튜플
            //("하나", "1")처럼 이름 없는 짝꿍 상자라고 보면 돼요.
            var chain = new List<(string, string)>();// !@#? ???
            foreach (var kvp in buckets[i])
                chain.Add((kvp.Key.ToString(), kvp.Value?.ToString()));

            yield return new BucketView(i, BucketState.Occupied, null, null, chain);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }








    ////////
    ///
    private void TestChainingHashTable()
    {
        Debug.Log("=== Chaining Hash Table Test ===");
        var hashTable = new ChainingHashTable<string, int>();

        hashTable.Add("하나", 1);
        hashTable.Add("둘", 2);
        hashTable.Add("셋", 3);

        foreach (var kvp in hashTable)
        {
            Debug.Log($"[{kvp.Key}, {kvp.Value}]");
        }

        Expect(hashTable.Count, 3, "Count");
        Expect(hashTable["둘"], 2, "값 둘");
        Expect(hashTable.ContainsKey("하나"), true, "탐색 하나");
        Expect(hashTable.Remove("하나"), true, "삭제 하나");
        Expect(hashTable.ContainsKey("하나"), false, "탐색 하나 (삭제 후)");
        Expect(hashTable.Count, 2, "Count (삭제 후)");

        Debug.Log("충돌 · 중복 키 · 리사이즈:");
        var collision = new ChainingHashTable<int, int>();
        collision.Add(1, 10);
        collision.Add(17, 170); // 1과 17은 % 16이 같아 같은 버킷에 들어간다
        Expect(collision[17], 170, "충돌한 키 17의 값");
        Expect(collision.Remove(1), true, "같은 버킷의 키 1 삭제");
        Expect(collision.ContainsKey(17), true, "삭제 후 키 17 탐색");
        ExpectThrows<ArgumentException>(() => collision.Add(17, 0), "중복 키 Add");

        for (int key = 100; key < 120; ++key)
        {
            collision.Add(key, key);
        }
        Expect(collision.Capacity > 16, true, $"리사이즈 발생 (Capacity {collision.Capacity})");
        Expect(collision.Count, 21, "리사이즈 후 Count");
        Expect(collision[17], 170, "리사이즈 후 키 17의 값");

        Debug.Log("인덱서 set:");
        collision[17] = 171;
        Expect(collision[17], 171, "인덱서로 기존 키 값 갱신");
        Expect(collision.Count, 21, "갱신 후 Count");
        collision[49] = 490;
        Expect(collision[49], 490, "인덱서로 새 키 추가");
        Expect(collision.Count, 22, "추가 후 Count");
    }

    private int Expect(string count, int v1)
    {
        throw new NotImplementedException();
    }

    private int Expect(int count, int v1)
    {
        throw new NotImplementedException();
    }

    private int Expect(int count, int v1, string v2)
    {
        throw new NotImplementedException();
    }
}