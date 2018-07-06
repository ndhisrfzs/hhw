using System.Collections.Generic;

public class Combination<T>
{
    public static void GetCombination(ref List<List<T>> list, List<T> arr, int index, int count, int[] b)
    {
        for (int i = index; i < arr.Count; i++)
        {
            b[b.Length - count] = i;
            if (count > 1)
            {
                GetCombination(ref list, arr, i + 1, count - 1, b);
            }
            else
            {
                if (list == null)
                {
                    list = new List<List<T>>();
                }

                List<T> temp = new List<T>();
                for (int j = 0; j < b.Length; j++)
                {
                    temp.Add(arr[b[j]]);
                }
                list.Add(temp);
            }
        }
    }

    public static List<List<T>> GetCombination(List<T> arr, int count)
    {
        int[] b = new int[count];
        List<List<T>> result = null;
        GetCombination(ref result, arr, 0, count, b);

        return result;
    }
}