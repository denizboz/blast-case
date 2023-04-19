using System;

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
            var end = maxSize - diff / 2 - 1 + size % 2;

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