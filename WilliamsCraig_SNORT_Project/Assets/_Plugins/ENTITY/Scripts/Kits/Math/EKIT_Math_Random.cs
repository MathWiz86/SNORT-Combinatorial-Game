/**************************************************************************************************/
/*!
\file   EKIT_Math_Random.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for Math functions. This file includes functions for generating random values via
  the RNGCryptoServiceProvider. Use this for extra-security when generating random values.
  For speed, use the default 'Random' class provided by Unity.

\par References:
  - https://stackify.com/csharp-random-numbers/
  - https://stackoverflow.com/questions/6299197/rngcryptoserviceprovider-generate-number-in-a-range-faster-and-retain-distribu
  - https://stackoverflow.com/questions/3414900/how-to-get-a-char-from-an-ascii-character-code-in-c-sharp
  - http://net-informations.com/q/faq/round.html
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Math
  {
    private static RNGCryptoServiceProvider math_RNG = new RNGCryptoServiceProvider(); //!< The RNGCRyptoServiceProvider used for randomizing values.

    /// <summary>
    /// A function which constructs a brand new RNGCryptoServiceProvider for random number generation.
    /// </summary>
    public static void ResetRNGProvider()
    {
      math_RNG = new RNGCryptoServiceProvider(); // Create a new provider.
    }

    /// <summary>
    /// A function which updates the RNGCryptoServiceProvider with a new one. Do this to
    /// use your own provider.
    /// </summary>
    /// <param name="provider">The new provider to use for random generation.</param>
    public static void UpdateRNGProvider(RNGCryptoServiceProvider provider)
    {
      math_RNG = provider; // Swap the provider.
    }

    /// <summary>
    /// A function which takes an array or list and shuffles it's elements indirectly.
    /// </summary>
    /// <typeparam name="T">The type contained in the array or list.</typeparam>
    /// <param name="al">The array or list.</param>
    /// <returns>Returns a shuffled copy of the given array or list.</returns>
    public static IList<T> MakeShuffledIList<T>(IList<T> al)
    {
      IList<T> copy = new List<T>(); // Create a copy list.
      // Add in all the values from the given list.
      for (int i = 0; i < al.Count; i++)
      {
        copy.Add(al[i]);
      }
      // Shuffle the elements.
      copy.Shuffle();
      return copy;
    }

    /// <summary>
    /// A function which generates a random int between two values. This uses RNGCryptoServiceProvider.
    /// Use 'Random.Range' for speed.
    /// </summary>
    /// <param name="min">The min value to generate.</param>
    /// <param name="max">The max value to generate.</param>
    /// <param name="inclusiveMin">A check determining if the min can be gotten as a random value.</param>
    /// <param name="inclusiveMax">A check determining if the max can be gotten as a random value.</param>
    /// <returns>Returns a random value between the min and max values.</returns>
    public static int GenerateRandomValue(int min, int max, bool inclusiveMin = true, bool inclusiveMax = true)
    {
      // If the two are the same, immediately return.
      if (min == max)
        return min;

      min = Mathf.Min(min, max); // Account for a possible input error by finding the real min.
      max = Mathf.Max(min, max); // Account for a possible input error by finding the real max.

      if (!inclusiveMin) // If not including the min, increment by one.
        min++;
      if (inclusiveMax) // If including the max, increment by one.
        max++;

      byte[] buffer = new byte[4]; // The buffer for the RNGCRyptoServiceProvider.

      long difference = max - min; // The difference between the max and min.

      // Generate numbers until a good one is found.
      while (true)
      {
        math_RNG.GetBytes(buffer); // Get the bytes.
        
        uint random = System.BitConverter.ToUInt32(buffer, 0); // Convert to a UInt32.
        long realMax = (1 + (long)int.MaxValue); // Get the max int value.
        long remainder = realMax % difference; // Get the modulo remainder.

        // If the random number is within range, return a fixed verison of it.
        if (random < realMax - remainder)
          return (int)(min + random % difference);
      }
    }

    /// <summary>
    /// A function which generates a random double between two values. This uses RNGCryptoServiceProvider.
    /// Use 'Random.Range' for speed.
    /// </summary>
    /// <param name="min">The min value to generate.</param>
    /// <param name="max">The max value to generate.</param>
    /// <param name="inclusiveMin">A check determining if the min can be gotten as a random value.</param>
    /// <param name="inclusiveMax">A check determining if the max can be gotten as a random value.</param>
    /// <param name="precision">The rounding of the double's decimal points. The max precision is 15 decimal points.</param>
    /// <returns>Returns a random value between the min and max values.</returns>
    public static double GenerateRandomValue(double min, double max, bool inclusiveMin = true, bool inclusiveMax = true, int precision = 15)
    {
      // If the two are the same, immediately return.
      if (min == max)
        return min;

      precision = Mathf.Clamp(precision, 0, 15); // Clamp the decimal precision. 15 is the maximum precision for a double.

      min = System.Math.Min(min, max);   // Account for a possible input error by finding the real min.
      max = System.Math.Max(min, max);   // Account for a possible input error by finding the real max.

      byte[] buffer = new byte[8]; // The buffer for the RNGCRyptoServiceProvider.

      // Generate numbers until a good one is found.
      while (true)
      {
        math_RNG.GetBytes(buffer); // Get the bytes.

        double random = System.BitConverter.ToDouble(buffer, 0); // Convert to a Double.

        // Perform checks based on if we are inclusive or not.
        if (double.IsNaN(random))
          continue;

        if (inclusiveMin)
        {
          if (random < min)
            continue;
        }
        else
        {
          if (random <= min)
            continue;
        }

        if (inclusiveMax)
        {
          if (random > max)
            continue;
        }
        else
        {
          if (random >= max)
            continue;
        }

        // If a good number was generated, return it, rounded to the right precision.
        return System.Math.Round(random, precision);
      }
    }

    /// <summary>
    /// A function which generates a random float between two values. This uses RNGCryptoServiceProvider.
    /// Use 'Random.Range' for speed.
    /// </summary>
    /// <param name="min">The min value to generate.</param>
    /// <param name="max">The max value to generate.</param>
    /// <param name="inclusiveMin">A check determining if the min can be gotten as a random value.</param>
    /// <param name="inclusiveMax">A check determining if the max can be gotten as a random value.</param>
    /// <param name="precision">The rounding of the float's decimal points. The max precision is 6 decimal points.</param>
    /// <returns>Returns a random value between the min and max values.</returns>
    public static float GenerateRandomValue(float min, float max, bool inclusiveMin = true, bool inclusiveMax = true, int precision = 6)
    {
      // If the two are the same, immediately return.
      if (min == max)
        return min;

      precision = Mathf.Clamp(precision, 0, 6); // Clamp the decimal precision. 6 is the maximum precision for a float.

      min = Mathf.Min(min, max);  // Account for a possible input error by finding the real min.
      max = Mathf.Max(min, max);  // Account for a possible input error by finding the real max.

      if (!inclusiveMin) // If not including the min, increment by one.
        min += 0.000001f;
      if (inclusiveMax) // If including the max, increment by one.
        max += 0.000001f;

      byte[] buffer = new byte[4]; // The buffer for the RNGCRyptoServiceProvider.
      double difference = max - min; // The difference between the max and min.
      // Generate numbers until a good one is found.
      while (true)
      {
        math_RNG.GetBytes(buffer);  // Get the bytes.

        byte[] copy = new byte[4]; // A copy of the buffer, used to fix the float.
        buffer.CopyTo(copy, 0); // Copy the buffer.

        // Reverse if the BitConverter is LittleEndian.
        if (System.BitConverter.IsLittleEndian)
          System.Array.Reverse(copy);

        float random = Mathf.Abs(System.BitConverter.ToSingle(copy, 0)); // Convert to a Float.
        double realMax = (0.000001f + (double)float.MaxValue); // Get the max int value.
        double remainder = realMax % difference; // Get the modulo remainder.

        // If a good number was generated, return it, rounded to the right precision.
        if (random < realMax - remainder)
          return (float)System.Math.Round(((min + random % difference) * 100.0f) / 100.0f, precision);
      }
    }

    /// <summary>
    /// A function which generates an array of random indexes. This is useful for getting
    /// values from an array or list randomly multiple times over.
    /// </summary>
    /// <param name="min">The minimum index, inclusive.</param>
    /// <param name="max">The maximum index, exclusive.</param>
    /// <param name="count">The number of indexes to generate.</param>
    /// <returns>Returns an array of randomized indexes.</returns>
    public static int[] GenerateRandomIndexes(int min, int max, int count)
    {
      if (min >= max)
        return null;

      int[] indexes = new int[count]; // The indexes that are generated.

      // Generate Random values up to the count. Max is exclusive due to the intended use for arrays.
      for (int i = 0; i < count; i++)
        indexes[i] = GenerateRandomValue(min, max, true, false);

      return indexes; // Return the indexes.
    }

    /// <summary>
    /// A function which generates an array of random indexes. This is useful for getting
    /// values from an array or list randomly multiple times over.
    /// </summary>
    /// <param name="array">The array to generate indexes from, between 0 and Count - 1.</param>
    /// <param name="count">The number of indexes to generate.</param>
    /// <returns>Returns an array of randomized indexes.</returns>
    public static int[] GenerateRandomIndexes(IList array, int count)
    {
      return GenerateRandomIndexes(0, array.Count, count); // Return the indexes, between 0 and [Count - 1].
    }

    /// <summary>
    /// A function which generates an array of unique random indexes. This is useful for getting
    /// values from an array or list randomly, with a unique index each time.
    /// </summary>
    /// <param name="min">The minimum index, inclusive.</param>
    /// <param name="max">The maximum index, exclusive.</param>
    /// <returns>Returns an array of randomized indexes.</returns>
    public static int[] GenerateUniqueRandomIndexes(int min, int max)
    {
      if (min >= max)
        return null;
      
      int count = max - min;

      int[] indexes = new int[count]; // The indexes generated.
      List<int> values = new List<int>(); // The values that are still valid to use.
      // Setup the values from the min to the max index.
      for (int i = min; i < max; i++)
      {
        values.Add(i);
      }
        

      // For each generated value, once a value is gotten, add it to the indexes and remove it from the value array so it can't be gotten again.
      for (int i = 0; i < count; i++)
      {
        int random = GenerateRandomValue(0, values.Count, true, false);
        indexes[i] = values[random];
        values.RemoveAt(random);
      }

      return indexes; // Return the indexes.
    }

    /// <summary>
    /// A function which generates an array of unique random indexes. This is useful for getting
    /// values from an array or list randomly, with a unique index each time.
    /// </summary>
    /// <param name="array">The array to generate indexes from, between 0 and Count - 1.</param>
    /// <returns>Returns an array of randomized indexes.</returns>
    public static int[] GenerateUniqueRandomIndexes(IList array)
    {
      return GenerateUniqueRandomIndexes(0, array.Count); // Return the indexes, between 0 and [Count - 1].
    }

    /// <summary>
    /// A function which generates a random string out of all valid ASCII characters.
    /// </summary>
    /// <param name="length">The length of the string.</param>
    /// <returns>Returns a randomly generated string.</returns>
    public static string GenerateRandomString(int length = 1)
    {
      // Return nothing if there is no length.
      if (length <= 0)
        return string.Empty;

      string random = string.Empty; // The random string.

      // For the length of the string, add a random character.
      // 32 is the minimum valid ASCII value. 126 is the maximum valid ASCII value.
      for (int i = 0; i < length; i++)
        random += (char)GenerateRandomValue(32, 126);

      return random; // Return the random string.
    }

    /// <summary>
    /// A function which generates a random string out of all valid ASCII characters.
    /// </summary>
    /// <param name="characters">The characters to generate a string from.</param>
    /// <param name="length">The length of the string.</param>
    /// <returns>Returns a randomly generated string.</returns>
    public static string GenerateRandomString(char[] characters, int length = 1)
    {
      // If there is no length or no valid character array, return an empty string.
      if (length <= 0)
        return string.Empty;
      if (characters.Length <= 0)
        return string.Empty;

      string random = string.Empty; // The random string.

      // For the length of the string, add a random character from the array.
      for (int i = 0; i < length; i++)
        random += characters[GenerateRandomValue(0, characters.Length, true, false)];

      return random; // Return the random string.
    }
  }
  /**********************************************************************************************************************/
}