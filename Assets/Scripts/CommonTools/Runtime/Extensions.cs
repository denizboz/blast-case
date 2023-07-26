using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonTools.Runtime
{
    public static class Extensions
    {
        #region ARRAYS
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
        
        public static T[,] ConvertTo2D<T>(this T[] array, int width, int height)
        {
            if (array.Length != width * height)
                throw new Exception("Array size must match width*height.");
            
            var array2D = new T[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var index = row * width + col;
                    array2D[row, col] = array[index];
                }
            }

            return array2D;
        }

        public static T[] ConvertTo1D<T>(this T[,] array)
        {
            var height = array.GetLength(0);
            var width = array.GetLength(1);

            var array1D = new T[width * height];

            for (var i = 0; i < array1D.Length; i++)
            {
                var row = i / width;
                var col = i % width;
            
                array1D[i] = array[row, col];
            }
            
            return array1D;
        }

        public static T[,] ReverseVertical<T>(this T[,] array)
        {
            var width = array.GetLength(1);
            var height = array.GetLength(0);

            var reversedArray = new T[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    reversedArray[height - i - 1, j] = array[i, j];
                }
            }

            return reversedArray;
        }

        public static T[,] ReverseHorizontal<T>(this T[,] array)
        {
            var width = array.GetLength(1);
            var height = array.GetLength(0);

            var reversedArray = new T[height, width];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    reversedArray[i, width - j - 1] = array[i, j];
                }
            }

            return reversedArray;
        }

        public static T[,] ReverseFull<T>(this T[,] array)
        {
            array = array.ReverseVertical();
            array = array.ReverseHorizontal();

            return array;
        }
        
        public static void Print<T>(this IEnumerable<T> array)
        {
            Debug.Log(string.Join(", ", array));
        }
        
        public static void Print<T>(this T[,] array)
        {
            var width = array.GetLength(1);
            var height = array.GetLength(0);

            var stringBuilder = StringBuilderPool.Get();
            
            for (int i = 0; i < height; i++)
            {
                stringBuilder.Clear();
                
                for (int j = 0; j < width; j++)
                {
                    stringBuilder.Append(array[i, j].ToString());
                    stringBuilder.Append(", ");
                }
                
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
                Debug.Log(stringBuilder);
            }
            
            StringBuilderPool.Return(stringBuilder);
        }
        #endregion

        #region VECTORS
        public static Vector2 WithX(this Vector2 v, float val)
        {
            v.x = val;
            return v;
        }

        public static Vector2 WithY(this Vector2 v, float val)
        {
            v.y = val;
            return v;
        }

        public static Vector3 WithX(this Vector3 v, float val)
        {
            v.x = val;
            return v;
        }

        public static Vector3 WithY(this Vector3 v, float val)
        {
            v.y = val;
            return v;
        }

        public static Vector3 WithZ(this Vector3 v, float val)
        {
            v.z = val;
            return v;
        }

        public static Vector3 WithXY(this Vector3 v, float x, float y)
        {
            v.x = x;
            v.y = y;
            
            return v;
        }

        public static Vector3 WithXZ(this Vector3 v, float x, float z)
        {
            v.x = x;
            v.z = z;

            return v;
        }

        public static Vector3 WithYZ(this Vector3 v, float y, float z)
        {
            v.y = y;
            v.z = z;

            return v;
        }

        public static Vector3 Convert(this Vector3 v, float x, float y, float z)
        {
            v.x = x; v.y = y; v.z = z;

            return v;
        }
        #endregion

        #region COLORS
        public static Color WithRed(this Color color, float red)
        {
            color.r = red;
            return color;
        }

        public static Color WithGreen(this Color color, float green)
        {
            color.g = green;
            return color;
        }

        public static Color WithBlue(this Color color, float blue)
        {
            color.b = blue;
            return color;
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color WithHue(this Color color, float hue)
        {
            Color.RGBToHSV(color, out _, out float s, out float v);

            return Color.HSVToRGB(hue, s, v);
        }

        public static Color WithSaturation(this Color color, float saturation)
        {
            Color.RGBToHSV(color, out float h, out _, out float v);

            return Color.HSVToRGB(h, saturation, v);
        }

        public static Color WithBrightness(this Color color, float brightness)
        {
            Color.RGBToHSV(color, out float h, out float s, out _);

            return Color.HSVToRGB(h, s, brightness);
        }
        #endregion
    }
}
