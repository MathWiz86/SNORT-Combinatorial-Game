/**************************************************************************************************/
/*!
\file   EKIT_Reflection_Values.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains functionality for getting values from a class in various ways. This includes
  from a SerializedProperty, a given object, a Field, or a Property. A Field is a standard variable.
  A Property is a { get; set; } variable.

\par References:
  - https://docs.microsoft.com/en-us/dotnet/api/system.reflection.bindingflags?view=netframework-4.8
  - https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo.getvalue?view=netframework-4.8
*/
/**************************************************************************************************/
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Reflection
  {
    /// <summary> The default flags to use to find variables via reflection.</summary>
    public static BindingFlags reflection_DefaultFlags { get { return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy; } }

    /// <summary>
    /// A function which gets MemberInfo for a variable at a specified path. Use this if you do not know if the variable
    /// is a field or a property.
    /// </summary>
    /// <param name="obj">The object to search for the variable.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <returns>Returns the MemberInfo of the variable, if it is found.</returns>
    public static MemberInfo GetMemberInfo(object obj, string path)
    {
      return GetMemberInfo(obj, path, reflection_DefaultFlags); // Get the member info with the default flags.
    }

    /// <summary>
    /// A function which gets MemberInfo for a variable at a specified path. Use this if you do not know if the variable
    /// is a field or a property.
    /// </summary>
    /// <param name="obj">The object to search for the variable.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the variable. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the MemberInfo of the variable, if it is found.</returns>
    public static MemberInfo GetMemberInfo(object obj, string path, BindingFlags flags)
    {
      // First, attempt to get the FieldInfo. If not a field, attempt to get the PropertyInfo.
      MemberInfo info = GetFieldInfo(obj, path, flags);
      if (info == null)
        info = GetPropertyInfo(obj, path, flags);

      return info; // Return what was found.
    }

    /// <summary>
    /// A function which gets FieldInfo for a variable within an object. Use this if you know that the variable is a normal field.
    /// </summary>
    /// <param name="obj">The object to search for the variable.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <returns>Returns the FieldInfo of the variable, if it is found.</returns>
    public static FieldInfo GetFieldInfo(object obj, string path)
    {
      return GetFieldInfo(obj, path, reflection_DefaultFlags); // Attempt to get the FieldInfo.
    }

    /// <summary>
    /// A function which gets FieldInfo for a variable within an object. Use this if you know that the variable is a normal field.
    /// </summary>
    /// <param name="obj">The object to search for the variable.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the variable. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the FieldInfo of the variable, if it is found.</returns>
    public static FieldInfo GetFieldInfo(object obj, string path, BindingFlags flags)
    {
      if (obj != null && path != null)
        return obj.GetType().GetField(path, flags); // Return the FieldInfo. This returns null if nothing is found.

      return null; // An input was null, so immediately return null.
    }

    /// <summary>
    /// A function which gets PropertyInfo for a variable within an object. Use this if you know that the variable is a property.
    /// </summary>
    /// <param name="obj">The object to search for the variable.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <returns>Returns the PropertyInfo of the variable, if it is found.</returns>
    public static PropertyInfo GetPropertyInfo(object obj, string path)
    {
      return GetPropertyInfo(obj, path, reflection_DefaultFlags); // Attempt to get the PropertyInfo.
    }

    /// <summary>
    /// A function which gets PropertyInfo for a variable within an object. Use this if you know that the variable is a property.
    /// </summary>
    /// <param name="obj">The object to search for the variable.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the variable. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the PropertyInfo of the variable, if it is found.</returns>
    public static PropertyInfo GetPropertyInfo(object obj, string path, BindingFlags flags)
    {
      if (obj != null && path != null)
        return obj.GetType().GetProperty(path, flags); // Return the PropertyInfo. This returns null if nothing is found.

      return null; // An input was null, so immediately return null.
    }

    /// <summary>
    /// A function which gets a value from a given object and property path. This merely calls 'GetFieldValue'
    /// and 'GetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property.
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are getting the variable from.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <returns>Returns the T value of the variable if found.</returns>
    public static T GetSerializedValue<T>(object obj, string path)
    {
      return GetSerializedValue<T>(obj, path, reflection_DefaultFlags);
    }

    /// <summary>
    /// A function which gets a value from a given object and property path. This merely calls 'GetFieldValue'
    /// and 'GetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property.
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are getting the variable from.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the T value of the variable if found.</returns>
    public static T GetSerializedValue<T>(object obj, string path, BindingFlags flags)
    {
      T value = GetFieldValue<T>(obj, path, flags); // Attempt to get the value through a Field.
      if (value == null)
        value = GetPropertyValue<T>(obj, path, flags); // If the value was not found, attempt to get the value through a Property.
      return value;
    }

    /// <summary>
    /// A function which sets a value into a serialized variable. This merely calls 'SetFieldValue'
    /// and 'SetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property.
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are setting the variable of.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetSerializedValue<T>(object obj, string path, T value)
    {
      return SetSerializedValue<T>(obj, path, reflection_DefaultFlags, value);
    }

    /// <summary>
    /// A function which sets a value into a serialized variable. This merely calls 'SetFieldValue'
    /// and 'SetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property.
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are setting the variable of.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetSerializedValue<T>(object obj, string path, BindingFlags flags, T value)
    {
      bool check = SetFieldValue<T>(obj, path, flags, value); // Attempt to set the value through a Field.
      if (!check)
        check = SetPropertyValue<T>(obj, path, flags, value); // If the value was not set, attempt to set the value through a Property.

      return check;
    }

    /// <summary>
    /// A function which gets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are getting the variable from.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetFieldValue<T>(object obj, string path)
    {
      return GetFieldValue<T>(obj, path, reflection_DefaultFlags);
    }

    /// <summary>
    /// A function which gets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are getting the variable from.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetFieldValue<T>(object obj, string path, BindingFlags flags)
    {
      if (obj == null || path == null)
        return default;

      // If the path ends with ']', then this is an array element. The property path needs to be parsed in a special way.
      if (path.EndsWith("]"))
      {
        int arrayIndex = 0; // The index of the variable we are trying to get.
        path = ParsePropertyPath(path, out arrayIndex); // Parse the property path and get the array index.
        string[] field_paths = path.Split('.');
        object current = obj;
        FieldInfo fieldInfo = null; // obj.GetType().GetField(field_paths[0], flags);
        for (int i = 0; i < field_paths.Length; i++)
        {
          fieldInfo = current.GetType().GetField(field_paths[i], flags);
          current = fieldInfo.GetValue(current);
        }
        // Continue if the Field was found.
        if (fieldInfo != null)
        {
          IList<T> array = current as IList<T>; // Get the array from the Field.
          // If the array is valid, and the index is valid, return the element at that index.
          if (array != null && array.IsValidIndex(arrayIndex))
            return array[arrayIndex];
        }

        return default; // Otherwise, return T's default value.
      }
      else
      {
        // If not an array element, we just have to get the FieldInfo as normal.
        //FieldInfo fieldInfo = obj.GetType().GetField(path, flags);
        string[] field_paths = path.Split('.');
        object current = obj;
        FieldInfo fieldInfo = null; // obj.GetType().GetField(field_paths[0], flags);
        for (int i = 0; i < field_paths.Length; i++)
        {
          fieldInfo = current.GetType().GetField(field_paths[i], flags);
          current = fieldInfo.GetValue(current);
        }
        // Continue if the Field was found.
        if (fieldInfo != null)
        {
          // If the value has the same type as T, return the value.
          object checkValue = current;
          if (checkValue != null && (checkValue is T))
            return (T)checkValue;
        }
      }
      
      return default; // Otherwise, return T's default value.
    }

    /// <summary>
    /// A function which sets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are setting the variable of.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetFieldValue<T>(object obj, string path, T value)
    {
      return SetFieldValue<T>(obj, path, reflection_DefaultFlags, value);
    }

    /// <summary>
    /// A function which sets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are setting the variable of.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetFieldValue<T>(object obj, string path, BindingFlags flags, T value)
    {
      if (obj == null || path == null)
        return false;

      // If the path ends with ']', then this is an array element. The property path needs to be parsed in a special way.
      if (path.EndsWith("]"))
      {
        int arrayIndex = 0; // The index of the variable we are trying to get.
        path = ParsePropertyPath(path, out arrayIndex); // Parse the property path and get the array index.
        string[] field_paths = path.Split('.');
        object current = obj;
        object previous = obj;
        FieldInfo fieldInfo = null;// current.GetType().GetField(field_paths[0], flags);
        for (int i = 0; i < field_paths.Length; i++)
        {
          fieldInfo = current.GetType().GetField(field_paths[i], flags);
          previous = current;
          current = fieldInfo.GetValue(current);
        }
        // Get the FieldInfo of the array, regardless of how the variable was made.
        //FieldInfo fieldInfo = obj.GetType().GetField(path, flags);
        // Continue if the Field was found.
        if (fieldInfo != null)
        {
          IList<T> array = current as IList<T>; // Get the array from the Field.
          // If the array is valid, and the index is valid, set the element's value.
          if (array != null && array.IsValidIndex(arrayIndex))
          {
            array[arrayIndex] = value; // Set the array element to the new value.
            fieldInfo.SetValue(previous, array); // Set the new list.
            return true; // The value was set.
          }
        }
        return false; // The value was not set.
      }
      else
      {
        // If not an array element, we just have to get the FieldInfo as normal.
        //FieldInfo fieldInfo = obj.GetType().GetField(path, flags);
        string[] field_paths = path.Split('.');
        object current = obj;
        object previous = obj;
        FieldInfo fieldInfo = null;// current.GetType().GetField(field_paths[0], flags);
        for (int i = 0; i < field_paths.Length; i++)
        {
          fieldInfo = current.GetType().GetField(field_paths[i], flags);
          previous = current;
          current = fieldInfo.GetValue(current);
        }
        // Continue if the Field was found.
        if (fieldInfo != null)
        {
          fieldInfo.SetValue(previous, value); // Set the value into the Field.
          return true; // The value was set.
        }
      }
      return false; // The value was not set.
    }
    
    /// <summary>
    /// A function which gets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are getting the variable from.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetPropertyValue<T>(object obj, string path)
    {
      return GetPropertyValue<T>(obj, path, reflection_DefaultFlags);
    }

    /// <summary>
    /// A function which gets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are getting the variable from.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetPropertyValue<T>(object obj, string path, BindingFlags flags)
    {
      if (obj == null || path == null)
        return default;

      // If the path ends with ']', then this is an array element. The property path needs to be parsed in a special way.
      if (path.EndsWith("]"))
      {
        int arrayIndex = 0; // The index of the variable we are trying to get.
        path = ParsePropertyPath(path, out arrayIndex); // Parse the property path and get the array index.

        // Get the PropertyInfo of the array, regardless of how the variable was made.
        PropertyInfo propertyInfo = obj.GetType().GetProperty(path, flags);
        // Continue if the Property was found.
        if (propertyInfo != null)
        {
          IList<T> array = (propertyInfo.GetValue(obj)) as IList<T>; // Get the array from the Property.

          // If the array is valid, and the index is valid, return the element at that index.
          if (array != null && array.IsValidIndex(arrayIndex))
            return array[arrayIndex];
        }

        return default; // Otherwise, return T's default value.
      }
      else
      {
        // If not an array element, we just have to get the PropertyInfo as normal.
        PropertyInfo propertyInfo = obj.GetType().GetProperty(path, flags);
        // Continue if the Property was found.
        if (propertyInfo != null)
        {
          // If the value has the same type as T, return the value.
          object checkValue = propertyInfo.GetValue(obj);
          if (checkValue != null && checkValue.GetType() == typeof(T))
            return (T)checkValue;
        }
      }

      return default; // Otherwise, return T's default value.
    }

    /// <summary>
    /// A function which sets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are setting the variable of.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetPropertyValue<T>(object obj, string path, T value)
    {
      return SetPropertyValue<T>(obj, path, reflection_DefaultFlags, value);
    }

    /// <summary>
    /// A function which sets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="obj">The object that we are setting the variable of.</param>
    /// <param name="path">The path to the variable within 'obj'. This should be what property we're trying to reach.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetPropertyValue<T>(object obj, string path, BindingFlags flags, T value)
    {
      if (obj == null || path == null)
        return false;

      // If the path ends with ']', then this is an array element. The property path needs to be parsed in a special way.
      if (path.EndsWith("]"))
      {
        int arrayIndex = 0; // The index of the variable we are trying to get.
        path = ParsePropertyPath(path, out arrayIndex); // Parse the property path and get the array index.

        // Get the PropertyInfo of the array, regardless of how the variable was made.
        PropertyInfo propertyInfo = obj.GetType().GetProperty(path, flags);
        // Continue if the Property was found.
        if (propertyInfo != null)
        {
          IList<T> array = (propertyInfo.GetValue(obj)) as IList<T>; // Get the array from the Property.

          // If the array is valid, and the index is valid, return the element at that index.
          if (array != null && array.IsValidIndex(arrayIndex))
          {
            array[arrayIndex] = value; // Set the array element to the new value.
            propertyInfo.SetValue(obj, array); // Set the new list.
            return true; // The value was set.
          }
        }
        return false; // The value was not set.
      }
      else
      {
        // If not an array element, we just have to get the PropertyInfo as normal.
        PropertyInfo propertyInfo = obj.GetType().GetProperty(path, flags);
        // Continue if the Property was found.
        if (propertyInfo != null)
        {
          propertyInfo.SetValue(obj, value); // Set the value into the Property.
          return true; // The value was set.
        }
      }

      return false; // The value was not set.
    }

    /// <summary>
    /// A helper function which parses a Property Path in order to get to an array element.
    /// Use this when getting the value of an array element.
    /// </summary>
    /// <param name="path">The path to parse</param>
    /// <param name="arrayIndex">The array index of the element. This returns the index the variable is in the array. Returns -1 if the element wasn't found.</param>
    /// <returns>Returns the property path to the array, rather than the array element.
    /// This is required to set values within an array.</returns>
    private static string ParsePropertyPath(string path, out int arrayIndex)
    {
      string parsed = path; // The parsed path.
      arrayIndex = -1; // Start at -1 as a default value.

      // Check if the path is an array path.
      if (parsed.EndsWith("]"))
      {
        int cutIndex = parsed.LastIndexOf(".data"); // The index to cut the string at. '.data' is a key part of an array element's path.

        string stringIndex = parsed.Substring(cutIndex + 6); // The index of the array element. It must be parsed out of the path, after '.data['
        stringIndex = stringIndex.Remove(stringIndex.Length - 1); // The last character is ']'. All other characters in this string are now just numbers.

        arrayIndex = int.Parse(stringIndex); // The array index of the element.

        cutIndex = parsed.LastIndexOf(".Array"); // Get the new cut index, which is at '.Array' in a property path.

        parsed = parsed.Remove(cutIndex); // Remove the last part of the path. We just want the raw name of the variable.
      }

      return parsed; // Return the parsed path.
    }

#if UNITY_EDITOR
    /// <summary>
    /// A function which gets MemberInfo for a variable from a serialized property. Use this if you do not know if the serialized variable
    /// is a field or a property.
    /// </summary>
    /// <param name="property">The SerializedProperty to get the info from. A SerializedProperty has the target object and path within it.</param>
    /// <returns>Returns the MemberInfo of the variable, if it is found.</returns>
    public static MemberInfo GetMemberInfo(SerializedProperty property)
    {
      return GetMemberInfo(property, reflection_DefaultFlags); // Attempt to get the MemberInfo.
    }

    /// <summary>
    /// A function which gets MemberInfo for a variable from a serialized property. Use this if you do not know if the serialized variable
    /// is a field or a property.
    /// </summary>
    /// <param name="property">The SerializedProperty to get the info from. A SerializedProperty has the target object and path within it.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the MemberInfo of the variable, if it is found.</returns>
    public static MemberInfo GetMemberInfo(SerializedProperty property, BindingFlags flags)
    {
      return GetMemberInfo(property.serializedObject.targetObject, property.propertyPath, flags); // Attempt to get the MemberInfo.
    }

    /// <summary>
    /// A function which gets FieldInfo for a variable within a serialized property. Use this if you know that the variable is a normal field.
    /// </summary>
    /// <param name="property">The SerializedProperty to get the info from. A SerializedProperty has the target object and path within it.</param>
    /// <returns>Returns the FieldInfo of the variable, if it is found.</returns>
    public static FieldInfo GetFieldInfo(SerializedProperty property)
    {
      return GetFieldInfo(property, reflection_DefaultFlags); // Attempt to get the FieldInfo.
    }

    /// <summary>
    /// A function which gets FieldInfo for a variable within a serialized property. Use this if you know that the variable is a normal field.
    /// </summary>
    /// <param name="property">The SerializedProperty to get the info from. A SerializedProperty has the target object and path within it.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the FieldInfo of the variable, if it is found.</returns>
    public static FieldInfo GetFieldInfo(SerializedProperty property, BindingFlags flags)
    {
      return GetFieldInfo(property.serializedObject.targetObject, property.propertyPath, flags); // Attempt to get the FieldInfo.
    }

    /// <summary>
    /// A function which gets PropertyInfo for a variable within a serialized property. Use this if you know that the variable is a property.
    /// </summary>
    /// <param name="property">The SerializedProperty to get the info from. A SerializedProperty has the target object and path within it.</param>
    /// <returns>Returns the PropertyInfo of the variable, if it is found.</returns>
    public static PropertyInfo GetPropertyInfo(SerializedProperty property)
    {
      return GetPropertyInfo(property, reflection_DefaultFlags); // Attempt to get the PropertyInfo.
    }

    /// <summary>
    /// A function which gets PropertyInfo for a variable within a serialized property. Use this if you know that the variable is a property.
    /// </summary>
    /// <param name="property">The SerializedProperty to get the info from. A SerializedProperty has the target object and path within it.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the PropertyInfo of the variable, if it is found.</returns>
    public static PropertyInfo GetPropertyInfo(SerializedProperty property, BindingFlags flags)
    {
      return GetPropertyInfo(property.serializedObject.targetObject, property.propertyPath, flags); // Attempt to get the PropertyInfo.
    }

    /// <summary>
    /// A function which gets a value from an object. This merely calls 'GetFieldValue'
    /// and 'GetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property. 
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SerializedProperty to get the value from. A SerializedProperty has the target object and path within it.</param>
    /// <returns>Returns the T value of the variable if found.</returns>
    public static T GetSerializedValue<T>(SerializedProperty property)
    {
      // Use the property's target object and property path to get the value.
      return GetSerializedValue<T>(property, reflection_DefaultFlags);
    }

    /// <summary>
    /// A function which gets a value from an object. This merely calls 'GetFieldValue'
    /// and 'GetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property. 
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SerializedProperty to get the value from. A SerializedProperty has the target object and path within it.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the T value of the variable if found.</returns>
    public static T GetSerializedValue<T>(SerializedProperty property, BindingFlags flags)
    {
      // Use the property's target object and property path to get the value.
      return GetSerializedValue<T>(property.serializedObject.targetObject, property.propertyPath, flags);
    }

    /// <summary>
    /// A function which sets a value into a serialized variable. This merely calls 'SetFieldValue'
    /// and 'SetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property.
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SeralizedProperty we are setting the value of.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetSerializedValue<T>(SerializedProperty property, T value)
    {
      // Use the property's target object and property path to set the value.
      return SetSerializedValue<T>(property, reflection_DefaultFlags, value);
    }

    /// <summary>
    /// A function which sets a value into a serialized variable. This merely calls 'SetFieldValue'
    /// and 'SetPropertyValue' to achieve this. Use this when you are unsure if the variable is a Field or Property.
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SeralizedProperty we are setting the value of.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetSerializedValue<T>(SerializedProperty property, BindingFlags flags, T value)
    {
      // Use the property's target object and property path to set the value.
      return SetSerializedValue<T>(property.serializedObject.targetObject, property.propertyPath, flags, value);
    }

    /// <summary>
    /// A function which gets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SerializedProperty to get the value from. A SerializedProperty has the target object and path within it.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetFieldValue<T>(SerializedProperty property)
    {
      // Use the property's target object and property path to get the value.
      return GetFieldValue<T>(property, reflection_DefaultFlags);
    }

    /// <summary>
    /// A function which gets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SerializedProperty to get the value from. A SerializedProperty has the target object and path within it.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetFieldValue<T>(SerializedProperty property, BindingFlags flags)
    {
      // Use the property's target object and property path to get the value.
      return GetFieldValue<T>(property.serializedObject.targetObject, property.propertyPath, flags);
    }

    /// <summary>
    /// A function which sets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SeralizedProperty we are setting the value of.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetFieldValue<T>(SerializedProperty property, T value)
    {
      // Use the property's target object and property path to set the value.
      return SetFieldValue<T>(property, reflection_DefaultFlags, value);
    }

    /// <summary>
    /// A function which sets a value within a Field variable. A Field variable is the standard
    /// version of a variable in C# (i.e. int testInt = 4;)
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SeralizedProperty we are setting the value of.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetFieldValue<T>(SerializedProperty property, BindingFlags flags, T value)
    {
      // Use the property's target object and property path to set the value.
      return SetFieldValue<T>(property.serializedObject.targetObject, property.propertyPath, flags, value);
    }

    /// <summary>
    /// A function which gets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SerializedProperty to get the value from. A SerializedProperty has the target object and path within it.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetPropertyValue<T>(SerializedProperty property)
    {
      // Use the property's target object and property path to get the value.
      return GetPropertyValue<T>(property, reflection_DefaultFlags);
    }

    /// <summary>
    /// A function which gets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SerializedProperty to get the value from. A SerializedProperty has the target object and path within it.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <returns>Returns the T value of the variable if found. Returns T's default value otherwise.</returns>
    public static T GetPropertyValue<T>(SerializedProperty property, BindingFlags flags)
    {
      // Use the property's target object and property path to get the value.
      return GetPropertyValue<T>(property.serializedObject.targetObject, property.propertyPath, flags);
    }

    /// <summary>
    /// A function which sets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SeralizedProperty we are setting the value of.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetPropertyValue<T>(SerializedProperty property, T value)
    {
      // Use the property's target object and property path to set the value.
      return SetPropertyValue<T>(property, reflection_DefaultFlags, value);
    }

    /// <summary>
    /// A function which sets a value within a Property variable. A Property variable is the has
    /// a 'get; set;' with it. (i.e. int testInt { get; private set; })
    /// </summary>
    /// <typeparam name="T">The type that the value of the variable should be.</typeparam>
    /// <param name="property">The SeralizedProperty we are setting the value of.</param>
    /// <param name="flags">The flags to use to determine how to find the value. If unsure, use 'EKIT_Reflection.reflection_DefaultFlags'.</param>
    /// <param name="value">The value that should be set to the variable.</param>
    /// <returns>Returns if the variable was successfully set or not.</returns>
    public static bool SetPropertyValue<T>(SerializedProperty property, BindingFlags flags, T value)
    {
      // Use the property's target object and property path to set the value.
      return SetPropertyValue<T>(property.serializedObject.targetObject, property.propertyPath, flags, value);
    }
#endif
  }
  /**********************************************************************************************************************/
}