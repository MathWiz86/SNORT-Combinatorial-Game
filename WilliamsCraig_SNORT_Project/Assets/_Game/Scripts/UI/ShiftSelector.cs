/**************************************************************************************************/
/*!
\file   ShiftSelector.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the base ShiftSelector component. This component is used for the Game Setup
  Menu, for selectors that show a text moving back and forth with arrows.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ENTITY;
using UnityEditor;

/**********************************************************************************************************************/
/// <summary>
/// The base class for a menu selection. This is a selection where two arrows are used to swap between options,
/// with the text scrolling from side to side.
/// </summary>
public class ShiftSelector : E_Component
{
  /// <summary> A toggle for whether the text should animate, or immediately swap when clicking an arrow. </summary>
  [SerializeField] protected bool immediateMovement = false;
  /// <summary> The positions that the text should take when scrolling. X is negative, Y is visible, Z is positive. </summary>
  [SerializeField] protected Vector3 typePositions = Vector3.zero;
  /// <summary> The text component that is generally shown for an option. </summary>
  [SerializeField] protected TextMeshProUGUI tmpTypeA = null;
  /// <summary> A secondary text component for animating the scrolling correctly. </summary>
  [SerializeField] protected TextMeshProUGUI tmpTypeB = null;
  /// <summary> The speed at which to animate the text scroll. </summary>
  [SerializeField] protected float typeMoveSpeed = 3.0f;
  /// <summary> An image panel used to block mouse input while scrolling text. </summary>
  [SerializeField] protected Image typeRaycastBlocker = null;

  /// <summary> The RectTransform of <see cref="tmpTypeA"/>. </summary>
  private RectTransform typeARTR = null;
  /// <summary> The RectTransform of <see cref="tmpTypeB"/>. </summary>
  private RectTransform typeBRTR = null;

  protected virtual void Awake()
  {
    // Get the RectTransforms of the text now, so we always have them later when scrolling.
    typeARTR = tmpTypeA.GetComponent<RectTransform>();
    typeBRTR = tmpTypeB.GetComponent<RectTransform>();
  }

  /// <summary>
  /// A button function used to move the selection for this element.
  /// </summary>
  /// <param name="increase">A bool determining if the value of this selection is increasing or decreasing.</param>
  public virtual void ChangeSelection(bool increase) { }

  /// <summary>
  /// A routine for scrolling the two text componenets when changing the value of the selector.
  /// </summary>
  /// <param name="increase">A bool determining if the scroll will be to the left or right.</param>
  /// <returns>Returns a routine representing the function.</returns>
  protected virtual IEnumerator MoveSelectorText(bool increase)
  {
    typeRaycastBlocker.raycastTarget = true; // Enable the raycast blocker.

    // If we are not immediately moving, we will scroll.
    if (!immediateMovement)
    {
      float startA = typeARTR.anchoredPosition.x; // Get the starting position of the main text.
      float startB = increase ? typePositions.x : typePositions.z; // The start position of the side text is based on which direction to scroll.

      float targetA = increase ? typePositions.z : typePositions.x; // The end position fo the main text is based on which direction to scroll.
      float targetB = typePositions.y; // The end position is always the visible position.

      // Animate both the main text and side text together, and wait for both to stop.
      ET_Routine aRoutine = ET_Routine.StartRoutine(EKIT_Math.SmoothLerp(pos => typeARTR.SetAnchoredPositionX(pos), startA, targetA, typeMoveSpeed));
      yield return EKIT_Math.SmoothLerp(pos => typeBRTR.SetAnchoredPositionX(pos), startB, targetB, typeMoveSpeed);
      yield return aRoutine;
    }

    typeARTR.SetAnchoredPositionX(typePositions.y); // Return the main text to the middle.
    typeBRTR.SetAnchoredPositionX(typePositions.x); // Move the side text away from view.
    tmpTypeA.text = tmpTypeB.text; // Set the two text equal to show the right selection.

    typeRaycastBlocker.raycastTarget = false; // Disable the raycast blocker.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(ShiftSelector))]
public class ShiftSelectorEditor : EGUI_Component
{
  protected SerializedProperty sp_ImmediateMovement;
  protected SerializedProperty sp_TypePositions;
  protected SerializedProperty sp_TMPTypeA;
  protected SerializedProperty sp_TMPTypeB;
  protected SerializedProperty sp_TypeMoveSpeed;
  protected SerializedProperty sp_TypeRaycastBlocker;

  private bool selectorExpanded;

  protected void Enable_ShiftSelector()
  {
    sp_ImmediateMovement = serializedObject.FindProperty("immediateMovement");
    sp_TypePositions = serializedObject.FindProperty("typePositions");
    sp_TMPTypeA = serializedObject.FindProperty("tmpTypeA");
    sp_TMPTypeB = serializedObject.FindProperty("tmpTypeB");
    sp_TypeMoveSpeed = serializedObject.FindProperty("typeMoveSpeed");
    sp_TypeRaycastBlocker = serializedObject.FindProperty("typeRaycastBlocker");
  }

  protected void Inspector_ShiftSelector()
  {
    selectorExpanded = EditorGUILayout.Foldout(selectorExpanded, "Shift Selector Properties", true);
    if (selectorExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_ImmediateMovement);
      DisplayBasicProperty(sp_TypePositions);
      DisplayBasicProperty(sp_TMPTypeA);
      DisplayBasicProperty(sp_TMPTypeB);
      DisplayBasicProperty(sp_TypeMoveSpeed);
      DisplayBasicProperty(sp_TypeRaycastBlocker);
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_ShiftSelector();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_ShiftSelector();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif