/**************************************************************************************************/
/*!
\file   BoardMarker.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing implementation for the board marker. This marker is used to show what spot on
  the board is being selected.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections;
using UnityEditor;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// The marker on the board is an icon used to show what spot is being selected by a player.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BoardMarker : E_Component
{
  /// <summary> The speed at which the marker moves around on the board. </summary>
  [SerializeField] private float moveSpeed = 5.0f;

  /// <summary> The sprite renderer of the marker. </summary>
  private SpriteRenderer spriteRend = null;
  /// <summary> The Routine for moving the board marker around. </summary>
  private ET_Routine movementRoutine = null;

  private void Awake()
  {
    spriteRend = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer.
  }

  /// <summary>
  /// A function to set the position of the marker directly.
  /// </summary>
  /// <param name="position">The position to set the marker at.</param>
  public void SetMarkerPosition(Vector3 position)
  {
    ET_Routine.StopRoutine(movementRoutine); // Stop the movement routine.
    transform.position = position; // Set the world position of the marker directly.
  }

  /// <summary>
  /// A function used to directly lerp the position of the marker.
  /// </summary>
  /// <param name="position">The position to lerp the marker to.</param>
  /// <returns>Returns the ET_Routine for the movement. This can be used to wait on the movement.</returns>
  public ET_Routine LerpMarkerPosition(Vector3 position)
  {
    ET_Routine.StopRoutine(movementRoutine); // Stop the current movement.
    movementRoutine = ET_Routine.StartRoutine(InternalLerpMarkerPosition(position)); // Create a new movement routine.
    return movementRoutine; // Return that routine.
  }

  /// <summary>
  /// The internal routine for lerping the position of the marker.
  /// </summary>
  /// <param name="position">The position to lerp the marker to.</param>
  /// <returns>Returns a routine representing the funciton.</returns>
  private IEnumerator InternalLerpMarkerPosition(Vector3 position)
  {
    Vector3 start = transform.position; // Get the initial position.
    // Wait for the lerp between the start and target positions.
    yield return EKIT_Math.SmoothLerp(pos => transform.position = pos, start, position, moveSpeed, EKIT_Math.EE_SmoothLerpMode.EaseBoth);
  }

  /// <summary>
  /// A function used to set the visibility of the marker.
  /// </summary>
  /// <param name="show">A bool determining if the marker will be visible or not.</param>
  public void SetMarkerVisiblity(bool show)
  {
    spriteRend.SetRendererColorChannel(show ? 1.0f : 0.0f, ColorChannel.Alpha); // Set the opacity based on if we're showing or not.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(BoardMarker))]
public sealed class BoardMarkerEditor : EGUI_Component
{
  private SerializedProperty sp_MoveSpeed;

  private bool markerExpanded;

  private void Enable_BoardMarker()
  {
    sp_MoveSpeed = serializedObject.FindProperty("moveSpeed");
  }

  private void Inspector_BoardMarker()
  {
    markerExpanded = EditorGUILayout.Foldout(markerExpanded, "Board Marker Properties", true);
    if (markerExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_MoveSpeed);
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_BoardMarker();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_BoardMarker();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif