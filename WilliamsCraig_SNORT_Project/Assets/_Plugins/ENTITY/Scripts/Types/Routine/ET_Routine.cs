/**************************************************************************************************/
/*!
\file   ET_Routine.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing a custom Coroutine class. This class allows for multiple things normal
  Coroutines cannot do, such as returning a value, pausing, running in editor, tagging groups
  of routines, and more.

\par References:
  - https://github.com/xenosl/Coroutine-Anywhere-In-Unity
*/
/**************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: ET_Routine</para>
  /// <summary>
  /// A custom Coroutine class. This class has several advantages over a normal Coroutine, such as
  /// returning a value, pausing, running in editor, tagging groups of routines, and more.
  /// NOTE ABOUT USAGE : 
  /// 'CreateRoutine' will create an ET_Routine that has not been started. By creating an ET_Routine this way,
  /// and then 'yield return'-ing the routine inside of another routine, the child routine will inherit the parent's
  /// update mode. If you wish to set an ET_Routine's update mode separate, use 'StartRoutine', passing in the child routine.
  /// </summary>
  public sealed class ET_Routine : IEnumerator
  {
    /// <summary> An enum which determines the current running status of the routine. </summary>
    public enum EE_RoutineRunStatus
    {
      /// <summary> The routine has not been started. </summary>
      Unstarted,
      /// <summary> The routine is running as normal. </summary>
      Running,
      /// <summary> The routine is paused. It isn't running, but can continue. </summary>
      Paused,
      /// <summary> The routine has completed. It is safe to get its return value. </summary>
      Complete,
    }

    /// <summary> An enum which determines how the routine will update. </summary>
    public enum EE_RoutineUpdateMode
    {
      /// <summary> The routine does not update automatically. The user updates it as they please. </summary>
      Manual,
      /// <summary> The routine runs on the 'Update' Monobehaviour loop. </summary>
      Update,
      /// <summary> The routine runs on the 'Fixed Update' Monobehaviour loop. </summary>
      FixedUpdate,
      /// <summary> The routine runs on the 'Late Update' Monobehaviour loop. </summary>
      LateUpdate,
      /// <summary> The routine runs on the 'OnGUI' Monobehaviour loop. </summary>
      OnGUI,
      /// <summary> The routine runs on the Editor-Only update loop. </summary>
      Editor,
    }

    /// <summary> An enum which determines how the routine will return a value. </summary>
    public enum EE_RoutineReturnMode
    {
      /// <summary> The routine returns the first valid object and immediately stops. </summary>
      ReturnFirstAndBreak,
      /// <summary> The routine returns the first valid object, but continues to completion. </summary>
      ReturnFirstAndComplete,
      /// <summary> The routine returns the last valid object, and continues to completion. </summary>
      ReturnLastAndComplete,
    }

    /// <summary> The returned value of the IEnumerator version of this. Only necessary if yielding this in a Unity Coroutine. </summary>
    object IEnumerator.Current { get { return routine.Current; } }

    /// <summary> The IEnumerator to go through in this routine. This is the function that will be iterated through. </summary>
    public readonly IEnumerator routine = null;
    /// <summary> The return type to cast the routine's value to. If null, this Routine returns nothing. (void) </summary>
    private Type return_type = null;
    /// <summary> The returned value of this routine. This only returns something if provided a type in 'CreateRoutine'. </summary>
    public object value { get; private set; }

    /// <summary> A tag for this routine. This can be used to perform mass operations on a group of routines. </summary>
    public string tag { get { return r_tag; } set { r_tag = value != null ? value : r_tag; } }
    /// <summary> Internal version of 'tag'. A tag for this routine. This can be used to perform mass operations on a group of routines. </summary>
    private string r_tag = "";

    /// <summary> The number of times to iterate the routine on each update pass. Defaults to 1. </summary>
    public int update_count { get { return r_update_count; } set { r_update_count = Mathf.Clamp(value, 1, 100); } }
    /// <summary> Internal version of 'update_count'. The number of times to iterate the routine on each update pass. Defaults to 1. </summary>
    private int r_update_count = 1;
    /// <summary> The number of times the routine has iterated through it's update pass. Used to keep track of awaited updates. </summary>
    private int current_update_count = 0;

    /// <summary> The current status of the routine. Use this to determine if it is running or not. </summary>
    public EE_RoutineRunStatus run_status { get; private set; } = EE_RoutineRunStatus.Unstarted;
    /// <summary> The way this routine updates. Use this to change how and when the routine iterates. </summary>
    public EE_RoutineUpdateMode update_mode { get { return r_update_mode; } set { SwapUpdateMode(value); } }
    /// <summary> Internal version of 'update_mode'. The way this routine updates. Use this to change how and when the routine iterates. </summary>
    private EE_RoutineUpdateMode r_update_mode = EE_RoutineUpdateMode.Update;
    /// <summary> The way this routine returns something. Only necessary for return routines. </summary>
    public EE_RoutineReturnMode return_mode = EE_RoutineReturnMode.ReturnFirstAndBreak;

    /// <summary> The delegate for an Action with no input. This is called upon completion of the routine. </summary>
    private Delegate completion_void = null;
    /// <summary> The delegate for an Action with an input. The input is whatever the routine returned. This is called upon completion of the routine. </summary>
    private Delegate completion_input = null;

    /// <summary> An attached routine, which is updated to completion before continuing the base routine. </summary>
    private ET_Routine nested_routine = null;
    /// <summary> A bool determining if this routine is attached to another. </summary>
    private bool is_nested_routine = false;
    /// <summary> A bool determining if this routine is waiting for a standard coroutine, and thus shouldn't update normally. </summary>
    private bool is_awaiting_coroutine = false;

    /// <summary> A dictionary of all running routines. These routines can be manipulated here. </summary>
    private static Dictionary<IEnumerator, ET_Routine> running_routines = null;
    /// <summary> Separate lists for each routine update mode. Routines are stored in the list matching their update mode. </summary>
    private static List<ET_Routine>[] updating_routines = null;
    /// <summary> A dictionary of conversions between Unity YieldInstructions to proper IEnumerator routines. </summary>
    private static Dictionary<Type, Func<ET_Routine, YieldInstruction, IEnumerator>> yield_conversions = null;

    /// <summary>
    /// The static constructor for the ET_Routine class. This handles setting up the static variables.
    /// </summary>
    static ET_Routine()
    {
      running_routines = new Dictionary<IEnumerator, ET_Routine>();

      int update_values = EKIT_General.GetEnumValueCount(typeof(EE_RoutineUpdateMode));
      updating_routines = new List<ET_Routine>[update_values];
      for (int i = 0; i < update_values; i++)
        updating_routines[i] = new List<ET_Routine>();

      yield_conversions = new Dictionary<Type, Func<ET_Routine, YieldInstruction, IEnumerator>>();
      yield_conversions.Add(typeof(WaitForSeconds), ConvertWaitForSeconds);
      yield_conversions.Add(typeof(AsyncOperation), ConvertAsyncOperation);
      yield_conversions.Add(typeof(WaitForEndOfFrame), ConvertWaitFor);
      yield_conversions.Add(typeof(WaitForFixedUpdate), ConvertWaitFor);
    }

    /// <summary>
    /// The default constructor for an ET_Routine. This is private so that everything is controlled by the ET_Routine itself.
    /// </summary>
    private ET_Routine()
    {
    }

    /// <summary>
    /// A constructor for an ET_Routine. This is private so that everything is controlled by the ET_Routine itself.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    private ET_Routine(IEnumerator routine, EE_RoutineUpdateMode update_mode, EE_RoutineReturnMode return_mode, string tag = "", int update_count = 1)
    {
      // Set the values to each other.
      this.routine = routine;
      this.update_mode = update_mode;
      this.return_mode = return_mode;
      this.tag = tag;
      this.update_count = update_count;
    }

    /// <summary>
    /// A function which gets the default update mode. When in the editor, the Editor mode is used if the game is not playing.
    /// Otherwise, during gameplay, this will always return 'Update'.
    /// </summary>
    /// <returns>Returns the current default mode.</returns>
    private static EE_RoutineUpdateMode GetDefaultUpdateMode()
    {
#if UNITY_EDITOR
      // If the game is not playing, use the Editor update loop.
      if (!EditorApplication.isPlaying && !EditorApplication.isPaused)
        return EE_RoutineUpdateMode.Editor;
#endif

      return EE_RoutineUpdateMode.Update; // Otherwise, use the Standard update loop.
    }

    public static IEnumerator AwaitAllRoutines(IList<ET_Routine> routines)
    {
      if (routines.HasData())
      {
        foreach (ET_Routine r in routines)
        {
          ET_Routine.StartRoutine(r);
          yield return null;
        }

        foreach (ET_Routine r in routines)
        {
          yield return r;
        }
          
      }
      yield return null;
    }

    /// <summary>
    /// A function which creates a routine from scratch. This does not start the routine.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine(IEnumerator routine, Action completion = null, string tag = "", int update_count = 1)
    {
      return CreateRoutine(routine, GetDefaultUpdateMode(), EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This does not start the routine.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine(IEnumerator routine, EE_RoutineUpdateMode update_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return CreateRoutine(routine, update_mode, EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This does not start the routine.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine(IEnumerator routine, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return CreateRoutine(routine, GetDefaultUpdateMode(), return_mode, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This does not start the routine.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine(IEnumerator routine, EE_RoutineUpdateMode update_mode, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      ET_Routine eroutine = new ET_Routine(routine, update_mode, return_mode, tag, update_count); // Create the routine.
      eroutine.completion_void = completion; // Set the completion action. This is the version without an input.
      return eroutine; // Return the created routine.
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue>(IEnumerator routine, Action completion = null, string tag = "", int update_count = 1)
    {
      return CreateRoutine<TValue>(routine, GetDefaultUpdateMode(), EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue>(IEnumerator routine, EE_RoutineUpdateMode update_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return CreateRoutine<TValue>(routine, update_mode, EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue>(IEnumerator routine, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return CreateRoutine<TValue>(routine, GetDefaultUpdateMode(), return_mode, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue>(IEnumerator routine, EE_RoutineUpdateMode update_mode, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      ET_Routine eroutine = new ET_Routine(routine, update_mode, return_mode, tag, update_count); // Create the routine.
      eroutine.completion_void = completion; // Set the completion action. This is the version without an input.
      eroutine.return_type = typeof(TValue); // Set what type we are looking for to return.
      return eroutine; // Return the created routine.
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value, and passes it to a function at the end. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue, TInput>(IEnumerator routine, Action<TInput> completion, string tag = "", int update_count = 1) where TValue : TInput
    {
      return CreateRoutine<TValue, TInput>(routine, GetDefaultUpdateMode(), EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value, and passes it to a function at the end. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue, TInput>(IEnumerator routine, EE_RoutineUpdateMode update_mode, Action<TInput> completion, string tag = "", int update_count = 1) where TValue : TInput
    {
      return CreateRoutine<TValue, TInput>(routine, update_mode, EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value, and passes it to a function at the end. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue, TInput>(IEnumerator routine, EE_RoutineReturnMode return_mode, Action<TInput> completion = null, string tag = "", int update_count = 1) where TValue : TInput
    {
      return CreateRoutine<TValue, TInput>(routine, GetDefaultUpdateMode(), return_mode, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch. This insinuates that the routine returns a value, and passes it to a function at the end. This does not start the routine.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is unstarted.</returns>
    public static ET_Routine CreateRoutine<TValue, TInput>(IEnumerator routine, EE_RoutineUpdateMode update_mode, EE_RoutineReturnMode return_mode, Action<TInput> completion, string tag = "", int update_count = 1) where TValue : TInput
    {
      ET_Routine eroutine = new ET_Routine(routine, update_mode, return_mode, tag, update_count); // Create the routine.
      eroutine.completion_input = completion; // Set the completion action. This is the version with an input.
      eroutine.return_type = typeof(TValue); // Set what type we are looking for to return.
      return eroutine; // Return the created routine.
    }

    /// <summary>
    /// A function which adds a routine to one of the update lists.
    /// This is private, so we know the routine is not null, and isn't already in another list.
    /// </summary>
    /// <param name="eroutine">The routine to add.</param>
    private static void AddToUpdates(ET_Routine eroutine)
    {
      if (!eroutine.is_nested_routine)
        updating_routines[(int)eroutine.update_mode].Add(eroutine); // Add the routine to the proper list.
    }

    /// <summary>
    /// A function which removes a routine from one of the update lists.
    /// This is private, so we know the routine is not null, and is in the list.
    /// </summary>
    /// <param name="eroutine">The routine to add.</param>
    private static void RemoveFromUpdates(ET_Routine eroutine)
    {
      updating_routines[(int)eroutine.update_mode].Remove(eroutine); // Remove the routine from the proper list.
    }

    /// <summary>
    /// A function which swaps the update mode for the routine.
    /// If running, the routine is moved between update lists.
    /// </summary>
    /// <param name="mode">The mode to swap to.</param>
    private void SwapUpdateMode(EE_RoutineUpdateMode mode)
    {
      // If able to update, we need to swap the lists.
      if (run_status == EE_RoutineRunStatus.Running || run_status == EE_RoutineRunStatus.Paused)
      {
        RemoveFromUpdates(this); // Remove from the old list.
        r_update_mode = mode; // Swap the mode.
        AddToUpdates(this); // Add to the new list.
      }
      else
      {
        r_update_mode = mode; // If not updating, just swap the mode directly.
      }
    }

    /// <summary>
    /// A function which starts a routine. If the routine is already started, nothing new will happen.
    /// </summary>
    /// <param name="eroutine">The routine to start.</param>
    public static void StartRoutine(ET_Routine eroutine)
    {
      // If the routine is not null, start the routine.
      if (eroutine != null && eroutine.routine != null)
        eroutine.Start();
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine(IEnumerator routine, Action completion = null, string tag = "", int update_count = 1)
    {
      return StartRoutine(routine, GetDefaultUpdateMode(), EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine(IEnumerator routine, EE_RoutineUpdateMode update_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return StartRoutine(routine, update_mode, EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine(IEnumerator routine, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return StartRoutine(routine, GetDefaultUpdateMode(), return_mode, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it.
    /// </summary>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine(IEnumerator routine, EE_RoutineUpdateMode update_mode, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      ET_Routine eroutine = null; // The routine to return.
      // make sure the enumerator is not null.
      if (routine != null)
      {
        // If the enumerator is already running, just grab it and return it.
        if (!running_routines.TryGetValue(routine, out eroutine))
        {
          // Otherwise, create a new routine and start it.
          eroutine = CreateRoutine(routine, update_mode, return_mode, completion, tag, update_count);
          eroutine.Start();
        }
      }

      return eroutine; // Return the routine.
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue>(IEnumerator routine, Action completion = null, string tag = "", int update_count = 1)
    {
      return StartRoutine<TValue>(routine, GetDefaultUpdateMode(), EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue>(IEnumerator routine, EE_RoutineUpdateMode update_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return StartRoutine<TValue>(routine, update_mode, EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue>(IEnumerator routine, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      return StartRoutine<TValue>(routine, GetDefaultUpdateMode(), return_mode, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action with no input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue>(IEnumerator routine, EE_RoutineUpdateMode update_mode, EE_RoutineReturnMode return_mode, Action completion = null, string tag = "", int update_count = 1)
    {
      ET_Routine eroutine = null; // The routine to return.

      // make sure the enumerator is not null.
      if (routine != null)
      {
        // If the enumerator is already running, just grab it and return it.
        if (!running_routines.TryGetValue(routine, out eroutine))
        {
          // Otherwise, create a new routine and start it.
          eroutine = CreateRoutine<TValue>(routine, update_mode, return_mode, completion, tag, update_count);
          eroutine.Start();
        }
      }

      return eroutine; // Return the routine.
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value, and passes it to a function at the end.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue, TInput>(IEnumerator routine, Action<TInput> completion, string tag = "", int update_count = 1) where TValue : TInput
    {
      return StartRoutine<TValue, TInput>(routine, GetDefaultUpdateMode(), EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value, and passes it to a function at the end.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue, TInput>(IEnumerator routine, EE_RoutineUpdateMode update_mode, Action<TInput> completion, string tag = "", int update_count = 1) where TValue : TInput
    {
      return StartRoutine<TValue, TInput>(routine, update_mode, EE_RoutineReturnMode.ReturnFirstAndBreak, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value, and passes it to a function at the end.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue, TInput>(IEnumerator routine, EE_RoutineReturnMode return_mode, Action<TInput> completion = null, string tag = "", int update_count = 1) where TValue : TInput
    {
      return StartRoutine<TValue, TInput>(routine, GetDefaultUpdateMode(), return_mode, completion, tag, update_count);
    }

    /// <summary>
    /// A function which creates a routine from scratch and immediately runs it. This insinuates that the routine returns a value, and passes it to a function at the end.
    /// </summary>
    /// <typeparam name="TValue">The type to look for in the IEnumerator. If you're unsure, set this to 'object'.</typeparam>
    /// <typeparam name="TInput">The type to return in the completion action. This must either be, or derive from, 'TValue'.</typeparam>
    /// <param name="routine">The IEnumerator to go through in this routine. This is the function that will be iterated through.</param>
    /// <param name="update_mode">The way this routine updates. Use this to change how and when the routine iterates.</param>
    /// <param name="return_mode">The way this routine returns something. Only necessary for return routines.</param>
    /// <param name="completion">An Action which is passed the returned value as an input. This is called upon completion of the routine.</param>
    /// <param name="tag">A tag for this routine. This can be used to perform mass operations on a group of routines.</param>
    /// <param name="update_count">The number of times to iterate the routine on each update pass. Defaults to 1.</param>
    /// <returns>Returns an ET_Routine with the provided specifications. It is running, so long as it was created correctly.</returns>
    public static ET_Routine StartRoutine<TValue, TInput>(IEnumerator routine, EE_RoutineUpdateMode update_mode, EE_RoutineReturnMode return_mode, Action<TInput> completion, string tag = "", int update_count = 1) where TValue : TInput
    {
      ET_Routine eroutine = null; // The routine to return.

      // make sure the enumerator is not null.
      if (routine != null)
      {
        // If the enumerator is already running, just grab it and return it.
        if (!running_routines.TryGetValue(routine, out eroutine))
        {
          // Otherwise, create a new routine and start it.
          eroutine = CreateRoutine<TValue, TInput>(routine, update_mode, return_mode, completion, tag, update_count);
          eroutine.Start();
        }
      }

      return eroutine; // Return the routine.
    }

    /// <summary>
    /// A function which will immediately stop a routine. You will have to create a new routine to restart it due to limitations with IEnumerator.
    /// </summary>
    /// <param name="eroutine">The routine to stop.</param>
    /// <param name="call_completion_action">A bool determining if the completion action should still be called.</param>
    /// <returns>Returns if the routine was successfully stopped.</returns>
    public static bool StopRoutine(ET_Routine eroutine, bool call_completion_action = false)
    {
      // Check if the routine and its enumerator are valid before trying to stop them.
      if (eroutine != null && eroutine.routine != null)
        return eroutine.Stop(call_completion_action);

      return false; // The routine was not valid.
    }

    /// <summary>
    /// A function which will immediately stop a routine. You will have to create a new routine to restart it due to limitations with IEnumerator.
    /// </summary>
    /// <param name="routine">The routine to stop.</param>
    /// <param name="call_completion_action">A bool determining if the completion action should still be called.</param>
    /// <returns>Returns if the routine was successfully stopped.</returns>
    public static bool StopRoutine(IEnumerator routine, bool call_completion_action = false)
    {
      // Check if the routine is valid.
      if (routine != null)
      {
        // Make sure the routine has already been started, and stop it.
        if (running_routines.TryGetValue(routine, out ET_Routine eroutine))
          return eroutine.Stop(call_completion_action);
      }

      return false; // The routine was not valid.
    }

    /// <summary>
    /// A function which will immediately stop all routines that have a specified tag. Provide an empty string to stop untagged routines.
    /// </summary>
    /// <param name="tag">The tag to look for and stop.</param>
    /// <param name="call_completion_action">A bool determining if the completion action should still be called.</param>
    public static void StopRoutinesByTag(string tag, bool call_completion_action = false)
    {
      // Make sure the tag isn't null.
      if (tag != null)
      {
        // Go through all running routines, stopping any with the specified tag.
        for (int i = 0; i < running_routines.Count; i++)
        {
          ET_Routine eroutine = running_routines.Values.ElementAt(i);
          if (eroutine.tag == tag)
            eroutine.Stop(call_completion_action);
        }
      }
    }

    /// <summary>
    /// A function which updates all routines of a given update mode.
    /// </summary>
    /// <param name="update_mode">The update mode. Started routines with this mode will be updated.</param>
    public static void UpdateRoutines(EE_RoutineUpdateMode update_mode)
    {
      List<ET_Routine> update = updating_routines[(int)update_mode]; // Get the list corresponding to the mode.

      // For every routine in the list, update it.
      for (int i = 0; i < update.Count; i++)
        update[i].Update();
    }

    /// <summary>
    /// A function which updates all routines of a given tag.
    /// </summary>
    /// <param name="tag">The tag to look for and update.</param>
    public static void UpdateRoutinesByTag(string tag)
    {
      // Make sure the tag isn't null.
      if (tag != null)
      {
        // Go through all running routines, updating any with the specified tag.
        for (int i = 0; i < running_routines.Count; i++)
        {
          ET_Routine eroutine = running_routines.Values.ElementAt(i);
          if (eroutine.tag == tag)
            eroutine.Update();
        }
      }
    }

    /// <summary>
    /// A function which will convert a YieldInstruction into a standard IEnumerator supported by ENTITY. This is only for the YieldInstructions
    /// provided by Unity. CustomYieldInstructions already inherit IEnumerator, and thus are unaffected.
    /// </summary>
    /// <param name="instruction"></param>
    /// <returns></returns>
    private IEnumerator GetConvertedYieldInstruction(YieldInstruction instruction)
    {
      Func<ET_Routine, YieldInstruction, IEnumerator> conversion_func; // The conversion function to use.

      // Attempt to get the corresponding conversion function. If it exists, return it.
      if (yield_conversions.TryGetValue(instruction.GetType(), out conversion_func))
        return conversion_func(this, instruction);

      return null; // If it does not exist, simply return null.
    }

    /// <summary>
    /// The conversion function for 'WaitForSeconds'. This will convert that YieldInstruction into a usable
    /// version for an ET_Routine.
    /// </summary>
    /// <param name="routine">The routine which called the wait.</param>
    /// <param name="instruction">The instruction to convert.</param>
    /// <returns>Returns the instruction converted to an 'EY_UnityWaitForSeconds'.</returns>
    private static IEnumerator ConvertWaitForSeconds(ET_Routine routine, YieldInstruction instruction)
    {
      float duration = EKIT_Reflection.GetFieldValue<float>(instruction, "m_Seconds"); // Get the duration via reflection.
      return new EY_UnityWaitForSeconds(routine, duration); // Create a new IEnumerator with the duration and return it.
    }

    /// <summary>
    /// An IEnumerator used to yield 'WaitForEndOfFrame' and 'WaitForFixedUpdate'. This will yield the instruction
    /// to a normal Unity Coroutine before performing a special Update loop.
    /// </summary>
    /// <param name="routine">The routine which called the wait.</param>
    /// <param name="instruction">The instruction to yield. This will be either 'WaitForEndOfFrame' or 'WaitForFixedUpdate'.</param>
    /// <returns>Returns an IEnumerator representing this function. Should only be used in a Unity Coroutine.</returns>
    private static IEnumerator YieldWaitFor(ET_Routine routine, YieldInstruction instruction)
    {
      yield return instruction; // Yield to the instruction.
      routine.is_awaiting_coroutine = false; // The routine is no longer awaiting.
      routine.UpdateLoop(); // Perform the special update. This performs the rest of the update loop, up until it can no longer continue.
      routine.nested_routine = null;
    }

    /// <summary>
    /// The conversion function for 'WaitForEndOfFrame' and 'WaitForFixedUpdate'. This will convert the
    /// instruction to a special IEnumerator, which is sent to a standard Unity Coroutine to properly wait.
    /// </summary>
    /// <param name="routine">The routine which called the wait.</param>
    /// <param name="instruction">The instruction to convert.</param>
    /// <returns>Returns nothing. Instead, the routine is told to await a standard Unity Coroutine.</returns>
    private static IEnumerator ConvertWaitFor(ET_Routine routine, YieldInstruction instruction)
    {
      // Check if the routine exists.
      if (ESYS_Routine.parent != null)
      {
        routine.is_awaiting_coroutine = true; // It is now awaiting a coroutine.
        ESYS_Routine.StartUnityCoroutine(YieldWaitFor(routine, instruction)); // Start the coroutine on the Routine System's object.
      }

      return null; // Return nothing. We do not want the Routine to manually wait for this routine.
    }

    /// <summary>
    /// The conversion function for an 'AsyncOperation'. AsyncOperations are special; they cannot be paused.
    /// As such, the routine returned by this merely waits for the AsyncOperation to have 'isDone' set to true.
    /// </summary>
    /// <param name="routine">The routine which called the wait.</param>
    /// <param name="instruction">The instruction to convert.</param>
    /// <returns>Returns the instruction convereted to an EY_UnityAsyncOperation'</returns>
    private static IEnumerator ConvertAsyncOperation(ET_Routine routine, YieldInstruction instruction)
    {
      return new EY_UnityAsyncOperation((AsyncOperation)instruction);
    }

    /// <summary>
    /// A function which creates an instruction to copy a parent routine's UpdateMode and ReturnMode.
    /// </summary>
    /// <param name="routine">The child routine who will have their settings changed.</param>
    /// <returns>Returns an object it hide the actual Instruction class. This instruction is set to copy the settings of the parent routine.</returns>
    public static object InstructCopyParentSettings(ET_Routine routine)
    {
      // If the routine exists, create an instruction for copying all settings.
      if (routine != null)
        return new ET_RoutineInstruction(routine, ET_RoutineInstruction.EE_RoutineInstruction.CopyAllSettings);

      return null; // The routine doesn't exist, so return null.
    }

    /// <summary>
    /// A function which creates an instruction to copy a parent routine's UpdateMode.
    /// </summary>
    /// <param name="routine">The child routine who will have their settings changed.</param>
    /// <returns>Returns an object it hide the actual Instruction class. This instruction is set to copy the settings of the parent routine.</returns>
    public static object InstructCopyParentUpdateMode(ET_Routine routine)
    {
      // If the routine exists, create an instruction for copying the UpdateMode.
      if (routine != null)
        return new ET_RoutineInstruction(routine, ET_RoutineInstruction.EE_RoutineInstruction.CopyUpdateMode);

      return null; // The routine doesn't exist, so return null.
    }

    /// <summary>
    /// A function which creates an instruction to copy a parent routine's ReturnMode.
    /// </summary>
    /// <param name="routine">The child routine who will have their settings changed.</param>
    /// <returns>Returns an object it hide the actual Instruction class. This instruction is set to copy the settings of the parent routine.</returns>
    public static object InstructCopyParentReturnMode(ET_Routine routine)
    {
      // If the routine exists, create an instruction for copying the ReturnMode.
      if (routine != null)
        return new ET_RoutineInstruction(routine, ET_RoutineInstruction.EE_RoutineInstruction.CopyReturnMode);

      return null; // The routine doesn't exist, so return null.
    }

    /// <summary>
    /// A function which pauses all routines of a given tag.
    /// </summary>
    /// <param name="tag">The tag to look for and pause.</param>
    public static void PauseRoutinesByTag(string tag)
    {
      // Make sure the tag isn't null.
      if (tag != null)
      {
        // Go through all running routines, updating any with the specified tag.
        for (int i = 0; i < running_routines.Count; i++)
        {
          ET_Routine eroutine = running_routines.Values.ElementAt(i);
          if (eroutine.tag == tag)
            eroutine.Pause();
        }
      }
    }

    /// <summary>
    /// A function which unpauses all routines of a given tag.
    /// </summary>
    /// <param name="tag">The tag to look for and unpause.</param>
    public static void UnPauseRoutinesByTag(string tag)
    {
      // Make sure the tag isn't null.
      if (tag != null)
      {
        // Go through all running routines, updating any with the specified tag.
        for (int i = 0; i < running_routines.Count; i++)
        {
          ET_Routine eroutine = running_routines.Values.ElementAt(i);
          if (eroutine.tag == tag)
            eroutine.UnPause();
        }
      }
    }

    /// <summary>
    /// A function which toggles the pause status of all routines of a given tag.
    /// </summary>
    /// <param name="tag">The tag to look for and toggle pause.</param>
    public static void TogglePauseRoutinesByTag(string tag)
    {
      // Make sure the tag isn't null.
      if (tag != null)
      {
        // Go through all running routines, updating any with the specified tag.
        for (int i = 0; i < running_routines.Count; i++)
        {
          ET_Routine eroutine = running_routines.Values.ElementAt(i);
          if (eroutine.tag == tag)
            eroutine.TogglePause();
        }
      }
    }

    /// <summary>
    /// A function which toggles the pause status of all routines of a given tag, with a given bool.
    /// </summary>
    /// <param name="tag">The tag to look for and toggle pause.</param>
    /// <param name="pause">A bool determining if the routine is paused.</param>
    public static void TogglePauseRoutinesByTag(string tag, bool pause)
    {
      // Make sure the tag isn't null.
      if (tag != null)
      {
        // Go through all running routines, updating any with the specified tag.
        for (int i = 0; i < running_routines.Count; i++)
        {
          ET_Routine eroutine = running_routines.Values.ElementAt(i);
          if (eroutine.tag == tag)
            eroutine.TogglePause(pause);
        }
      }
    }

    /// <summary>
    /// A shorthand function for checking if the routine is paused.
    /// </summary>
    /// <returns>Returns if the routine is paused.</returns>
    public bool IsPaused()
    {
      return run_status == EE_RoutineRunStatus.Paused; // Return if the routine is paused.
    }

    /// <summary>
    /// A function which pauses the routine, if it is running.
    /// </summary>
    public void Pause()
    {
      // If running, the routine is now paused.
      if (run_status == EE_RoutineRunStatus.Running)
        run_status = EE_RoutineRunStatus.Paused;
    }

    /// <summary>
    /// A function which unpauses the routine, if it is paused.
    /// </summary>
    public void UnPause()
    {
      // If paused, the routine is now running.
      if (run_status == EE_RoutineRunStatus.Paused)
        run_status = EE_RoutineRunStatus.Running;
    }

    /// <summary>
    /// A function which toggles the routine between paused and unpaused.
    /// </summary>
    public void TogglePause()
    {
      // If paused, unpause. If not paused, pause.
      if (IsPaused())
        UnPause();
      else
        Pause();
    }

    /// <summary>
    /// A function which sets the routine's paused status based on a given boolean.
    /// </summary>
    /// <param name="pause">A bool determining if the routine is paused.</param>
    public void TogglePause(bool pause)
    {
      // If pausing, pause the routine. Otherwise, unpause the routine.
      if (pause)
        Pause();
      else
        UnPause();
    }

    /// <summary>
    /// The implementation for starting a routine. You must start a Routine via the 'StartRoutine' method.
    /// </summary>
    /// <returns>Returns if the routine was started. Returns false upon failure, or if the routine was already started.</returns>
    private bool Start()
    {
      // We can start if the routine has not previously been started, and it's run status states as such.
      if (!running_routines.ContainsKey(routine) && run_status == EE_RoutineRunStatus.Unstarted)
      {
        running_routines.Add(routine, this); // Add to the dictionary.
        run_status = EE_RoutineRunStatus.Running; // The routine is now running.
        AddToUpdates(this); // Add to an update list.
        return true; // The routine was started.
      }

      return false; // The routine was not started.
    }

    /// <summary>
    /// The implementation for updating a single routine. Call this when updating manually.
    /// </summary>
    /// <returns>Returns if the routine was updated by any steps.</returns>
    public bool Update()
    {
      // Do not update normally if this is nested. The parent will handle that itself.
      if (!is_nested_routine)
      {
        // Reset the update count and perform the loop.
        current_update_count = 0;
        return UpdateLoop();
      }

      return false; // No update occurred.
    }

    /// <summary>
    /// The implementation for the update loop. This is called internally, and should not be called on your own.
    /// </summary>
    /// <returns>Returns if any steps were progressed.</returns>
    private bool UpdateLoop()
    {
      bool update_occured = false; // A bool determining if any updates occurred.

      // Loop for all remaining loops. Coroutine-Awaiting routines pause themselves.
      for (; current_update_count < update_count; current_update_count++)
      {
        // Make sure the routine is running, and not awaiting a Unity Coroutine.
        if (run_status == EE_RoutineRunStatus.Running && !is_awaiting_coroutine)
        {
          // If the nested routine exists, update that first. If it completes, nullify our reference to it.
          if (nested_routine != null)
          {
            nested_routine.UpdateLoop();
            if (nested_routine.run_status == EE_RoutineRunStatus.Complete)
              nested_routine = null;
          }
          // If the nested routine doesn't exist, update this routine.
          if (nested_routine == null)
          {
            NextStep();
            // If awaiting a coroutine now, we want to break before updating the count.
            if (is_awaiting_coroutine)
              break;
          }

          update_occured = true; // An update occured.
        }
        else
          break;
      }

      // If no longer awaiting a coroutine, reset the update count.
      if (!is_awaiting_coroutine)
        current_update_count = 0;

      return update_occured; // Return if any update happened.
    }

    /// <summary>
    /// The implementation for moving a step forward in a routine.
    /// </summary>
    private void NextStep()
    {
      bool proceed = routine.MoveNext(); // Check if there is more to do in the routine.

      // If there is more, check the current value of the routine.
      if (proceed)
      {
        object yield_return = routine.Current; // Get the current value.

        // If it's null, simply return. The step is over.
        if (yield_return == null)
          return;

        //Type type = yield_return.GetType(); // Get the type. We will now handle the return based on the type.

        // TYPE 1: ET_Routine - Make this routine nested and start it if it hasn't been started before.
        if (yield_return is ET_Routine)
        {
          nested_routine = (ET_Routine)yield_return; // Cast the return.
          nested_routine.is_nested_routine = true; // The routine is now nested.
          switch (nested_routine.run_status)
          {
            case EE_RoutineRunStatus.Unstarted:
              StartRoutine(nested_routine);
              break;
            case EE_RoutineRunStatus.Complete:
              NextStep();
              break;
            default:
              RemoveFromUpdates(nested_routine);
              break;
          }
          if (nested_routine.run_status == EE_RoutineRunStatus.Unstarted)
            StartRoutine(nested_routine);
        }
        // TYPE 2: IEnumerator - Create a nested routine based on the IEnumerator.
        else if (yield_return is IEnumerator)
        {
          // Start a new ET_Routine on this update mode. It is now nested.
          nested_routine = CreateRoutine((IEnumerator)yield_return, update_mode);
          nested_routine.is_nested_routine = true;
          StartRoutine(nested_routine);
        }
        // TYPE 3: Yield Instruction - Convert the YieldInstruction into an IEnumerator and create a routine out of it.
        else if (yield_return is YieldInstruction)
        {
          // Attempt to cast the YieldInstruction to a converted ET_Routine. If successful, make it nested.
          nested_routine = CreateRoutine(GetConvertedYieldInstruction((YieldInstruction)yield_return), null);
          if (nested_routine != null)
          {
            nested_routine.is_nested_routine = true;
            StartRoutine(nested_routine);
          }
        }
        // TYPE 4: General Return - Check if we are returning anything. If so, see if the returned value is what we want.
        else if (return_type != null)
        {
          // The return is just a normal variable. Check if it's the value this routine wants, based on its return mode.
          switch (return_mode)
          {
            case EE_RoutineReturnMode.ReturnFirstAndBreak: // Return the first valid value. If successful, stop the routine.
              if (ReturnFirst())
                Stop();
              break;
            case EE_RoutineReturnMode.ReturnFirstAndComplete: // Return the first valid value. Do not stop the routine.
              ReturnFirst();
              break;
            case EE_RoutineReturnMode.ReturnLastAndComplete: // Update with the most recent valid value.
              CheckYieldReturn();
              break;
          }
        }
        // TYPE 5: Instruction Return - Perform a copy instruction before immediately going to the next step.
        else if (yield_return is ET_RoutineInstruction)
        {
          ((ET_RoutineInstruction)yield_return).InvokeInstruction(this);
          NextStep();
        }
      }
      else
        Stop(); // If no longer proceeding, stop the routine.
    }

    /// <summary>
    /// A helper function which checks if the returned value of an enumerator is what the routine is looking for.
    /// </summary>
    /// <returns>Returns if the value was set for the routine.</returns>
    private bool CheckYieldReturn()
    {
      object yield_return = routine.Current; // Get the current object.

      if (return_type.IsAssignableFrom(yield_return.GetType())) // Check if the object is the type that we want.
      {
        value = yield_return; // Set the value.
        return true; // The value was updated.
      }

      return false; // The value was not updated.
    }

    /// <summary>
    /// A helper function which only tries to update the routine's value if it has not been previously set.
    /// </summary>
    /// <returns>Returns if the value was set for the routine.</returns>
    private bool ReturnFirst()
    {
      // If the value is not set, attempt to set it.
      if (value == null)
        return CheckYieldReturn();

      return false; // The value was not set.
    }

    /// <summary>
    /// The implementation for stopping a single routine. This will be called when the routine has no more steps.
    /// </summary>
    /// <param name="call_completion_action">A bool determining if the completion action should still be called.</param>
    /// <returns>Returns if the routine was successfully stopped.</returns>
    private bool Stop(bool call_completion_action = true)
    {
      // Check if the routine is being run.
      if (running_routines.ContainsKey(routine))
      {
        // Stop the nested routine first.
        if (nested_routine != null)
          nested_routine.Stop();

        run_status = EE_RoutineRunStatus.Complete; // The routine is complete.
        RemoveFromUpdates(this); // Remove from the update list.
        running_routines.Remove(routine); // Remove from the running list.
        // Invoke one of the completion actions, if either of them are set.
        if (completion_void != null)
          completion_void.DynamicInvoke();
        else if (completion_input != null)
          completion_input.DynamicInvoke(value);

        return true; // The routine was stopped.
      }

      return false; // The routine was not stopped.
    }

    /// <summary>
    /// The function for moving the IEnumerator version of this class forward. Only necessary if using these in a Unity Coroutine.
    /// </summary>
    /// <returns>Returns if the routine is finished or not.</returns>
    bool IEnumerator.MoveNext()
    {
      // If the routine hasn't been started, we just have to move the stored routine forward.
      // Otherwise, wait for the ESYS_Routine system to finish this instead.
      if (run_status == EE_RoutineRunStatus.Unstarted)
        return routine.MoveNext();

      return run_status == EE_RoutineRunStatus.Complete;
    }

    void IEnumerator.Reset()
    {
      throw new NotImplementedException();
    }

    /**********************************************************************************************************************/
    /// <para>Class Name: EY_UnityWaitForSeconds</para>
    /// <summary>
    /// A converted YieldInstruction for 'WaitForSeconds'. This converts the instruction to a proper IEnumerator.
    /// </summary>
    private sealed class EY_UnityWaitForSeconds : IEnumerator
    {
      /// <summary> The currently yielded object. </summary>
      object IEnumerator.Current { get; }

      /// <summary> The amount of time to wait before returning control. </summary>
      float duration = 0.0f;
      /// <summary> The amount of time that has already been waited. </summary>
      float elapsed = 0.0f;

#if UNITY_EDITOR
      /// <summary> A bool determining if the routine has started. Only used for Editor Routines. </summary>
      bool started = false;
      /// <summary> The time since startup as of the last frame. Only used for Editor Routines. </summary>
      float last_time = 0.0f;
      /// <summary> The routine which is waiting for the duration to finish. </summary>
      ET_Routine routine = null;
#endif

      bool IEnumerator.MoveNext()
      {
#if UNITY_EDITOR
        if (routine != null)
        {
          if (routine.update_mode == EE_RoutineUpdateMode.Editor)
          {
            // If the routine hasn't been started, initialize the last time.
            if (!started)
            {
              started = true;
              last_time = (float)EditorApplication.timeSinceStartup;
            }
            float current_time = (float)EditorApplication.timeSinceStartup; // Get the current time since startup.

            if (routine.run_status == EE_RoutineRunStatus.Running)
              elapsed += current_time - last_time; // Update the elapsed time.
            last_time = current_time; // Update the last time.
          }
          else if (EditorApplication.isPlaying)
          {
            elapsed += Time.deltaTime;
          }
            
        }
#else
      elapsed += Time.deltaTime; // In standard gameplay, update using delta time.
#endif

        return (elapsed < duration); // Continue waiting if the elapsed time has not reached the duration.
      }

      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// The constructor for the converted yield instruction..
      /// </summary>
      /// <param name="routine">The routine which called for this enumerator.</param>
      /// <param name="seconds">The duration to wait for.</param>
      public EY_UnityWaitForSeconds(ET_Routine routine, float seconds)
      {
        duration = seconds; // Set the duration.
#if UNITY_EDITOR
        this.routine = routine;
#endif
      }
    }
    /**********************************************************************************************************************/
    /// <para>Class Name: EY_UnityAsyncOperation</para>
    /// <summary>
    /// A converted YieldInstruction for 'AsyncOperation'. This converts the instruction to a proper IEnumerator. However,
    /// it merely waits for the original AsyncOperation to return 'isDone' as true.
    /// </summary>
    private sealed class EY_UnityAsyncOperation : IEnumerator
    {
      /// <summary> The currently yielded object. </summary>
      object IEnumerator.Current { get; }
      /// <summary> The original AsyncOperation being observed. </summary>
      AsyncOperation operation;

      bool IEnumerator.MoveNext()
      {
        return !operation.isDone; // Return if the operation is done. When it is, we no longer move.
      }

      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// The constructor for the converted yield instruction.
      /// </summary>
      /// <param name="operation">The AsyncOperation to observe.</param>
      public EY_UnityAsyncOperation(AsyncOperation operation)
      {
        this.operation = operation; // Set the operation.
      }
    }
    /**********************************************************************************************************************/
    /// <para>Class Name: ET_RoutineInstruction</para>
    /// <summary>
    /// A special instruction used to copy settings on a child routine. These are used in IEnumerator functions, as they do not
    /// know they are ET_Routines.
    /// </summary>
    private sealed class ET_RoutineInstruction
    {
      /// <summary> The child routine who's settings will be updated. </summary>
      private ET_Routine child = null;
      /// <summary> The function to use to update the child's settings. </summary>
      private Action<ET_Routine> instruction = null;

      /// <summary> An enum determining what settings to copy. This is handled internally due to the short amount of instructions. </summary>
      public enum EE_RoutineInstruction
      {
        /// <summary> Copy the UpdateMode and ReturnMode. </summary>
        CopyAllSettings,
        /// <summary> Copy the UpdateMode. </summary>
        CopyUpdateMode,
        /// <summary> Copy the ReturnMode. </summary>
        CopyReturnMode,
      }

      /// <summary>
      /// The construtor for the Routine Instruction.
      /// </summary>
      /// <param name="child">The child routine that will be updated.</param>
      /// <param name="mode">The mode for how to update the child.</param>
      public ET_RoutineInstruction(ET_Routine child, EE_RoutineInstruction mode)
      {
        this.child = child; // Set the child.
        // Based on the mode, set a different instruction to call.
        switch (mode)
        {
          case EE_RoutineInstruction.CopyUpdateMode:
            instruction = InstructCopyUpdateMode;
            break;
          case EE_RoutineInstruction.CopyReturnMode:
            instruction = InstructCopyReturnMode;
            break;
          default:
            instruction = InstructCopySettings;
            break;
        }
      }

      /// <summary>
      /// A function used to invoke the selected instruction with the parent routine.
      /// </summary>
      /// <param name="parent">The routine to copy the settings from.</param>
      public void InvokeInstruction(ET_Routine parent)
      {
        instruction?.Invoke(parent); // Invoke the instruction, passing in the parent routine.
      }

      /// <summary>
      /// The instruction to copy a parent routine's UpdateMode and ReturnMode.
      /// </summary>
      /// <param name="parent">The routine to copy the settings from.</param>
      private void InstructCopySettings(ET_Routine parent)
      {
        child.update_mode = parent.update_mode; // Copy the update mode.
        child.return_mode = parent.return_mode; // Copy the return mode.
      }

      /// <summary>
      /// The instruction to copy a parent routine's UpdateMode.
      /// </summary>
      /// <param name="parent">The routine to copy the settings from.</param>
      private void InstructCopyUpdateMode(ET_Routine parent)
      {
        child.update_mode = parent.update_mode; // Copy the update mode.
      }

      /// <summary>
      /// The instruction to copy a parent routine's ReturnMode.
      /// </summary>
      /// <param name="parent">The routine to copy the settings from.</param>
      private void InstructCopyReturnMode(ET_Routine parent)
      {
        child.return_mode = parent.return_mode; // Copy the return mode.
      }
    }
    /**********************************************************************************************************************/
  }
  /**********************************************************************************************************************/
}