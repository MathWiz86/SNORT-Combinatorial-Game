/**************************************************************************************************/
/*!
\file   EP_ReadOnly.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A property attribute for preventing a variable from being edited in editor. It can still be viewed.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ENTITY
{
  /// <summary>
  /// A PropertyAttribute for preventing a variable from being edited in the Inspector. You should place
  /// this Property immediately after the 'SerializeField' Property.
  /// NOTE: Arrays still allow their size to be changed, but not their elements. It is recommended you
  /// instead use the STATIC variant of the array in a Custom Editor. (EKIT_EditorGUI.DrawArray)
  /// </summary>
  public class EP_ReadOnly : E_Attribute { }

#if UNITY_EDITOR
  /// <summary>
  /// The PropertyDrawer for EP_ReadOnly This handles removing the ability to edit the variable.
  /// </summary>
  [CustomPropertyDrawer(typeof(EP_ReadOnly))]
  public class EP_ReadOnly_Drawer : E_PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Temporarily disable the GUI and draw the property.
      bool currentEnabled = GUI.enabled;
      GUI.enabled = false;
      EditorGUI.PropertyField(position, property, label, true);
      GUI.enabled = currentEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }
  }
#endif
}

