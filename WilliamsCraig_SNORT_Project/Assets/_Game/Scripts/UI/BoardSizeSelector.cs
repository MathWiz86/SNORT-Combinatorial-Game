/**************************************************************************************************/
/*!
\file   BoardSizeSelector.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the UI selector for the board size. This goes from 1 to the max size allowed
  by the game's settings.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using UnityEngine;
using UnityEditor;

/**********************************************************************************************************************/
/// <summary>
/// The <see cref="ShiftSelector"/> for the Board Size. This goes from 1 to the max size allowed, as set in the
/// <see cref="SnortSystem"/>.
/// </summary>
public class BoardSizeSelector : ShiftSelector
{
  /// <summary> A bool simply determining if the value set is the X Axis or the Y axis. </summary>
  [SerializeField] private bool xAxis = true;

  /// <summary> The formatting used for the text. </summary>
  private string sizeFormatting = "D2";

  private void Start()
  {
    // Get the size of the board, either the X or Y axis based on the toggle.
    tmpTypeA.text = xAxis ? SnortSystem.BoardSize.x.ToString(sizeFormatting) : SnortSystem.BoardSize.y.ToString(sizeFormatting);
  }

  public override void ChangeSelection(bool increase)
  {
    Vector2Int currentSize = SnortSystem.BoardSize; // Get the current board size.
    int currentIndex = xAxis ? currentSize.x : currentSize.y; // Get the axis value we currently have.

    currentIndex += increase ? 1 : -1; // Increase or decrease the size.

    // Loop the index around, based on the board boundaries (X = min, Y = max).
    if (currentIndex < SnortSystem.boardSizeBoundaries.x)
      currentIndex = SnortSystem.boardSizeBoundaries.y;
    if (currentIndex > SnortSystem.boardSizeBoundaries.y)
      currentIndex = SnortSystem.boardSizeBoundaries.x;

    // Reset the axis based on the toggle.
    if (xAxis)
      currentSize.x = currentIndex;
    else
      currentSize.y = currentIndex;

    SnortSystem.BoardSize = currentSize; // Set back the board size to the system.

    tmpTypeB.text = currentIndex.ToString(sizeFormatting); // Set the value of the side text.

    ET_Routine.StartRoutine(MoveSelectorText(increase)); // Start the movement routine.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(BoardSizeSelector))]
public sealed class BoardSizeSelectorEditor : ShiftSelectorEditor
{
  private SerializedProperty sp_XAxis;
  private bool boardExpanded;

  private void Enable_BoardSize()
  {
    sp_XAxis = serializedObject.FindProperty("xAxis");
  }

  private void Inspector_BoardSize()
  {
    boardExpanded = EditorGUILayout.Foldout(boardExpanded, "Board Size Properties", true);
    if (boardExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_XAxis);
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_ShiftSelector();
    Enable_BoardSize();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_ShiftSelector();
    Inspector_BoardSize();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif