/**************************************************************************************************/
/*!
\file   MenuGameSetup.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing implementation for the Game Setup Menu. This menu handles making sure all panels
  for selecting players update correctly.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/**********************************************************************************************************************/
/// <summary>
/// The <see cref="Menu"/> for the Game Setup. This is used to update colors for the <see cref="PlayerSetupPanel"/>s.
/// </summary>
public class MenuGameSetup : E_Component
{
  /// <summary> The <see cref="PlayerSetupPanel"/>s for all players. </summary>
  [SerializeField] private List<PlayerSetupPanel> setupPanels = new List<PlayerSetupPanel>();
  
  /// <summary>
  /// A function to update the available colors on all <see cref="PlayerSetupPanel"/>s.
  /// </summary>
  public void UpdatePanelColors()
  {
    // For all panels, update the available colors.
    foreach (PlayerSetupPanel p in setupPanels)
      p.UpdateAvailableColors();
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(MenuGameSetup))]
public sealed class MenuGameSetupEditor : EGUI_Component
{
  private SerializedProperty sp_SetupPanels;

  private bool gamesetupExpanded;

  private void Enable_GameSetup()
  {
    sp_SetupPanels = serializedObject.FindProperty("setupPanels");
  }

  private void Inspector_GameSetup()
  {
    gamesetupExpanded = EditorGUILayout.Foldout(gamesetupExpanded, "Game Setup Properties", true);
    if (gamesetupExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_SetupPanels);
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_GameSetup();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_GameSetup();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif