/**************************************************************************************************/
/*!
\file   SnortSystem.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the Snort System. This manages storing variables related to the game, and handling
  play.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ENTITY;

/**********************************************************************************************************************/
/// <summary>
/// A system for playing Snort. Handles determining player moves and actions.
/// </summary>
public class SnortSystem  : E_System<SnortSystem>
{
  /// <summary> The boundaries for the board size. X is the min, Y is the max. </summary>
  public static readonly Vector2Int boardSizeBoundaries = new Vector2Int(1, 10);
  /// <summary> The maximum number of colors allowed for players to choose from. </summary>
  public static readonly int maxColors = 5;

  /// <summary> The size of the board. X is the width, and Y is the height. </summary>
  public static Vector2Int BoardSize { get { return _BoardSize; } set { SetBoardSize(value); } }
  /// <summary> The internal value for <see cref="BoardSize"/>. </summary>
  private static Vector2Int _BoardSize = new Vector2Int(3, 3);

  /// <summary> The previously chosen spot by the previous player. Used by the CPU to determine tactics. </summary>
  public static BoardSpot PreviousMove { get; private set; }
  /// <summary> The data for each Player. </summary>
  public static Palette<PlayerID, PlayerData> playerData = new Palette<PlayerID, PlayerData>();
  /// <summary> The ID of the player who will move first. </summary>
  public static PlayerID startingPlayer = PlayerID.Player1;

  /// <summary> The camera of the game. </summary>
  [SerializeField] private SnortCamera snortCamera = null;
  /// <summary> The marker showing which <see cref="BoardSpot"/> is being selected. </summary>
  [SerializeField] private BoardMarker marker = null;
  /// <summary> The main UI of the game. </summary>
  [SerializeField] private GameUI gameUI = null;
  /// <summary> The main menu of the game. Simply used for returning after the game is complete. </summary>
  [SerializeField] private GameMenu gameMenu = null;

  /// <summary> The prefab for a Board Spot. </summary>
  [SerializeField] private BoardSpot prefabSpot = null;
  /// <summary> The prefab for a Board Line. </summary>
  [SerializeField] private BoardLine prefabLine = null;

  /// <summary> The list of colors allowed for players to choose from. </summary>
  public static List<Color> AvailableColors { get { return singleton ? singleton._AvailableColors : null; } }
  /// <summary> The internal value of <see cref="AvailableColors"/>. These are colors players can choose from. </summary>
  [SerializeField] private List<Color> _AvailableColors = new List<Color>(maxColors);

  /// <summary> The starting location <see cref="BoardSpot"/>s are spawned in. </summary>
  [SerializeField] private Vector3 startPosition = Vector3.zero;
  /// <summary> The amount of distance to put between each <see cref="BoardSpot"/>. X is horizontal, Y is vertical. </summary>
  [SerializeField] private Vector2 spotSpacing = Vector2.zero;
  /// <summary> The speed at which the <see cref="BoardLine"/>s should lerp to grow at. </summary>
  [SerializeField] private float lineGrowthSpeed = 3.0f;
  /// <summary> The alpha level to use for <see cref="BoardSpot"/>s that are not available to a player. </summary>
  [SerializeField] private float invalidAlpha = 0.4f;

  /// <summary> A parent for the <see cref="BoardSpot"/>s. Only for organization. </summary>
  [SerializeField] private Transform spotGroup = null;
  /// <summary> A parent for the <see cref="BoardLine"/>s. Only for organization. </summary>
  [SerializeField] private Transform lineGroup = null;

  /// <summary> The amount of time to wait between player turns. </summary>
  [SerializeField] private float turnWaitTime = 1.5f;
  /// <summary> The amount of time to wait before removing the 'Player's Turn' UI Bar. </summary>
  [SerializeField] private float uiBarTurnWait = 0.4f;
  /// <summary> The amount of time to wait before removing the 'No More Moves' UI Bar. </summary>
  [SerializeField] private float uiBarNoMovesWait = 0.8f;
  /// <summary> The amount of time to wait before removing the 'Player Wins' UI Bar. </summary>
  [SerializeField] private float uiBarWinWait = 1.2f;

  // [SerializeField] private Vector2Int debugStartingSize = Vector2Int.zero;
  
  /// <summary> The current <see cref="BoardSpot"/>s that are spawned in the game. </summary>
  private List<List<BoardSpot>> currentSpots = new List<List<BoardSpot>>();
  /// <summary> The current <see cref="BoardLine"/>s that are spawned in the game. </summary>
  private List<BoardLine> currentLines = new List<BoardLine>();
  /// <summary> The current controllers being used by the two players. </summary>
  private Palette<PlayerID, PlayerController> playerControllers = new Palette<PlayerID, PlayerController>();
  /// <summary> The index of the current player. </summary>
  private int currentPlayer = 0;

  protected override void Awake()
  {
    SetStartingSingleton(this);
    //BoardSize = debugStartingSize;

    // Initialize the PlayerData Palette.
    for (int i = 0; i < playerData.Count; i++)
    {
      playerData[i] = new PlayerData(); // Create new data.
      playerData[i].colorIndex = i; // Set the Color Index.
      playerData[i].color = AvailableColors[i]; // Set the Color based on said index.
    }
  }

  /// <summary>
  /// A function called to start the game of Snort.
  /// </summary>
  public static void StartGame()
  {
    // If the singleton is valid, start the game.
    if (singleton)
      ET_Routine.StartRoutine(singleton.InternalStartGame());
  }

  /// <summary>
  /// The internal routine for beginning the game.
  /// </summary>
  /// <returns>Returns a routine representing the function.</returns>
  private IEnumerator InternalStartGame()
  {
    InternalResetBoard(); // Reset the board entirely.
    InitializePlayers(); // Initialize the player's based on starting information.
    currentPlayer = (int)startingPlayer; // Initialize the starting player, based on starting information.
    yield return LayoutBoard(); // Layout the board in its entirety.
    gameUI.ToggleUIDisplay(true); // Turn on the game's UI.
    ET_Routine.StartRoutine(CheckCurrentPlayer()); // Begin the game.
  }

  /// <summary>
  /// A helper function for setting the board size within boundary limits.
  /// </summary>
  /// <param name="size">The new size for the board.</param>
  private static void SetBoardSize(Vector2Int size)
  {
    _BoardSize.x = (int)Mathf.Clamp(size.x, boardSizeBoundaries.x, boardSizeBoundaries.y); // Clamp the width.
    _BoardSize.y = (int)Mathf.Clamp(size.y, boardSizeBoundaries.x, boardSizeBoundaries.y); // Clamp the height.
  }

  /// <summary>
  /// A routine for laying out the board. This animates the board's loading as well.
  /// </summary>
  /// <returns>Returns an IEnumerator to iterate through this function. Useful with E_Routines.</returns>
  private IEnumerator LayoutBoard()
  {
    snortCamera.DisplayFullBoard(); // Make sure the full board is displayed.
    marker.SetMarkerVisiblity(false); // Turn off the marker.
    yield return LayoutBoardSpots(); // Layout the BoardSpots.
    marker.SetMarkerPosition(InternalGetMiddleBoardSpot().transform.position); // Set the marker into the middle position.
    yield return LayoutBoardLines(); // Layout the BoardLines.
    marker.SetMarkerVisiblity(true); // Make the marker visible again.
  }

  /// <summary>
  /// A function to reset the board. Called by the Game Menu once the board is out of view.
  /// </summary>
  public static void ResetBoard()
  {
    singleton?.InternalResetBoard(); // If the singleton is set, reset the board.
  }

  /// <summary>
  /// The internal function for resetting the board to normal.
  /// </summary>
  private void InternalResetBoard()
  {
    PreviousMove = null; // Reset the previous move.

    // For every spot, destroy the GameObject it is a part of.
    for (int i = 0; i < currentSpots.Count; i++)
    {
      List<BoardSpot> current = currentSpots[i]; // Get the current row of spots.

      // Destroy all spots in that row.
      for (int j = 0; j < current.Count; j++)
        Destroy(current[j].gameObject);

      current.Clear(); // Clear out that row.
    }

    currentSpots.Clear(); // Clear out the entire 2D List.

    // For every line, destroy the GameObject it is a part of.
    for (int i = 0; i < currentLines.Count; i++)
      Destroy(currentLines[i].gameObject);

    currentLines.Clear(); // Clear out the entire List.
  }

  /// <summary>
  /// A routine for laying out the BoardSpots in a grid.
  /// </summary>
  /// <returns>Returns an IEnumerator to iterate through this function. Useful with E_Routines.</returns>
  private IEnumerator LayoutBoardSpots()
  {
    Vector3 currentPosition = startPosition; // The current position to spawn a BoardSpot at.

    // Clear out old spot validity for all players.
    foreach (PlayerData data in playerData)
    {
      data.validSpots.Clear();
      data.invalidSpots.Clear();
    }

    // Go through every row.
    for (int i = 0; i < _BoardSize.x; i++)
    {
      List<BoardSpot> current = new List<BoardSpot>(); // Create a new row of BoardSpots.

      // Go through every column in the current row.
      for (int j = 0; j < _BoardSize.y; j++)
      {
        BoardSpot spot = Instantiate(prefabSpot, currentPosition, Quaternion.identity, spotGroup);
        current.Add(spot); // Instantiate a spot and add it to the list.

        spot.spotIndex = new Vector2Int(i, j); // Set the spot's indexes.
        // Add this spot as a valid move for all players.
        foreach (PlayerData data in playerData)
          data.validSpots.Add(spot.spotIndex);

        currentPosition.x += spotSpacing.x; // Move the current position forward on the horizontal axis.
        yield return new WaitForSecondsRealtime(0.05f);
      }

      currentPosition.y -= spotSpacing.y; // Move down a row with the current position.
      currentPosition.x = startPosition.x; // Reset the horizontal position.

      currentSpots.Add(current); // Add the current list to the 2D List.
    }
  }

  /// <summary>
  /// A routine for laying out the BoardLines in a grid, while also animating their growth.
  /// </summary>
  /// <returns>Returns an IEnumerator to iterate through this function. Useful with E_Routines.</returns>
  private IEnumerator LayoutBoardLines()
  {
    ET_Routine growthRoutine = null; // The last routine for growing the lines.

    // Start by displaying horizontal lines.
    for (int i = 0; i < _BoardSize.x; i++)
    {
      List<BoardSpot> current = currentSpots[i]; // Get the current row.
      BoardSpot spotL = current[0]; // Get the spot on the left.
      BoardSpot spotR = current.LastElement(); // Get the spot on the right.

      Vector3 middle = (spotR.transform.position + spotL.transform.position) / 2.0f; // Get the midpoint between the two spots.

      BoardLine line = Instantiate(prefabLine, middle, Quaternion.identity, lineGroup); // Instantiate a line between the two spots.
      currentLines.Add(line); // Add the line to the list.

      float width = (spotR.transform.position.x - spotL.transform.position.x) + line.GetLineHeight(); // Get the max width of this line.
      growthRoutine = ET_Routine.StartRoutine(line.LerpLineWidth(width, lineGrowthSpeed)); // Lerp the line width.
    }

    yield return growthRoutine; // Wait for the last growth routine.

    List<BoardSpot> topRow = currentSpots[0]; // Get the top row of spots.
    List<BoardSpot> bottomRow = currentSpots.LastElement(); // Get the bottom row of spots.

    // Go through every column of the grid.
    for (int i = 0; i < _BoardSize.y; i++)
    {
      BoardSpot spotT = topRow[i]; // Get the top spot.
      BoardSpot spotB = bottomRow[i]; // Get the bottom spot.
      
      Vector3 middle = (spotB.transform.position + spotT.transform.position) / 2.0f; // Get the midpoint between the two spots.
      
      BoardLine line = Instantiate(prefabLine, middle, Quaternion.identity, lineGroup); // Instantiate a line between the two spots.
      currentLines.Add(line); // Add the line to the list.

      float height = (spotB.transform.position.y - spotT.transform.position.y) + line.GetLineWidth(); // Get the max height of this line.
      growthRoutine = ET_Routine.StartRoutine(line.LerpLineHeight(height, lineGrowthSpeed)); // Lerp the line height.
    }

    yield return growthRoutine; // Wait for the last growth routine.
  }

