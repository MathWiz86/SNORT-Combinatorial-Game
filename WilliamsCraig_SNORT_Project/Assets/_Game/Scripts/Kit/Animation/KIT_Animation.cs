/**************************************************************************************************/
/*!
\file   KIT_Animation.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing extra tools for Animations. Taken from a prior project.

\par References:
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// A kit of extra functions for dealing with Animations.
/// </summary>
public static class KIT_Animation
{
  /// <summary>
  /// A routine that waits for an animation to end.
  /// </summary>
  /// <param name="ator">The animator the animation will play on.</param>
  /// <param name="stateID">The state name of the animation.</param>
  /// <param name="layer">The layer of the animation state. Defaults to 0.</param>
  /// <returns>Returns and IEnumerator to be used in a Routine.</returns>
  public static IEnumerator WaitForAnimationEnd(Animator ator, string stateID, int layer = 0)
  {
    // End immediately if there is no animator.
    if (ator == null)
      yield break;

    float waitTime = 0.5f; // A buffer time, to make sure the states have cahnged.
    ator.Play(stateID, layer); // Begin the animation.
    yield return new WaitForSecondsRealtime(waitTime); // Wait the buffer time.
    // Get the state info on the designated layer.
    AnimatorStateInfo info = ator.GetCurrentAnimatorStateInfo(layer);
    // Either wait for Realtime or Gametime based on the animator's update mode.
    if (ator.updateMode == AnimatorUpdateMode.UnscaledTime)
      yield return new WaitForSecondsRealtime(info.length * Mathf.Abs(info.speed) - waitTime);
    else
      yield return new WaitForSeconds(info.length * Mathf.Abs(info.speed) - waitTime);
  }
}
/**********************************************************************************************************************/