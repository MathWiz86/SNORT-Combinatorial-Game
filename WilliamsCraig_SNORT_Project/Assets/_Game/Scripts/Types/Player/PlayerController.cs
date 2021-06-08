/**************************************************************************************************/
/*!
\file   PlayerController.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the basics for a PlayerController. These are made on the spot for a match,
  based on the information provided at the start.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections;

/**********************************************************************************************************************/
/// <summary>
/// The base class for a controller. These are created for the game and poll inputs differently for humans and CPUs.
/// </summary>
public abstract class PlayerController
{
  /// <summary> The data this player has. </summary>
  protected PlayerData data = null;
  /// <summary> A reference to the game's camera. </summary>
  protected SnortCamera camera = null;
  /// <summary> A reference to the game's board marker. </summary>
  protected BoardMarker marker = null;

  /// <summary>
  /// A function called to initialize the controller with initial information.
  /// </summary>
  /// <param name="data">A reference to this player's data.</param>
  public void InitializePlayerController(PlayerData data)
  {
    this.data = data; // Set the data reference.
    camera = SnortSystem.GetSnortCamera(); // Get a reference to the camera.
    marker = SnortSystem.GetBoardMarker(); // Get a reference to the marker.
    HandleInitialization(); // Handle any extra initialization.
  }

  /// <summary>
  /// A function for any extra initialization that a controller might need to do.
  /// </summary>
  protected virtual void HandleInitialization() { }

  /// <summary>
  /// A function called by the game to begin getting the player's input.
  /// </summary>
  public void PollInput()
  {
    // Start the routine for handling the input. When it completes, call the EndTurn function on the SnortSystem.
    ET_Routine.StartRoutine(HandlePlayerInput(), SnortSystem.EndTurn);
  }

  /// <summary>
  /// A routine for handling a controller getting a player's input.
  /// </summary>
  /// <returns>Returns a routine representing the function.</returns>
  protected abstract IEnumerator HandlePlayerInput();
}
/**********************************************************************************************************************/