  /// <summary>
  /// A function to get the whole Board layout. Only used by a CPU player.
  /// </summary>
  /// <returns>Returns all of the BoardSpots.</returns>
  public static List<List<BoardSpot>> GetBoardLayout()
  {
    // If the singleton exists, return its list of spots. Otherwise, return null.
    return singleton ? singleton.currentSpots : null;
  }

  /// <summary>
  /// A function to get the middle world position of the board. Used by the camera.
  /// </summary>
  /// <returns>Returns the board's middle world position.</returns>
  public static Vector3 GetMiddleOfBoard()
  {
    // If the singleton exists, return the middle board transform. Otherwise, return a zero vector.
    return singleton ? singleton.InternalGetMiddleOfBoard() : Vector3.zero;
  }

  /// <summary>
  /// The internal function for getting the middle world position of the board.
  /// </summary>
  /// <returns>Returns the board's middle world position.</returns>
  private Vector3 InternalGetMiddleOfBoard()
  {
    Vector3 spotT = startPosition; // The position of the first space.
    Vector3 spotB = startPosition; // The position of the second space.

    // Calculate where the final position will be.
    spotB.x = startPosition.x + (spotSpacing.x * (_BoardSize.y - 1)); 
    spotB.y = startPosition.y + (spotSpacing.y * -(_BoardSize.x - 1));

    return (spotT + spotB) / 2.0f; // Return the midpoint between the two positions.
  }

  /// <summary>
  /// A function to get the Snort Camera. Used to call camera functions.
  /// </summary>
  /// <returns>Returns the camera.</returns>
  public static SnortCamera GetSnortCamera()
  {
    // If the singleton exists, return the camera. Otherwise, return null.
    return singleton ? singleton.snortCamera : null;
  }

  /// <summary>
  /// A function used to get the board marker. Used by player controllers to move the marker around.
  /// </summary>
  /// <returns>Returns the <see cref="BoardMarker"/></returns>
  public static BoardMarker GetBoardMarker()
  {
    // If the singleton exists, return the marker. Otherwise, return null.
    return singleton ? singleton.marker : null;
  }

  /// <summary>
  /// A function used to get the <see cref="BoardSpot"/> in the dead center of the board.
  /// </summary>
  /// <returns>Returns the middle board spot.</returns>
  public static BoardSpot GetMiddleBoardSpot()
  {
    // If the singleton exists, return the middle board spot. Otherwise, return null.
    return singleton ? singleton.InternalGetMiddleBoardSpot() : null;
  }

  /// <summary>
  /// The internal function for getting the <see cref="BoardSpot"/> in the dead center of the board.
  /// </summary>
  /// <returns>Returns the middle board spot.</returns>
  private BoardSpot InternalGetMiddleBoardSpot()
  {
    return currentSpots[_BoardSize.x / 2][_BoardSize.y / 2]; // Get the middle spot on the board. At least, as close to middle as possible.
  }

  /// <summary>
  /// A function used to get a specific <see cref="BoardSpot"/>, given an index. Used by the CPU to make a selection.
  /// </summary>
  /// <param name="spotIndex">The index of the spot.</param>
  /// <returns>Returns the specified board spot.</returns>
  public static BoardSpot GetBoardSpot(Vector2Int spotIndex)
  {
    // If the singleton exists, attempt to return the spot at the given index. Otherwise, return null.
    return singleton ? singleton.currentSpots[spotIndex.x][spotIndex.y] : null;
  }

  /// <summary>
  /// A helper function used to set the opacity on a variety of <see cref="BoardSpot"/>s.
  /// </summary>
  /// <param name="spots">The spots on the board to change the opacity of.</param>
  /// <param name="opacity">The opacity to set the spots to.</param>
  private void SetBoardSpotOpacity(List<Vector2Int> spots, float opacity)
  {
    // For all indexes, get the corresponding spot, and set its opacity.
    foreach (Vector2Int s in spots)
      currentSpots[s.x][s.y].SetSpotOpacity(opacity);
  }

  /// <summary>
  /// A function used to invalidate a given <see cref="BoardSpot"/> for both players, and neighboring spots for opponents.
  /// </summary>
  /// <param name="spotIndex">The index of the spot.</param>
  public static void InvalidateSpot(Vector2Int spotIndex)
  {
    singleton?.HandleSpotInvalidation(spotIndex); // If the singleton exists, invalidate the given spot.
  }

  /// <summary>
  /// The internal function to invalidate a given <see cref="BoardSpot"/>, based on an index.
  /// </summary>
  /// <param name="spotIndex">The index of the spot.</param>
  private void HandleSpotInvalidation(Vector2Int spotIndex)
  {
    PreviousMove = currentSpots[spotIndex.x][spotIndex.y]; // Set the previous move for the CPU controller to use.

    Vector2Int up = spotIndex + new Vector2Int(1, 0); // Get the index of the spot above the selected spot.
    Vector2Int down = spotIndex + new Vector2Int(-1, 0); // Get the index of the spot below the selected spot.
    Vector2Int left = spotIndex + new Vector2Int(0, -1); // Get the index of the spot to the left of the selected spot.
    Vector2Int right = spotIndex + new Vector2Int(0, 1); // Get the index of the spot to the right of the selected spot.

    // Clamp the four neighboring spots based on the board size. This is used to prevent going out of bounds on edge spots.
    up.x = Mathf.Clamp(up.x, 0, _BoardSize.x - 1);
    down.x = Mathf.Clamp(down.x, 0, _BoardSize.x - 1);
    left.y = Mathf.Clamp(left.y, 0, _BoardSize.y - 1);
    right.y = Mathf.Clamp(right.y, 0, _BoardSize.y - 1);

    // Go through all players to invalidate spots.
    for (int i = 0; i < playerData.Count; i++)
    {
      List<Vector2Int> valid = playerData[i].validSpots; // Get the valid spots.
      List<Vector2Int> invalid = playerData[i].invalidSpots; // Get the invalid spots.

      valid.Remove(spotIndex); // Remove the spot from the valid list.
      invalid.Add(spotIndex); // Add the spot to the invalid list.

      // For all players that aren't the current player, invalidate neighboring spots.
      if (i != currentPlayer)
      {
        // Remove the neighbors from the valid list.
        valid.Remove(up);
        valid.Remove(down);
        valid.Remove(left);
        valid.Remove(right);

        // Add the neighbors from the invalid list.
        invalid.Add(up);
        invalid.Add(down);
        invalid.Add(left);
        invalid.Add(right);
      }
    }
  }

  /// <summary>
  /// A function used to initialize the player controllers for th ecurrent round.
  /// </summary>
  private void InitializePlayers()
  {
    // Go through all players.
    for (int i = 0; i < playerData.Count; i++)
    {
      PlayerData data = playerData[i]; // Get the current data.

      // Set the HumanController or CPUController based on the player type.
      if (data.type == PlayerType.Human)
        playerControllers[i] = new HumanController();
      else
        playerControllers[i] = new CPUController();

      data.validSpots.Clear(); // Clear out the valid spots.
      data.invalidSpots.Clear(); // Clear out the invalid spots.
      playerControllers[i].InitializePlayerController(data); // Initialize the player controller with the data.
    }
  }

  /// <summary>
  /// A function used to increment the current player, looping around to the first index at the end.
  /// </summary>
  private void IncrementCurrentPlayer()
  {
    currentPlayer++; // Increment the player index.

    // If the index is above the max index of players, return to index 0.
    if (currentPlayer >= playerControllers.Count)
      currentPlayer = 0;
  }

