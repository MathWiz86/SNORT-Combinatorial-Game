/**************************************************************************************************/
/*!
\file   BoardLine.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file for the BoardLine type, which is used to draw the connections between board spots.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections;
using UnityEditor;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// A line drawn between the rows and columns on the board. Purely for visuals.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BoardLine : E_Component
{
  /// <summary> The number of pixels represented in one sliced size unit. </summary>
  private const float LinePixels = 100.0f;

  /// <summary> The SpriteRenderer for this line. </summary>
  private SpriteRenderer spriteRend = null;

  private void Awake()
  {
    spriteRend = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer.
  }

  /// <summary>
  /// A helper function to get the width of the line. Used for line growth.
  /// </summary>
  /// <returns>Returns the width of the sprite renderer for the line.</returns>
  public float GetLineWidth()
  {
    return spriteRend.size.x; // Return the width.
  }

  /// <summary>
  /// A helper function to get the height of the line. Used for line growth.
  /// </summary>
  /// <returns>Returns the height of the sprite renderer for the line.</returns>
  public float GetLineHeight()
  {
    return spriteRend.size.y; // Return the height.
  }

  /// <summary>
  /// A function for setting the width of this line.
  /// </summary>
  /// <param name="width">The width of the sprite. This is divided by <see cref="LinePixels"/>.</param>
  public void SetLineWidth(float width)
  {
    spriteRend.size = new Vector2(width, spriteRend.size.y); // Set the new size.
  }

  /// <summary>
  /// A function for setting the height of this line.
  /// </summary>
  /// <param name="height">The height of the sprite. This is divided by <see cref="LinePixels"/>.</param>
  public void SetLineHeight(float height)
  {
    spriteRend.size = new Vector2(spriteRend.size.x, height); // Set the new size.
  }

  /// <summary>
  /// A routine for lerping the line's current width to a specified one.
  /// </summary>
  /// <param name="width">The target width of the line.</param>
  /// <param name="speed">The speed the line should lerp.</param>
  /// <returns>Returns an IEnumerator to iterate through this function. Useful with E_Routines.</returns>
  public IEnumerator LerpLineWidth(float width, float speed)
  {
    float start = spriteRend.size.x; // Get the starting width.
    // Smoothly lerp between the start and target width.
    yield return EKIT_Math.SmoothLerp(wi => SetLineWidth(wi), start, width, speed, EKIT_Math.EE_SmoothLerpMode.EaseBoth);
  }

  /// <summary>
  /// A routine for lerping the line's current height to a specified one.
  /// </summary>
  /// <param name="height">The target height of the line.</param>
  /// <param name="speed">The speed the line should lerp.</param>
  /// <returns>Returns an IEnumerator to iterate through this function. Useful with E_Routines.</returns>
  public IEnumerator LerpLineHeight(float height, float speed)
  {
    float start = spriteRend.size.y; // Get the starting height.
    // Smoothly lerp between the start and target height.
    yield return EKIT_Math.SmoothLerp(he => SetLineHeight(he), start, height, speed, EKIT_Math.EE_SmoothLerpMode.EaseBoth);
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(BoardLine))]
public sealed class BoardLineEditor : EGUI_Component
{
  private bool lineExpanded;

  private void Inspector_BoardLine()
  {
    lineExpanded = EditorGUILayout.Foldout(lineExpanded, "Board Line Properties", true);
    if (lineExpanded)
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
    Inspector_BoardLine();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif