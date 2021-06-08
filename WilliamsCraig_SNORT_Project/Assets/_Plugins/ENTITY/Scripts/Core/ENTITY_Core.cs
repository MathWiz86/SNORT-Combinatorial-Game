/**************************************************************************************************/
/*!
\file   ENTITY_Core.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the core of all Entity files, containing shared variables.

\par References:
  - https://stackoverflow.com/questions/7611685/how-do-i-fix-the-indentation-of-selected-lines-in-visual-studio
  - https://docs.microsoft.com/en-us/dotnet/api/system.reflection.bindingflags?view=netframework-4.8
  - https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo.getvalue?view=netframework-4.8
  - https://stackoverflow.com/questions/4849672/how-to-show-method-parameter-tooltip-in-c
  - https://ayende.com/blog/2088/creating-documentation-from-xml-comments-using-doxygen
  - https://unity3d.college/2017/05/22/unity-attributes/
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Reflection;
using System.Linq;

/// <summary>
/// The namespace to contain all Entity classes and variables.
/// </summary>
namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: ENTITY</para>
  /// <summary>
  /// The core of all Entity Monobehaviour classes. This holds default information for Entity components.
  /// </summary>
  public abstract class ENTITY : MonoBehaviour
  {

    /// <summary> A label for this component. This only serves to help differentiate components in the editor. </summary>
    [SerializeField] protected string core_Label;
    /// <summary> The method of timescaling for this behaviour. </summary>
    [SerializeField] protected EMAN_Time.EE_TimeScaling core_TimeScaling = EMAN_Time.EE_TimeScaling.FullScaled;
    /// <summary> The time scale of this particular behaviour. </summary>
    [SerializeField] [Range(EMAN_Time.time_MinTimeScale, EMAN_Time.time_MaxTimeScale)] protected float core_TimeScale = 1.0f;
    /// <summary> An event ca </summary>
    public UnityEvent core_OnTimeScaleChange;
    /// <summary>
    /// A function which sets and clamps the behaviour's time scale.
    /// </summary>
    /// <param name="scale"></param>
    public virtual void setTimeScale(float scale)
    {
      core_TimeScale = scale; // Set the scale. The 'Range' attribute clamps it automatically.
      core_OnTimeScaleChange.Invoke();
    }

    /// <summary>
    /// A function which gets this object's time scale. This returns 'core_TimeScale'. Use 'GetCalculatedTimeScale' to get the current time scale based on other factors.
    /// </summary>
    /// <returns>Returns this object's time scale.</returns>
    public virtual float GetTimeScale()
    {
      return core_TimeScale; // Return the time scale.
    }

    /// <summary>
    /// A function which calculates this object's time scale, based on Time Scaling Modes.
    /// This should be used when performing calculations based on time.
    /// </summary>
    /// <returns>Returns the calculated time scale.</returns>
    public virtual float GetCalculatedTimeScale()
    {
      // First switch based on the Global Time Mode.
      switch (EMAN_Time.time_GlobalTimeMode)
      {
        // Normal means use this object's standard time scaling calculation.
        case EMAN_Time.EE_GlobalTimeMode.Normal:
          // Switch based on this object's Time Scale Mode
          switch (core_TimeScaling)
          {
            // Full Scale means using the standard time scale and this object's time scale.
            case EMAN_Time.EE_TimeScaling.FullScaled:
              return Time.timeScale * core_TimeScale;
            // World Scale means only use the standard time scale.
            case EMAN_Time.EE_TimeScaling.WorldScaled:
              return Time.timeScale;
            // Self Scale means only use this object's time scale.
            case EMAN_Time.EE_TimeScaling.SelfScaled:
              return core_TimeScale;
            // By default, return 1.0f, which is an unscaled time scale.
            default:
              return 1.0f;
          }
        // Force World Scale means returning the standard time scale.
        case EMAN_Time.EE_GlobalTimeMode.ForceWorldScaled:
          return Time.timeScale;
        // Force Self Scale means returning this object's time scale.
        case EMAN_Time.EE_GlobalTimeMode.ForceSelfScaled:
          return core_TimeScale;
        // By default, return 1.0f, which is an unscaled time scale.
        default:
          return 1.0f;
      }

    }

    /// <summary>
    /// A function which calculates this object's delta time, based on Time Scaling Modes.
    /// This should be used when performing calculations based on time.
    /// </summary>
    /// <returns>Returns the calculated delta time.</returns>
    public virtual float GetCalculatedDeltaTime()
    {
      // First switch based on the Global Time Mode.
      switch (EMAN_Time.time_GlobalTimeMode)
      {
        // Normal means use this object's standard delta time calculation.
        case EMAN_Time.EE_GlobalTimeMode.Normal:
          switch (core_TimeScaling)
          {
            // Full Scale means using the standard delta time scaled to this object's time scale.
            case EMAN_Time.EE_TimeScaling.FullScaled:
              return Time.deltaTime * core_TimeScale;
            // World Scale means only use the standard delta time.
            case EMAN_Time.EE_TimeScaling.WorldScaled:
              return Time.deltaTime;
            // Self Scale means scale the unscaled delta time with this object's time scale.
            case EMAN_Time.EE_TimeScaling.SelfScaled:
              return Time.unscaledDeltaTime * core_TimeScale;
            // By default, return the standard unscaled delta time.
            default:
              return Time.unscaledDeltaTime;
          }
        // Force World Scale means returning the standard delta time.
        case EMAN_Time.EE_GlobalTimeMode.ForceWorldScaled:
          return Time.deltaTime;
        // Force Self Scale means returning the unscaled delta time scaled with this object's time scale.
        case EMAN_Time.EE_GlobalTimeMode.ForceSelfScaled:
          return Time.unscaledDeltaTime * core_TimeScale;
        // By default, return the standard unscaled delta time.
        default:
          return Time.unscaledDeltaTime;
      }
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: E_Component</para>
  /// <summary>
  /// The core of all Entity Components. This is used to separate an E_Object from other types of components.
  /// </summary>
  [RequireComponent(typeof(E_Object))]
  public abstract class E_Component : ENTITY
  {
    /// <summary> The E_Object this component is attached to. </summary>
    public E_Object entityObject { get { return GetComponent<E_Object>(); } }
    /// <summary> A bool determining if this object ignores using its parented E_Object's time scale.</summary>
    [SerializeField] protected bool ec_IgnoreObjectTimeScale = false;

    /// <summary>
    /// A function which calculates this object's time scale, based on Time Scaling Modes.
    /// This should be used when performing calculations based on time.
    /// </summary>
    /// <returns>Returns the calculated time scale.</returns>
    public override float GetCalculatedTimeScale()
    {
      float result = base.GetCalculatedTimeScale(); // Get the standard result.

      // If not ignoring the E_Object's time scale, multiply this component's attached E_Object's time scale.
      if (EMAN_Time.time_GlobalTimeMode == EMAN_Time.EE_GlobalTimeMode.Normal && !ec_IgnoreObjectTimeScale)
        result *= entityObject.GetCalculatedTimeScale();

      return result;
    }

    /// <summary>
    /// A function which calculates this object's delta time, based on Time Scaling Modes.
    /// This should be used when performing calculations based on time.
    /// </summary>
    /// <returns>Returns the calculated delta time.</returns>
    public override float GetCalculatedDeltaTime()
    {
      float result = base.GetCalculatedDeltaTime(); // Get the standard result.

      // If not ignoring the E_Object's time scale, multiply this component's attached E_Object's time scale.
      if (EMAN_Time.time_GlobalTimeMode == EMAN_Time.EE_GlobalTimeMode.Normal && !ec_IgnoreObjectTimeScale)
        result *= entityObject.GetCalculatedTimeScale();

      return result;
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: E_PrivateSingleton</para>
  /// <summary>
  /// The core of a Singleton in Entity. Singletons are used to make static-like Monobehaviours.
  /// This version is used to allow the instance to be private.
  /// </summary>
  /// <typeparam name="T">The type of the inheriting class.</typeparam>
  public abstract class E_Singleton<T> : E_Component where T : E_Singleton<T>
  {
    /// <summary> The singleton instance for this class. </summary>
    protected static T singleton { get; set; }
    /// <summary> A shorthand for the parent GameObject of the singleton. </summary>
    public static GameObject parent { get { return singleton ? singleton.gameObject : null; } } //!< A shorthand for the parent object to the singleton.

    /// <summary> A bool determining if the singleton remains active upon loading a new scene. </summary>
    [SerializeField] protected bool si_DontDestroyOnLoad = false;
    /// <summary> A bool determining if the singleton status should be cleared upon disabling the script instance. </summary>
    [SerializeField] protected bool si_ClearSingletonOnDisable = false;

    /// <summary> A bool determining if the original singleton instance should be destroyed upon replacement.</summary>
    [SerializeField] protected bool si_DestroyOnReplace = false;
    /// <summary> A bool determining if the script instance should be destroyed if it fails to be set as the singleton. </summary>
    [SerializeField] protected bool si_DestroyIfFailed = true;
    /// <summary> A bool determining if the script instance's parent GameObject should be destroyed with it. This occurs both as or not as the singleton. </summary>
    [SerializeField] protected bool si_DestroyParentOnDestroy = false;

    /// <summary> Events that are called when the singleton is first properly set. </summary>
    [SerializeField] protected UnityEvent si_OnSingletonFirstSet;
    /// <summary> Events that are called when the singleton is no longer set. This is not called when the script instance is destroyed. </summary>
    [SerializeField] protected UnityEvent si_OnSingletonRemove;

    /// <summary>
    /// A function which will attempt to set a new instance of the singleton class as the singleton instance.
    /// </summary>
    /// <param name="instance">The script instance to attempt to set as the singleton.</param>
    /// <param name="replace">A bool that determines if the old singleton should be replaced regardless of if it is already set and active.</param>
    /// <returns>Returns if the singleton was properly set anew or not.</returns>
    public static bool DeclareSingleton(T instance, bool replace = false)
    {
      // If the instance is not null, attempt to set the singleton with it. Otherwise, return false.
      return instance != null ? instance.HandlePublicSingletonDeclaration(instance, replace) : false;
    }

    /// <summary>
    /// A function used to override how public singleton declarations are handled. Change this, for example, to prevent
    /// the public 'DeclareSingleton' from actually setting a new singleton instance.
    /// </summary>
    /// <param name="instance">The script instance to attempt to set as the singleton.</param>
    /// <param name="replace">A bool that determines if the old singleton should be replaced regardless of if it is already set and active.</param>
    /// <returns>Returns if the singleton was properly set anew or not.</returns>
    protected virtual bool HandlePublicSingletonDeclaration(T instance, bool replace = false)
    {
      return SetSingleton(instance, replace);
    }

    /// <summary>
    /// A function to declare a script instance as the singleton.
    /// </summary>
    /// <param name="instance">The script instance to attempt to set as the singleton.</param>
    /// <param name="replace">A bool that determines if the old singleton should be replaced regardless of if it is already set and active.</param>
    /// <returns>Returns if the singleton was properly set anew or not.</returns>
    protected virtual bool SetSingleton(T instance, bool replace = false)
    {
      // Make sure the passed-in script is valid.
      if (instance != null)
      {
        // Check if a singleton instance already exists.
        if (singleton != null)
        {
          // Check if the instance and passed-in script are not the same. If not, we want to do more checks.
          if (singleton != instance)
          {
            // If replacing, we want to remove the previous singleton and carry on.
            if (replace)
            {
              bool destroy = singleton.si_DestroyOnReplace; // Hold a copy of if the old instance should be destroyed.
              singleton.RemoveSingleton();
              // If the old instance was to be replaced, do so.
              if (destroy)
                Destroy(singleton);
            }
            else
            {
              // If the passed-in script should be destroyed, destroy it.
              if (instance.si_DestroyIfFailed)
                Destroy(instance);

              return false;
            }
          }
          else
            return false;
        }
        // Set the singleton to not destroy on load if it shouldn't be.
        if (instance.si_DontDestroyOnLoad && !instance.gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy))
          EMAN_Object.DontDestroyObjectOnLoad(instance.gameObject);

        // Set the passed-in script as the new singleton.
        singleton = instance;

        return instance.isActiveAndEnabled; // Return if the singleton was set, and is active.
      }

      return false; // No new singleton was set.
    }

    /// <summary>
    /// An internal function to declare the first instance of the singleton. This should be called in Awake in inheriting clases.
    /// This function specifiallly calls the 'si_OnSingletonSet' event.
    /// </summary>
    /// <param name="instance">The script instance to attempt to set as the singleton.</param>
    /// <param name="replace">A bool that determines if the old singleton should be replaced regardless of if it is already set and active.</param>
    /// <returns>Returns if the singleton was properly set anew or not.</returns>
    protected virtual bool SetStartingSingleton(T instance, bool replace = false)
    {
      // If the singleton was declared, call the Set event.
      if (SetSingleton(instance, replace))
      {
        si_OnSingletonFirstSet?.Invoke(); // Invoke the event.
        return true; // The singleton was declared.
      }

      return false; // The singleton was not declared.
    }

    /// <summary>
    /// A function which tells a singleton to remove itself, if it is indeed the singleton.
    /// </summary>
    public virtual void RemoveSingleton()
    {
      // If this instance is the singleton, call the Remove event and remove the singleton status.
      if (singleton == this)
      {
        si_OnSingletonRemove?.Invoke(); // Invoke the event.
        singleton = null; // Make sure the singleton reference is nulled.
      }
    }

    public static bool IsValid()
    {
      return singleton != null;
    }

    /// <summary>
    /// A Unity function called when an object is disabled.
    /// </summary>
    protected virtual void OnDisable()
    {
      // If clearing the singleton on disable, remove the singleton.
      if (si_ClearSingletonOnDisable)
        RemoveSingleton();
    }

    /// <summary>
    /// A Unity function called when an object is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
      // If this is the singleton, remove it's status and call the Destroy event.
      if (singleton == this)
      {
        singleton = null; // Make sure the singleton reference is nulled.

        // If destroying the parent, destroy the parent object.
        if (si_DestroyParentOnDestroy)
          Destroy(gameObject);
      }
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: E_PublicSingleton</para>
  /// <summary>
  /// The core of a Singleton in Entity. Singletons are used to make static-like Monobehaviours.
  /// This version is used to allow the instance to be public.
  /// </summary>
  /// <typeparam name="T">The type of the inheriting class.</typeparam>
  public abstract class E_PublicSingleton<T> : E_Singleton<T> where T : E_Singleton<T>
  {
    /// <summary> A public variable to get the singleton instance. </summary>
    public static T Singleton { get { return singleton; } protected set { singleton = value; } }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: E_System</para>
  /// <summary>
  /// The base class for all Systems in Entity. A System is a private singleton class who's functions can only be
  /// accessed via static functions. These functions will call the internal functions on the singleton instance.
  /// </summary>
  /// <typeparam name="T">The type of the inheriting class.</typeparam>
  public abstract class E_System<T> : E_Singleton<T> where T : E_Singleton<T>
  {
    [SerializeField] protected bool sys_IsActive = true;

    protected virtual void Awake()
    {
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: E_Attribute</para>
  /// <summary>
  /// The core of all Property Attributes made using Entity.
  /// </summary>
  public class E_Attribute : PropertyAttribute
  {
  }
  /**********************************************************************************************************************/
#if UNITY_EDITOR
  /**********************************************************************************************************************/
  /// <summary>
  /// The base class for all PropertyDrawers built into Entity.
  /// </summary>
  public class E_PropertyDrawer : PropertyDrawer
  {
    protected const string ep_DefaultIconPath = "Assets/ENTITY/Editor/Sprites/Icons/";
    /// <summary> The string used to tell an Attribute to find a variable. </summary>
    public const string VariableCall = "!";
    /// <summary> The string used to tell an Attribute to use a reserved variable. </summary>
    public const string ReservedCall = "@";
    protected T CheckDynamicField<T>(object obj, string path, T default_return)
    {
      if (path != null && path != string.Empty && path.First().ToString() == VariableCall)
      {
        string sub_path = path.Substring(1);

        FieldInfo info = EKIT_Reflection.GetFieldInfo(obj, sub_path);

        if (info != null)
        {
          object value = info.GetValue(obj);
          if (value is T)
            return (T)value;
        }
      }

      return default_return;
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: E_GUIEditor</para>
  /// <summary>
  /// The base class for all Custom Editors built into Entity.
  /// </summary>
  /// <para>How To Use:
  /// - In each class, create a custom editor inheriting from the previous GUI class.
  /// - Create a 'Enable_[ClassName]' and a 'Inspector_[ClassName]' function.
  /// - In 'OnEnable', call each 'Enable' function in the order you want.
  /// - In 'OnInspectorGUI', call each 'Inspector' function in the order you want.</para>
  public class E_GUIEditor : Editor { }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: EGUI_ENTITY</para>
  /// <summary>
  /// The custom editor for ENTITY properties.
  /// </summary>
  [CustomEditor(typeof(ENTITY))]
  public class EGUI_ENTITY : E_GUIEditor
  {
    protected float spacing_Default = 10.0f;

    /// <summary> The SerializedProperty of 'core_Label'. </summary>
    protected SerializedProperty sp_CoreLabel;
    /// <summary> The SerializedProperty of 'core_TimeScaling'. </summary>
    protected SerializedProperty sp_CoreTimeScaling;
    /// <summary> The SerializedProperty of 'core_TimeScale'. </summary>
    protected SerializedProperty sp_CoreTimeScale;

    /// <summary> The GUI Label for 'core_Label'. </summary>
    protected GUIContent gui_CoreLabel = new GUIContent("Label", "A label for this component. This only serves to help differentiate components in the editor.");
    /// <summary> The GUI Label for 'core_TimeScaling'. </summary>
    protected GUIContent gui_CoreTimeScaling = new GUIContent("Time Scale Mode", "The way this component scales with time.");
    /// <summary> The GUI Label for 'core_TimeScale'. </summary>
    protected GUIContent gui_CoreTimeScale = new GUIContent("Time Scale", "The custom time scale of this object."); 

    /// <summary> A bool determining if the 'Core Properties' foldout is expanded. </summary>
    private bool expand_Core;

    /// <summary>
    /// A function which handles null checks befeore displaying a basic SerializedProperty.
    /// </summary>
    /// <param name="property">The property to display.</param>
    /// <param name="gui">The GUI Label to use for the display.</param>
    protected void DisplayBasicProperty(SerializedProperty property, GUIContent gui = null)
    {
      // If the property and gui are valid, display using a basic field.
      if (property != null)
        EditorGUILayout.PropertyField(property, gui);
    }

    /// <summary>
    /// A function which handles null checks befeore displaying a SerializedProperty with a Left Toggle.
    /// </summary>
    /// <param name="property">The property to display.</param>
    /// <param name="gui">The GUI Label to use for the display.</param>
    protected void DisplayBasicToggleLeft(SerializedProperty property, GUIContent gui = null)
    {
      // If the property and gui are valid, display using a Left Toggle.
      if (property != null)
        property.boolValue = EditorGUILayout.ToggleLeft(gui, property.boolValue);
    }

    /// <summary>
    /// A function which handles null checks befeore displaying a SerializedProperty with a Toggle.
    /// </summary>
    /// <param name="property">The property to display.</param>
    /// <param name="gui">The GUI Label to use for the display.</param>
    protected void DisplayBasicToggle(SerializedProperty property, GUIContent gui = null)
    {
      // If the property and gui are valid, display using a Toggle.
      if (property != null)
        property.boolValue = EditorGUILayout.Toggle(gui, property.boolValue);
    }

    protected void DisplayReadOnlyProperty<T>(object obj, string property_name, GUIContent gui)
    {
      bool expanded = true;
      DisplayReadOnlyProperty<T>(obj, property_name, gui, ref expanded);
    }

    protected void DisplayReadOnlyProperty<T>(object obj, string property_name, GUIContent gui, ref bool arrayExpanded)
    {
      if (property_name != null && gui != null)
      {
        T value = EKIT_Reflection.GetSerializedValue<T>(obj, property_name);
        bool enabled = GUI.enabled;
        GUI.enabled = false;
        DisplayValue(value, gui, ref arrayExpanded);
        GUI.enabled = enabled;
      }
    }

    protected void DisplayValue<T>(T value, GUIContent gui, ref bool arrayExpanded)
    {
      if (value == null)
      { 
        if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
          EditorGUILayout.ObjectField(gui, null, typeof(T), true);
        else
        {
          bool on = GUI.enabled;
          GUI.enabled = false;
          EditorGUILayout.TextField(gui, "NO VALID VALUE");
          GUI.enabled = on;
        }

        return;
      }
        

      switch (value)
      {
        case int i:
          EditorGUILayout.IntField(gui, i);
          break;
        case double d:
          EditorGUILayout.DoubleField(gui, d);
          break;
        case float f:
          EditorGUILayout.FloatField(gui, f);
          break;
        case string s:
          EditorGUILayout.TextField(gui, s);
          break;
        case bool b:
          EditorGUILayout.Toggle(gui, b);
          break;
        case Object uo:
          EditorGUILayout.ObjectField(gui, uo, uo.GetType(), true);
          break;
        case IList al:
          if (al == null)
            EditorGUILayout.IntField(gui, 0);
          else
          {
            int count = al.Count;
            bool on = GUI.enabled;
            GUI.enabled = true;
            gui.text += " [READ ONLY]";
            arrayExpanded = EditorGUILayout.Foldout(arrayExpanded, gui);
            GUI.enabled = on;
            if (arrayExpanded)
            {
              EditorGUI.indentLevel++;
              EditorGUILayout.IntField(new GUIContent("Size"), count);
              for (int i = 0; i < count; i++)
                DisplayValue(al[i], new GUIContent("Element " + i), ref arrayExpanded);
              EditorGUI.indentLevel--;
            }
          }
          break;
        case Vector2Int v2i:
          EditorGUILayout.Vector2IntField(gui, v2i);
          break;
        case Vector3Int v3i:
          EditorGUILayout.Vector3IntField(gui, v3i);
          break;
        case Vector2 v2:
          EditorGUILayout.Vector2Field(gui, v2);
          break;
        case Vector3 v3:
          EditorGUILayout.Vector3Field(gui, v3);
          break;
        case Vector4 v4:
          EditorGUILayout.Vector4Field(gui, v4);
          break;

      }
    }

    /*array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, new GUIContent(arrayName, arrayTip));
      // Only draw if the array is expanded.
      if (array.isExpanded)
      {
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(array.FindPropertyRelative("Array.size")); // Draw the array's size

        // Draw every element with the given name and tip.
        for (int i = 0; i < array.arraySize; i++)
          EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(elementName + " " + i.ToString(), elementTip), true);

        EditorGUI.indentLevel--;
      }*/

    /// <summary>
    /// A function containing all OnEnable functionality for ENTITY core properties.
    /// </summary>
    protected void Enable_Core()
    {
      sp_CoreLabel = serializedObject.FindProperty("core_Label"); // Find the 'core_Label' property.
      sp_CoreTimeScaling = serializedObject.FindProperty("core_TimeScaling"); // Find the 'core_TimeScaling' property.
      sp_CoreTimeScale = serializedObject.FindProperty("core_TimeScale"); // Find the 'core_TimeScale' property.
    }

    /// <summary>
    /// A function containing all OnInspectorGUI functionality for ENTITY core properties.
    /// </summary>
    protected void Inspector_Core()
    {
      expand_Core = EditorGUILayout.Foldout(expand_Core, "Core Properties", true); // Create a foldout for the Core Properties.

      // If the foldout is expanded, show all Core Properties.
      if (expand_Core)
      {
        // Update the indentation level.
        int startIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;
        // Display all properties.
        DisplayBasicProperty(sp_CoreLabel, gui_CoreLabel);
        DisplayBasicProperty(sp_CoreTimeScaling, gui_CoreTimeScaling);
        DisplayBasicProperty(sp_CoreTimeScale, gui_CoreTimeScale);
        // Reset to the starting indent level.
        EditorGUI.indentLevel = startIndent;
      }
    }

    /// <summary>
    /// A Unity function for when the GUI is first enabled and displayed.
    /// </summary>
    private void OnEnable()
    {
      Enable_Core(); // Enable the core.
    }

    /// <summary>
    /// A Unity function for when the custom Inspector is redrawn.
    /// </summary>
    public override void OnInspectorGUI()
    {
      Inspector_Core(); // Draw the core inspector.
      serializedObject.ApplyModifiedProperties(); // Serialize all modified properties.
    }
  }
  /**********************************************************************************************************************/
    /**********************************************************************************************************************/
    /// <para>Class Name: EGUI_Component</para>
    /// <summary>
    /// The custom editor class for a E_Component.
    /// </summary>
    [CustomEditor(typeof(E_Component))]
  public class EGUI_Component : EGUI_ENTITY
  {
    /// <summary> The SerializedProperty for 'ec_IgnoreObjectTimeScale'. </summary>
    protected SerializedProperty sp_ECIgnoreObjectTimeScale;

    /// <summary> The GUI Label for 'ec_IgnoreObjectTimeScale'. </summary>
    protected GUIContent gui_ECIgnoreObjectTimeScale = new GUIContent("Ignore Object TimeScale", "If true, this component will only use it's own TimeScale rather than multiplying with the E_Object's scale.");

    /// <summary> A bool determining if the 'Component Properties' foldout is expanded. </summary>
    private bool expand_Component;

    /// <summary>
    /// A function containing all OnEnable functionality for E_Component properties.
    /// </summary>
    protected void Enable_Component()
    {
      sp_ECIgnoreObjectTimeScale = serializedObject.FindProperty("ec_IgnoreObjectTimeScale"); // Find the core_Label property.
      gui_CoreTimeScaling = new GUIContent("Time Scale Mode", "The way this component scales with time.");
      gui_CoreTimeScale = new GUIContent("Time Scale", "The custom time scale of this object. This gets multiplied with the E_Object's TimeScale, unless set otherwise.");
    }

    /// <summary>
    /// A function containing all OnInspectorGUI functionality for E_Component properties.
    /// </summary>
    protected void Inspector_Component()
    {
      expand_Component = EditorGUILayout.Foldout(expand_Component, "Component Properties", true); // Create a foldout for the Core Properties.

      // If the foldout is expanded, show all Core Properties.
      if (expand_Component)
      {
        int startIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;
        DisplayBasicToggleLeft(sp_ECIgnoreObjectTimeScale, gui_ECIgnoreObjectTimeScale);

        EditorGUI.indentLevel = startIndent;
      }
    }

    private void OnEnable()
    {
      Enable_Core();
      Enable_Component();
    }

    public override void OnInspectorGUI()
    {
      Inspector_Core();
      Inspector_Component();
      serializedObject.ApplyModifiedProperties();
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: EGUI_Singleton</para>
  /// <summary>
  /// A collection of custom editor functions for an E_Singleton/E_PublicSingleton-inheriting class.
  /// </summary>
  public class EGUI_Singleton : EGUI_Component
  {
    /// <summary> The SerializedProperty for 'si_DontDestroyOnLoad'. </summary>
    protected SerializedProperty sp_SIDontDestroyOnLoad;
    /// <summary> The SerializedProperty for 'si_ClearSingletonOnDisable'. </summary>
    protected SerializedProperty sp_SIClearSingletonOnDisable;
    /// <summary> The SerializedProperty for 'si_DestroyOnReplace'. </summary>
    protected SerializedProperty sp_SIDestroyOnReplace;
    /// <summary> The SerializedProperty for 'si_DestroyIfFailed'. </summary>
    protected SerializedProperty sp_SIDestroyIfFailed;
    /// <summary> The SerializedProperty for 'si_DestroyParentOnDestroy'. </summary>
    protected SerializedProperty sp_SIDestroyParentOnDestroy;
    /// <summary> The SerializedProperty for 'si_OnSingletonFirstSet'. </summary>
    protected SerializedProperty sp_SIOnSingletonFirstSet;
    /// <summary> The SerializedProperty for 'si_OnSingletonRemove'. </summary>
    protected SerializedProperty sp_SIOnSingletonRemove;

    /// <summary> The GUI Label for 'si_DontDestroyOnLoad'. </summary>
    protected GUIContent gui_SIDontDestroyOnLoad = new GUIContent("Don't Destroy On Load", "Should this object not be destroyed upon loading a new scene?");
    /// <summary> The GUI Label for 'si_ClearSingletonOnDisable'. </summary>
    protected GUIContent gui_SIClearSingletonOnDisable = new GUIContent("Clear On Disable", "If this script is disabled, should it remove its singleton status?");
    /// <summary> The GUI Label for 'si_DestroyOnReplace'. </summary>
    protected GUIContent gui_SIDestroyOnReplace = new GUIContent("Destroy Script On Replacement", "If this singleton is replaced, should this script be destroyed?");
    /// <summary> The GUI Label for 'si_DestroyIfFailed'. </summary>
    protected GUIContent gui_SIDestroyIfFailed = new GUIContent("Destroy On Failed Set", "If this script fails to become the singleton, should this script be destroyed?");
    /// <summary> The GUI Label for 'si_DestroyParentOnDestroy'. </summary>
    protected GUIContent gui_SIDestroyParentOnDestroy = new GUIContent("Destroy Parent If Destroyed", "If this script is destroyed, should the parent GameObject be destroyed as well?");
    /// <summary> The GUI Label for 'si_OnSingletonFirstSet'. </summary>
    protected GUIContent gui_SIOnSingletonFirstSet = new GUIContent("On Singleton Set", "Events to call when this script becomes the singleton");
    /// <summary> The GUI Label for 'si_OnSingletonRemove'. </summary>
    protected GUIContent gui_SIOnSingletonRemove = new GUIContent("On Singleton Remove", "Events to call when this script loses the singleton status.");

    /// <summary>
    /// A function containing all OnEnable functionality for E_Singleton/E_PublicSingleton properties.
    /// </summary>
    protected void Enable_Singleton()
    {
      sp_SIDontDestroyOnLoad = serializedObject.FindProperty("si_DontDestroyOnLoad");
      sp_SIClearSingletonOnDisable = serializedObject.FindProperty("si_ClearSingletonOnDisable");
      sp_SIDestroyOnReplace = serializedObject.FindProperty("si_DestroyOnReplace");
      sp_SIDestroyIfFailed = serializedObject.FindProperty("si_DestroyIfFailed");
      sp_SIDestroyParentOnDestroy = serializedObject.FindProperty("si_DestroyParentOnDestroy");
      sp_SIOnSingletonFirstSet = serializedObject.FindProperty("si_OnSingletonFirstSet");
      sp_SIOnSingletonRemove = serializedObject.FindProperty("si_OnSingletonRemove");
    }

    /// <summary>
    /// A function for displaying if the Singleton has been set. This is merely a simple, unusable toggle.
    /// </summary>
    protected void DisplaySetSingleton()
    {
      // Attempt to get the singleton property.
      PropertyInfo singleton_info = EKIT_Reflection.GetPropertyInfo(serializedObject.targetObject, "singleton", EKIT_Reflection.reflection_DefaultFlags);
      // If the information was found, we can attempt to display if the singleeton value has been set.
      if (singleton_info != null)
      {
        bool gui_enabled = GUI.enabled; // Get if the GUI is enabled or not. We want to set it to false for this display.
        GUI.enabled = false; // Disable the GUI.
        EditorGUILayout.Toggle(new GUIContent("Singleton Set", "Is the singleton set?"), singleton_info.GetValue(null) != null); // Display if the singleton is set.
        GUI.enabled = gui_enabled; // Reset the GUI.
      }
    }

    /// <summary>
    /// A function for displaying all toggles for a Singleton at once.
    /// </summary>
    protected void DisplaySingletonSettings()
    {
      DisplayBasicToggleLeft(sp_SIDontDestroyOnLoad, gui_SIDontDestroyOnLoad);
      DisplayBasicToggleLeft(sp_SIClearSingletonOnDisable, gui_SIClearSingletonOnDisable);
      DisplayBasicToggleLeft(sp_SIDestroyOnReplace, gui_SIDestroyOnReplace);
      DisplayBasicToggleLeft(sp_SIDestroyIfFailed, gui_SIDestroyIfFailed);
      DisplayBasicToggleLeft(sp_SIDestroyParentOnDestroy, gui_SIDestroyParentOnDestroy);
    }

    /// <summary>
    /// A function for displaying all events for a Singleton at once.
    /// </summary>
    protected void DisplaySingletonEvents()
    {
      DisplayBasicProperty(sp_SIOnSingletonFirstSet, gui_SIOnSingletonFirstSet);
      DisplayBasicProperty(sp_SIOnSingletonRemove, gui_SIOnSingletonRemove);
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: EGUI_System</para>
  /// <summary>
  /// A collection of custom editor functions for an E_System-inheriting class.
  /// </summary>
  public class EGUI_System : EGUI_Singleton
  {
    /// <summary> A bool determining if the 'Base System Properties' foldout is expanded. </summary>
    private bool expand_System = false;

    /// <summary>
    /// A function containing all OnInspectorGUI functionality for E_System properties.
    /// This contains the basic system and singleton properties.
    /// </summary>
    /// <param name="enable_singleton_settings">A bool determining if the settings for the singleton should be publically editable.</param>
    protected void Inspector_System(bool enable_singleton_settings = false)
    {
      expand_System = EditorGUILayout.Foldout(expand_System, "Base System Properties", true); // Create a foldout for the Base System Properties.

      // If the foldout is expanded, show all Base System Properties.
      if (expand_System)
      {
        int startIndent = EditorGUI.indentLevel; // Get the current indent level.
        bool gui_enabled = GUI.enabled; // Get the current status of the GUI.
        EditorGUI.indentLevel++;  // Increment the indent.
        DisplaySetSingleton(); // Display if the singleton has been set.
        EditorGUILayout.Space(5); // Space out the sections.
        GUI.enabled = enable_singleton_settings; // Determine if the options should be changeable or not.
        DisplaySingletonSettings(); // Display the singleton settings.
        GUI.enabled = gui_enabled; // Rest the GUI.
        EditorGUILayout.Space(5); // Space out the sections.
        DisplaySingletonEvents(); // Display the events.
        EditorGUI.indentLevel = startIndent; // Reset the indent.
      }
    }
  }
  /**********************************************************************************************************************/
#endif
}