  /// <summary>
  /// A routine for checking if the current player is able to move. If so, this starts up their <see cref="PlayerController"/>.
  /// </summary>
  /// <returns>Returns a routine representing the function.</returns>
  private IEnumerator CheckCurrentPlayer()
  {
    PlayerController current = playerControllers[currentPlayer]; // Get the current controller.
    PlayerData data = playerData[currentPlayer]; // Get the current player data.

    // If there are no valid spots, the current player loses.
    if (data.validSpots.IsEmpty())
      ET_Routine.StartRoutine(DeclareWinner());
    else
    {
      gameUI.SetBarTextPlayerTurn(currentPlayer); // Set the text of the current player.
      yield return gameUI.AnimateBar(true); // Animate the UI Bar.
      yield return new WaitForSecondsRealtime(uiBarTurnWait); // Wait a specified time.
      SetBoardSpotOpacity(data.invalidSpots, invalidAlpha); // All invalid spots for this player are turned transparent.
      yield return gameUI.AnimateBar(false); // Remove the UI Bar.
      marker.SetMarkerVisiblity(true); // Make the marker visible.
      current.PollInput(); // Poll the current controller's input.
    }
  }

  /// <summary>
  /// A function called by a <see cref="PlayerController"/> to end the current turn.
  /// </summary>
  public static void EndTurn()
  {
    // If the singleton exists, end the turn.
    if (singleton)
      ET_Routine.StartRoutine(singleton.HandleEndTurn());
  }

  /// <summary>
  /// The internal routine for ending the current turn.
  /// </summary>
  /// <returns>Returns a routine representing the function.</returns>
  private IEnumerator HandleEndTurn()
  {
    PlayerController current = playerControllers[currentPlayer]; // Get the current controller.
    PlayerData data = playerData[currentPlayer]; // Get the current player data.

    marker.SetMarkerVisiblity(false); // Hide the marker.
    marker.SetMarkerPosition(InternalGetMiddleBoardSpot().transform.position); // Reset the marker's position to the board's middle.
    SetBoardSpotOpacity(data.invalidSpots, 1.0f); // Return all invalid spots to opaque opacity.
    IncrementCurrentPlayer(); // Increment the player index.
    yield return new WaitForSecondsRealtime(turnWaitTime); // Wait a bit to give a clear shot of the current board state.
    ET_Routine.StartRoutine(CheckCurrentPlayer()); // Being checking the next player.
  }

