/**************************************************************************************************/
/*!
\file   GameUI.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the implementation of the main UI for the game. This appears during actual play.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;
using System.Text;
using ENTITY;
using TMPro;
using UnityEditor;

/**********************************************************************************************************************/
/// <summary>
/// The main UI of the Snort game. This appears during actual gameplay.
/// </summary>
public class GameUI : E_Component
{
  /// <summary> The Animator Component for the UI Bar that appears to display information. </summary>
  [SerializeField] private Animator barAnimator = null;
  /// <summary> The Text Component for the UI Bar that appears to display information. </summary>
  [SerializeField] private TextMeshProUGUI tmpBar = null;
  /// <summary> The name of the Animator State to show the UI Bar. </summary>
  [SerializeField] private string stateShow = "Show";
  /// <summary> The name of the Animator State to hide the UI Bar. </summary>
  [SerializeField] private string stateHide = "Hide";

  /// <summary> The Text to show which player is currently moving. </summary>
  [SerializeField] private TextMeshProUGUI tmpCurrentPlayer = null;
  /// <summary> The Text that shows the instructions for a player on their turn. </summary>
  [SerializeField] private TextMeshProUGUI tmpInstructions = null;

  /// <summary>
  /// A function which sets the UI Bar's text to show who's turn it is.
  /// </summary>
  /// <param name="playerIndex">The index of the player, based on the <see cref="PlayerID"/> enum.</param>
  public void SetBarTextPlayerTurn(int playerIndex)
  {
    // Set the bar text. The index is incremented by one, as the index starts at 0.
    tmpBar.text = new StringBuilder("Player ").Append(playerIndex + 1).Append("'s Turn!").ToString();
    // Set the text for who's turn it currently is.
    tmpCurrentPlayer.text = new StringBuilder("Current Player: Player ").Append(playerIndex + 1).ToString();
  }

  /// <summary>
  /// A function which sets the UI Bar's text to display that there are no more moves.
  /// </summary>
  public void SetBarTextNoMoves()
  {
    tmpBar.text = "No More Moves..."; // Set the text to display that there are no more moves.
  }

  /// <summary>
  /// A function which sets the UI Bar's text to display the winner of the game.
  /// </summary>
  /// <param name="playerIndex">The index of the player, based on the <see cref="PlayerID"/> enum.</param>
  public void SetBarTextPlayerWin(int playerIndex)
  {
    // Set the text to display the winner. The player index is increased by one, as it starts at 0.
    tmpBar.text = new StringBuilder("Player ").Append(playerIndex + 1).Append(" Wins!").ToString();
  }

  /// <summary>
  /// A routine to show or hide the UI Bar.
  /// </summary>
  /// <param name="show">A bool determining if the bar is being shown or hidden.</param>
  /// <returns>Returns a routine representing the function.</returns>
  public IEnumerator AnimateBar(bool show)
  {
    yield return KIT_Animation.WaitForAnimationEnd(barAnimator, show ? stateShow : stateHide); // Animate the bar based on if we are showing or hiding it.
  }

  /// <summary>
  /// A function which toggles the extra game UI on or off. This toggles the <see cref="tmpCurrentPlayer"/> and <see cref="tmpInstructions"/> text.
  /// </summary>
  /// <param name="show">A bool determining if the UI is being shown or hidden.</param>
  public void ToggleUIDisplay(bool show)
  {
    tmpInstructions.gameObject.SetActive(show); // Toggle the instruction text.
    tmpCurrentPlayer.gameObject.SetActive(show); // Toggle the current player text.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(GameUI))]
public sealed class GameUIEditor : EGUI_Component
{
  private SerializedProperty sp_BarAnimator;
  private SerializedProperty sp_TMPBar;
  private SerializedProperty sp_StateShow;
  private SerializedProperty sp_StateHide;

  private SerializedProperty sp_TMPCurrentPlayer;
  private SerializedProperty sp_TMPInstructions;

  private bool uiExpanded;

  private void Enable_GameUI()
  {
    sp_BarAnimator = serializedObject.FindProperty("barAnimator");
    sp_TMPBar = serializedObject.FindProperty("tmpBar");
    sp_StateShow = serializedObject.FindProperty("stateShow");
    sp_StateHide = serializedObject.FindProperty("stateHide");

    sp_TMPCurrentPlayer = serializedObject.FindProperty("tmpCurrentPlayer");
    sp_TMPInstructions = serializedObject.FindProperty("tmpInstructions");
  }

  private void Inspector_GameUI()
  {
    uiExpanded = EditorGUILayout.Foldout(uiExpanded, "Setup Properties", true);
    if (uiExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_BarAnimator);
      DisplayBasicProperty(sp_TMPBar);
      EditorGUILayout.Space(5.0f);
      DisplayBasicProperty(sp_StateShow);
      DisplayBasicProperty(sp_StateHide);
      EditorGUILayout.Space(5.0f);
      DisplayBasicProperty(sp_TMPCurrentPlayer);
      DisplayBasicProperty(sp_TMPInstructions);
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_GameUI();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_GameUI();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif