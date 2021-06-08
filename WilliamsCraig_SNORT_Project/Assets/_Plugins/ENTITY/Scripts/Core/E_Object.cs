/**************************************************************************************************/
/*!
\file   E_Object.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  The base GameObject for using Entity. All Entity Components require this.

\par References:
  - https://forum.unity.com/threads/passing-ref-variable-to-coroutine.379640/
  - https://answers.unity.com/questions/24640/how-do-i-return-a-value-from-a-coroutine.html
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: E_Object</para>
  /// <summary>
  /// A base class for all Objects in Entity. This is required by all E_Components.
  /// </summary>
  [DisallowMultipleComponent]
  public sealed class E_Object : ENTITY
  {
    /// <summary> A list of tags for this object. This is used in all Entity checks. </summary>
    [SerializeField] [EP_TagList] private List<string> eo_TagList = new List<string>();
  }
  /**********************************************************************************************************************/
#if UNITY_EDITOR
  /**********************************************************************************************************************/
  /// <para>Class Name: ENTITY_GUI</para>
  /// <summary>
  /// The core of all Custom Editors in Entity.
  /// </summary>
  [CustomEditor(typeof(E_Object))]
  public sealed class EGUI_Object : EGUI_ENTITY
  {
    /// <summary> The SerializedProperty for 'eo_TagList'. </summary>
    private SerializedProperty sp_EOTagList;

    /// <summary> A bool determining if the 'Object Properties' foldout is expanded. </summary>
    private bool expand_Object;

    /// <summary>
    /// A function containing all OnEnable functionality for ENTITY core properties.
    /// </summary>
    private void Enable_Object()
    {
      sp_EOTagList = serializedObject.FindProperty("eo_TagList");
    }

    /// <summary>
    /// A function containing all OnInspectorGUI functionality for ENTITY core properties.
    /// </summary>
    private void Inspector_Object()
    {
      expand_Object = EditorGUILayout.Foldout(expand_Object, "Object Properties", true); // Create a foldout for the Core Properties.

      // If the foldout is expanded, show all Core Properties.
      if (expand_Object)
      {
        int startIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;
        EKIT_EditorGUI.DrawArray(sp_EOTagList, "A list of tags used by this object. These are used in Entity functions.", "A tag on this object. Set tags as normal in Unity.", "Tag List", "Tag", EKIT_EditorGUI.EE_GUIArrayType.Button);
        EditorGUI.indentLevel = startIndent;
      }
    }

    private void OnEnable()
    {
      Enable_Core();
      Enable_Object();
    }

    public override void OnInspectorGUI()
    {
      Inspector_Core();
      Inspector_Object();
      serializedObject.ApplyModifiedProperties();
    }
  }
  /**********************************************************************************************************************/
#endif
}