  /// <summary>
  /// A routine for declaring the winner of the game.
  /// </summary>
  /// <returns>Returns a routine represneting the function.</returns>
  private IEnumerator DeclareWinner()
  {
    IncrementCurrentPlayer(); // The player who wins is the next player for a 2 person game.  
    gameUI.SetBarTextNoMoves(); // Set the UI Bar text to say therea re no more moves.

    yield return gameUI.AnimateBar(true); // Animate the UI Bar.
    yield return new WaitForSecondsRealtime(uiBarNoMovesWait); // Wait the specified amount of time.
    yield return gameUI.AnimateBar(false); // Hide the UI Bar.

    gameUI.ToggleUIDisplay(false); // Turn off the game UI.
    gameUI.SetBarTextPlayerWin(currentPlayer); // Set the winner of the game.
    yield return gameUI.AnimateBar(true); // Animate the UI Bar.
    yield return new WaitForSecondsRealtime(uiBarWinWait); // Wait the specified amount of time.
    yield return gameUI.AnimateBar(false); // Hide the UI Bar.

    yield return new WaitForSecondsRealtime(uiBarWinWait); // Wait the specified amount of time before returning to the menu.
    gameMenu.ReturnToMenu(); // Return to the main menu.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(SnortSystem))]
public sealed class SnortSystemEditor : EGUI_System
{
  private SerializedProperty sp_SnortCamera;
  private SerializedProperty sp_Marker;
  private SerializedProperty sp_GameUI;
  private SerializedProperty sp_GameMenu;

  private SerializedProperty sp_PrefabSpot;
  private SerializedProperty sp_PrefabLine;

  private SerializedProperty sp_AvailableColors;
  private SerializedProperty sp_StartPosition;
  private SerializedProperty sp_SpotSpacing;
  private SerializedProperty sp_LineGrowthSpeed;

  private SerializedProperty sp_InvalidAlpha;

  private SerializedProperty sp_SpotGroup;
  private SerializedProperty sp_LineGroup;

  private SerializedProperty sp_TurnWaitTime;
  private SerializedProperty sp_UIBarTurnWait;
  private SerializedProperty sp_UIBarNoMovesWait;
  private SerializedProperty sp_UIBarWinWait;

  private bool snortExpanded;

  private void Enable_SnortSystem()
  {
    sp_SnortCamera = serializedObject.FindProperty("snortCamera");
    sp_Marker = serializedObject.FindProperty("marker");
    sp_GameUI = serializedObject.FindProperty("gameUI");
    sp_GameMenu = serializedObject.FindProperty("gameMenu");

    sp_PrefabSpot = serializedObject.FindProperty("prefabSpot");
    sp_PrefabLine = serializedObject.FindProperty("prefabLine");

    sp_AvailableColors = serializedObject.FindProperty("_AvailableColors");
    sp_StartPosition = serializedObject.FindProperty("startPosition");
    sp_SpotSpacing = serializedObject.FindProperty("spotSpacing");
    sp_LineGrowthSpeed = serializedObject.FindProperty("lineGrowthSpeed");

    sp_InvalidAlpha = serializedObject.FindProperty("invalidAlpha");

    sp_SpotGroup = serializedObject.FindProperty("spotGroup");
    sp_LineGroup = serializedObject.FindProperty("lineGroup");

    sp_TurnWaitTime = serializedObject.FindProperty("turnWaitTime");
    sp_UIBarTurnWait = serializedObject.FindProperty("uiBarTurnWait");
    sp_UIBarNoMovesWait = serializedObject.FindProperty("uiBarNoMovesWait");
    sp_UIBarWinWait = serializedObject.FindProperty("uiBarWinWait");
  }

  private void Inspector_SnortSystem()
  {
    snortExpanded = EditorGUILayout.Foldout(snortExpanded, "Snort System Properties", true);
    if (snortExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_SnortCamera);
      DisplayBasicProperty(sp_Marker);
      DisplayBasicProperty(sp_GameUI);
      DisplayBasicProperty(sp_GameMenu);
      EditorGUILayout.Space(5.0f);  
      DisplayBasicProperty(sp_PrefabSpot);
      DisplayBasicProperty(sp_PrefabLine);
      EditorGUILayout.Space(5.0f);
      EKIT_EditorGUI.DrawArray(sp_AvailableColors, "", "", "Player Colors", "Color", EKIT_EditorGUI.EE_GUIArrayType.Static);
      DisplayBasicProperty(sp_StartPosition);
      DisplayBasicProperty(sp_SpotSpacing);
      DisplayBasicProperty(sp_LineGrowthSpeed);
      DisplayBasicProperty(sp_InvalidAlpha);
      EditorGUILayout.Space(5.0f);
      DisplayBasicProperty(sp_SpotGroup);
      DisplayBasicProperty(sp_LineGroup);
      EditorGUILayout.Space(5.0f);
      DisplayBasicProperty(sp_TurnWaitTime);
      DisplayBasicProperty(sp_UIBarTurnWait);
      DisplayBasicProperty(sp_UIBarNoMovesWait);
      DisplayBasicProperty(sp_UIBarWinWait);
      EditorGUILayout.Space(5.0f);
      DisplayBasicProperty(serializedObject.FindProperty("debugStartingSize"));
      DisplayReadOnlyProperty<Vector2Int>(serializedObject.targetObject, "_BoardSize", new GUIContent("Board Size"));
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_Singleton();
    Enable_SnortSystem();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_System();
    Inspector_SnortSystem();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif