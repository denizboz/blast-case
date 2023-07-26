using System;

namespace Utilities
{
    public static class BoardUtility
    {
        /// <summary>
        /// Returns first and last indices of the centered subarray of size 'targetSize' of the given array.
        /// Left biased in asymmetric cases, i.e., when 'targetSize' is even and array length is odd, or vice versa.
        /// </summary>
        public static IntRange GetMidRange(int targetSize, int arraySize)
        {
            if (targetSize < 1)
                throw new Exception("Target size must be positive.");

            if (targetSize > arraySize)
                throw new Exception("Target size must be less than the array size.");
            
            var diff = arraySize - targetSize;
            var start = diff / 2;
            var end = arraySize - diff / 2 - 2 + targetSize % 2;

            return new IntRange(start, end);
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