/**************************************************************************************************/
/*!
\file   EKIT_Math_Range.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for Math functions. This file includes functions for finding if a value is within a
  given range.

\par References:
*/
/**************************************************************************************************/
using System.Linq;
using System;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Math
  {
    /// <summary>
    /// A function for seeing if a value is between two numbers. This is the inclusive version.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum (inclusive) value.</param>
    /// <param name="max">The maximum (inclusive) value.</param>
    /// <returns>Returns if the value is between the two given numbers.</returns>
    public static bool InRangeInclusive<T>(T value, T min, T max) where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
      return ((value.CompareTo(min) >= 0) && (value.CompareTo(max) <= 0));
    }

    /// <summary>
    /// A function for seeing if a value is between two numbers. This is the exclusive version.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum (inclusive) value.</param>
    /// <param name="max">The maximum (inclusive) value.</param>
    /// <returns>Returns if the value is between the two given numbers.</returns>
    public static bool InRangeExclusive<T>(T value, T min, T max) where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
      return ((value.CompareTo(min) > 0) && (value.CompareTo(max) < 0));
    }

    /// <summary>
    /// A simple function which returns the given value, or a minimum value if the given one is too low.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="minimum">The minimum number the value can be.</param>
    /// <returns>Returns either the given value, or the minimum if the value is lower.</returns>
    public static T NoLessThan<T>(T value, T minimum) where T: IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
      // If the value is less than the min, return the min. Otherwise, return the value.
      return value.CompareTo(minimum) < 0 ? minimum : value;
    }

    /// <summary>
    /// A simple function which returns the given value, or a maximum value if the given one is too high
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="maximum">The maximum number the value can be.</param>
    /// <returns>Returns either the given value, or the maximum if the value is higher.</returns>
    public static T NoMoreThan<T>(T value, T maximum) where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
      // If the value is less than the min, return the min. Otherwise, return the value.
      return value.CompareTo(maximum) > 0 ? maximum : value;
    }
  }
  /**********************************************************************************************************************/
}