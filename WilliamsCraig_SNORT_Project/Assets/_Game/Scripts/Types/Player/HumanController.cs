/**************************************************************************************************/
/*!
\file   HumanController.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the implementation of a human player's controller. This is created upon
  starting the game.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// A class for the human controller. This is created for normal players.
/// </summary>
public class HumanController : PlayerController
{
  /// <summary> The current <see cref="BoardSpot"/> that the player is hovering over. </summary>
  private BoardSpot currentSpot = null;

  protected override IEnumerator HandlePlayerInput()
  {
    // Continuously check the board until the player makes a selection.
    while (true)
    {
      CheckBoardHover(); // Check what spot is being hovered.

      // If the spot is selected, break the loop.
      if (CheckBoardSpotSelect())
        break;

      yield return null;
    }

    yield return null;
  }

  /// <summary>
  /// A helper function to check what <see cref="BoardSpot"/> is being hovered over by the player's mouse.
  /// </summary>
  private void CheckBoardHover()
  {
    Ray mouseRay = camera.GetScreenPointToRay(); // Get the ray from the screen to the world.
    RaycastHit2D mouseHit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10000); // Raycast from the mouse to an infinite void.

    // Check that something was hit.
    if (mouseHit.transform != null)
    {
      BoardSpot spot = mouseHit.transform.gameObject.GetComponent<BoardSpot>(); // Get the board spot of the object.

      // Check that the spot exists.
      if (spot != null)
      {
        // If the spot is not the current spot, and is a valid move for the player, move the marker.
        if (currentSpot != spot && data.IsValidMove(spot))
        {
          marker.LerpMarkerPosition(spot.transform.position); // Lerp the marker position.
          currentSpot = spot; // Set this spot as our current spot.
          // Play SFX
        }
      }
      else
        currentSpot = null; // If what's highlighted isn't a BoardSpot, return null.
    }
    else
      currentSpot = null; // If nothing is highlighted, return null.
  }

  /// <summary>
  /// A function used for checking if the player has selected a spot on the board.
  /// </summary>
  /// <returns>Returns if the player has made their selection.</returns>
  private bool CheckBoardSpotSelect()
  {
    // Check that the current spot is not null, and that the player has clicked.
    if (currentSpot != null && Input.GetMouseButtonDown(0))
    {
      currentSpot.SetSpotColor(data.color); // Set the color of the spot to this player's color.
      SnortSystem.InvalidateSpot(currentSpot.spotIndex); // Invalidate other spots for all players.
      return true; // Return that the player has selected.
    }

    return false; // Return that the player has not selected yet.
  }
}
/**********************************************************************************************************************/