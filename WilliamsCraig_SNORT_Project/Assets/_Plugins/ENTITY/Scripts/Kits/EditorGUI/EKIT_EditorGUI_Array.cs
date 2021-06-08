/**************************************************************************************************/
/*!
\file   EKIT_EditorGUI_Array.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A piece of the EditorGUI Kit. This contains functionality for how to change how Arrays are
  drawn in the Inspector.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_EditorGUI
  {
    //! An enum determining how to draw an array in the Editor.
    public enum EE_GUIArrayType
    {
      Default, //!< Draw the array like normal.
      Static, //!< Draw the array without being able to change the size in the editor.
      Button, //!< Draw the array with buttons to add and remove elements.
    }

    /// <summary>
    /// A function used to customize how an array is drawn in the Editor.
    /// </summary>
    /// <param name="array">The array Serialized Property. If not an array, it is drawn normal.</param>
    /// <param name="arrayTip">The tooltip for the array</param>
    /// <param name="elementTip">The tooltip for each element</param>
    /// <param name="arrayName">The displayed name of the array.</param>
    /// <param name="elementName">The displayed name of each element. ' [INDEX]' is appended.</param>
    /// <param name="type">The type of array to draw.</param>
    public static void DrawArray(SerializedProperty array, string arrayTip = "", string elementTip = "", string arrayName = "", string elementName = "", EE_GUIArrayType type = EE_GUIArrayType.Default)
    {
      // If the property isn't an array, just draw it normally.
      if (array != null && array.isArray)
      {
        arrayName = (arrayName == string.Empty) ? array.displayName : arrayName; // If no array name is provided, simply use the display name.
        elementName = (elementName == string.Empty) ? "Element" : elementName; // If no element name is provided, simply use 'Element'.

        // Call a different Draw function based on the array type.
        switch (type)
        {
          case EE_GUIArrayType.Button:
            DrawArray_Button(array, arrayTip, elementTip, arrayName, elementName);
            break;
          case EE_GUIArrayType.Static:
            DrawArray_Static(array, arrayTip, elementTip, arrayName, elementName);
            break;
          default:
            DrawArray_Default(array, arrayTip, elementTip, arrayName, elementName);
            break;
        }
      }
      else
        EditorGUILayout.PropertyField(array);
    }



    /// <summary>
    /// A function used to draw an array in the default way.
    /// </summary>
    /// <param name="array">The array Serialized Property. If not an array, it is drawn normal.</param>
    /// <param name="arrayTip">The tooltip for the array</param>
    /// <param name="elementTip">The tooltip for each element</param>
    /// <param name="arrayName">The displayed name of the array.</param>
    /// <param name="elementName">The displayed name of each element. ' [INDEX]' is appended.</param>
    private static void DrawArray_Default(SerializedProperty array, string arrayTip, string elementTip, string arrayName, string elementName)
    {
      //EditorGUILayout.PropertyField(array, new GUIContent(arrayName, arrayTip), false);
      array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, new GUIContent(arrayName, arrayTip), true);
      // Only draw if the array is expanded.
      if (array.isExpanded)
      {
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(array.FindPropertyRelative("Array.size")); // Draw the array's size

        // Draw every element with the given name and tip.
        for (int i = 0; i < array.arraySize; i++)
          EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(elementName + " " + i.ToString(), elementTip), true);

        EditorGUI.indentLevel--;
      }
    }

    public static void DrawDefaultGUIArray(Rect position, SerializedProperty array, string arrayTip, string elementTip, string arrayName, string elementName)
    {
      
      position.height = 15.0f;
      //EditorGUILayout.PropertyField(array, new GUIContent(arrayName, arrayTip), false);
      array.isExpanded = EditorGUI.Foldout(position, array.isExpanded, new GUIContent(arrayName, arrayTip), true);
      // Only draw if the array is expanded.
      if (array.isExpanded)
      {
        EditorGUI.indentLevel++;
        position.y += 20.0f;
        position.height = 20.0f;
        EditorGUI.PropertyField(position, array.FindPropertyRelative("Array.size")); // Draw the array's size
        position.y += 3.0f;
        // Draw every element with the given name and tip.
        for (int i = 0; i < array.arraySize; i++)
        {
          position.y += 22.0f;
          EditorGUI.PropertyField(position, array.GetArrayElementAtIndex(i), new GUIContent(elementName + " " + i.ToString(), elementTip), true);
        }
          

        EditorGUI.indentLevel--;
      }
    }

    /// <summary>
    /// A function used to draw an array with buttons to add and remove elements.
    /// </summary>
    /// <param name="array">The array Serialized Property. If not an array, it is drawn normal.</param>
    /// <param name="arrayTip">The tooltip for the array</param>
    /// <param name="elementTip">The tooltip for each element</param>
    /// <param name="arrayName">The displayed name of the array.</param>
    /// <param name="elementName">The displayed name of each element. ' [INDEX]' is appended.</param>
    private static void DrawArray_Button(SerializedProperty array, string arrayTip, string elementTip, string arrayName, string elementName)
    {
       //EditorGUILayout.PropertyField(array, new GUIContent(arrayName, arrayTip), false);
       array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, new GUIContent(arrayName, arrayTip), true);
      // Only draw if the array is expanded.
      if (array.isExpanded)
      {
        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("Add/Remove " + elementName, EditorStyles.miniBoldLabel); // Draw a label for the buttons.
        // Draw an Add and Remove Button.
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15.0f * EditorGUI.indentLevel);

        // If clicked, add an element.
        if (GUILayout.Button(new GUIContent("Add", "Adds a new element."), EditorStyles.miniButtonLeft, GUILayout.MaxWidth(80.0f)))
        {
          if (array.arraySize > 1)
            array.InsertArrayElementAtIndex(array.arraySize - 1);
          else
            array.arraySize++;
        }

        // If clicked, remove an element
        if (GUILayout.Button(new GUIContent("Remove", "Removes the last element."), EditorStyles.miniButtonRight, GUILayout.MaxWidth(80.0f)))
        {
          if (array.arraySize > 0)
            array.arraySize--;
        }
        

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10.0f);

        // Draw every element with the given name and tip.
        for (int i = 0; i < array.arraySize; i++)
          EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(elementName + " " + i.ToString(), elementTip), true);

        EditorGUI.indentLevel--;
      }
    }

    /// <summary>
    /// A function used to draw an array without being able to change the size in the editor.
    /// </summary>
    /// <param name="array">The array Serialized Property. If not an array, it is drawn normal.</param>
    /// <param name="arrayTip">The tooltip for the array</param>
    /// <param name="elementTip">The tooltip for each element</param>
    /// <param name="arrayName">The displayed name of the array.</param>
    /// <param name="elementName">The displayed name of each element. ' [INDEX]' is appended.</param>
    private static void DrawArray_Static(SerializedProperty array, string arrayTip, string elementTip, string arrayName, string elementName)
    {
      // EditorGUILayout.PropertyField(array, new GUIContent(arrayName, arrayTip));
      array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, new GUIContent(arrayName, arrayTip), true);
      // Only draw if the array is expanded.
      if (array.isExpanded)
      {
        EditorGUI.indentLevel++;

        // Draw the Size Property without allowing the GUI to be enabled.
        bool currentEnabled = GUI.enabled;
        GUI.enabled = false;
        EditorGUILayout.PropertyField(array.FindPropertyRelative("Array.size"));
        GUI.enabled = currentEnabled;

        // Draw every element with the given name and tip.
        for (int i = 0; i < array.arraySize; i++)
          EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(elementName + " " + i.ToString(), elementTip), true);

        EditorGUI.indentLevel--;
      }
    }

    public static void DrawEnumArray<TEnum>(SerializedProperty array, string arrayTip = "", string elementTip = "", string arrayName = "") where TEnum : System.Enum
    {
      // If the property isn't an array, just draw it normally.
      if (array != null && array.isArray)
      {
        array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, new GUIContent(arrayName, arrayTip), true);
        // Only draw if the array is expanded.
        if (array.isExpanded)
        {
          EditorGUI.indentLevel++;
          string[] enum_names = KIT_Enum.GetEnumStringNames<TEnum>();
          array.arraySize = enum_names.Length;
          // Draw the Size Property without allowing the GUI to be enabled.
          bool currentEnabled = GUI.enabled;
          GUI.enabled = false;
          EditorGUILayout.PropertyField(array.FindPropertyRelative("Array.size"));
          GUI.enabled = currentEnabled;

          
          // Draw every element with the given name and tip.
          for (int i = 0; i < array.arraySize; i++)
            EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(enum_names[i], elementTip), true);

          EditorGUI.indentLevel--;
        }
      }
      else
        EditorGUILayout.PropertyField(array);
    }

    public static void DrawEnumArray(System.Type type, SerializedProperty array, string arrayTip = "", string elementTip = "", string arrayName = "")
    {
      // If the property isn't an array, just draw it normally.
      if (array != null && array.isArray && type.IsEnum)
      {
        array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, new GUIContent(arrayName, arrayTip), true);
        // Only draw if the array is expanded.
        if (array.isExpanded)
        {
          EditorGUI.indentLevel++;
          string[] enum_names = KIT_Enum.GetEnumStringNames(type);
          array.arraySize = enum_names.Length;
          // Draw the Size Property without allowing the GUI to be enabled.
          bool currentEnabled = GUI.enabled;
          GUI.enabled = false;
          EditorGUILayout.PropertyField(array.FindPropertyRelative("Array.size"));
          GUI.enabled = currentEnabled;


          // Draw every element with the given name and tip.
          for (int i = 0; i < array.arraySize; i++)
            EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(enum_names[i], elementTip), true);

          EditorGUI.indentLevel--;
        }
      }
      else
        EditorGUILayout.PropertyField(array);
    }

    public static Rect DrawEnumArray(System.Type type, Rect position, ref float height, SerializedProperty array, string arrayTip = "", string elementTip = "", string arrayName = "")
    {
      // If the property isn't an array, just draw it normally.
      if (array != null && array.isArray && type.IsEnum)
      {
        
        position.height = 15.0f;
        height += 15.0f;
        array.isExpanded = EditorGUI.Foldout(position, array.isExpanded, new GUIContent(arrayName, arrayTip), true);
        // Only draw if the array is expanded.
        if (array.isExpanded)
        {
          EditorGUI.indentLevel++;
          position.y += 20.0f;
          position.height = 20.0f;
          height += 20.0f;
          string[] enum_names = KIT_Enum.GetEnumStringNames(type);
          array.arraySize = enum_names.Length;
          // Draw the Size Property without allowing the GUI to be enabled.
          bool currentEnabled = GUI.enabled;
          GUI.enabled = false;
          EditorGUI.PropertyField(position, array.FindPropertyRelative("Array.size"));
          GUI.enabled = currentEnabled;

          //position.height = 50.0f;
          // Draw every element with the given name and tip.
          position.y += 3.0f;
          height += 3.0f;
          for (int i = 0; i < array.arraySize; i++)
          {
            position.y += 22.0f;
            height += 22.0f;
            // position.height += 15.0f;
            SerializedProperty element = array.GetArrayElementAtIndex(i);
            
            EditorGUI.PropertyField(position, element, new GUIContent(enum_names[i], elementTip), true);
            if (element.isExpanded)
            {
              position.y += EditorGUI.GetPropertyHeight(element, true);
              height += EditorGUI.GetPropertyHeight(element, true);
            }
              
          }
            

          EditorGUI.indentLevel--;
        }
      }
      else
        EditorGUI.PropertyField(position, array);

      return position;
    }
  }
  /**********************************************************************************************************************/
}
#endif