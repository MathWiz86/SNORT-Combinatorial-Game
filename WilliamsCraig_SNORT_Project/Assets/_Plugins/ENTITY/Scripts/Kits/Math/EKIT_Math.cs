/**************************************************************************************************/
/*!
\file   EKIT_Math.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for Math functions. This includes various helper functions not included in built-in
  classes.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: EKIT_Math</para>
  /// <summary>
  /// A toolkit for Math functions. These are shortcuts for mathematical functions not available elsewhere (i.e. Mathf).
  /// </summary>
  public static partial class EKIT_Math
  {
    /// <summary>
    /// A function which generates an IList (array or list) of numbers from min to max. The max is exclusive.
    /// </summary>
    /// <param name="min">The minimum number, inclusive.</param>
    /// <param name="max">The maximum number, exclusive.</param>
    /// <returns>Returns an IList (array or list) of numbers from (min to max]. Returns null on an error.</returns>
    public static IList<int> GenerateNumberIList(int min, int max)
    {
      // If the min is less than or equal, return null;
      if (min >= max)
        return null;

      // Initialize the array.
      int[] array = new int[max - min];

      // Add in the elements from min to max.
      for (int i = min; i < max; i++)
        array[i] = i;

      return array; // Return the array.
    }

    public static int[] GenerateNumberArray(int min, int max)
    {
      // If the min is less than or equal, return null;
      if (min >= max)
        return null;

      // Initialize the array.
      int[] array = new int[max - min];

      // Add in the elements from min to max.
      for (int i = min; i < max; i++)
        array[i] = i;

      return array; // Return the array.
    }

    public static List<int> GenerateNumberList(int min, int max)
    {
      // If the min is less than or equal, return null;
      if (min >= max)
        return null;

      // Initialize the array.
      List<int> list = new List<int>(max - min);
      // Add in the elements from min to max.
      for (int i = 0; i < list.Capacity; i++)
      {
        list.Add(min);
        min++;
      }

      return list; // Return the array.
    }
  }
  /**********************************************************************************************************************/
}