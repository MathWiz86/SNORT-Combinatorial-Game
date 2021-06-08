using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Extensions
  {
    /// <summary>
    /// A function which removes the first instance of a given substring from a main string.
    /// </summary>
    /// <param name="original">The original string to look through.</param>
    /// <param name="removal">The substring to remove.</param>
    /// <returns>Returns the string with the substring removed.</returns>
    public static string RemoveFirstSubstring(this string original, string removal)
    {
      // Make sure the strings are valid.
      if (original != null && removal != null)
      {
        int index = original.IndexOf(removal); // Get the index of the substring.
        // If the index is valid, remove the substring. Otherwise, just return the same string.
        return index < 0 ? original : original.Remove(index, removal.Length);
      }

      return original; // Return the original string if the strings aren't valid.
    }

    /// <summary>
    /// A function which removes the last instance of a given substring from a main string.
    /// </summary>
    /// <param name="original">The original string to look through.</param>
    /// <param name="removal">The substring to remove.</param>
    /// <returns>Returns the string with the substring removed.</returns>
    public static string RemoveLastSubstring(this string original, string removal)
    {
      // Make sure the strings are valid.
      if (original != null && removal != null)
      {
        int index = original.LastIndexOf(removal); // Get the index of the substring.
        // If the index is valid, remove the substring. Otherwise, just return the same string.
        return index < 0 ? original : original.Remove(index, removal.Length);
      }

      return original; // Return the original string if the strings aren't valid.
    }

    /// <summary>
    /// A function which removes all instances of a given substring from a main string.
    /// </summary>
    /// <param name="original">The original string to look through.</param>
    /// <param name="removal">The substring to remove.</param>
    /// <returns>Returns the string with the substring removed.</returns>
    public static string RemoveAllSubstrings(this string original, string removal)
    {
      // Make sure the strings are valid.
      if (original != null && removal != null)
      {
        string current = original; // Create a copy of the string.
        int index = original.LastIndexOf(removal); // Get the first index of the substring.
        // Loop through the string until all substrings are found and removed.
        while (index >= 0)
        {
          current = current.Remove(index, removal.Length); // Remove the substring.
          index = original.IndexOf(removal); // See if there's another substring.
        }
        return current; // Return the fixed-up string.
      }

      return original; // Return the oriignal string if the strings aren't valid.
    }
  }
  /**********************************************************************************************************************/
}