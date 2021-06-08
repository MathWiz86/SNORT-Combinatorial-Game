/**************************************************************************************************/
/*!
\file   ExitPanel.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the Exit Panel implementation. This is used to let the player leave the game
  from the main menu.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// A UI menu for quitting the game entirely. This is accessible from the title screen.
/// </summary>
[RequireComponent(typeof(Animator))]
public class ExitPanel : E_Component
{
  /// <summary> The name of the Animator State for showing the panel. </summary>
  [SerializeField] private string stateShow = "Display";
  /// <summary> The name of the Animator State for hiding the panel. </summary>
  [SerializeField] private string stateHide = "Hide";

  /// <summary> The Animator Component on this panel. </summary>
  private Animator panelAnimator = null;

  private void Awake()
  {
    panelAnimator = GetComponent<Animator>(); // Get the Animator Component.
  }

  /// <summary>
  /// A button function for showing or hiding the panel.
  /// </summary>
  /// <param name="show">A bool determining if the panel is to be shown orn ot.</param>
  public void TogglePanel(bool show)
  {
    panelAnimator.Play(show ? stateShow : stateHide); // Play eiether the show or hide animation.
  }

  /// <summary>
  /// A button function to quit the game entirely.
  /// </summary>
  public void QuitGame()
  {
    Application.Quit(); // Quit the game.
  }
}
/**********************************************************************************************************************/