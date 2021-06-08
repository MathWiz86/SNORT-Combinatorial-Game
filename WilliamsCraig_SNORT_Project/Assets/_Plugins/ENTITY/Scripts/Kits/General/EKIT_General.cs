/**************************************************************************************************/
/*!
\file   EKIT_General.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for General functions. These are functions that are useful anywhere in code.

\par References:
  - https://www.geeksforgeeks.org/iterative-quick-sort/
  - https://www.geeksforgeeks.org/quicksort-using-random-pivoting/
*/
/**************************************************************************************************/

using System.Collections.Generic;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: EKIT_General</para>
  /// <summary>
  /// A toolkit for General functions. These are functions that are useful anywhere in code.
  /// </summary>
  public static partial class EKIT_General
  {
    /// <summary>
    /// A function which swaps two values with each other. A becomes B, B becomes A.
    /// </summary>
    /// <typeparam name="T">The type of the variables to swap.</typeparam>
    /// <param name="a">The first variable to swap.</param>
    /// <param name="b">The second variable to swap.</param>
    public static void SwapValues<T>(ref T a, ref T b)
    {
      T temp = a; // Create a temporary variable.
      a = b; // A becomes B.
      b = temp; // B becomes the temp A.
    }

    /// <summary>
    /// A function which swaps two values with each other. A becomes B, B becomes A.
    /// </summary>
    /// <typeparam name="T">The type of the variables to swap.</typeparam>
    /// <param name="al">The list to edit.</param>
    /// <param name="indexA">The first index to swap.</param>
    /// <param name="indexB">The second index to swap.</param>
    public static void SwapValues<T>(IList<T> al, int indexA, int indexB)
    {
      if (al.IsValidIndex(indexA) && al.IsValidIndex(indexB))
      {
        T temp = al[indexA]; // Create a temporary variable.
        al[indexA] = al[indexB]; // A becomes B.
        al[indexB] = temp; // B becomes the temp A.
      }
    }

    /// <summary>
    /// A function which gets the number of values within an enum.
    /// </summary>
    /// <param name="enum_type">The type to test. This should be an enum.</param>
    /// <returns>Returns the number of values in the enum. If this isn't a valid type, returns -1.</returns>
    public static int GetEnumValueCount(System.Type enum_type)
    {
      // If the type is an enum, get the number of values.
      if (enum_type.IsEnum)
        return System.Enum.GetValues(enum_type).Length;

      return -1; // Not valid. Return -1.
    }

    public static bool XOR(bool a, bool b)
    {
      return !(a && b) && (a || b);
    }
  }
  /**********************************************************************************************************************/
}