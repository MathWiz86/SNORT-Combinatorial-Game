using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
// https://stackoverflow.com/questions/3025361/c-sharp-datetime-to-yyyymmddhhmmss-format
// https://forum.unity.com/threads/drag-and-drop-in-the-editor-explanation.223242/
// https://wiki.unity3d.com/index.php?title=FramesPerSecond
namespace ENTITY
{
  public sealed class ESYS_Debug : E_System<ESYS_Debug>
  {
    /// <summary> The default time format for displaying the date and time. </summary>
    private const string debug_TimeFormat = "yyyy_MM_dd-HH_mm_ss_ffff";

    /// <summary> The time recorded for when logging started. This is typically when this system is awoken. </summary>
    private static DateTime debug_StartTime = default;
    /// <summary> A function for getting information about the class and function. Simply call this when logging. </summary>
    public static Func<MethodBase> debug_GetMethodInfo { get { return MethodBase.GetCurrentMethod; } }

    /// <summary> The writer for the Debug Log. This is managed by the system itself. </summary>
    private static StreamWriter debug_Writer = null;
    /// <summary> The file path for the Debug Log. This is set when the system is first awoken. </summary>
    private static string debug_FullFilePath = null;

    /// <summary> The directory for the Debug Log. </summary>
    [SerializeField] [EP_FolderPath(AssertExistingPath = true)] private string debug_FileDirectory = EKIT_File.file_CurrentDirectory + "/Debug/";
    /// <summary> The filename for the Debug Log, not including the file extension. </summary>
    [SerializeField] private string debug_FileName = "ENTITY_Debug";
    /// <summary> The file extension for the Debug Log. Defaults to '.entity'. </summary>
    [SerializeField] private string debug_FileExtension = ".entity";

    /// <summary> A bool determining if the class a message is sent from should be printed in the debug message. </summary>
    [SerializeField] private bool debug_PrintClass = true;
    /// <summary> A bool determining if the function a message is sent from should be printed in the debug message. </summary>
    [SerializeField] private bool debug_PrintMethodName = true;
    /// <summary> A bool determining if the error messages should automatically be printed to the Log. </summary>
    [SerializeField] private bool debug_AutoPrintErrors = false;

    private static float lowest_framerate = float.MaxValue;
    /// <summary>
    /// The constructor for the Debug System, setting various Singleton settings.
    /// </summary>
    private ESYS_Debug()
    {
      si_DestroyIfFailed = true; // This system should be destroyed if not set as the singleton.
      si_DestroyParentOnDestroy = false; // The parent does not have to die.
      si_DontDestroyOnLoad = true; // This system should not be destroyed when loading a new scene.
      si_DestroyOnReplace = true; // This system should be destroyed when replaced with an updated version.
    }

    /// <summary>
    /// A function which will write a debug message to the Debug Log, if the system is active.
    /// </summary>
    /// <param name="message">The message to print to the Debug Log.</param>
    /// <returns>Returns if the message was logged or not.</returns>
    public static bool WriteDebugMessage(string message)
    {
      // If the singleton exists, is active, and the message isn't null, write the message to the log.
      if (singleton && singleton.sys_IsActive && message != null)
        return singleton.WriteDebugMessageToFile(message);
      // No message was printed. Return false.
      return false;
    }

    /// <summary>
    /// A function which will write a debug message to the Debug Log, if the system is active.
    /// </summary>
    /// <param name="message">The message to print to the Debug Log.</param>
    /// <param name="method">Method information about the function. Simply call 'ESYS_Debug.debug_GetDebugInfo' here.</param>
    /// <returns>Returns if the message was logged or not.</returns>
    public static bool WriteDebugMessage(string message, MethodBase method)
    {
      // If the singleton exists, is active, and the message isn't null, write the message to the log.
      if (singleton && singleton.sys_IsActive && message != null)
        return singleton.WriteDebugMessageToFile(message, method);
      // No message was printed. Return false.
      return false;
    }

    /// <summary>
    /// A function which will write a debug message to the Debug Log, if the system is active.
    /// </summary>
    /// <param name="message">The message to print to the Debug Log.</param>
    /// <param name="method">Method information about the function. Simply call 'ESYS_Debug.debug_GetDebugInfo' here.</param>
    /// <param name="caller">The calling object. Simply pass 'this'.</param>
    /// <returns>Returns if the message was logged or not.</returns>
    public static bool WriteDebugMessage(string message, MethodBase method, UnityEngine.Object caller)
    {
      // If the singleton exists, is active, and the message isn't null, write the message to the log.
      if (singleton && singleton.sys_IsActive && message != null)
        return singleton.WriteDebugMessageToFile(message, method, caller);
      // No message was printed. Return false.
      return false;
    }

    /// <summary>
    /// A function which gets the current frame rate. This does not require a Debug System instance to be present.
    /// </summary>
    /// <returns>Returns the current, unscaled frame rate.</returns>
    public static float CalculateCurrentFrameRate()
    {
      float rate = 1.0f / Time.unscaledDeltaTime;
      lowest_framerate = rate < lowest_framerate ? rate : lowest_framerate;
      return rate; // Calculate the current frame rate for this frame.
    }

    /// <summary>
    /// A function which gets the average frame rate over the entire play time. This does not require a Debug System instance to be present.
    /// </summary>
    /// <returns>Returns the average frame rate for the entire play time.</returns>
    public static float CalculateAverageFrameRate()
    {
      return Time.frameCount / Time.time; // Calculate the game's average frame rate.
    }

    /// <summary>
    /// A routine which calculates the average frame rate over a given time period. This does not require a Debug System instance to be present.
    /// This should be used with an ET_Routine, as it returns the frame rate.
    /// </summary>
    /// <param name="time">The amount of time to calculate the frame rate over.</param>
    /// <returns>Returns an IEnumerator for the routine. If using an ET_Routine, returns the final average frame rate.</returns>
    public static IEnumerator CalculateFrameRateOverTime(float time)
    {
      int start_frame_count = Time.frameCount; // Get the starting frame count.
      float start_time = Time.time; // Get the original start time. We use this instead of 'time' to get the most accurate amount of time passed.
      yield return new WaitForSecondsRealtime(time); // Wait in real time for the specified amount of seconds.
      yield return ((Time.frameCount - start_frame_count) / (Time.time - start_time)); // Calculate the amount of frames that have passed each second.
    }

    /// <summary>
    /// A routine which calculates the average frame rate over a given time period. This does not require a Debug System instance to be present.
    /// This should be used with an ET_Routine, as it returns the frame rate.
    /// </summary>
    /// <param name="time">The amount of time to calculate the frame rate over.</param>
    /// <param name="frame_rate">An action used to return the frame rate. (i.e. fr => myRate = fr) </param>
    /// <returns>Returns an IEnumerator for the routine.</returns>
    public static IEnumerator CalculateFrameRateOverTime(float time, Action<float> frame_rate)
    {
      int start_frame_count = Time.frameCount; // Get the starting frame count.
      float start_time = Time.time; // Get the original start time. We use this instead of 'time' to get the most accurate amount of time passed.
      yield return new WaitForSecondsRealtime(time); // Wait in real time for the specified amount of seconds.
      frame_rate(((Time.frameCount - start_frame_count) / (Time.time - start_time))); // Calculate the amount of frames that have passed each second.
    }

    public static float GetLowestFrameRate()
    {
      return lowest_framerate;
    }

    /// <summary>
    /// A helper function which puts together all the information for the debug file name.
    /// </summary>
    /// <returns>Returns the pieced-together file name.</returns>
    private void CreateDebugLogFile()
    {
      string filename = debug_FileName + "_" + debug_StartTime.ToString(debug_TimeFormat) + debug_FileExtension;
      EKIT_File.CleanupFilePath(ref debug_FileDirectory, ref filename);
      debug_FullFilePath = EKIT_File.CreateFilePath(new string[] { debug_FileDirectory, filename });
      EKIT_File.CreateFile(ref debug_FullFilePath, true, false, false);
    }

    /// <summary>
    /// The internal function which will write a debug message to the Debug Log, if the system is active.
    /// </summary>
    /// <param name="message">The message to print to the Debug Log.</param>
    /// <returns>Returns if the message was logged or not.</returns>
    private bool WriteDebugMessageToFile(string message)
    {
      return WriteDebugMessageToFile(message, null, null);
    }

    /// <summary>
    /// The internal function which will write a debug message to the Debug Log, if the system is active.
    /// </summary>
    /// <param name="message">The message to print to the Debug Log.</param>
    /// <param name="method">Method information about the function. Simply call 'ESYS_Debug.debug_GetDebugInfo' here.</param>
    /// <returns>Returns if the message was logged or not.</returns>
    private bool WriteDebugMessageToFile(string message, MethodBase method)
    {
      return WriteDebugMessageToFile(message, method, null);
    }

    /// <summary>
    /// The internal function which will write a debug message to the Debug Log, if the system is active.
    /// </summary>
    /// <param name="message">The message to print to the Debug Log.</param>
    /// <param name="method">Method information about the function. Simply call 'ESYS_Debug.debug_GetDebugInfo' here.</param>
    /// <param name="caller">The calling object. Simply pass 'this'.</param>
    /// <returns>Returns if the message was logged or not.</returns>
    private bool WriteDebugMessageToFile(string message, MethodBase method, UnityEngine.Object caller)
    {
      string full_message = string.Empty;
      full_message += DateTime.Now.ToString(debug_TimeFormat) + " :: ";
      full_message += caller != null ? "{" + caller.name + "} :: " : string.Empty;

      if (method != null)
      {
        full_message += debug_PrintClass ? "Type: " + method.DeclaringType.Name + " - " : string.Empty;
        full_message += debug_PrintMethodName ? "Method: " + method.Name + " >> " : string.Empty;
      }
      
      full_message += message;

      try
      {
        if (debug_Writer == null)
          debug_Writer = new StreamWriter(debug_FullFilePath);

        debug_Writer.WriteLine(full_message);
        return true;
      }
      catch
      {
        return false;
      }
    }

    protected override void Awake()
    {
      // If this is the singleton, set the start time and the file information.
      if (SetStartingSingleton(this))
      {
        debug_StartTime = DateTime.Now; // Set the date and time.
        CreateDebugLogFile();
      }
    }

    private void OnApplicationQuit()
    {
      WriteDebugMessage("APPLICATION HAS QUIT", null, null); // Write that the application has quit.
    }

    protected override void OnDestroy()
    {
      if (debug_Writer != null)
        debug_Writer.Dispose();
      base.OnDestroy();
    }

    private sealed class ET_DebugStats
    {
      private float st_MinFrameRate = float.MaxValue;
      private float st_MaxFrameRate = float.MinValue;

      public void UpdateStats()
      {
        UpdateFrameRateStats();
      }

      private void UpdateFrameRateStats()
      {
        float current_frame_rate = CalculateCurrentFrameRate();
        st_MinFrameRate = Math.Min(st_MinFrameRate, current_frame_rate);
        st_MaxFrameRate = Math.Max(st_MaxFrameRate, current_frame_rate);
      }

      public void PrintDebugStats()
      {

      }
    }
  }

#if UNITY_EDITOR
  [CustomEditor(typeof(ESYS_Debug))]
  public sealed class EGUI_Debug : EGUI_System
  {
    /// <summary> The SerializedProperty of 'core_Label'. </summary>
    private SerializedProperty sp_DebugFileDirectory;
    /// <summary> The SerializedProperty of 'core_TimeScaling'. </summary>
    private SerializedProperty sp_DebugFileName;
    /// <summary> The SerializedProperty of 'core_TimeScale'. </summary>
    private SerializedProperty sp_DebugFileExtension;

    /// <summary> The GUI Label for 'core_Label'. </summary>
    private GUIContent gui_DebugFileDirectory = new GUIContent("Log Directory", "The directory path of the Debug Log.");
    /// <summary> The GUI Label for 'core_TimeScaling'. </summary>
    private GUIContent gui_DebugFileName = new GUIContent("Log Filename", "The base name of the Debug Log, not including extension.");
    /// <summary> The GUI Label for 'core_TimeScale'. </summary>
    private GUIContent gui_DebugFileExtension = new GUIContent("Log Extension", "The extension of the Debug Log. Defautls to '.entity'.");

    private bool expand_Debug;
    private bool expand_FilePath;
    private void DisplayFilePath()
    {
      expand_FilePath = EditorGUILayout.Foldout(expand_FilePath, "Log File Path", true); // Create a foldout for the Core Properties.

      // If the foldout is expanded, show all Core Properties.
      if (expand_FilePath)
      {
        // Update the indentation level.
        int startIndent = EditorGUI.indentLevel;
        bool gui_enabled = GUI.enabled;
        EditorGUI.indentLevel++;
        GUI.enabled = !(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused);
        // Display all properties.
        DisplayBasicProperty(sp_DebugFileDirectory, gui_DebugFileDirectory);
        DisplayBasicProperty(sp_DebugFileName, gui_DebugFileName);
        DisplayBasicProperty(sp_DebugFileExtension, gui_DebugFileExtension);
        // Reset to the starting indent level.
        EditorGUI.indentLevel = startIndent;
        GUI.enabled = gui_enabled;
      }
    }

    /// <summary>
    /// A function containing all OnEnable functionality for ENTITY core properties.
    /// </summary>
    private void Enable_Debug()
    {
      sp_DebugFileDirectory = serializedObject.FindProperty("debug_FileDirectory"); // Find the 'core_Label' property.
      sp_DebugFileName = serializedObject.FindProperty("debug_FileName"); // Find the 'core_TimeScaling' property.
      sp_DebugFileExtension = serializedObject.FindProperty("debug_FileExtension"); // Find the 'core_TimeScale' property.
    }

    private void Inspector_Debug()
    {
      expand_Debug = EditorGUILayout.Foldout(expand_Debug, "Debug System Properties", true); // Create a foldout for the Core Properties.

      // If the foldout is expanded, show all Core Properties.
      if (expand_Debug)
      {
        // Update the indentation level.
        int startIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;
        // Display all properties.
        DisplayFilePath();
        // Reset to the starting indent level.
        EditorGUI.indentLevel = startIndent;
      }
    }

    private void OnEnable()
    {
      Enable_Core();
      Enable_Singleton();
      Enable_Debug();
    }

    public override void OnInspectorGUI()
    {
      DisplayBasicProperty(sp_CoreLabel, gui_CoreLabel);
      Inspector_System();
      Inspector_Debug();
      serializedObject.ApplyModifiedProperties();
    }
  }
#endif
}


