/**************************************************************************************************/
/*!
\file   EP_LayerList.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A property attribute for displaying a Popup list of Unity Layers.

\par References:
*/
/**************************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <summary>
  /// A PropertyAttribute for displaying a Popup list of Unity Layers.
  /// </summary>
  public class EP_LayerList : E_Attribute { }
  /**********************************************************************************************************************/

#if UNITY_EDITOR
  /**********************************************************************************************************************/
  /// <summary>
  /// The PropertyDrawer for EP_LayerList. This handles displaying the Popup.
  /// </summary>
  [CustomPropertyDrawer(typeof(EP_LayerList))]
  public class EP_LayerList_Drawer : E_PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // This will only work on String properties.
      if (property.propertyType == SerializedPropertyType.String)
      {
        List<string> layers = new List<string>(); // The list of layers.

        EditorGUI.BeginProperty(position, label, property);
        layers.AddRange(UnityEditorInternal.InternalEditorUtility.layers); // Add all the layers stored in Unity to the list.

        // Create a Popup list out of the layers.
        EKIT_EditorGUI.CreatePopupList(position, property, label, layers);
      }
      else
        EditorGUI.PropertyField(position, property, label); // If not valid, just draw the property normally.

      EditorGUI.EndProperty();
    }
  }
  /**********************************************************************************************************************/
#endif
}