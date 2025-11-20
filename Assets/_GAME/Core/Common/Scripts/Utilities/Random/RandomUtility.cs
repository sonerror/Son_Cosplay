using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
  public static class RandomUtility
  {

    public static float GetValue01(int data, int seed)
    {
      return Klak.Math.XXHash.GetValue01(data, seed);
    }

    public static int GetRange(int minInclusive, int maxExclusive)
    {
      return UnityEngine.Random.Range(minInclusive, maxExclusive);
    }


    public static int GetRange(int minInclusive, int maxExclusive, int data, int seed)
    {
      return Klak.Math.XXHash.GetRange(data, minInclusive, maxExclusive, seed);
    }

    public static float GetRange(float min, float max, int data, int seed)
    {
      return Klak.Math.XXHash.GetRange(data, min, max, seed);
    }

    public static int[] GetShuffledIndexArray(int length, int data, int seed)
    {
      // shuffer indexs - O(n)
      int[] indexArray = new int[length];
      for (int i = 0; i < length; i++) indexArray[i] = i;
      int randomPickIndex, swapValue;
      for (int pickCount = 0; pickCount < length - 1; pickCount++)
      {
        randomPickIndex = Klak.Math.XXHash.GetRange(data + pickCount, pickCount, length, seed);
        if (randomPickIndex != pickCount)
        {
          swapValue = indexArray[pickCount];
          indexArray[pickCount] = indexArray[randomPickIndex];
          indexArray[randomPickIndex] = swapValue;
        }
      }
      return indexArray;
    }

    public static int[] GetShuffledIndexArray(int length)
    {
      // shuffle indexs - O(n)
      int[] indexArray = new int[length];
      for (int i = 0; i < length; i++) indexArray[i] = i;
      int randomPickIndex, swapValue;
      for (int pickCount = 0; pickCount < length - 1; pickCount++)
      {
        randomPickIndex = UnityEngine.Random.Range(pickCount, length);
        if (randomPickIndex != pickCount)
        {
          swapValue = indexArray[pickCount];
          indexArray[pickCount] = indexArray[randomPickIndex];
          indexArray[randomPickIndex] = swapValue;
        }
      }
      return indexArray;
    }

    #region Position
    public static Vector2[] GetRandomPositionsInDividedAreaNormalized(Vector2Int division, Vector2 randomness, int numPoints)
    {
      Vector2[] positions = new Vector2[numPoints];

      int[] shuffledAreaIndex = GetShuffledIndexArray(division.x * division.y);

      for (int i = 0; i < numPoints; i++)
      {
        int areaIndex = shuffledAreaIndex[i % shuffledAreaIndex.Length];
        int areaX = areaIndex % division.x;
        int areaY = areaIndex / division.x;

        positions[i].x = (areaX + 0.5f + 0.5f * UnityEngine.Random.Range(-randomness.x, randomness.x)) / division.x;
        positions[i].y = (areaY + 0.5f + 0.5f * UnityEngine.Random.Range(-randomness.y, randomness.y)) / division.y;
      }

      return positions;
    }

    public static Vector2[] GetRandomPositionsInDividedAreaNormalized(Vector2Int division, Vector2 randomness, int numPoints, int data, int seed)
    {
      Vector2[] positions = new Vector2[numPoints];

      int[] shuffledAreaIndex = GetShuffledIndexArray(division.x * division.y, data, seed);

      for (int i = 0; i < numPoints; i++)
      {
        int areaIndex = shuffledAreaIndex[i % shuffledAreaIndex.Length];
        int areaX = areaIndex % division.x;
        int areaY = areaIndex / division.x;

        positions[i].x = (areaX + 0.5f + 0.5f * GetRange(-randomness.x, randomness.x, data + i, seed)) / division.x;
        positions[i].y = (areaY + 0.5f + 0.5f * GetRange(-randomness.y, randomness.y, data, seed)) / division.y;
      }

      return positions;
    }

    public static Vector2 RandomInsideInitCircle(int data, int seed)
    {
      const float PI2 = Mathf.PI * 2;
      float angle = GetRange(0f, PI2, data + seed, seed);
      float radius = GetRange(0f, 1f, data - seed, seed);
      return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }
    #endregion
  }
}