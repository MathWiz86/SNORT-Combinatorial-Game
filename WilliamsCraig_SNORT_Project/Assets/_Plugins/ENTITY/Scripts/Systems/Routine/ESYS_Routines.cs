/**************************************************************************************************/
/*!
\file   ESYS_Routines.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains functionality for the Routine System. This system handles updating
  gameplay ET_Routines, and instantiating itself into the world.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: ESYS_Routine</para>
  /// <summary>
  /// A system for maintaining ET_Routines. Routines are updated here. The system also handles instantiating itself.
  /// </summary>
  [DisallowMultipleComponent]
  public sealed class ESYS_Routine : E_System<ESYS_Routine>
  {
    /// <summary>
    /// A function which initializes the manager immediately once a scene is loaded.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeManager()
    {
      // If the singleton does not exist, create it.
      if (!singleton)
      {
        // Create a hidden game object to hold this component, and hide it.
        GameObject obj = new GameObject("ENTITY ROUTINE MANAGER");
        obj.hideFlags = HideFlags.HideAndDontSave;
        EMAN_Object.DontDestroyObjectOnLoad(obj);

        ESYS_Routine instance = obj.AddComponent<ESYS_Routine>(); // Add this component to the object.

        instance.SetStartingSingleton(instance, true); // Attempt to declare the singleton.
      }
    }

    /// <summary>
    /// An empty constructor. This is to prevent this from being created.
    /// </summary>
    private ESYS_Routine()
    {
      si_DestroyIfFailed = true; // Destroy failed instance setting.
    }

    protected override void Awake()
    {
      SetStartingSingleton(this); // Attempt to declare the singleton. This will destroy all bad instances.
    }

    private void Update()
    {
      // Update the Update routines.
      if (Application.isPlaying)
        ET_Routine.UpdateRoutines(ET_Routine.EE_RoutineUpdateMode.Update);
    }

    private void FixedUpdate()
    {
      // Update the FixedUpdate routines.
      if (Application.isPlaying)
        ET_Routine.UpdateRoutines(ET_Routine.EE_RoutineUpdateMode.FixedUpdate);
    }

    private void LateUpdate()
    {
      // Update the LateUpdate routines.
      if (Application.isPlaying)
        ET_Routine.UpdateRoutines(ET_Routine.EE_RoutineUpdateMode.LateUpdate);
    }

    private void OnGUI()
    {
      // Update the OnGUI routines.
      if (Application.isPlaying)
        ET_Routine.UpdateRoutines(ET_Routine.EE_RoutineUpdateMode.OnGUI);
    }

    private static void EditorUpdate()
    {
      // Update the EditorUpdate routines.
      ET_Routine.UpdateRoutines(ET_Routine.EE_RoutineUpdateMode.Editor);
    }

    /// <summary>
    /// A function which allows creating standard Unity Coroutines from anywhere.
    /// Useful for non Monobehaviour classes.
    /// </summary>
    /// <param name="routine">The routine to use.</param>
    /// <returns>Returns a Coroutine started with the IEnumerator.</returns>
    public static Coroutine StartUnityCoroutine(IEnumerator routine)
    {
      // If the singleton exists, start the given routine on it.
      if (singleton)
        return singleton.StartCoroutine(routine);

      return null; // No coroutine was made.
    }

#if UNITY_EDITOR
    /// <summary>
    /// A function which attaches this object to the Editor's update loop. This is to update Editor Routines.
    /// </summary>
    [InitializeOnLoadMethod]
    private static void InitializeEditorUpdating()
    {
      EditorApplication.update += EditorUpdate; // Attach to the update loop.
    }
#endif
  }
  /**********************************************************************************************************************/
}