using System;
using System.Collections.Generic;
using UnityEngine;

public class TestChainingHashTable : MonoBehaviour
{

    private void TestChainingHashTables()
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
        //() => collision.Add(17, 0) 넘길 때 "이 코드를 지금 실행하지 말고 상자에 담아"

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


    // 실제값(actual)과 기대값(expected)이 같으면 PASS
    private void Expect<T>(T actual, T expected, string label)
    {
        if (EqualityComparer<T>.Default.Equals(actual, expected))
            Debug.Log($"PASS {label}: {actual}");
        else
            Debug.LogError($"FAIL {label}: 기대 {expected}, 실제 {actual}");
    }
    //왜 T 값을 생각 못했지?????



    // action을 실행했을 때 TException이 터지면 PASS
    //에러가 나는지 확인하는 거예요.
    private void ExpectThrows<TException>
        //<TException>	어떤 에러를 기대하는지 나중에 정함 (Expect<T>의 T와 같은 원리)
        (Action action, string label) where TException : Exception
        //where TException : Exception    단, 에러 종류만 들어올 수 있음
        //Action action	실행할 코드를 상자에 담아 받음

    {
        try
        {
            //상자 열기 = collision.Add(17, 0) 실행
            //17은 이미 있으니 ArgumentException 발생!
            //→ 아래 줄 건너뛰고 바로 catch로 점프
            action();
            Debug.LogError($"FAIL {label}: 예외가 안 났음");
        }
        catch (TException)
        {
            Debug.Log($"PASS {label}"); //"PASS"  여기로 옴
        }
        catch (Exception e)
        {
            Debug.LogError($"FAIL {label}: 다른 예외 {e.GetType().Name}");
        }
    }


}
