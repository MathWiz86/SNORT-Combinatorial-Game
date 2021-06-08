/**************************************************************************************************/
/*!
\file   StartPlayerSelector.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the base ShiftSelector component. This component is used for the Game Setup
  Menu, for selectors that show a text moving back and forth with arrows.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using UnityEditor;

/**********************************************************************************************************************/
/// <summary>
/// The <see cref="ShiftSelector"/> for the starting player. This is based on <see cref="PlayerID"/>.
/// </summary>
public class StartPlayerSelector : ShiftSelector
{
  /// <summary> The maximum number of players in the game. This is to hold onto the value early. </summary>
  private int maxPlayerTypes = KIT_Enum.GetEnumValueCount<PlayerID>();

  private void Start()
  {
    // Format the enum string. There should be a space between the name and number.
    string typeName = SnortSystem.startingPlayer.ConvertToString(KIT_Enum.StringType.CamelCase);
    tmpTypeA.text = typeName.Insert(typeName.Length - 1, " ");
  }

  public override void ChangeSelection(bool increase)
  {
    PlayerID current = SnortSystem.startingPlayer; // Get the current player.
    int currentIndex = (int)current; // Get the int value of the current player.

    currentIndex += increase ? 1 : -1; // Increase or decrease the index.

    // Loop around the value if going past the boundary values.
    if (currentIndex < 0)
      currentIndex = maxPlayerTypes - 1;
    if (currentIndex >= maxPlayerTypes)
      currentIndex = 0;

    PlayerID newType = (PlayerID)currentIndex; // Get the enum value of the index.
    SnortSystem.startingPlayer = newType; // Set back the value to the system.

    // Format the type name and set it to the side text.
    string typeName = newType.ConvertToString(KIT_Enum.StringType.CamelCase);
    tmpTypeB.text = typeName.Insert(typeName.Length - 1, " ");

    ET_Routine.StartRoutine(MoveSelectorText(increase)); // Start the movement routine.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(StartPlayerSelector))]
public sealed class StartPlayerSelectorEditor : ShiftSelectorEditor
{

  private bool startExpanded;

  private void Enable_StartPlayer()
  {
  }

  private void Inspector_StartPlayer()
  {
    startExpanded = EditorGUILayout.Foldout(startExpanded, "Starting Player Properties", true);
    if (startExpanded)
    {
      EditorGUI.indentLevel++;
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_ShiftSelector();
   // Enable_StartPlayer();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_ShiftSelector();
   // Inspector_StartPlayer();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif