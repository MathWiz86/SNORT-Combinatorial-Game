/**************************************************************************************************/
/*!
\file   MenuButton.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing implementation for a button on a UI Menu. These can be given events for being
  hovered, unhovered, and clicked.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

/**********************************************************************************************************************/
/// <summary>
/// A type of UI Button that can be given event functions for hovering, unhovering, and clicking.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MenuButton : E_Component, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
  /// <summary> The Image representing this button. This has its properties changed through events. </summary>
  [SerializeField] private Image buttonImage = null;
  /// <summary> The Text for this button. </summary>
  [SerializeField] private TextMeshProUGUI buttonText = null;

  /// <summary> Events for when the button is clicked. </summary>
  [SerializeField] private UnityEvent clickEvents = null;
  /// <summary> Events for when the button is hovered. </summary>
  [SerializeField] private UnityEvent enterEvents = null;
  /// <summary> Events for when the button is unhovered. </summary>
  [SerializeField] private UnityEvent exitEvents = null;

  /// <summary> A List of colors for the button. Index into this for events to get around UnityEvent limitations. </summary>
  [SerializeField] private List<Color> buttonColors = null;
  /// <summary> A List of vectors for the button. Index into this for events to get around UnityEvent limitations. </summary>
  [SerializeField] private List<Vector3> buttonVectors = null;

  /// <summary> The RectTransform of the whole button. </summary>
  private RectTransform buttonRTR = null;
  /// <summary> The RectTransform of the <see cref="buttonImage"/>. </summary>
  private RectTransform imageRTR = null;
  /// <summary> The RectTransform of <see cref="buttonText"/>. </summary>
  private RectTransform textRTR = null;

  private void Awake()
  {
    buttonRTR = GetComponent<RectTransform>(); // Get the RectTransform.
    imageRTR = buttonImage ? buttonImage.GetComponent<RectTransform>() : null; // Get the image's RectTransform, if it exists.
    textRTR = buttonText ? buttonText.GetComponent<RectTransform>() : null; // Get the text's RectTransform, if it exists.
  }

  /// <summary>
  /// An event function for setting the button color.
  /// </summary>
  /// <param name="index">The index of the color in <see cref="buttonColors"/>.</param>
  public void SetButtonColor(int index)
  {
    // If the image exists, and the index is valid, set the button's color to the specified color.
    if (buttonImage && buttonColors.IsValidIndex(index))
      buttonImage.color = buttonColors[index];
  }

  /// <summary>
  /// An event function for setting the button text.
  /// </summary>
  /// <param name="text">The text to set on the button.</param>
  public void SetButtonText(string text)
  {
    // If the text component exists, set the text.
    if (buttonText)
      buttonText.text = text;
  }

  /// <summary>
  /// An event function for setting the button text color.
  /// </summary>
  /// <param name="index">The index of the color in <see cref="buttonColors"/>.</param>
  public void SetButtonTextColor(int index)
  {
    // If the text component exists, and the index is valid, set the text color to the specified color.
    if (buttonText && buttonColors.IsValidIndex(index))
      buttonText.color = buttonColors[index];
  }

  /// <summary>
  /// An event function for setting the button position.
  /// </summary>
  /// <param name="index">The index of the vector in <see cref="buttonVectors"/>.</param>
  public void SetButtonPosition(int index)
  {
    // If the index is valid, set the button's anchored position to the specified vector.
    if (buttonVectors.IsValidIndex(index))
      buttonRTR.anchoredPosition = buttonVectors[index];
  }

  /// <summary>
  /// An event function for setting the button rotation.
  /// </summary>
  /// <param name="index">The index of the vector in <see cref="buttonVectors"/>.</param>
  public void SetButtonRotation(int index)
  {
    // IF the index is valid, set the button's euler angles to the specified vector.
    if (buttonVectors.IsValidIndex(index))
      buttonRTR.localEulerAngles = buttonVectors[index];
  }

  /// <summary>
  /// The function called upon the button being clicked on.
  /// </summary>
  /// <param name="eventData">The event data of the pointer.</param>
  public void OnPointerClick(PointerEventData eventData)
  {
    clickEvents?.Invoke(); // If there are events, invoke all click events.
  }

  /// <summary>
  /// The function called upon the button being hovered over.
  /// </summary>
  /// <param name="eventData">The event data of the pointer.</param>
  public void OnPointerEnter(PointerEventData eventData)
  {
    enterEvents?.Invoke(); // If there are events, invoke all hover events.
  }

  /// <summary>
  /// The function called upon the button no longer being hovered over.
  /// </summary>
  /// <param name="eventData">The event data of the pointer.</param>
  public void OnPointerExit(PointerEventData eventData)
  {
    exitEvents?.Invoke(); // If there are events, invoke all dehover events.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(MenuButton))]
public sealed class MenuButtonEditor : EGUI_Component
{
  private SerializedProperty sp_ButtonImage;
  private SerializedProperty sp_ButtonText;

  private SerializedProperty sp_ClickEvents;
  private SerializedProperty sp_EnterEvents;
  private SerializedProperty sp_ExitEvents;

  private SerializedProperty sp_ButtonColors;
  private SerializedProperty sp_ButtonVectors;

  private bool buttonExpanded;
  private bool infoExpanded;
  private bool eventsExpanded;
  private bool eventpropExpanded;

  private void Enable_MenuButton()
  {
    sp_ButtonImage = serializedObject.FindProperty("buttonImage");
    sp_ButtonText = serializedObject.FindProperty("buttonText");

    sp_ClickEvents = serializedObject.FindProperty("clickEvents");
    sp_EnterEvents = serializedObject.FindProperty("enterEvents");
    sp_ExitEvents = serializedObject.FindProperty("exitEvents");

    sp_ButtonColors = serializedObject.FindProperty("buttonColors");
    sp_ButtonVectors = serializedObject.FindProperty("buttonVectors");
  }

  private void Inspector_MenuButton()
  {
    buttonExpanded = EditorGUILayout.Foldout(buttonExpanded, "Button Properties", true);
    if (buttonExpanded)
    {
      EditorGUI.indentLevel++;

      infoExpanded = EditorGUILayout.Foldout(infoExpanded, "Button Info", true);
      if (infoExpanded)
      {
        EditorGUI.indentLevel++;
        DisplayBasicProperty(sp_ButtonImage);
        DisplayBasicProperty(sp_ButtonText);
        EditorGUI.indentLevel--;
      }

      eventpropExpanded = EditorGUILayout.Foldout(eventpropExpanded, "Event Properties", true);
      if (eventpropExpanded)
      {
        EditorGUI.indentLevel++;
        DisplayBasicProperty(sp_ButtonColors);
        DisplayBasicProperty(sp_ButtonVectors);
        EditorGUI.indentLevel--;
      }

      eventsExpanded = EditorGUILayout.Foldout(eventsExpanded, "Events", true);
      if (eventsExpanded)
      {
        EditorGUI.indentLevel++;
        DisplayBasicProperty(sp_EnterEvents);
        DisplayBasicProperty(sp_ExitEvents);
        DisplayBasicProperty(sp_ClickEvents);
        EditorGUI.indentLevel--;
      }
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_MenuButton();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_MenuButton();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif