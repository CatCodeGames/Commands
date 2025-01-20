using System;


namespace CatCode
{
    public static class ArrayExtensions
    {
        public static T[] InsertFirst<T>(this T[] array, T value)
            => array.Insert(0, value);

        public static T[] InsertLast<T>(this T[] array, T value)
            => array.Insert(array.Length, value);

        public static T[] Insert<T>(this T[] array, int index, T value)
        {
            if (array != null)
            {
                T[] result = new T[array.Length + 1];
                array.AsSpan(0, index).CopyTo(result);
                result[index] = value;
                array.AsSpan(index).CopyTo(result.AsSpan(index + 1));
                return result;
            }
            else
                return new T[] { value };
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            T[] result = new T[array.Length - 1];
            array.AsSpan(0, index).CopyTo(result);
            array.AsSpan(index + 1).CopyTo(result.AsSpan(index));
            return result;
        }
    }
}