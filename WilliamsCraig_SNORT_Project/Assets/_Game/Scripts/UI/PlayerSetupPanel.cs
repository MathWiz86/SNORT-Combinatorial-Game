/**************************************************************************************************/
/*!
\file   PlayerSetupPanel.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing implementation for the setup panel for a Player's information.

\par References:
*/
/**************************************************************************************************/
using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
/// <summary>  </summary>
/**********************************************************************************************************************/
/// <summary>
/// The <see cref="ShiftSelector"/>  for setting up a player's information, such as color and controller.
/// </summary>
public class PlayerSetupPanel : ShiftSelector
{
  /// <summary> The ID of the Player this panel is connected to. </summary>
  [SerializeField] private PlayerID playerID = PlayerID.Player1;

  [SerializeField] private MenuGameSetup setupMenu = null;

  /// <summary> The marker used to show which color the panel is giving to the player. </summary>
  [SerializeField] private RectTransform colorMarker = null;
  /// <summary> The images showing the available colors for the player to choose from. </summary>
  [SerializeField] private List<Image> colorSwatches = new List<Image>(SnortSystem.maxColors);
  /// <summary> The speed to move the marker when a new color is selected. </summary>
  [SerializeField] private float colorMoveSpeed = 3.0f;
  /// <summary> The alpha level to set a swatch when the other player has a color selected. </summary>
  [SerializeField] private float colorInvalidAlpha = 0.4f;

  /// <summary> The maximum types of players. This is used for updating the selection. </summary>
  private int maxPlayerTypes = KIT_Enum.GetEnumValueCount<PlayerType>();
  /// <summary> The Routine used to move the marker. </summary>
  private ET_Routine markerRoutine = null;

  private void Start()
  {
    tmpTypeA.text = SnortSystem.playerData[playerID].type.ConvertToString(); // Set the initial text.
    InitializeColorSwatches(); // Initialize the color swatches.
  }

  public override void ChangeSelection(bool increase)
  {
    PlayerType current = SnortSystem.playerData[playerID].type; // Get the current type.
    int currentIndex = (int)current; // Get the index of the type.

    currentIndex += increase ? 1 : -1; // Increase or decrease the value of the index.

    // Wrap around the value if it reaches either boundary.
    if (currentIndex < 0)
      currentIndex = maxPlayerTypes - 1;
    if (currentIndex >= maxPlayerTypes)
      currentIndex = 0;

    PlayerType newType = (PlayerType)currentIndex; // Get the enum value of the index.
    SnortSystem.playerData[playerID].type = newType; // Set back the value for the corresponding player.

    tmpTypeB.text = newType.ConvertToString(); // Convert the enum to a string and set it on the side text.

    ET_Routine.StartRoutine(MoveSelectorText(increase)); // Start the movement routine.
  }

  /// <summary>
  /// A helper function for setting the colors and availability of the color swatches.
  /// </summary>
  private void InitializeColorSwatches()
  {
    List<Color> colors = SnortSystem.AvailableColors; // Get the list of available colors.

    // Set the colors onto the swatch images.
    for (int i = 0; i < colors.Count; i++)
      colorSwatches[i].color = colors[i];

    // Invalidate the colors selected by all players.
    for (int i = 0; i < SnortSystem.playerData.Count; i++)
      colorSwatches[SnortSystem.playerData[i].colorIndex].SetImageColorChannel(colorInvalidAlpha, ColorChannel.Alpha);

    // Make the color selected by this player available again.
    Image current = colorSwatches[SnortSystem.playerData[playerID].colorIndex];
    current.SetImageColorChannel(1.0f, ColorChannel.Alpha);

    colorMarker.anchoredPosition = current.GetComponent<RectTransform>().anchoredPosition; // Move the marker to the selected color.
  }

