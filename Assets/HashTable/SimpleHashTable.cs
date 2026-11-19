using System;
using System.Collections;
using System.Collections.Generic;

// 충돌 처리 없는 "단순" 해시테이블
// - 한 칸(버킷)에 데이터 1개만 저장
// - 다른 키가 같은 칸에 오면(해시 충돌) 예외를 던진다
// BucketView / BucketState 는 과제에서 제공된 타입을 그대로 사용
public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;

    private KeyValuePair<TKey, TValue>[] table; // 실제 데이터
    private bool[] occupied;                    // 그 칸이 사용 중인지
    private int size;                           // 배열 길이(칸 수)
    private int count;                          // 저장된 데이터 수

    public SimpleHashTable()
    {
        table = new KeyValuePair<TKey, TValue>[DefaultCapacity];
        occupied = new bool[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
    }

    // ───────── 해시 ─────────
    public int GetHash(TKey key)
    {
        return GetHash(key, size);
    }

    private int GetHash(TKey key, int capacity)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        return (hash & 0x7fffffff) % capacity; // 음수 제거 후 칸 번호로
    }

    private static bool KeyEquals(TKey a, TKey b)
    {
        return EqualityComparer<TKey>.Default.Equals(a, b);
    }

    // ───────── 추가 ─────────
    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if ((double)count / size > LoadFactor) // 나누기 전에 double로!
            Resize();

        int index = GetHash(key);

        if (!occupied[index])
        {
            table[index] = new KeyValuePair<TKey, TValue>(key, value);
            occupied[index] = true;
            ++count;
        }
        else if (KeyEquals(table[index].Key, key))
        {
            throw new ArgumentException($"키 ({key}) 중복");
        }
        else
        {
            throw new InvalidOperationException($"해시 충돌 (index {index})");
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    // ───────── 크기 2배 ─────────
    private void Resize()
    {
        int newSize = size * 2;
        var newTable = new KeyValuePair<TKey, TValue>[newSize];
        var newOccupied = new bool[newSize];

        for (int i = 0; i < size; i++)           // "옛날" 배열을 돈다
        {
            if (!occupied[i])
                continue;

            int newIndex = GetHash(table[i].Key, newSize); // 새 크기로 다시 계산
            if (newOccupied[newIndex])
                throw new InvalidOperationException("리사이즈 중 해시 충돌");

            newTable[newIndex] = table[i];       // 옛 배열 → 새 배열
            newOccupied[newIndex] = true;
        }

        // 새 배열로 교체 (이걸 안 하면 Resize 한 의미가 없음)
        table = newTable;
        occupied = newOccupied;
        size = newSize;
    }

    // ───────── 조회 ─────────
    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = GetHash(key); // null 체크는 GetHash 안에서

        if (occupied[index] && KeyEquals(table[index].Key, key))
        {
            value = table[index].Value;
            return true;
        }
        value = default;
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        int index = GetHash(key);
        return occupied[index] && KeyEquals(table[index].Key, key);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        // 키도 있고, 값도 같아야 true
        return TryGetValue(item.Key, out TValue value)
            && EqualityComparer<TValue>.Default.Equals(value, item.Value);
    }

    // ───────── 인덱서 ─────────
    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
                return value;
            throw new KeyNotFoundException($"키 ({key}) 없음");
        }
        set
        {
            int index = GetHash(key);

            if (occupied[index] && KeyEquals(table[index].Key, key))
            {
                // 이미 있는 키 → 값만 덮어쓰기
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                // 없는 키 → 새로 추가 (충돌이면 Add가 예외)
                Add(key, value);
            }
        }
    }

    // ───────── 삭제 ─────────
    public bool Remove(TKey key)
    {
        int index = GetHash(key);

        if (occupied[index] && KeyEquals(table[index].Key, key))
        {
            occupied[index] = false;
            table[index] = default;
            --count;
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
            return Remove(item.Key);
        return false;
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        Array.Clear(occupied, 0, size);
        count = 0;
    }

    // ───────── 속성 ─────────
    public int Count => count;
    public int Capacity => size;
    public bool IsReadOnly => false;

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>(count);
            for (int i = 0; i < size; i++)
                if (occupied[i]) keys.Add(table[i].Key);
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>(count);
            for (int i = 0; i < size; i++)
                if (occupied[i]) values.Add(table[i].Value);
            return values;
        }
    }

    // ───────── 순회 / 복사 ─────────
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i])
                yield return table[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || array.Length - arrayIndex < count)
            throw new ArgumentException("배열 공간 부족");

        foreach (var pair in this)
            array[arrayIndex++] = pair;
    }

    // ───────── 시각화용 (과제 제공 부분 유지) ─────────
    public IEnumerable<BucketView> GetBucketViews()
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i])
                yield return new BucketView(i, BucketState.Occupied, table[i].ToString(), table[i].ToString(), null);
            else
                yield return new BucketView(i, BucketState.Empty, null, null, null);
        }
    }
}