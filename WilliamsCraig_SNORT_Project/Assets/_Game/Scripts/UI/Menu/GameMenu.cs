/**************************************************************************************************/
/*!
\file   GameMenu.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the main menu system for the game.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/**********************************************************************************************************************/
/// <summary>
/// The main Menu System for the Snort Game. This handles animating each <see cref="Menu"/> and loading the game up.
/// </summary>
public class GameMenu : E_Component
{
  /// <summary> The Menu for the Title Screen. </summary>
  [SerializeField] protected Menu menuTitle = null;
  /// <summary> The Menu for the Credits Screen. </summary>
  [SerializeField] protected Menu menuCredits = null;
  /// <summary> The Menu for the Game Setup Screen. </summary>
  [SerializeField] protected Menu menuGame = null;

  /// <summary> The name of the Animator State for showing a Menu. </summary>
  [SerializeField] protected string stateShowNormal = "Show";
  /// <summary> The name of the Animator State for hiding a Menu. </summary>
  [SerializeField] protected string stateHideNormal = "Hide";

  /// <summary> The image screen used to block the mouse while changing <see cref="Menu"/>s. </summary>
  [SerializeField] protected Image raycastBlock = null;
  /// <summary> The background image for the menu. </summary>
  [SerializeField] protected Image menuBG = null;
  /// <summary> The speed to fade <see cref="menuBG"/> in and out. </summary>
  [SerializeField] protected float bgFadeSpeed = 1.0f;

  /// <summary>
  /// A button function to swap from the Title Screen to the Credits Screen.
  /// </summary>
  public void DisplayCredits()
  {
    ET_Routine.StartRoutine(SwapMenus(menuTitle, menuCredits));
  }

  /// <summary>
  /// A button function to swap from the Credits Screen to the Title Screen.
  /// </summary>
  public void ReturnFromCredits()
  {
    ET_Routine.StartRoutine(SwapMenus(menuCredits, menuTitle));
  }

  /// <summary>
  /// A button function to swap from the Title Screen to the Game Setup Screen.
  /// </summary>
  public void DisplayGameSetup()
  {
    ET_Routine.StartRoutine(SwapMenus(menuTitle, menuGame));
  }

  /// <summary>
  /// A button function to swap from the Game Setup Screen to the Title Screen.
  /// </summary>
  public void ReturnFromGameSetup()
  {
    ET_Routine.StartRoutine(SwapMenus(menuGame, menuTitle));
  }

  /// <summary>
  /// A button function to start the game, after initializing player settings.
  /// </summary>
  public void StartGame()
  {
    ET_Routine.StartRoutine(LoadGame());
  }

  /// <summary>
  /// A function used to return to the main menu after completing the game.
  /// </summary>
  public void ReturnToMenu()
  {
    ET_Routine.StartRoutine(HandleReturnToMenu());
  }

  /// <summary>
  /// A routine for swapping between two <see cref="Menu"/>s
  /// </summary>
  /// <param name="current">The current menu that is being displayed.</param>
  /// <param name="next">The new menu to display.</param>
  /// <returns>Returns a routine representing the function.</returns>
  private IEnumerator SwapMenus(Menu current, Menu next)
  {
    raycastBlock.raycastTarget = true; // Enable the raycast blocker.
    yield return KIT_Animation.WaitForAnimationEnd(current.Animator, stateHideNormal); // Hide the current panel.
    current.Canvas.enabled = false; // Disable the canvas of the old Menu for optimization.
    next.Canvas.enabled = true; // Enable the new menu's canvas.
    yield return KIT_Animation.WaitForAnimationEnd(next.Animator, stateShowNormal); // Show the new panel.
    raycastBlock.raycastTarget = false; // Disable the raycast blocker.
  }

  /// <summary>
  /// A routine for loading the game from the main menu.
  /// </summary>
  /// <returns>Returns a routine representing the function.</returns>
  private IEnumerator LoadGame()
  {
    raycastBlock.raycastTarget = true; // Enable the raycast blocker.
    yield return KIT_Animation.WaitForAnimationEnd(menuGame.Animator, stateHideNormal); // Hide the current menu.
    menuGame.Canvas.enabled = false; // Disable the canvas for optimization.
    yield return new WaitForSecondsRealtime(0.2f); // Wait a moment for fluid movement.
    yield return EKIT_Math.SmoothLerp(alp => menuBG.SetImageColorChannel(alp, ColorChannel.Alpha), 1.0f, 0.0f, bgFadeSpeed); // Fade out the background.
    SnortSystem.StartGame(); // Start the game with the system.
    raycastBlock.raycastTarget = false; // Disable the raycast blocker.
  }

  /// <summary>
  /// A routine for returning from the game to the main menu. This occurs when a game is complete.
  /// </summary>
  /// <returns>Returns a routine representing the function.</returns>
  private IEnumerator HandleReturnToMenu()
  {
    raycastBlock.raycastTarget = true; // Enable the raycast blocker.
    yield return EKIT_Math.SmoothLerp(alp => menuBG.SetImageColorChannel(alp, ColorChannel.Alpha), 0.0f, 1.0f, bgFadeSpeed); // Fade in the background.
    yield return new WaitForSecondsRealtime(0.2f); // Wait a moment for fluid movement.
    menuGame.Canvas.enabled = true; // Enable the Game Setup Menu.
    yield return KIT_Animation.WaitForAnimationEnd(menuGame.Animator, stateShowNormal); // Show the Game Setup Menu.
    SnortSystem.ResetBoard(); // Reset the board once out of view.
    raycastBlock.raycastTarget = false; // Disable the raycast blocker.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(GameMenu))]
public sealed class GameMenuEditor : EGUI_Component
{
  private SerializedProperty sp_MenuTitle;
  private SerializedProperty sp_MenuCredits;
  private SerializedProperty sp_MenuGame;

  private SerializedProperty sp_StateShowNormal;
  private SerializedProperty sp_StateHideNormal;

  private SerializedProperty sp_RaycastBlock;
  private SerializedProperty sp_MenuBG;
  private SerializedProperty sp_BGFadeSpeed;

  private bool gamemenuExpanded;

  private void Enable_GameMenu()
  {
    sp_MenuTitle = serializedObject.FindProperty("menuTitle");
    sp_MenuCredits = serializedObject.FindProperty("menuCredits");
    sp_MenuGame = serializedObject.FindProperty("menuGame");

    sp_StateShowNormal = serializedObject.FindProperty("stateShowNormal");
    sp_StateHideNormal = serializedObject.FindProperty("stateHideNormal");

    sp_RaycastBlock = serializedObject.FindProperty("raycastBlock");
    sp_MenuBG = serializedObject.FindProperty("menuBG");
    sp_BGFadeSpeed = serializedObject.FindProperty("bgFadeSpeed");
  }

  private void Inspector_GameMenu()
  {
    gamemenuExpanded = EditorGUILayout.Foldout(gamemenuExpanded, "Game Menu Properties", true);
    if (gamemenuExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_MenuTitle);
      DisplayBasicProperty(sp_MenuCredits);
      DisplayBasicProperty(sp_MenuGame);
      EditorGUILayout.Space(10.0f);
      DisplayBasicProperty(sp_StateShowNormal);
      DisplayBasicProperty(sp_StateHideNormal);
      EditorGUILayout.Space(10.0f);
      DisplayBasicProperty(sp_RaycastBlock);
      DisplayBasicProperty(sp_MenuBG);
      DisplayBasicProperty(sp_BGFadeSpeed);
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_GameMenu();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_GameMenu();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif