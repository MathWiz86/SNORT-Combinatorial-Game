/**************************************************************************************************/
/*!
\file   EKIT_Math_Lerp.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for Math functions. This includes various helper functions for lerping.

\par References:
  - https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
  - https://forum.unity.com/threads/passing-ref-variable-to-coroutine.379640/
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  public static partial class EKIT_Math
  {
    //! An enum determining how a value should be interpolated.
    public enum EE_SmoothLerpMode
    {
      None, //!< Perform a standard Lerp or Slerp.
      EaseIn, //!< Ease the start of the Lerp or Slerp.
      EaseOut, //!< Ease the end of the Lerp or Slerp.
      EaseBoth, //!< Ease both the start and end of the Lerp or Slerp.
      SuperEaseBoth, //!< A super smooth easing to the start and end of the Lerp or Slerp.
      Exponential2, //!< An exponential (x^2) increase to the Lerp or Slerp.
    }

    //! An enum determining how a Ping Pong should wait.
    public enum EE_WaitTimeMode
    {
      None, //!< Do not wait at all.
      Standard, //!< Wait using WaitForSeconds.
      Realtime, //!< Wait using WaitForSecondsRealtime.
      ENTITY, //!< Wait using EY_WaitForSeconds.
      EndOfFrame, //!< Wait using WaitForEndOfFrame.
      EndOfFixedUpdate //!< Wait using WaitForFixedUpdate.
    }

    /// <summary>
    /// A function which calculates a lerp time based on a given time and lerp mode.
    /// </summary>
    /// <param name="t">The time that has passed so far.</param>
    /// <param name="mode">The method of lerping.</param>
    /// <returns>Returns the time, updated based on the mode.</returns>
    private static float CalculateLerpedTime(float t, EE_SmoothLerpMode mode)
    {
      switch (mode)
      {
        case EE_SmoothLerpMode.EaseIn:
          return 1.0f - Mathf.Cos(t * Mathf.PI * 0.5f); // The Ease In calculation.
        case EE_SmoothLerpMode.EaseOut:
          return Mathf.Sin(t * Mathf.PI * 0.5f); // The Ease Out calculation.
        case EE_SmoothLerpMode.EaseBoth:
          return t * t * (3.0f - 2.0f * t); // The Ease In and Out calculation.
        case EE_SmoothLerpMode.SuperEaseBoth:
          return t * t * t * (t * (6.0f * t - 15.0f) + 10.0f); // A super smooth Ease In and Out calculation
        case EE_SmoothLerpMode.Exponential2:
          return t * t; // The Exponential calculation.
        default:
          return t; // By default, return a standard lerp, which is just what t is.
      }
    }

    /// <summary>
    /// A function which awaits a given amount of time, scaled based on the given mode.
    /// </summary>
    /// <param name="time">The amount of time to wait. Not necessary for waiting till End of Frame or FixedUpdate.</param>
    /// <param name="mode">The method of waiting for a given time.</param>
    /// <param name="e">The ENTITY to get a delta time from. Only needed if using the ENTITY time mode.</param>
    /// <returns>Returns a routine for waiting time.</returns>
    private static IEnumerator AwaitTime(float time, EE_WaitTimeMode mode, ENTITY e = null)
    {
      switch (mode)
      {
        case EE_WaitTimeMode.Standard:
          yield return new WaitForSeconds(time);
          break;
        case EE_WaitTimeMode.Realtime:
          yield return new WaitForSecondsRealtime(time);
          break;
        case EE_WaitTimeMode.ENTITY:
          yield return new EY_WaitForSeconds(time, e);
          break;
        case EE_WaitTimeMode.EndOfFrame:
          yield return new WaitForEndOfFrame();
          break;
        case EE_WaitTimeMode.EndOfFixedUpdate:
          yield return new WaitForFixedUpdate();
          break;
        default:
          yield return null;
          break;
      }
    }

    //FLOAT/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b">The final value.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static float SmoothLerp(float a, float b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp value, incrementing t by the speed.
      return Mathf.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="valueChangedAction">An Action used to lerp a given value. (i.e. result => myValue = result)</param>
    /// <param name="a">The start value.</param>
    /// <param name="b">The final value.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<float> valueChangedAction, float a, float b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, bool is_scaled = false)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      if (is_scaled)
      {
        while (lerpTimer <= 1.0f)
        {
          valueChangedAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.deltaTime, mode));
          yield return null;
        }
      }
      else
      {
        while (lerpTimer <= 1.0f)
        {
          valueChangedAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.unscaledDeltaTime, mode));
          yield return null;
        }
      }
      

      valueChangedAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which calculates a lerped angle value, able to wrap around 360 degrees, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start value.</param>
    /// <param name="b">The final value.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static float SmoothLerpAngle(float a, float b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp value, incrementing t by the speed.
      return Mathf.LerpAngle(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerped angle value, able to wrap around 360 degrees, given a certain mode of lerping.
    /// </summary>
    /// <param name="valueChangedAction">An Action used to lerp a given value. (i.e. result => myValue = result)</param>
    /// <param name="a">The start value.</param>
    /// <param name="b">The final value.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerpAngle(System.Action<float> valueChangedAction, float a, float b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      while (lerpTimer <= 1.0f)
      {
        valueChangedAction(SmoothLerpAngle(a, b, ref lerpTimer, speed, mode));
        yield return null;
      }

      valueChangedAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which performs a Sin Wave lerp on a value.
    /// </summary>
    /// <param name="start">The initial value of the target. This is the position at t=0, the middle of the lerp.</param>
    /// <param name="t">The time that has passed so far. This is updated by the speed.</param>
    /// <param name="speed">The speed of the lerp.</param>
    /// <param name="period">The time period of the sin wave.</param>
    /// <param name="amplitude">The amplitude of the sin wave.</param>
    /// <returns>Returns the lerped value.</returns>
    public static float SinLerp(float start, ref float t, float speed, float period, float amplitude)
    {
      t += speed; // Update the time passed.
      float theta = t / period; // Get the new theta value.
      float distance = amplitude * Mathf.Sin(theta); // Calculate the distance traveled.
      return start + distance; // Apply the distance to the middle value.
    }

    /// <summary>
    /// A routine which performs a Sin Wave lerp on a value. This will perform infinitely until the routine is manually stopped.
    /// </summary>
    /// <param name="valueChangeAction">The Action for updating a value. (i.e. result => myFloat = result)</param>
    /// <param name="startValue">The initial value of the target at t=0</param>
    /// <param name="startTime">The initial time that has passed already. If unsure, set this to 0.</param>
    /// <param name="speed">The speed of the lerp.</param>
    /// <param name="period">The time period of the sin wave.</param>
    /// <param name="amplitude">The amplitude of the sin wave.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SinLerp(System.Action<float> valueChangeAction, float startValue, float startTime, float speed, float period, float amplitude)
    {
      float start = startValue; // Initialize the start value.
      float timer = startTime; // Initialize the timer.

      // Infinitely lerp and update the value.
      while (true)
      {
        float sin = SinLerp(start, ref timer, speed, period, amplitude);
        valueChangeAction(sin);
        yield return null;
      }
    }

    /// <summary>
    /// A routine which performs a Sin Wave lerp on a value. This will perform infinitely until the the loopRequirement returns false.
    /// </summary>
    /// <param name="valueChangeAction">The Action for updating a value. (i.e. result => myFloat = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="startValue">The initial value of the target at t=0</param>
    /// <param name="startTime">The initial time that has passed already. If unsure, set this to 0.</param>
    /// <param name="speed">The speed of the lerp.</param>
    /// <param name="period">The time period of the sin wave.</param>
    /// <param name="amplitude">The amplitude of the sin wave.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SinLerp(System.Action<float> valueChangeAction, System.Func<bool> loopRequirement, float startValue, float startTime, float speed, float period, float amplitude)
    {
      float start = startValue; // Initialize the start value.
      float timer = startTime; // Initialize the timer.

      // Infinitely lerp and update the value until the loopRequirement returns false.
      while (loopRequirement())
      {
        float sin = SinLerp(start, ref timer, speed, period, amplitude);
        valueChangeAction(sin);
        yield return null;
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two values back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="valueChangeAction">The Action for updating a value. (i.e. result => myFloat = result)</param>
    /// <param name="a">The start value.</param>
    /// <param name="b">The final value.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<float> valueChangeAction, float a, float b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(valueChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two values back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="valueChangeAction">The Action for updating a value. (i.e. result => myFloat = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start value.</param>
    /// <param name="b">The final value.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<float> valueChangeAction, System.Func<bool> loopRequirement, float a, float b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(valueChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //COLOR/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start Color.</param>
    /// <param name="b">The final Color.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static Color SmoothLerp(Color a, Color b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color, incrementing t by the speed.
      return Color.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="colorChangeAction">An Action used to lerp a given Color. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Color.</param>
    /// <param name="b">The final Color.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<Color> colorChangeAction, Color a, Color b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, bool is_scaled = false)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      // While the timer is less than 1, lerp the value and set the value in the action.
      if (is_scaled)
      {
        while (lerpTimer <= 1.0f)
        {
          colorChangeAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.deltaTime, mode));
          yield return null;
        }
      }
      else
      {
        while (lerpTimer <= 1.0f)
        {
          colorChangeAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.unscaledDeltaTime, mode));
          yield return null;
        }
      }

      colorChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Colors back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="colorChangeAction">The Action for updating a Color. (i.e. result => myColor = result)</param>
    /// <param name="a">The start Color.</param>
    /// <param name="b">The final Color.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Color> colorChangeAction, Color a, Color b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(colorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Colors back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="colorChangeAction">The Action for updating a Color. (i.e. result => myColor = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Color.</param>
    /// <param name="b">The final Color.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Color> colorChangeAction, System.Func<bool> loopRequirement, Color a, Color b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(colorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //COLOR32///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start Color32.</param>
    /// <param name="b">The final Color32.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static Color32 SmoothLerp(Color32 a, Color32 b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color32, incrementing t by the speed.
      return Color32.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="colorChangeAction">An Action used to lerp a given Color32. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Color32.</param>
    /// <param name="b">The final Color32.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<Color32> colorChangeAction, Color32 a, Color32 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      while (lerpTimer <= 1.0f)
      {
        colorChangeAction(SmoothLerp(a, b, ref lerpTimer, speed, mode));
        yield return null;
      }

      colorChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Color32s back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="colorChangeAction">The Action for updating a Color32. (i.e. result => myColor32 = result)</param>
    /// <param name="a">The start Color32.</param>
    /// <param name="b">The final Color32.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Color32> colorChangeAction, Color32 a, Color32 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(colorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Color32s back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="colorChangeAction">The Action for updating a Color32. (i.e. result => myColor32 = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Color32.</param>
    /// <param name="b">The final Color32.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Color32> colorChangeAction, System.Func<bool> loopRequirement, Color32 a, Color32 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(colorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //VECTOR2///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start Vector2.</param>
    /// <param name="b">The final Vector2.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static Vector2 SmoothLerp(Vector2 a, Vector2 b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color, incrementing t by the speed.
      return Vector2.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="vectorChangeAction">An Action used to lerp a given Vector2. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Vector2.</param>
    /// <param name="b">The final Vector2.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<Vector2> vectorChangeAction, Vector2 a, Vector2 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      while (lerpTimer <= 1.0f)
      {
        vectorChangeAction(SmoothLerp(a, b, ref lerpTimer, speed, mode));
        yield return null;
      }

      vectorChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Vector2s back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Color. (i.e. result => myVector2 = result)</param>
    /// <param name="a">The start Vector2.</param>
    /// <param name="b">The final Vector2.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Vector2> vectorChangeAction, Vector2 a, Vector2 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Vector2s back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Color. (i.e. result => myVector2 = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Vector2.</param>
    /// <param name="b">The final Vector2.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Vector2> vectorChangeAction, System.Func<bool> loopRequirement, Vector2 a, Vector2 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //VECTOR3///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static Vector3 SmoothLerp(Vector3 a, Vector3 b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color, incrementing t by the speed.
      return Vector3.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a slerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="vectorChangeAction">An Action used to lerp a given Vector3. (i.e. result => myVector3 = result)</param>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<Vector3> vectorChangeAction, Vector3 a, Vector3 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, bool is_scaled = false)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      if (is_scaled)
      {
        // While the timer is less than 1, lerp the value and set the value in the action.
        while (lerpTimer <= 1.0f)
        {
          vectorChangeAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.deltaTime, mode));
          yield return null;
        }
      }
      else
      {
        // While the timer is less than 1, lerp the value and set the value in the action.
        while (lerpTimer <= 1.0f)
        {
          vectorChangeAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.unscaledDeltaTime, mode));
          yield return null;
        }
      }
      

      vectorChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which calculates a slerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the slerped value.</returns>
    public static Vector3 SmoothSlerp(Vector3 a, Vector3 b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color, incrementing t by the speed.
      return Vector3.Slerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="vectorChangeAction">An Action used to lerp a given Vector3. (i.e. result => myVector3 = result)</param>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothSlerp(System.Action<Vector3> vectorChangeAction, Vector3 a, Vector3 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      while (lerpTimer <= 1.0f)
      {
        vectorChangeAction(SmoothSlerp(a, b, ref lerpTimer, speed, mode));
        yield return null;
      }

      vectorChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Vector2s back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Vector3. (i.e. result => myVector3 = result)</param>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Vector3> vectorChangeAction, Vector3 a, Vector3 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Vector3s back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Vector3. (i.e. result => myVector3 = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Vector3> vectorChangeAction, System.Func<bool> loopRequirement, Vector3 a, Vector3 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly Slerps between two Vector2s back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Vector3. (i.e. result => myVector3 = result)</param>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="speed">The speed at which the Slerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of Slerping this value. 'None' means using a standard Slerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to Slerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for Slerping.</returns>
    public static IEnumerator SmoothSPingPong(System.Action<Vector3> vectorChangeAction, Vector3 a, Vector3 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothSlerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly Slerps between two Vector3s back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Vector3. (i.e. result => myVector3 = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Vector3.</param>
    /// <param name="b">The final Vector3.</param>
    /// <param name="speed">The speed at which the Slerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of Slerping this value. 'None' means using a standard Slerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to Slerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for Slerping.</returns>
    public static IEnumerator SmoothSPingPong(System.Action<Vector3> vectorChangeAction, System.Func<bool> loopRequirement, Vector3 a, Vector3 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothSlerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //VECTOR4///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start Vector4.</param>
    /// <param name="b">The final Vector4.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static Vector4 SmoothLerp(Vector4 a, Vector4 b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color, incrementing t by the speed.
      return Vector4.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="vectorChangeAction">An Action used to lerp a given Vector4. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Vector4.</param>
    /// <param name="b">The final Vector4.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<Vector4> vectorChangeAction, Vector4 a, Vector4 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      while (lerpTimer <= 1.0f)
      {
        vectorChangeAction(SmoothLerp(a, b, ref lerpTimer, speed, mode));
        yield return null;
      }

      vectorChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Vector4s back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Color. (i.e. result => myVector4 = result)</param>
    /// <param name="a">The start Vector4.</param>
    /// <param name="b">The final Vector4.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Vector4> vectorChangeAction, Vector4 a, Vector4 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Vector4s back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="vectorChangeAction">The Action for updating a Color. (i.e. result => myVector4 = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Vector4.</param>
    /// <param name="b">The final Vector4.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Vector4> vectorChangeAction, System.Func<bool> loopRequirement, Vector4 a, Vector4 b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(vectorChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //QUATERNION////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static Quaternion SmoothLerp(Quaternion a, Quaternion b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color, incrementing t by the speed.
      return Quaternion.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="quaternionChangeAction">An Action used to lerp a given Quaternion. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<Quaternion> quaternionChangeAction, Quaternion a, Quaternion b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, bool is_scaled = false)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      if (is_scaled)
      {
        // While the timer is less than 1, lerp the value and set the value in the action.
        while (lerpTimer <= 1.0f)
        {
          quaternionChangeAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.deltaTime, mode));
          yield return null;
        }
      }
      else
      {
        // While the timer is less than 1, lerp the value and set the value in the action.
        while (lerpTimer <= 1.0f)
        {
          quaternionChangeAction(SmoothLerp(a, b, ref lerpTimer, speed * Time.unscaledDeltaTime, mode));
          yield return null;
        }
      }

      quaternionChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which calculates a slerp value, given a certain mode of Slerping.
    /// </summary>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of slerping this value. 'None' means using a standard Slerp.</param>
    /// <returns>Returns the Slerped value.</returns>
    public static Quaternion SmoothSlerp(Quaternion a, Quaternion b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Slerp using the calculated Slerp Color, incrementing t by the speed.
      return Quaternion.Slerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a slerp value, given a certain mode of Slerping.
    /// </summary>
    /// <param name="quaternionChangeAction">An Action used to Slerp a given Quaternion. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of slerping this value. 'None' means using a standard Slerp.</param>
    /// <returns>Returns the Slerped value.</returns>
    public static IEnumerator SmoothSlerp(System.Action<Quaternion> quaternionChangeAction, Quaternion a, Quaternion b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float SlerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, Slerp the value and set the value in the action.
      while (SlerpTimer <= 1.0f)
      {
        quaternionChangeAction(SmoothSlerp(a, b, ref SlerpTimer, speed, mode));
        yield return null;
      }

      quaternionChangeAction(b); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Quaternions back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Quaternion> quaternionChangeAction, Quaternion a, Quaternion b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(quaternionChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Quaternions back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Quaternion> quaternionChangeAction, System.Func<bool> loopRequirement, Quaternion a, Quaternion b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(quaternionChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly Slerps between two Quaternions back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the Slerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of Slerping this value. 'None' means using a standard Slerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to Slerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for Slerping.</returns>
    public static IEnumerator SmoothSPingPong(System.Action<Quaternion> quaternionChangeAction, Quaternion a, Quaternion b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothSlerp(quaternionChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly Slerps between two Quaternions back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the Slerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of Slerping this value. 'None' means using a standard Slerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to Slerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for Slerping.</returns>
    public static IEnumerator SmoothSPingPong(System.Action<Quaternion> quaternionChangeAction, System.Func<bool> loopRequirement, Quaternion a, Quaternion b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothSlerp(quaternionChangeAction, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //QUATERNION VIA VECTOR3////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A special version of SmoothLerp which allows deciding whether or not to lerp a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)
    /// or the shortest path (i.e. 0 to -190 goes 0 to 170)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static Quaternion SmoothLerp(Vector3 a, Vector3 b, ref float t, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      if (useLongPath)
        return Quaternion.Euler(SmoothLerp(a, b, ref t, speed, mode)); // Using the long path requires not creating a Quaternion until the resulting Vector3 is made.

      return SmoothLerp(Quaternion.Euler(a), Quaternion.Euler(b), ref t, speed, mode); // Using the short path requires creating Quaternions out of the endpoints first.
    }

    /// <summary>
    /// A special version of SmoothLerp which allows deciding whether or not to lerp a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="quaternionChangeAction">An Action used to lerp a given Quaternion. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)
    /// or the shortest path (i.e. 0 to -190 goes 0 to 170)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(System.Action<Quaternion> quaternionChangeAction, Vector3 a, Vector3 b, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, bool is_scaled = false)
    {
      float timer = 0; // Initialize a timer.

      if (is_scaled)
      {
        while (timer < 1.0f) // While the timer is less than 1, lerp the value and set the value in the action.
        {
          quaternionChangeAction(SmoothLerp(a, b, ref timer, speed * Time.deltaTime, useLongPath, mode));
          yield return null;
        }
      }
      else
      {
        while (timer < 1.0f) // While the timer is less than 1, lerp the value and set the value in the action.
        {
          quaternionChangeAction(SmoothLerp(a, b, ref timer, speed * Time.unscaledDeltaTime, useLongPath, mode));
          yield return null;
        }
      }
      

      quaternionChangeAction(Quaternion.Euler(b)); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A special version of SmoothLerp which allows deciding whether or not to slerp a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)
    /// or the shortest path (i.e. 0 to -190 goes 0 to 170)</param>
    /// <param name="mode">The method of slerping this value. 'None' means using a standard lerp.</param>
    public static Quaternion SmoothSlerp(Vector3 a, Vector3 b, ref float t, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      if (useLongPath)
        return Quaternion.Euler(SmoothLerp(a, b, ref t, speed, mode)); // Using the long path requires not creating a Quaternion until the resulting Vector3 is made.

      return SmoothLerp(Quaternion.Euler(a), Quaternion.Euler(b), ref t, speed, mode); // Using the short path requires creating Quaternions out of the endpoints first.
    }

    /// <summary>
    /// A special version of SmoothLerp which allows deciding whether or not to slerp a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="quaternionChangeAction">An Action used to lerp a given Quaternion. (i.e. result => myValue = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)
    /// or the shortest path (i.e. 0 to -190 goes 0 to 170)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the slerped value.</returns>
    public static IEnumerator SmoothSlerp(System.Action<Quaternion> quaternionChangeAction, Vector3 a, Vector3 b, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float timer = 0; // Initialize a timer.

      while (timer < 1.0f)
      {
        quaternionChangeAction(SmoothSlerp(a, b, ref timer, speed, useLongPath, mode)); // While the timer is less than 1, slerp the value and set the value in the action.
        yield return null;
      }

      quaternionChangeAction(Quaternion.Euler(b)); // Finalize with the final value in case of an error.
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Quaternions back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// It allows whether or not to lerp  a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Quaternion> quaternionChangeAction, Vector3 a, Vector3 b, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(quaternionChangeAction, a, b, speed, useLongPath, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Quaternions back and forth. This will perform infinitely until the loopRequirement returns false.
    /// It allows whether or not to lerp  a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(System.Action<Quaternion> quaternionChangeAction, System.Func<bool> loopRequirement, Vector3 a, Vector3 b, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(quaternionChangeAction, a, b, speed, useLongPath, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly Slerps between two Quaternions back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// It allows whether or not to Slerp a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the Slerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)</param>
    /// <param name="mode">The method of Slerping this value. 'None' means using a standard Slerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to Slerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for Slerping.</returns>
    public static IEnumerator SmoothSPingPong(System.Action<Quaternion> quaternionChangeAction, Vector3 a, Vector3 b, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothSlerp(quaternionChangeAction, a, b, speed, useLongPath, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly Slerps between two Quaternions back and forth. This will perform infinitely until the loopRequirement returns false.
    /// It allows whether or not to Slerp a Quaternion on the shortest or longest path.
    /// </summary>
    /// <param name="quaternionChangeAction">The Action for updating a Quaternion. (i.e. result => myQuaternion = result)</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Quaternion.</param>
    /// <param name="b">The final Quaternion.</param>
    /// <param name="speed">The speed at which the Slerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="useLongPath">A bool determining if the quaternion should rotate on the longest path (i.e. 0 to -190 goes 0 to -190)</param>
    /// <param name="mode">The method of Slerping this value. 'None' means using a standard Slerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to Slerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for Slerping.</returns>
    public static IEnumerator SmoothSPingPong(System.Action<Quaternion> quaternionChangeAction, System.Func<bool> loopRequirement, Vector3 a, Vector3 b, float speed, bool useLongPath = true, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothSlerp(quaternionChangeAction, a, b, speed, useLongPath, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    //MATERIAL//////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// A function which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="toChange">The material to change.</param>
    /// <param name="a">The start Material.</param>
    /// <param name="b">The final Material.</param>
    /// <param name="t">A reference to the time that has passed so far. This should be a value between 0 and 1.</param>
    /// <param name="speed">The speed at which t should increment. This is ADDED to t. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static void SmoothLerp(Material toChange, Material a, Material b, ref float t, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      // Lerp using the calculated lerp Color, incrementing t by the speed.
      toChange.Lerp(a, b, CalculateLerpedTime(t += speed, mode));
    }

    /// <summary>
    /// A routine which calculates a lerp value, given a certain mode of lerping.
    /// </summary>
    /// <param name="toChange">The material to change.</param>
    /// <param name="a">The start Material.</param>
    /// <param name="b">The final Material.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <returns>Returns the lerped value.</returns>
    public static IEnumerator SmoothLerp(Material toChange, Material a, Material b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None)
    {
      float lerpTimer = 0.0f; // Initialize a timer.

      // While the timer is less than 1, lerp the value and set the value in the action.
      while (lerpTimer <= 1.0f)
      {
        SmoothLerp(toChange, a, b, ref lerpTimer, speed, mode);
        yield return null;
      }

      toChange.Lerp(a, b, 1);
      yield return null;
    }

    /// <summary>
    /// A function which smoothly lerps between two Materials back and forth. This will perform infinitely until the Coroutine is manually stopped.
    /// </summary>
    /// <param name="toChange">The material to change.</param>
    /// <param name="a">The start Material.</param>
    /// <param name="b">The final Material.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(Material toChange, Material a, Material b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop forever
      while (true)
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(toChange, a, b, speed, mode);
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }

        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }

    /// <summary>
    /// A function which smoothly lerps between two Materials back and forth. This will perform infinitely until the loopRequirement returns false.
    /// </summary>
    /// <param name="toChange">The material to change.</param>
    /// <param name="loopRequirement">The Func determining if the routine continues. When this returns false, the routine ends (i.e. () => myBoolStatement)</param>
    /// <param name="a">The start Material.</param>
    /// <param name="b">The final Material.</param>
    /// <param name="speed">The speed at which the lerp should increment. This is ADDED to the passed time. (i.e. speed = Time.deltaTime * 2.0f)</param>
    /// <param name="mode">The method of lerping this value. 'None' means using a standard lerp.</param>
    /// <param name="waitTime">The amount of time to wait between each movement period.</param>
    /// <param name="period">The number of times to lerp before waiting. Each travel from one end point to another is 1 period.</param>
    /// <param name="waitMode">The way to wait for time to pass.</param>
    /// <param name="caller">The caller to get the custom delta time from. Only needed if using the 'ENTITY' wait mode.</param>
    /// <returns>Returns an IEnumerator for lerping.</returns>
    public static IEnumerator SmoothPingPong(Material toChange, System.Func<bool> loopRequirement, Material a, Material b, float speed, EE_SmoothLerpMode mode = EE_SmoothLerpMode.None, float waitTime = 0.0f, int period = 1, EE_WaitTimeMode waitMode = EE_WaitTimeMode.None, ENTITY caller = null)
    {
      period = NoLessThan(period, 1); // The period must be at least 1.

      // Loop until the requirement is broken.
      while (loopRequirement())
      {
        int currentPeriod = 0; // The current period.
        while (currentPeriod < period) // Lerp while still underneath the number of periods before a delay.
        {
          yield return SmoothLerp(toChange, a, b, speed, mode); // Lerp between the values.
          EKIT_General.SwapValues(ref a, ref b); // Swap the values.
          currentPeriod++; // Increment the period count.
        }
        yield return AwaitTime(waitTime, waitMode, caller); // Await a given amount of time before the next cycle.
      }
    }
  }
}