/**************************************************************************************************/
/*!
\file   EP_SortingLayerList.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A property attribute for displaying a Popup list of Unity Sorting Layers.

\par References:
  - https://forum.unity.com/threads/list-of-sorting-layers.210683/
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <summary>
  /// A PropertyAttribute for displaying a Popup list of Unity Sorting Layers
  /// </summary>
  public class EP_SortLayerList : E_Attribute { }
  /**********************************************************************************************************************/

#if UNITY_EDITOR
  /**********************************************************************************************************************/
  /// <summary>
  /// The PropertyDrawer for EP_SortLayerList This handles displaying the Popup.
  /// </summary>
  [CustomPropertyDrawer(typeof(EP_SortLayerList))]
  public class EP_SortLayerList_Drawer : E_PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // This will only work on String properties.
      if (property.propertyType == SerializedPropertyType.String)
      {
        List<string> slayers = new List<string>(); // The list of sorting layers.

        EditorGUI.BeginProperty(position, label, property);

        // The sorting layers must be gotten through the Reflection System. Don't ask me why we can get tags and layers normally, but not sorting layers.
        string[] layerNames = ((string[])typeof(UnityEditorInternal.InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, new object[0]));
        slayers.AddRange(layerNames); // Add all the sorting layers stored in Unity to the list.

        // Create a Popup list out of the sorting layers.
        EKIT_EditorGUI.CreatePopupList(position, property, label, slayers);
      }
      else
        EditorGUI.PropertyField(position, property, label); // If not valid, just draw the property normally.

      EditorGUI.EndProperty();
    }
  }
  /**********************************************************************************************************************/
#endif
}