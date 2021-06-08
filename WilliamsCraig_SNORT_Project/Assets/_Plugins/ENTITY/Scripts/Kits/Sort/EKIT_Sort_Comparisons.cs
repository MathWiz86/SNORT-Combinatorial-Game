/**************************************************************************************************/
/*!
\file   EKIT_Sort_Comparisons.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains functionality for various Comparison functions. Use these when sorting
  arrays and lists.

\par References:
  - https://docs.microsoft.com/en-us/dotnet/api/system.icomparable.compareto?view=netcore-3.1
*/
/**************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Sort
  {
    /// <summary>
    /// A Comparison for sorting arrays and lists from least to greatest. The type must be comparable (i.e. able to use 'CompareTo')
    /// </summary>
    /// <typeparam name="T">The type of the sorting algorithm. T must be comparable (i.e. able to use 'CompareTo')</typeparam>
    /// <param name="a">The first element to compare.</param>
    /// <param name="b">The second element to compare.</param>
    /// <returns>Returns whether or not a is less than or greater than b.</returns>
    public static int CompareLeastToGreatest<T>(T a, T b) where T : IComparable<T>
    {
      // Return a's comparison to b.
      return a.CompareTo(b);
    }

    /// <summary>
    /// A Comparison for sorting arrays and lists from greatest to least. The type must be comparable (i.e. able to use 'CompareTo')
    /// </summary>
    /// <typeparam name="T">The type of the sorting algorithm. T must be comparable (i.e. able to use 'CompareTo')</typeparam>
    /// <param name="a">The first element to compare.</param>
    /// <param name="b">The second element to compare.</param>
    /// <returns>Returns whether or not a is less than or greater than b.</returns>
    public static int CompareGreatestToLeast<T>(T a, T b) where T: IComparable<T>
    {
      // Return b's comparison to a.
      return b.CompareTo(a);
    }

    /// <summary>
    /// A Comparison for sorting strings from shortest to longest in length.
    /// </summary>
    /// <param name="a">The first string to compare.</param>
    /// <param name="b">The second string to compare.</param>
    /// <returns>Returns whether or not a is shorter or longer than b.</returns>
    public static int CompareShortestToLongest(string a, string b)
    {
      // Return a's length's comparison to b's length.
      return a.Length.CompareTo(b.Length);
    }

    /// <summary>
    /// A Comparison for sorting strings from longest to shortest in length.
    /// </summary>
    /// <param name="a">The first string to compare.</param>
    /// <param name="b">The second string to compare.</param>
    /// <returns>Returns whether or not a is shorter or longer than b.</returns>
    public static int CompareLongestToShortest(string a, string b)
    {
      // Return b's length's comparison to a's length.
      return b.Length.CompareTo(a.Length);
    }

    /// <summary>
    /// A Comparison for sorting an array or list of arrays or lists, ordering from smallest to largest in count.
    /// </summary>
    /// <typeparam name="T">The type contained inside the array or list.</typeparam>
    /// <param name="a">The first array or list to compare.</param>
    /// <param name="b">The second array or list to compare.</param>
    /// <returns>Returns whether or not a is shorter or longer than b.</returns>
    public static int CompareShortestToLongest<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
      // Return a's length's comparison to b's length.
      return a.LongCount().CompareTo(b.LongCount());
    }

    /// <summary>
    /// A Comparison for sorting an array or list of arrays or lists, ordering from largest to smallest in count.
    /// </summary>
    /// <typeparam name="T">The type contained inside the array or list.</typeparam>
    /// <param name="a">The first array or list to compare.</param>
    /// <param name="b">The second array or list to compare.</param>
    /// <returns>Returns whether or not a is shorter or longer than b.</returns>
    public static int CompareLongestToShortest<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
      // Return b's length's comparison to a's length.
      return b.LongCount().CompareTo(a.LongCount());
    }

    public static int CompareFileLinestLeastToGreatest(EKIT_File.ES_FileLine a, EKIT_File.ES_FileLine b)
    {
      return a.index.CompareTo(b.index);
    }
  }
  /**********************************************************************************************************************/
}