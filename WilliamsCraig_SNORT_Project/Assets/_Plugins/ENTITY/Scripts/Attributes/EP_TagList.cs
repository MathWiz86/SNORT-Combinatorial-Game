/**************************************************************************************************/
/*!
\file   EP_TagList.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A property attribute for displaying a Popup list of Unity Tags.

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
  /// A PropertyAttribute for displaying a Popup list of Unity Tags.
  /// </summary>
  public class EP_TagList : E_Attribute { }
  /**********************************************************************************************************************/

#if UNITY_EDITOR
  /**********************************************************************************************************************/
  /// <summary>
  /// The PropertyDrawer for EP_TagList. This handles displaying the Popup.
  /// </summary>
  [CustomPropertyDrawer(typeof(EP_TagList))]
  public class EP_TagList_Drawer : E_PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // This will only work on String properties.
      if (property.propertyType == SerializedPropertyType.String)
      {
        List<string> tags = new List<string>(); // The list of tags.

        EditorGUI.BeginProperty(position, label, property);
        tags.AddRange(UnityEditorInternal.InternalEditorUtility.tags); // Add all the tags stored in Unity to the list.

        // Create a Popup list out of the tags.
        EKIT_EditorGUI.CreatePopupList(position, property, label, tags);
      }
      else
        EditorGUI.PropertyField(position, property, label); // If not valid, just draw the property normally.

      EditorGUI.EndProperty();
    }
  }
  /**********************************************************************************************************************/
#endif
}