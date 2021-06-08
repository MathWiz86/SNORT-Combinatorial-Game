
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Counter
  {
    /// <summary>
    /// A function which updates a given counter by a given value. The change will be added regardless of if it is positive or negative.
    /// However, it will only return true for positive target values.
    /// </summary>
    /// <param name="counter">The counter to increment.</param>
    /// <param name="target_value">The target value. This should be positive.</param>
    /// <param name="change">The amount to change the counter. This can be positive or negative.</param>
    /// <param name="reset_counter">A bool determining if the counter should be reset to a value after reaching it's target.</param>
    /// <param name="reset_value">The value to reset the counter to if it is reset.</param>
    /// <returns>Returns true if the value reaches or exceeds the target value. Returns false otherwise.</returns>
    public static bool IncrementCounter(ref int counter, int target_value, int change = 1, bool reset_counter = true, int reset_value = 0)
    {
      counter += change; // Update the counter.

      // If the counter has reached the target value, return true.
      if (counter >= target_value)
      {
        counter = reset_counter ? reset_value : counter; // Reset the counter if requested.
        return true; // Return true.
      }

      return false; // The counter has not reached it's target.
    }

    /// <summary>
    /// A function which updates a given counter by a given value. The change will be added regardless of if it is positive or negative.
    /// However, it will only return true for positive target values.
    /// </summary>
    /// <param name="counter">The counter to increment.</param>
    /// <param name="target_value">The target value. This should be positive.</param>
    /// <param name="change">A delegate to update the counter. Use this for increments that aren't just adding or subtracting.</param>
    /// <param name="reset_counter">A bool determining if the counter should be reset to a value after reaching it's target.</param>
    /// <param name="reset_value">The value to reset the counter to if it is reset.</param>
    /// <returns>Returns true if the value reaches or exceeds the target value. Returns false otherwise.</returns>
    public static bool IncrementCounter(ref int counter, int target_value, Func<int, int> change, bool reset_counter = true, int reset_value = 0)
    {
      counter = change(counter); // Update the counter.

      // If the counter has reached the target value, return true.
      if (counter >= target_value)
      {
        counter = reset_counter ? reset_value : counter; // Reset the counter if requested.
        return true; // Return true.
      }

      return false; // The counter has not reached it's target.
    }

    /// <summary>
    /// A function which updates a given counter by a given value. The change will be added regardless of if it is positive or negative.
    /// However, it will only return true for negative target values.
    /// </summary>
    /// <param name="counter">The counter to decrement.</param>
    /// <param name="target_value">The target value. This should be positive.</param>
    /// <param name="change">The amount to change the counter. This can be positive or negative.</param>
    /// <param name="reset_counter">A bool determining if the counter should be reset to a value after reaching it's target.</param>
    /// <param name="reset_value">The value to reset the counter to if it is reset.</param>
    /// <returns>Returns true if the value reaches or exceeds the target value. Returns false otherwise.</returns>
    public static bool DecrementCounter(ref int counter, int target_value, int change = 1, bool reset_counter = true, int reset_value = 0)
    {
      counter += change; // Update the counter.

      // If the counter has reached the target value, return true.
      if (counter <= target_value)
      {
        counter = reset_counter ? reset_value : counter; // Reset the counter if requested.
        return true; // Return true.
      }

      return false; // The counter has not reached it's target.
    }

    /// <summary>
    /// A function which updates a given counter by a given value. The change will be added regardless of if it is positive or negative.
    /// However, it will only return true for negative target values.
    /// </summary>
    /// <param name="counter">The counter to increment.</param>
    /// <param name="target_value">The target value. This should be positive.</param>
    /// <param name="change">A delegate to update the counter. Use this for increments that aren't just adding or subtracting.</param>
    /// <param name="reset_counter">A bool determining if the counter should be reset to a value after reaching it's target.</param>
    /// <param name="reset_value">The value to reset the counter to if it is reset.</param>
    /// <returns>Returns true if the value reaches or exceeds the target value. Returns false otherwise.</returns>
    public static bool DecrementCounter(ref int counter, int target_value, Func<int, int> change, bool reset_counter = true, int reset_value = 0)
    {
      counter = change(counter); // Update the counter.

      // If the counter has reached the target value, return true.
      if (counter <= target_value)
      {
        counter = reset_counter ? reset_value : counter; // Reset the counter if requested.
        return true; // Return true.
      }

      return false; // The counter has not reached it's target.
    }
  }
}