/**************************************************************************************************/
/*!
\file   EKIT_Extensions_IList.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for extended functions. This file includes functions for IList variables
  (Arrays and Lists).

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Extensions
  {
    private const int list_MaxLoopCount = 2000;

    public static bool HasData<T>(this IList<T> al)
    {
      return al != null && al.Count > 0;
    }

    /// <summary>
    /// A function which returns if a given array or list is empty.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <returns>Returns true if the array or list is empty. Returns false otherwise.</returns>
    public static bool IsEmpty<T>(this IList<T> al)
    {
      return al ==  null || al.Count <= 0; // Return if there are no elements.
    }

    /// <summary>
    /// A function that returns the last valid index for the array or list.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <returns>Returns the last valid index of the array or list. Returns -1 if the size is 0.</returns>
    public static int LastIndex<T>(this IList<T> al)
    {
      return al.Count - 1; // The last valid index is 1 less than the size.
    }

    /// <summary>
    /// A function which returns the last element stored in the array or list.
    /// This function handles error checking that the Linq function 'Last' does not.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <returns>Returns the last element in the array. If there are no elements, the default value of T is returned.</returns>
    public static T LastElement<T>(this IList<T> al)
    {
      // If there are elements, return the one at the last element.
      if (!al.IsEmpty())
        return al[al.LastIndex()];
      
      return default; // Otherwise, return the default value.
    }

    /// <summary>
    /// A function which removes any values within an array or list that are null. The array is shrunk down to the new size.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    public static void RemoveAllNullElements<T>(this IEnumerable<T> al)
    {
      // Check that the array contains nullable items.
      if (Nullable.GetUnderlyingType(typeof(T)) != null)
      {
        al = al.Where(e => e != null); // If so, create a new version of the array or list with only non-null elements.
      }
    }

    /// <summary>
    /// A function for seeing if a given index is valid for a given array.
    /// </summary>
    /// <typeparam name="T">The type stored in the array.</typeparam>
    /// /// <param name="array">The array to check the index against.</param>
    /// <param name="index">The index to check.</param>
    /// <returns>Returns if the index is valid.</returns>
    public static bool IsValidIndex<T>(this IEnumerable<T> array, int index)
    {
      return (array != null && (index >= 0) && (index < array.Count()));
    }

    /// <summary>
    /// A function which accesses and returns a random element in an array or list.
    /// </summary>
    /// <typeparam name="T">The type contained in the array or list.</typeparam>
    /// <param name="al">The array or list.</param>
    /// <returns>Returns a random element. Returns the default value of the list's type if the size is 0.</returns>
    public static T GetRandomElement<T>(this IList<T> al)
    {
      // If the list is empty, return a default value.
      if (al.Count <= 0)
        return default;

      // Return an element at a randomly generated index.
      return al[EKIT_Math.GenerateRandomValue(0, al.Count, true, false)];
    }

    public static int GetRandomIndex<T>(this IList<T> al)
    {
      // If the list is empty, return a default value.
      if (al.Count <= 0)
        return default;

      // Return an element at a randomly generated index.
      return EKIT_Math.GenerateRandomValue(0, al.Count, true, false);
    }

    /// <summary>
    /// A function which takes an array or list and shuffles it's elements directly.
    /// </summary>
    /// <typeparam name="T">The type contained in the array or list.</typeparam>
    /// <param name="al">The array or list.</param>
    public static void Shuffle<T>(this IList<T> al)
    {
      if (al == null)
        return;

      int count = al.Count; // Store the count. We only shuffle if there's more than 1 element.
      if (count > 1)
      {
        // For every element, swap at least once.
        for (int i = 0; i < count - 1; i++)
        {
          int random = EKIT_Math.GenerateRandomValue(i, count, true, false);
          EKIT_General.SwapValues(al, i, random);
        }
      }
    }

    /// <summary>
    /// A function which takes an array or list and shuffles it's elements directly.
    /// This shuffles indexes [low, high).
    /// </summary>
    /// <typeparam name="T">The type contained in the array or list.</typeparam>
    /// <param name="al">The array or list.</param>
    /// <param name="low">The starting index to shuffle from, inclusive.</param>
    /// <param name="high">The ending index to shuffle from, exclusive.</param>
    public static void Shuffle<T>(this IList<T> al, int low, int high)
    {
      // The list must exist, the low and high points must be valid.
      if (al == null || low < 0 || high > al.Count)
        return;

      if (al.Count > 1)
      {
        // For every element, swap at least once.
        for (int i = low; i < high; i++)
        {
          int random = EKIT_Math.GenerateRandomValue(low, high - 1, true, false);
          EKIT_General.SwapValues(al, i, random);
        }
      }
    }

    /// <summary>
    /// A function which takes an array or list and shuffles it's elements directly.
    /// </summary>
    /// <typeparam name="T">The type contained in the array or list.</typeparam>
    /// <param name="al">The array or list.</param>
    /// <returns>Returns and IEnumerator for use in a coroutine.</returns>
    public static IEnumerator ShuffleAsync<T>(this IList<T> al)
    {
      // If the list isn't valid, immediately stop.
      if (al == null)
        yield break;

      yield return al.ShuffleAsync(0, al.Count); // Shuffle all elements.
    }

    /// <summary>
    /// A function which takes an array or list and shuffles it's elements directly.
    /// This shuffles indexes [low, high).
    /// </summary>
    /// <typeparam name="T">The type contained in the array or list.</typeparam>
    /// <param name="al">The array or list.</param>
    /// <param name="low">The starting index to shuffle from, inclusive.</param>
    /// <param name="high">The ending index to shuffle from, exclusive.</param>
    /// <returns>Returns and IEnumerator for use in a coroutine.</returns>
    public static IEnumerator ShuffleAsync<T>(this IList<T> al, int low, int high)
    {
      // The list must exist, the low and high points must be valid.
      if (al == null || low < 0 || high > al.Count)
        yield break;

      if (al.Count > 1)
      {
        int loop_counter = 0; // The loop counter for swapping.
        // For every element, swap at least once.
        for (int i = low; i < high; i++)
        {
          int random = EKIT_Math.GenerateRandomValue(low, high - 1, true, false);
          EKIT_General.SwapValues(al, i, random);
          // Update the loop counter, yielding if required.
          if (EKIT_Counter.IncrementCounter(ref loop_counter, list_MaxLoopCount))
            yield return null;
        }
      }
    }

    /// <summary>
    /// A function which prints out the elements in an array or list. You can customize what is used to separate the elements, and what surrounds each element.
    /// </summary>
    /// <typeparam name="T">The type contained in the array.</typeparam>
    /// <param name="al">The array or list to print out.</param>
    /// <param name="separator">The separation between each element.</param>
    /// <param name="prefix">The characters to print before each element.</param>
    /// <param name="suffix">The characters to print after each element.</param>
    /// <returns>Returns the string of all the printed elements.</returns>
    public static string PrintElements<T>(this IEnumerable<T> al, string separator = ", ", string prefix = "[", string suffix = "]")
    {
      // If the array/list is invalid or has no elements, return an empty string.
      if (al == null || al.LongCount() <= 0)
        return string.Empty;

      string message = string.Empty; // The message to print out.

      // For each element, wrap up the element in the prefix and suffix, and separate with the separator.
      foreach (T element in al)
        message += prefix + element.ToString() + suffix + separator;

      // Remove the final separator.
      message = message.Substring(0, message.LastIndexOf(separator));

      return message; // Return the message.
    }
  }
  /**********************************************************************************************************************/
}