using System;
using UnityEngine;

namespace Utility
{
    public static class Utilities
    {
        /// <summary>
        /// Returns first and last indices of the middle subarray of size 'size' for a given array of size 'maxSize'.
        /// Left biased in asymmetric cases, i.e., when 'size' is even and 'maxSize' is odd, or vice versa.
        /// </summary>
        public static IntRange GetMidRange(int size, int maxSize = 9)
        {
            if (size < 1)
                throw new Exception("Size must be positive.");

            maxSize = size > maxSize ? size : maxSize;
            
            var diff = maxSize - size;
            var start = diff / 2;
            var end = maxSize - diff / 2 - 2 + size % 2;

            return new IntRange(start, end);
        }

        public static T[] GetRow<T>(this T[,] array, int n)
        {
            var count = array.GetLength(1);
            
            var row = new T[count];
            
            for (int i = 0; i < count; i++)
            {
                row[i] = array[n, i];
            }

            return row;
        }
        
        public static T[] GetColumn<T>(this T[,] array, int n)
        {
            var count = array.GetLength(0);
            
            var row = new T[count];
            
            for (int i = 0; i < count; i++)
            {
                row[i] = array[i, n];
            }

            return row;
        }

        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }
        
        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }
        
        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static void Debug(object obj1)
        {
            UnityEngine.Debug.Log(obj1.ToString());
        }
        public static void Debug(object obj1, object obj2)
        {
            UnityEngine.Debug.Log($"{obj1.ToString()} | {obj2.ToString()}");
        }
        
        public static void Debug(object obj1, object obj2, object obj3)
        {
            UnityEngine.Debug.Log($"{obj1.ToString()} | {obj2.ToString()} | {obj3.ToString()}");
        }
    }

    
    /// <summary>
    /// Same as System.Range, but returns int values instead of Index.
    /// </summary>
    public struct IntRange
    {
        public readonly int Start;
        public readonly int End;

        public IntRange(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
