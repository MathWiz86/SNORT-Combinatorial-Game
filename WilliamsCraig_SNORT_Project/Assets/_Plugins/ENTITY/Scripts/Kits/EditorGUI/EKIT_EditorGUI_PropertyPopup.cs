/**************************************************************************************************/
/*!
\file   EKIT_EditorGUI_PropertyPopup.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A piece of the EditorGUI Kit. This allows for getting a Popup's value for a Serialized Property.

\par References:
  - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/how-to-convert-a-string-to-a-number
  - https://stackoverflow.com/questions/497261/how-do-i-get-the-first-element-from-an-ienumerablet-in-net
  - https://stackoverflow.com/questions/2912079/how-to-go-to-particular-item-in-ienumerable

*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_EditorGUI
  {
    /// <summary>
    /// A function that creates an EditorGUI Popup list out of a given list. This will make a list of strings out of the given list.
    /// </summary>
    /// <typeparam name="T">The type inside the list.</typeparam>
    /// <param name="position">The Rect position of the popup.</param>
    /// <param name="property">The serialized property to display.</param>
    /// <param name="label">The label of the property.</param>
    /// <param name="list">The list to create options out of.</param>
    public static void CreatePopupList<T>(Rect position, SerializedProperty property, GUIContent label, List<T> list)
    {
      // Check if the list is valid.
      if (list != null && list.Count > 0)
      {
        var value = EKIT_Reflection.GetSerializedValue<T>(property); // Attempt to get the property's stored value.

        // If the value is valid, try to get its index.
        if (value !=  null)
        {
          int index = 0; // The index of the current selection.

          // Go through the list and find the value. If it's found, set that as the index.
          for (int i = 0; i < list.Count; i++)
          {
            if (object.Equals(list[i], value))
            {
              index = i;
              break;
            }
          }

          // Make a list of strings out of the options in the list.
          List<string> listToString = new List<string>();
          foreach (T o in list)
          {
            listToString.Add(o.ToString());
          }

          index = EditorGUI.Popup(position, label.text, index, listToString.ToArray()); // Get the correct popup value.

          // If the index is valid, set that value to the property.
          if (index >= 0)
            EKIT_Reflection.SetSerializedValue(property, list[index]);
        }
      }
      else
      {
        // If the list isn't valid, do not allow it to be changed.
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property);
        property.stringValue = "THE REQUESTED LIST IS EMPTY";
        GUI.enabled = true;
      }

      property.serializedObject.ApplyModifiedProperties(); // Apply the modifications.
    }
  }
  /**********************************************************************************************************************/
}
#endif