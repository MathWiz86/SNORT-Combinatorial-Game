/**************************************************************************************************/
/*!
\file   Menu.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the base Menu component. Menus are used to show information and choose
  setup up options.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using UnityEditor;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// A group of UI elements that together create an interactive display. These can be used for several different implementations.
/// </summary>
[RequireComponent(typeof(Animator))] [RequireComponent(typeof(Canvas))]
public class Menu : E_Component
{
  /// <summary> The Animator component on this menu. </summary>
  public Animator Animator { get; private set; }
  /// <summary> The Canvas component on this menu. </summary>
  public Canvas Canvas { get; private set; }

  private void Awake()
  {
    Animator = GetComponent<Animator>(); // Get the Animator component.
    Canvas = GetComponent<Canvas>(); // Get the Canvas component.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(Menu))]
public sealed class MenuEditor : EGUI_Component
{
  private SerializedProperty sp_Animator;
  private SerializedProperty sp_Canvas;

  private bool menuExpanded;

  private void Enable_Menu()
  {
    sp_Animator = serializedObject.FindProperty("_animator");
    sp_Canvas = serializedObject.FindProperty("_canvas");
  }

  private void Inspector_Menu()
  {
    menuExpanded = EditorGUILayout.Foldout(menuExpanded, "Menu Properties", true);
    if (menuExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_Animator);
      DisplayBasicProperty(sp_Canvas);
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_Menu();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_Menu();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif