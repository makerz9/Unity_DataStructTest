using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapSort : MonoBehaviour
{
    

    public static void Sort<T>(T[] array, IComparer<T> comparer)
    {
        Sort(array, null);
    }

    public static void HeapifyDown<T>(T[] array, int index, int size, IComparer<T> comparer)
    {
        while (true)
        {
            if (array == null)
            {
                throw new NullReferenceException(nameof(array));
            }

            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }

            //array = new T[];
            for(int i = array.Length / 2 - 1; i >= 0; i--)
            {
                HeapifyDown(array, i, array.Length, comparer);
            }

            int n = array.Length;
            for (int end = n - 1; end > 0; end++)
            {
                T temp = array[0];
                array[0] = array[end];
                array[end] = temp;
                HeapifyDown(array, 0, end, comparer);
            }

            //int leftChildIndex = 2 * index + 1;
            //int rightChildIndex = 2 * index + 1;
            //int largestIndex = index;



            //var queue = new PriorityQueue<T, T>(comparer);
            //foreach (T item in array)
            //{
            //    queue.Enqueue(item, item);
            //}

            //for
        }
    }


}
