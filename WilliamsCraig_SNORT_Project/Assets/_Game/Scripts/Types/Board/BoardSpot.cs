/**************************************************************************************************/
/*!
\file   BoardSpot.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file for the BoardSpot type, which is where players are able to select for their moves.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A position on the board that a player can select as their move. These change colors based on the
/// color of the player.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))] [RequireComponent(typeof(Collider))]
public class BoardSpot : E_Component
{
  /// <summary> The SpriteRenderer for this line. </summary>
  private SpriteRenderer spriteRend = null;
  /// <summary> The index of this particular spot on the board. </summary>
  public Vector2Int spotIndex = Vector2Int.zero;

  private void Awake()
  {
    spriteRend = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer.
  }

  /// <summary>
  /// A function used to set the entire color of this spot.
  /// </summary>
  /// <param name="color">The color to set the spot to.</param>
  public void SetSpotColor(Color color)
  {
    spriteRend.color = color; // Set the color.
  }

  /// <summary>
  /// A function used to set the opacity of the spot.
  /// </summary>
  /// <param name="opacity">The opacity to set the spot to.</param>
  public void SetSpotOpacity(float opacity)
  {
    spriteRend.SetRendererColorChannel(opacity, ColorChannel.Alpha); // Set the opacity for the renderer.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(BoardSpot))]
public sealed class BoardSpotEditor : EGUI_Component
{
  private bool spotExpanded;

  private void Inspector_BoardSpot()
  {
    spotExpanded = EditorGUILayout.Foldout(spotExpanded, "Board Spot Properties", true);
    if (spotExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayReadOnlyProperty<SpriteRenderer>(serializedObject.targetObject, "spriteRend", new GUIContent("Sprite Renderer"));
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_BoardSpot();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif