using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace VinhLB
{
  public static class VinhLBRandomUtility
  {
    public class RandomWithWeight
    {
      private int[] _prefixSums;

      public RandomWithWeight(int[] weights)
      {
        int size = weights.Length;
        _prefixSums = new int[size + 1];
        // Generate prefix sums array where each element represents the sum of weights up to that index
        for (int i = 0; i < size; i++)
        {
          _prefixSums[i + 1] = _prefixSums[i] + weights[i];
        }
      }

      public int PickIndex()
      {
        // Generate a random number between 1 and the total sum of weights
        int x = 1 + UnityRandom.Range(0, _prefixSums[_prefixSums.Length - 1]);
        int left = 1;
        int right = _prefixSums.Length - 1;

        // Perform binary search to find the index for which prefixSums[index] is greater than or equal to x
        while (left < right)
        {
          int mid = (int)((uint)(left + right) >> 1); // Use unsigned right shift to avoid potential overflow
          if (_prefixSums[mid] >= x)
          {
            // If the mid-index satisfies the condition, we search the left subarray
            right = mid;
          }
          else
          {
            // Otherwise, we search the right subarray
            left = mid + 1;
          }
        }

        // Since we have shifted our prefixSums array by one, we subtract one to get the original index
        return left - 1;
      }
    }

    public static int[] GetUniqueRandomsInRange(int length, int min, int max)
    {
      int rangeSize = max - min;
      if (length <= 0 || length > rangeSize)
      {
        return null;
      }

      int[] results = new int[length];
      List<int> numberList = new List<int>(Enumerable.Range(min, rangeSize));
      for (int i = 0; i < results.Length; i++)
      {
        int randomIndex = UnityRandom.Range(0, numberList.Count);
        results[i] = numberList[randomIndex];
        numberList.RemoveAt(randomIndex);
      }

      return results;
    }
  }
}