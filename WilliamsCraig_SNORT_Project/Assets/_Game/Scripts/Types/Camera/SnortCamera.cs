/**************************************************************************************************/
/*!
\file   SnortCamera.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing the class for the Snort Camera. This camera handles zooming in and out and around the
  individual BoardSpots.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using UnityEngine;
using UnityEditor;

/**********************************************************************************************************************/
/// <summary>
/// The camera used for the Snort game. This handles zooming in and out, and moving to where the player wants.
/// </summary>
public class SnortCamera : E_Component
{
  /// <summary> The amount of distance for the camera to zoom out for every board spot step. </summary>
  [SerializeField] private float fullZoomStep = 10.0f;
  /// <summary> The actual Camera component on this object. </summary>
  private Camera snCamera = null;

  private void Awake()
  {
    snCamera = GetComponent<Camera>(); // Get the Camera component.
  }

  /// <summary>
  /// A function used to zoom the camera out to see the entirety of the game board.
  /// </summary>
  public void DisplayFullBoard()
  {
    transform.position = SnortSystem.GetMiddleOfBoard(); // Get the middle of the board.
    transform.SetPositionZ(-10.0f); // Set the Z position to -10, which is a good distance to see everything.
    int steps = System.Math.Max(SnortSystem.BoardSize.x, SnortSystem.BoardSize.y); // Get how many steps to zoom out, based on the max board boundary.
    snCamera.orthographicSize = steps * fullZoomStep; // Set the orthographic size based on the step count.
  }

  /// <summary>
  /// A function used to get a Ray from the the mouse to the game world.
  /// </summary>
  /// <returns></returns>
  public Ray GetScreenPointToRay()
  {
    return snCamera.ScreenPointToRay(Input.mousePosition); // Return the Ray, based on the mouse position on the screen.
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomEditor(typeof(SnortCamera))]
public sealed class SnortCameraEditor : EGUI_Component
{
  private SerializedProperty sp_ZoomLevels;
  private SerializedProperty sp_FullZoomStep;

  private bool cameraExpanded;

  private void Enable_SnortCamera()
  {
    sp_ZoomLevels = serializedObject.FindProperty("zoomLevels");
    sp_FullZoomStep = serializedObject.FindProperty("fullZoomStep");
  }

  private void Inspector_SnortCamera()
  {
    cameraExpanded = EditorGUILayout.Foldout(cameraExpanded, "Camera Properties", true);
    if (cameraExpanded)
    {
      EditorGUI.indentLevel++;
      DisplayBasicProperty(sp_ZoomLevels);
      DisplayBasicProperty(sp_FullZoomStep);
      EditorGUILayout.Space(5.0f);
      DisplayReadOnlyProperty<Camera>(serializedObject.targetObject, "snCamera", new GUIContent("Camera"));
      EditorGUI.indentLevel--;
    }
  }

  private void OnEnable()
  {
    Enable_Core();
    Enable_SnortCamera();
  }

  public override void OnInspectorGUI()
  {
    DisplayBasicProperty(sp_CoreLabel);
    Inspector_SnortCamera();
    serializedObject.ApplyModifiedProperties();
  }
}
/**********************************************************************************************************************/
#endif