/**************************************************************************************************/
/*!
\file   KIT_Enum.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing extra tools for Enums. Taken from a prior project.

\par References:
*/
/**************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/**********************************************************************************************************************/
/// <para>Class Name: KIT_Enum</para>
/// <summary>
/// A kit of useful functions for getting information on Enum Types.
/// </summary>
public static class KIT_Enum
{
  /// <summary> An enum for determining how to convert an enum into a string. </summary>
  public enum StringType
  {
    /// <summary> Return the default string. </summary>
    Normal,
    /// <summary> Replace all underscores '_' with spaces ' '. </summary>
    Underscore,
    /// <summary> Add a space between Camel Case. </summary>
    CamelCase,
  }

  /// <summary>
  /// A function which gets the names of all values of an Enum.
  /// </summary>
  /// <typeparam name="TEnum">The type of Enum the values will be obtained from.</typeparam>
  /// <param name="type">The method of changing the string's formatting.</param>
  /// <returns>Returns an array of string names of the Enum values.</returns>
  public static string[] GetEnumStringNames<TEnum>(StringType type = StringType.Normal) where TEnum : System.Enum
  {
    TEnum[] values = (TEnum[])System.Enum.GetValues(typeof(TEnum)); // Get the values in an array.
    string[] names = new string[values.Length]; // Create an array of strings of the same length.

    // For each value, convert it to a string and store into the array.
    for (int i = 0; i < values.Length; i++)
      names[i] = values[i].ConvertToString(type);

    return names; // Return the array.
  }

  /// <summary>
  /// A function which gets the names of all values of an Enum.
  /// </summary>
  /// <param name="type">The Type of the Enum the values will be obtained from. Make sure it inherits 'System.Enum'.</param>
  /// <param name="stype">The method of changing the string's formatting.</param>
  /// <returns>Returns an array of string names of the Enum values.</returns>
  public static string[] GetEnumStringNames(System.Type type, StringType stype = StringType.Normal)
  {
    // If the type isn't an enum, return null immediately.
    if (!type.IsEnum)
      return null;

    System.Array values = System.Enum.GetValues(type); // Get the values in an array.
    string[] names = new string[values.Length]; // Create an array of strings of the same length.
    // For each value, convert it to a string and store into the array.
    for (int i = 0; i < values.Length; i++)
      names[i] = ((System.Enum)values.GetValue(i)).ConvertToString(stype);
    
    return names; // Return the array.
  }

  /// <summary>
  /// A function which gets the number of values an enum has.
  /// </summary>
  /// <typeparam name="TEnum">The type of Enum the values will be obtained from.</typeparam>
  /// <returns>Returns the number of Enum values.</returns>
  public static int GetEnumValueCount<TEnum>() where TEnum : System.Enum
  {
    return System.Enum.GetValues(typeof(TEnum)).Length; // Get the values in an array and return the length.
  }

  /// <summary>
  /// A function which gets the number of values an enum has.
  /// </summary>  
  /// <typeparam name="TEnum">The type of Enum the values will be obtained from.</typeparam>
  /// <returns>Returns the number of Enum values.</returns>
  public static int GetEnumValueCount(System.Type type)
  {
    return type.IsEnum ? System.Enum.GetValues(type).Length : -1; // Get the values in an array and return the length.
  }

  /// <summary>
  /// A function which gets a list of all values of an Enum.
  /// </summary>
  /// <typeparam name="TEnum">The type of Enum the values will be obtained from.</typeparam>
  /// <returns>Returns a list of enum values based on the type.</returns>
  public static List<TEnum> GetEnumList<TEnum>() where TEnum : System.Enum
  {
    return ((TEnum[])System.Enum.GetValues(typeof(TEnum))).ToList(); // Get all of the values, and convert to a List.
  }

  /// <summary>
  /// A function which converts any enum value into a string. It can be customized to use different types.
  /// </summary>
  /// <param name="enumeration">This enum to convert. Call as 'myEnum.ConvertToString(type)'</param>
  /// <param name="type">The way to convert this enum.</param>
  /// <returns>Returns the converted string.</returns>
  public static string ConvertToString(this System.Enum enumeration, StringType type = StringType.Normal)
  {
    string str = enumeration.ToString(); // Get the base version of the enum conversion.

    // Switch on the tpe.
    switch (type)
    {
      // Replace all Underscores with spaces.
      case StringType.Underscore:
        return str.Replace('_', ' ');
      // Add a space between each Camel Case.
      case StringType.CamelCase:
        return Regex.Replace(str, "((?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]))", " $1").Trim();
      // Return the original string, untouched.
      default:
        return str;
    }
  }
}
/**********************************************************************************************************************/