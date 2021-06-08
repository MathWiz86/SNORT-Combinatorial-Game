/**************************************************************************************************/
/*!
\file   CPUController.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the implementation of the CPU's controller. This is used by the AI to make
  selections.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System.Collections;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// A controller for CPUs. This is made for AI players to select spots on the board.
/// </summary>
public class CPUController : PlayerController
{
  private static readonly int maxNeighbors = 4;
  /// <summary> The target spot index of this CPU's move. </summary>
  private Vector2Int targetSpot = Vector2Int.zero;
  /// <summary> The index of the final spot on the board. </summary>
  private Vector2Int finalIndex = Vector2Int.zero;

  protected override void HandleInitialization()
  {
    finalIndex.x = SnortSystem.BoardSize.x - 1; // The row is the max index for the board rows.
    finalIndex.y = SnortSystem.BoardSize.y - 1; // The column is the max index for the board columns.
  }

  protected override IEnumerator HandlePlayerInput()
  {
    yield return GetTargetSpot(); // Get the target position we want.
    yield return new WaitForSecondsRealtime(0.2f); // Await a small period of time for fluidity.

    BoardSpot currentSpot = SnortSystem.GetBoardSpot(targetSpot); // Get the actual board spot at the target index.
    yield return marker.LerpMarkerPosition(currentSpot.transform.position); // Lerp the marker position to the target spot.
    yield return new WaitForSecondsRealtime(0.5f); // Await a small period of time for fluidity.
    currentSpot.SetSpotColor(data.color); // Set the color of the spot to the CPU's color.
    SnortSystem.InvalidateSpot(currentSpot.spotIndex); // Invalidate the spot and neighbors for all other players.
  }

  /// <summary>
  /// A helper routine for finding the target spot index for this CPU. This is the main AI function.
  /// </summary>
  /// <returns>Returns a routine representing the function.</returns>
  private IEnumerator GetTargetSpot()
  {
    BoardSpot previousMove = SnortSystem.PreviousMove; // Get the previous move made.

    // CASE 1: There is no previous move. This CPU moves first.
    if (previousMove == null)
    {
      targetSpot = SnortSystem.GetMiddleBoardSpot().spotIndex; // The target spot is as close to the middle as possible.
      yield break;
    }

    // CASE 2: There is a previous move. We are making a Symmetry Strategy move, as if the board were rotated 180 degrees.

    /**********************************************************************************************************/
    // There is never a time during gameplay that the board will become symmetrical after not being symmetrical.
    /**********************************************************************************************************/

    // Calculate a temporary target. The symmetrical move is the same distance from the end of the board as the previous move is from the start.
    Vector2Int targetIndex = finalIndex;
    targetIndex -= previousMove.spotIndex;
    // Check that the move is valid. If so, set this index as our target.
    if (data.IsValidMove(targetIndex))
    {
      targetSpot = targetIndex;
      yield break;
    }

    // CASE 3: There is no valid symmetrical move. Choose randomly.
    targetSpot = data.validSpots.GetRandomElement();
    yield return null;
  }
}
/**********************************************************************************************************************/