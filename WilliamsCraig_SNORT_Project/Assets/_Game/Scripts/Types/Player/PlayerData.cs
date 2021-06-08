/**************************************************************************************************/
/*!
\file   PlayerData.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the different enum types to represent player data.

\par References:
*/
/**************************************************************************************************/

/**********************************************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// An enum representing the different input types for a Player.
/// </summary>
public enum PlayerType
{
  /// <summary> The Player is controlled by a Human. </summary>
  Human,
  /// <summary> The Player is controlled by an AI. </summary>
  CPU,
}
/**********************************************************************************************************************/
/**********************************************************************************************************************/
/// <summary>
/// An enum representing the player indexes.
/// </summary>
public enum PlayerID
{
  /// <summary> The first player. </summary>
  Player1,
  /// <summary> The second player. </summary>
  Player2,
}
/**********************************************************************************************************************/
/**********************************************************************************************************************/
/// <summary>
/// A class representing different data for a Player.
/// </summary>
[System.Serializable]
public class PlayerData
{
  /// <summary> The type of input the Player has. </summary>
  public PlayerType type;
  /// <summary> The color of the Player's spots. </summary>
  public Color color;
  /// <summary> The index of the Player's color in the <see cref="SnortSystem.AvailableColors"/> list. </summary>
  public int colorIndex = 0;
  /// <summary> The valid spots for this Player. Used to determine where the player can move. </summary>
  public List<Vector2Int> validSpots = new List<Vector2Int>();
  /// <summary> The invalid spots for this Player. Used to determine where the player cannot move. </summary>
  public List<Vector2Int> invalidSpots = new List<Vector2Int>();

  /// <summary>
  /// The default constructor for PlayerData.
  /// </summary>
  public PlayerData()
  {
    type = PlayerType.Human; // Set the type.
    color = Color.white; // Set the color.
    validSpots = new List<Vector2Int>();
    new List<Vector2Int>();
  }

  /// <summary>
  /// A function for determining if a specified <see cref="BoardSpot"/> is a valid move for this player.
  /// </summary>
  /// <param name="spot">The spot to check.</param>
  /// <returns>Returns if the given spot is valid for this player.</returns>
  public bool IsValidMove(BoardSpot spot)
  {
    return IsValidMove(spot.spotIndex); // Check using the spot's index.
  }

  /// <summary>
  /// A function for determining if a specified <see cref="BoardSpot"/> is a valid move for this player.
  /// </summary>
  /// <param name="spotIndex">The 2D index of the <see cref="BoardSpot"/> to check. </param>
  /// <returns>Returns if the given spot is valid for this player.</returns>
  public bool IsValidMove(Vector2Int spotIndex)
  {
    // Determine which list is smaller. The smaller list is faster to use 'Contains' on.
    if (validSpots.Count < invalidSpots.Count)
      return validSpots.Contains(spotIndex); // If ValidSpots is smaller, check if that list contains the wanted index.

    return !invalidSpots.Contains(spotIndex); // Otherwise, check if the invalid list does NOT contain the wanted index.
  }
    
}
/**********************************************************************************************************************/