  /// <summary>
  /// A helper function to change the color for this player.
  /// </summary>
  /// <param name="index"></param>
  public void ChangePlayerColor(int index)
  {
    // Make sure the index is valid.
    if (index >= 0 && index < SnortSystem.maxColors)
    {
      Image wanted = colorSwatches[index]; // Get the corresponding image.

      // Make sure that the color is not invalidated.
      if (wanted.color.a != colorInvalidAlpha)
      {
        ET_Routine.StopRoutine(markerRoutine); // Stop the marker movement.
        markerRoutine = ET_Routine.StartRoutine(MoveColorMarker(index)); // Startup the movement again to the new index.

        // Set the color index and color to the data stored in the system.
        SnortSystem.playerData[playerID].colorIndex = index;
        SnortSystem.playerData[playerID].color = SnortSystem.AvailableColors[index];

        setupMenu.UpdatePanelColors(); // Update all panel colors set to the game menu.
      }
    }
  }

  /// <summary>
  /// A function called by the <see cref="MenuGameSetup"/> to update what colors can be selected by this panel.
  /// </summary>
  public void UpdateAvailableColors()
  {
    // Reset all swatches to available.
    for (int i = 0; i < colorSwatches.Count; i++)
      colorSwatches[i].SetImageColorChannel(1.0f, ColorChannel.Alpha);

    // Invalidate all colors selected by other players.
    for (int i = 0; i < SnortSystem.playerData.Count; i++)
      colorSwatches[SnortSystem.playerData[i].colorIndex].SetImageColorChannel(colorInvalidAlpha, ColorChannel.Alpha);

    // Make this player's selected color available again.
    Image current = colorSwatches[SnortSystem.playerData[playerID].colorIndex];
    current.SetImageColorChannel(1.0f, ColorChannel.Alpha);
  }

  /// <summary>
  /// A routine for moving the marker for the selected color.
  /// </summary>
  /// <param name="newIndex">The new index for the marker to go to.</param>
  /// <returns>Returns a routine representing the function.</returns>
  public IEnumerator MoveColorMarker(int newIndex)
  {
    Vector3 start = colorMarker.anchoredPosition; // Get the starting position.
    Vector3 target = colorSwatches[newIndex].rectTransform.anchoredPosition; // Get the target position at the new swatch.
    // Wait for the marker to move to the new position.
    yield return EKIT_Math.SmoothLerp(pos => colorMarker.anchoredPosition = pos, start, target, colorMoveSpeed);
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(PlayerSetupPanel))]
public sealed class PlayerSetupPanelEditor : ShiftSelectorEditor
{
  private SerializedProperty sp_PlayerID;
  private SerializedProperty sp_SetupMenu;

  private SerializedProperty sp_ColorMarker;
  private SerializedProperty sp_ColorSwatches;
  private SerializedProperty sp_ColorMoveSpeed;
  private SerializedProperty sp_ColorInvalidAlpha;

  

  private bool setupExpanded;

  private void Enable_PlayerSetup()
  {
    sp_PlayerID = serializedObject.FindProperty("playerID");
    sp_SetupMenu = serializedObject.FindProperty("setupMenu");

    sp_ColorMarker = serializedObject.FindProperty("colorMarker");
    sp_ColorSwatches = serializedObject.FindProperty("colorSwatches");
    sp_ColorMoveSpeed = serializedObject.FindProperty("colorMoveSpeed");
    sp_ColorInvalidAlpha = serializedObject.FindProperty("colorInvalidAlpha");
    
  }

  private void Inspector_PlayerSetup()
  {
    setupExpanded = EditorGUILayout.Foldout(setupExpanded, "Setup Properties", true);
    if (setupExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_PlayerID);
      DisplayBasicProperty(sp_SetupMenu);
      EditorGUILayout.Space(5.0f);
      DisplayBasicProperty(sp_ColorMarker);
      DisplayBasicProperty(sp_ColorSwatches);
      DisplayBasicProperty(sp_ColorMoveSpeed);
      DisplayBasicProperty(sp_ColorInvalidAlpha);
      
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_ShiftSelector();
    Enable_PlayerSetup();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_ShiftSelector();
    Inspector_PlayerSetup();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif