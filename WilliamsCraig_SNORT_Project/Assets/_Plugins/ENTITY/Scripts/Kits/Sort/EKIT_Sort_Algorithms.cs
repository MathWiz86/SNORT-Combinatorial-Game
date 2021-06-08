/**************************************************************************************************/
/*!
\file   EKIT_Sort_Comparisons.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains functionality for various sorting algorithms.

\par References:
   - https://github.com/w0rthy/SortingVisualizer/blob/master/SortingVisualizer/sorts
   - https://www.geeksforgeeks.org/iterative-quick-sort/
   - https://www.geeksforgeeks.org/bubble-sort/
*/
/**************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Sort
  {
    /// <summary> The maximum amount of times an async sort will allow a loop before performing a yield. The higher this value is, the quicker the function at the risk of a frame drop. </summary>
    private const int sort_MaxLoopCount = 10000;
    /// <summary> The maximum amount of times an async sort will allow a bogo sort loop before performing a yield. The higher this value is, the quicker the function at the risk of a frame drop. </summary>
    private const int sort_MaxBogoLoopCount = 100;

    /// <summary>
    /// A function which checks if an array or list is sorted, based on a given comparison. This version uses the default sort algorithm: 'Divided'.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSorted<T>(this IList<T> al, Func<T, T, int> compare)
    {
      return al != null ? al.IsSorted(compare, 0, al.Count) : false; // Sort from start to finish.
    }

    /// <summary>
    /// A function which checks if an array or list is sorted from [index_low, index_high), based on a given comparison. This version uses the default sort algorithm: 'Divided'.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <param name="index_low">The minimum index to check from. This index is inclusive.</param>
    /// <param name="index_high">The maximum index to check to. This index is exclusive.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSorted<T>(this IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      return al != null ? al.IsSortedDivided(compare, index_low, index_high) : false; // Sort from start to finish. 'Divided' is the most balanced in terms of time.
    }

    /// <summary>
    /// A function which checks if an array or list is sorted, based on a given comparison.
    /// This version checks using a linear algorithm.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSortedLinear<T>(this IList<T> al, Func<T, T, int> compare)
    {
      return al != null ? al.IsSortedLinear(compare, 0, al.Count) : false; // Sort from start to finish.
    }

    /// <summary>
    /// A function which checks if an array or list is sorted from [index_low, index_high), based on a given comparison.
    /// This version checks using a linear algorithm.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <param name="index_low">The minimum index to check from. This index is inclusive.</param>
    /// <param name="index_high">The maximum index to check to. This index is exclusive.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSortedLinear<T>(this IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure that the inputs are valid.
      if (al == null || compare == null || al.Count <= 1 || index_high > al.Count)
        return false;
      // Make sure the low index is within range.
      if (!EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return false;
      // If the indexes are already equal, just return true. One element is sorted.
      if (index_low == index_high)
        return true;

      // Linearly compare each element with the one behind it. If out of order, return false immediately.
      for (int i = index_low + 1; i < index_high; i++)
      {
        if (compare(al[i - 1], al[i]) > 0)
          return false;
      }

      return true; // If the loop completes, then we can return true. The list is sorted. 
    }

    /// <summary>
    /// A function which checks if an array or list is sorted, based on a given comparison.
    /// This version checks using a cocktail algorithm, checking from both ends of the array or list.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSortedCocktail<T>(this IList<T> al, Func<T, T, int> compare)
    {
      return al != null ? al.IsSortedCocktail(compare, 0, al.Count) : false; // Sort from start to finish.
    }

    /// <summary>
    /// A function which checks if an array or list is sorted from [index_low, index_high), based on a given comparison.
    /// This version checks using a cocktail algorithm, checking from both ends of the array or list.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <param name="index_low">The minimum index to check from. This index is inclusive.</param>
    /// <param name="index_high">The maximum index to check to. This index is exclusive.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSortedCocktail<T>(this IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure that the inputs are valid.
      if (al == null || compare == null || al.Count <= 1 || index_high > al.Count)
        return false;
      // Make sure the low index is within range.
      if (!EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return false;
      // If the indexes are already equal, just return true. One element is sorted.
      if (index_low == index_high)
        return true;

     // return al.IsSortedCocktailPartition(compare, index_low, index_high);
      int moves = (Mathf.CeilToInt((index_high - index_low) / 2.0f)); // The number of moves to make. This is half of the array's size.
      index_high--; // Decrement the high index by 1, as the input is exclusive.

      // Perform the required amount of moves.
      for (int i = 0; i < moves; i++)
      {
        // Check each end next to it's adjacent element. If either side is out of order, return false.
        if (compare(al[index_low], al[index_low + 1]) > 0 || compare(al[index_high - 1], al[index_high]) > 0)
          return false;

        // Move the indexes closer to each other.
        index_low++;
        index_high--;
      }

      return true; // If the loop completes, then we can return true. The list is sorted. 
    }

    /// <summary>
    /// A function which checks if an array or list is sorted, based on a given comparison.
    /// This version checks using a divide-and-conquer algorithm, splitting the array or list in half and checking each half.
    /// This is the most stable version of the check, being low-time in almost all cases, without getting much better or worse in any particular case.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSortedDivided<T>(this IList<T> al, Func<T, T, int> compare)
    {
      return al != null ? al.IsSortedDivided(compare, 0, al.Count) : false; // Sort from start to finish.
    }

    /// <summary>
    /// A function which checks if an array or list is sorted from [index_low, index_high), based on a given comparison.
    /// This version checks using a divide-and-conquer algorithm, splitting the array or list in half and checking each half.
    /// This is the most stable version of the check, being low-time in almost all cases, without getting much better or worse in any particular case.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <param name="index_low">The minimum index to check from. This index is inclusive.</param>
    /// <param name="index_high">The maximum index to check to. This index is exclusive.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    public static bool IsSortedDivided<T>(this IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure that the inputs are valid.
      if (al == null || compare == null || al.Count <= 1 || index_high > al.Count)
        return false;
      // Make sure the low index is within range.
      if (!EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return false;
      // If the indexes are already equal, just return true. One element is sorted.
      if (index_low == index_high)
        return true;

      // Get two middle index points. We'll be dividing the array into
      int index_mid_low = index_high / 2;
      int index_mid_high = index_mid_low;

      // If the array is of even size, we need to compare the two middle indexes first before splitting up.
      if (index_high % 2 == 0)
      {
        index_mid_low -= 1; // Decrement the low end to properly split.
        // Make sure the middle elements are in order.
        if (compare(al[index_mid_low], al[index_mid_high]) > 0)
          return false;
      }
      index_high -= 1; // Decrement the high index by 1, as the input is exclusive.

      // First, check the left side. If it's successful, check the right side.
      if (al.IsSortedDividedPartition(compare, index_low, index_mid_low))
          return al.IsSortedDividedPartition(compare, index_mid_high, index_high);

      return false; // The list was not sorted.
    }

    /// <summary>
    /// A helper function for the Divide-and-Conquer version of checking if an array or list is sorted. This is used to check each half
    /// of the array or list separately.
    /// </summary>
    /// <typeparam name="T">The value type stored in the array or list.</typeparam>
    /// <param name="al">The array or list to check.</param>
    /// <param name="compare">The comparison used to determine if the array or list is sorted.</param>
    /// <param name="index_low">The minimum index to check from. This index is inclusive.</param>
    /// <param name="index_high">The maximum index to check to. This index is exclusive.</param>
    /// <returns>Returns true if the array or list is sorted. Returns false otherwise.</returns>
    private static bool IsSortedDividedPartition<T>(this IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Continue while the indexes do not pass each other.
      while (index_low < index_high)
      {
        // If the indexes ever equal each other, do a final special check on both sides of the element.
        if (index_low == index_high)
          return (compare(al[index_low], al[index_low + 1]) < 0) && (compare(al[index_low], al[index_low - 1]) < 0);
        
        // If the elements are out of order, return false.
        if (compare(al[index_low], al[index_high]) > 0)
          return false;

        // Move the indexes closer to each other.
        index_low++;
        index_high--;
      }

      return true; // If the loop completes, then we can return true. The list is sorted. 
    }

    /// <summary>
    /// A function which uses the Safe Bogo Sort algorithm to swap all elements in an array or list. This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    public static void BogoSort<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the array exists, swap the elements from index 0 to the max index.
      if (al != null)
        BogoSort(al, compare, 0, al.Count);
    }

    /// <summary>
    /// A function which uses the Safe Bogo Sort algorithm to swap elements in an array or list from [index_low, index_high). This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    public static void BogoSort<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
        return;
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return;

      int position = index_low; // The position in the array.

      // Continue until all positions are sorted.
      while (position < index_high)
      {
        T temp = al[position]; // Get a temporary value.
        int i = 0; // The current position in the iteration.
        for (i = position; i < index_high; i++)
        {
          // If the comparison is out of order, break early. We need to shuffle again.
          if (compare(temp, al[i]) > 0)
            break;
        }

        // If the comparison so far is fine, increment position. Otherwise, shuffle again.
        if (i == index_high)
          position++;
        else
          al.Shuffle(position, index_high);
      }
    }

    /// <summary>
    /// A function which uses the Safe Bogo Sort algorithm to swap elements in an array or list. This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator BogoSortAsync<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the list is not null, sort the whole array.
      if (al != null)
        yield return BogoSortAsync(al, compare, 0, al.Count);

      yield return al; // Return the sorted array.
    }

    /// <summary>
    /// A function which uses the Safe Bogo Sort algorithm to swap elements in an array or list from [index_low, index_high). This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator BogoSortAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
      {
        yield return al;
        yield break;
      }
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
      {
        yield return al;
        yield break;
      }

      int position = index_low; // The position in the array.

      // Continue until all positions are sorted.
      while (position < index_high)
      {
        T temp = al[position]; // Get a temporary value.
        int i = 0; // The current position in the iteration.
        int loop_counter = 0; // The counter for the inner loop.
        for (i = position; i < index_high; i++)
        {
          // If the comparison is out of order, break early. We need to shuffle again.
          if (compare(temp, al[i]) > 0)
            break;

          // Due to the inherent issues with BogoSort, this does not use the normal loop counter setting. Instead, set your own AT YOUR OWN RISK.
          if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxBogoLoopCount))
            yield return null;
        }

        // If the comparison so far is fine, increment position. Otherwise, shuffle again.
        if (i == index_high)
          position++;
        else
          yield return al.ShuffleAsync(position, index_high);
      }

      // Return the sorted list.
      yield return al;
    }

    /// <summary>
    /// A function which uses the True Bogo Sort algorithm to swap all elements in an array or list. This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    public static void TrueBogoSort<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the array exists, swap the elements from index 0 to the max index.
      if (al != null)
        TrueBogoSort(al, compare, 0, al.Count);
    }

    /// <summary>
    /// A function which uses the True Bogo Sort algorithm to swap elements in an array or list from [index_low, index_high). This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    public static void TrueBogoSort<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
        return;
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return;

      // Continue until all positions are sorted.
      while (true)
      {
        al.Shuffle(index_low, index_high);
        bool is_sorted = true;
        for (int i = index_low + 1; i < index_high; i++)
        {
          // If the comparison is out of order, break early. We need to shuffle again.
          if (compare(al[i - 1], al[i]) > 0)
          {
            is_sorted = false;
            break;
          }
        }

        // If the list is sorted, end the function.
        if (is_sorted)
          break;
      }
    }

    /// <summary>
    /// A function which uses the True Bogo Sort algorithm to swap elements in an array or list. This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator TrueBogoSortAsync<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the list is not null, sort the whole array.
      if (al != null)
        yield return TrueBogoSortAsync(al, compare, 0, al.Count);

      yield return al; // Return the sorted array.
    }

    /// <summary>
    /// A function which uses the Safe Bogo Sort algorithm to swap elements in an array or list from [index_low, index_high). This sort
    /// should never be used in normal code. This algorithm is solely for educational purposes.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator TrueBogoSortAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
      {
        yield return al;
        yield break;
      }
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
      {
        yield return al;
        yield break;
      }

      int loop_counter = 0; // The counter for the inner loop.
      // Continue until all positions are sorted.
      while (true)
      {
        al.Shuffle(index_low, index_high);
        bool is_sorted = true;
        for (int i = index_low + 1; i < index_high; i++)
        {
          // If the comparison is out of order, break early. We need to shuffle again.
          if (compare(al[i - 1], al[i]) > 0)
          {
            is_sorted = false;
            break;
          }
        }

        // If the list is sorted, end the function.
        if (is_sorted)
          break;

        // Due to the inherent issues with BogoSort, this does not use the normal loop counter setting. Instead, set your own AT YOUR OWN RISK.
        if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxBogoLoopCount))
          yield return null;
      }

      // Return the sorted list.
      yield return al;
    }

    /// <summary>
    /// A function which uses the Bubble Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    public static void BubbleSort<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the array exists, swap the elements from index 0 to the max index.
      if (al != null)
        BubbleSort(al, compare, 0, al.Count);
    }

    /// <summary>
    /// A function which uses the Bubble Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    public static void BubbleSort<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
        return;
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return;

      // Create two temporary values to store data.
      T temp1 = default;
      T temp2 = default;
      
      // Swap for the entire section of the array.
      for (int i = index_low; i < index_high - 1; i++)
      {
        bool swapped = false; // A bool determining if any swaps were made.
        temp1 = al[index_low]; // Get the first comparison value.
        // Go through the remainder of the section.
        for (int j = index_low + 1; j < index_high - (i - index_low); j++)
        {
          temp2 = al[j]; // Get the second comparison value.
          // If the values are not sorted, swap adjacent values.
          if (compare(temp1, temp2) > 0)
          {
            EKIT_General.SwapValues(al, j, j - 1);
            swapped = true;
          }
          else
            temp1 = temp2; // Otherwise, replace what is the first comparison value. This is our new pivot.
        }
        // If nothing was swapped, the array is in order. We can end early.
        if (!swapped)
          break;
      }
    }

    /// <summary>
    /// A function which uses the Bubble Sort algorithm to swap elements in an array or list.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator BubbleSortAsync<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the list is not null, sort the whole array.
      if (al != null)
        yield return BubbleSortAsync(al, compare, 0, al.Count);

      yield return al; // Return the sorted array.
    }

    /// <summary>
    /// A function which uses the Bubble Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator BubbleSortAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
      {
        yield return al;
        yield break;
      }
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
      {
        yield return al;
        yield break;
      }

      // Create two temporary values to store data.
      T temp1 = default;
      T temp2 = default;

      // Swap for the entire section of the array.
      for (int i = index_low; i < index_high - 1; i++)
      {
        bool swapped = false; // A bool determining if any swaps were made.
        int loop_counter = 0; // The counter for the inner loop.
        temp1 = al[index_low]; // Get the first comparison value.
        // Go through the remainder of the section.
        for (int j = index_low + 1; j < index_high - (i - index_low); j++)
        {
          temp2 = al[j]; // Get the second comparison value.
          // If the values are not sorted, swap adjacent values.
          if (compare(temp1, temp2) > 0)
          {
            EKIT_General.SwapValues(al, j, j - 1);
            swapped = true;
          }
          else
            temp1 = temp2; // Otherwise, replace what is the first comparison value. This is our new pivot.

          // Yield if the loop counter is at max.
          if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxLoopCount))
            yield return null;
        }
        // If nothing was swapped, the array is in order. We can end early.
        if (!swapped)
          break;
      }

      yield return al; // Return the array.
    }

    /// <summary>
    /// A function which uses the Heap Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    public static void HeapSort<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the array exists, swap the elements from index 0 to the max index.
      if (al != null)
        HeapSort(al, compare, 0, al.Count);
    }

    /// <summary>
    /// A function which uses the Heap Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    public static void HeapSort<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
        return;
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return;

      int last_index = index_high - index_low; // The last valid index to sort.

      // Create a max heap out of the specified section.
      for (int i = index_low + (last_index - 2) / 2; i >= index_low; i--)
        HeapSortPartition(al, compare, i, index_low, index_high);

      int partition_high = 0; // The highest index to go to in the partition.
      for (int i = 1; i < last_index; i++)
      {
        partition_high = index_high - i; // Update the partition.
        EKIT_General.SwapValues(al, index_low, partition_high); // Swap the values.
        HeapSortPartition(al, compare, index_low, index_low, partition_high); // Partition to create a new heap.
      }
    }

    /// <summary>
    /// A partition function used for finding a pivot position when heap sorting.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="pivot">The pivot index to partition around.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    private static void HeapSortPartition<T>(IList<T> al, Func<T, T, int> compare, int pivot, int index_low, int index_high)
    {
      T temp = al[pivot]; // A temporary pivot value.
      int p_low = 0; // The low index to pivot around.
      int p_high = 0; // The high index to pivot around.
      T compare_l = default; // The first value to compare.
      T compare_r = default; // The second value to compare.

      // Continue until sorted.
      while (true)
      {
        // Update the pivot indexes. If accessing greater than the max, break immediately.
        p_low = index_low + (pivot - index_low) * 2 + 1;
        p_high = p_low + 1;
        if (p_low >= index_high)
          break;

        compare_l = al[p_low]; // Get the first compare value.
        // If the high pivot is valid, compare with a second value.
        if (p_high < index_high)
        {
          compare_r = al[p_high]; // Get the second compare value.
          // If the values are in order, continue on.
          if (compare(compare_l, compare_r) < 0)
          {
            p_low = p_high;
            compare_l = compare_r;
          }
        }

        // If the temporary value is in order, break.
        if (compare(compare_l, temp) < 0)
          break;

        EKIT_General.SwapValues(al, pivot, p_low); // Swap the values otherwise with the pivot.
        pivot = p_low; // The pivot is now the low index.
      }
    }

    /// <summary>
    /// A function which uses the Heap Sort algorithm to swap elements in an array or list.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator HeapSortAsync<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the list is not null, sort the whole array.
      if (al != null)
        yield return HeapSortAsync(al, compare, 0, al.Count);

      yield return al; // Return the sorted array.
    }

    /// <summary>
    /// A function which uses the Heap Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator HeapSortAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
      {
        yield return al;
        yield break;
      }
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
      {
        yield return al;
        yield break;
      }

      int last_index = index_high - index_low; // The last valid index to sort.

      // Create a max heap out of the specified section.
      for (int i = index_low + (last_index - 2) / 2; i >= index_low; i--)
        yield return HeapSortPartitionAsync(al, compare, i, index_low, index_high);

      int partition_high = 0; // The highest index to go to in the partition.
      for (int i = 1; i < last_index; i++)
      {
        partition_high = index_high - i; // Update the partition.
        EKIT_General.SwapValues(al, index_low, partition_high); // Swap the values.
        yield return HeapSortPartitionAsync(al, compare, index_low, index_low, partition_high); // Partition to create a new heap.
      }

      yield return al; // Return the array.
    }

    /// <summary>
    /// A partition function used for finding a pivot position when heap sorting.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="pivot">The pivot index to partition around.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    private static IEnumerator HeapSortPartitionAsync<T>(IList<T> al, Func<T, T, int> compare, int pivot, int index_low, int index_high)
    {
      T temp = al[pivot]; // A temporary pivot value.
      int p_low = 0; // The low index to pivot around.
      int p_high = 0; // The high index to pivot around.
      T compare_l = default; // The first value to compare.
      T compare_r = default; // The second value to compare.

      int loop_counter = 0; // Initialize a loop counter.
      // Continue until sorted.
      while (true)
      {
        // Update the pivot indexes. If accessing greater than the max, break immediately.
        p_low = index_low + (pivot - index_low) * 2 + 1;
        p_high = p_low + 1;
        if (p_low >= index_high)
          break;

        compare_l = al[p_low]; // Get the first compare value.
        // If the high pivot is valid, compare with a second value.
        if (p_high < index_high)
        {
          compare_r = al[p_high]; // Get the second compare value.
          // If the values are in order, continue on.
          if (compare(compare_l, compare_r) < 0)
          {
            p_low = p_high;
            compare_l = compare_r;
          }
        }

        // If the temporary value is in order, break.
        if (compare(compare_l, temp) < 0)
          break;

        EKIT_General.SwapValues(al, pivot, p_low); // Swap the values otherwise with the pivot.
        pivot = p_low; // The pivot is now the low index.

        // Update the loop counter, and yield to the thread if necessary.
        if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxLoopCount))
          yield return null;
      }
    }

    /// <summary>
    /// A function which uses the Selection Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    public static void SelectionSort<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the array exists, swap the elements from index 0 to the max index.
      if (al != null)
        SelectionSort(al, compare, 0, al.Count);
    }

    /// <summary>
    /// A function which uses the Selection Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    public static void SelectionSort<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
        return;
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return;

      // Go from the start index, to one before the last index.
      for (int i = index_low; i < index_high - 1; i++)
      {
        int min_index = i; // The index containing the smallest value.
        // For the remaining indexes, continuously compare until getting the one with the minimum value.
        for (int j = i; j < index_high; j++)
        {
          if (compare(al[j], al[min_index]) > 0)
            min_index = j;
        }

        EKIT_General.SwapValues(al, min_index, i); // Swap the minimum index with the current index.
      }
    }

    /// <summary>
    /// A function which uses the Selection Sort algorithm to swap elements in an array or list.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator SelectionSortAsync<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the list is not null, sort the whole array.
      if (al != null)
        yield return SelectionSortAsync(al, compare, 0, al.Count);

      yield return al; // Return the sorted array.
    }

    /// <summary>
    /// A function which uses the Selection Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator SelectionSortAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
      {
        yield return al;
        yield break;
      }
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
      {
        yield return al;
        yield break;
      }

      // Go from the start index, to one before the last index.
      for (int i = index_low; i < index_high - 1; i++)
      {
        int min_index = i; // The index containing the smallest value.
        int loop_counter = 0; // Initialize a loop counter.
        // For the remaining indexes, continuously compare until getting the one with the minimum value.
        for (int j = i; j < index_high; j++)
        {
          if (compare(al[j], al[min_index]) > 0)
            min_index = j;

          // Update the loop counter, and yield to the thread if necessary.
          if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxLoopCount))
            yield return null;
        }

        EKIT_General.SwapValues(al, min_index, i); // Swap the minimum index with the current index.
      }

      yield return al; // Return the array.
    }

    /// <summary>
    /// A function which uses the Stalin Sort algorithm to remove unorganized elements from [index_low, index_high).
    /// Please note that the IList MUST be resizable! i.e. A List rather than an Array.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The resizable list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    public static void StalinSort<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the array exists, swap the elements from index 0 to the max index.
      if (al != null)
        StalinSort(al, compare, 0, al.Count);
    }

    /// <summary>
    /// A function which uses the Stalin Sort algorithm to remove unorganized elements from [index_low, index_high).
    /// Please note that the IList MUST be resizable! i.e. A List rather than an Array.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The resizable list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    public static void StalinSort<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
        return;
      // The IList cannot be of a fixed size.
      if ((al as IList) == null || (al as IList).IsFixedSize)
        return;
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return;

      T pivot = al[index_low]; // Get the first pivot.
      IList<T> copy = new List<T>(al); // Create a copy of the list.
      int copy_index = index_low + 1; // The index we currently are at in the copied list.

      // Go through the entire original list.
      for (int i = index_low + 1; i < index_high; i++)
      {
        // If the latest element is out of order, remove it from the copy.
        if (compare(al[i], pivot) > 0)
          copy.RemoveAt(copy_index);
        else
        {
          // Otherwise, increase our copy index and update the pivot.
          copy_index++;
          pivot = al[i];
        }
      }
      // Clear out the old list and add in the copied items.
      al.Clear();
      foreach (T item in copy)
        al.Add(item);
    }

    /// <summary>
    /// A function which uses the Stalin Sort algorithm to remove unorganized elements from [index_low, index_high).
    /// Please note that the IList MUST be resizable! i.e. A List rather than an Array.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The resizable list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator StalinSortAsync<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the list is not null, sort the whole array.
      if (al != null)
        yield return StalinSortAsync(al, compare, 0, al.Count);

      yield return al; // Return the sorted array.
    }

    /// <summary>
    /// A function which uses the Stalin Sort algorithm to remove unorganized elements from [index_low, index_high).
    /// Please note that the IList MUST be resizable! i.e. A List rather than an Array.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The resizable list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator StalinSortAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
      {
        yield return al;
        yield break;
      }
      // The IList cannot be of a fixed size.
      if ((al as IList) == null || (al as IList).IsFixedSize)
      {
        yield return al;
        yield break;
      }
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
      {
        yield return al;
        yield break;
      }

      T pivot = al[index_low]; // Get the first pivot.
      IList<T> copy = new List<T>(al); // Create a copy of the list.
      int copy_index = index_low + 1; // The index we currently are at in the copied list.
      int loop_counter = 0; // Initialize a loop counter.
      // Go through the entire original list.
      for (int i = index_low + 1; i < index_high; i++)
      {
        // If the latest element is out of order, remove it from the copy.
        if (compare(al[i], pivot) > 0)
          copy.RemoveAt(copy_index);
        else
        {
          // Otherwise, increase our copy index and update the pivot.
          copy_index++;
          pivot = al[i];
        }

        // Update the loop counter, and yield to the thread if necessary.
        if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxLoopCount))
          yield return null;
      }

      loop_counter = 0; // Reset the loop counter.
      // Clear out the old list and add in the copied items.
      al.Clear();
      foreach (T item in copy)
      {
        al.Add(item);
        // Update the loop counter, and yield to the thread if necessary.
        if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxLoopCount))
          yield return null;
      }

      yield return al;
    }

    /// <summary>
    /// A function which uses the Quick Sort algorithm to swap elements in an array or list.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    public static void QuickSort<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the array exists, swap the elements from index 0 to the max index.
      if (al != null)
        QuickSort(al, compare, 0, al.Count);
    }

    /// <summary>
    /// A function which uses the Quick Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    public static void QuickSort<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
        return;
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
        return;


      int[] stack = new int[index_high - index_low]; // Create a stack to keep track of the swaps required.
      int stack_top = -1; // The top index of the stack.
      int low = index_low; // The low value of the stack;
      int high = index_high - 1; // The high value of the stack.

      // Initialize the stack by pushing the initial low and high values.
      stack[++stack_top] = low;
      stack[++stack_top] = high;

      // Continue removing from the stack until all moves are made.
      while (stack_top >= 0)
      {
        // Remove the high and low values from the stack.
        high = stack[stack_top--];
        low = stack[stack_top--];

        // Find a pivot value at random and set it to its sorted position.
        int pivot_high = QuickSortPartition(al, compare, low, high);

        // If there are left elements to the pivot, add the left to the stack.
        if (pivot_high - 1 > low)
        {
          stack[++stack_top] = low;
          stack[++stack_top] = pivot_high - 1;
        }

        // If there are right elements to the pivot, add the right to the stack.
        if (pivot_high + 1 < high)
        {
          stack[++stack_top] = pivot_high + 1;
          stack[++stack_top] = high;
        }
      }
    }

    /// <summary>
    /// A partition function used for finding a pivot position when quick sorting.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The low index in the array. This is where the sort begins.</param>
    /// <param name="index_high">The high index in the array. This is where the sort ends.</param>
    /// <returns>Returns the pivot point of the swap.</returns>
    private static int QuickSortPartition<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Generate a random pivot position for better accuracy and speed.
      EKIT_General.SwapValues(al, EKIT_Math.GenerateRandomValue(index_low, index_high), index_high);
      T pivot = al[index_high];

      int index = index_low - 1; // Get an index.

      for (int i = index_low; i < index_high; i++)
      {
        // Make a comparison. If the pivot is sorted ahead of the element, swap the values.
        if (compare(pivot, al[i]) > 0)
        {
          index++;
          EKIT_General.SwapValues(al, index, i);
        }
      }

      // Make one final swap with the high point.
      index++;
      EKIT_General.SwapValues(al, index, index_high);

      return index; // Return the pivot.
    }

    /// <summary>
    /// A function which uses the Quick Sort algorithm to swap elements in an array or list.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator QuickSortAsync<T>(IList<T> al, Func<T, T, int> compare)
    {
      // If the list is not null, sort the whole array.
      if (al != null)
        yield return QuickSortAsync(al, compare, 0, al.Count);

      yield return al; // Return the sorted array.
    }

    /// <summary>
    /// A function which uses the Quick Sort algorithm to swap elements in an array or list from [index_low, index_high).
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The inclusive low index to sort from.</param>
    /// <param name="index_high">The exclusive high index to sort to. For example, sorting to the last index is 'al.Count'.</param>
    /// <returns>Returns an IEnumerator for use in a routine. In an ET_Routine, returns the same list, but sorted.</returns>
    public static IEnumerator QuickSortAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high)
    {
      // Make sure all values are valid and logical.
      if (al == null || compare == null || index_high > al.Count)
      {
        yield return al;
        yield break;
      }
      // Make sure there is enough data to compare.
      if (al.Count <= 1 || !EKIT_Math.InRangeInclusive(index_low, 0, index_high - 2))
      {
        yield return al;
        yield break;
      }

      int[] stack = new int[index_high - index_low]; // Create a stack to keep track of the swaps required.
      int stack_top = -1; // The top index of the stack.
      int low = index_low; // The low value of the stack;
      int high = index_high - 1; // The high value of the stack.

      // Initialize the stack by pushing the initial low and high values.
      stack[++stack_top] = low;
      stack[++stack_top] = high;

      int loop_counter = 0; // Initialize a loop counter.
      // Continue removing from the stack until all moves are made.
      while (stack_top >= 0)
      {
        // Remove the high and low values from the stack.
        high = stack[stack_top--];
        low = stack[stack_top--];

        // Find a pivot value at random and set it to its sorted position.
        int pivot_high = 0;
        yield return QuickSortPartitionAsync(al, compare, low, high, pi => pivot_high = pi);

        // If there are left elements to the pivot, add the left to the stack.
        if (pivot_high - 1 > low)
        {
          stack[++stack_top] = low;
          stack[++stack_top] = pivot_high - 1;
        }

        // If there are right elements to the pivot, add the right to the stack.
        if (pivot_high + 1 < high)
        {
          stack[++stack_top] = pivot_high + 1;
          stack[++stack_top] = high;
        }

        // Update the loop counter, and yield to the thread if necessary.
        if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxLoopCount))
          yield return null;
      }

      yield return al;
    }

    /// <summary>
    /// A partition function used for finding a pivot position when quick sorting.
    /// </summary>
    /// <typeparam name="T">The type stored within the array or list.</typeparam>
    /// <param name="al">The array or list to sort.</param>
    /// <param name="compare">The comparison function to use. Some common comparisons can be found in the 'EKIT_Sort' class.</param>
    /// <param name="index_low">The low index in the array. This is where the sort begins.</param>
    /// <param name="index_high">The high index in the array. This is where the sort ends.</param>
    /// <param name="pivot_high">An action for updating the pivot. (i.e. pi => pivot = pi)</param>
    /// <returns>Returns an IEnumerator for use in a routine.</returns>
    private static IEnumerator QuickSortPartitionAsync<T>(IList<T> al, Func<T, T, int> compare, int index_low, int index_high, Action<int> pivot_high)
    {
      // Generate a random pivot position for better accuracy and speed.
      EKIT_General.SwapValues(al, EKIT_Math.GenerateRandomValue(index_low, index_high), index_high);
      T pivot = al[index_high];

      int index = index_low - 1; // Get an index.

      int loop_counter = 0; // Initialize a loop counter.
      // Make a comparison. If the pivot is sorted ahead of the element, swap the values.
      for (int i = index_low; i < index_high; i++)
      {
        if (compare(pivot, al[i]) > 0)
        {
          index++;
          EKIT_General.SwapValues(al, index, i);
        }

        // Update the loop counter, and yield to the thread if necessary.
        if (EKIT_Counter.IncrementCounter(ref loop_counter, sort_MaxLoopCount))
          yield return null;
      }

      // Make one final swap with the high point.
      index++;
      EKIT_General.SwapValues(al, index, index_high);
      pivot_high(index); // Return the pivot.
    }
  }
  /**********************************************************************************************************************/
}