/**************************************************************************************************/
/*!
\file   EMAN_Time.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains a manager for Time manipulation. This affects anything using an Entity time scale.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: EMAN_Time</para>
  /// <summary>
  /// A manager for Time in Entity. This contains variables used for calculating delta time and time scales.
  /// </summary>
  public static class EMAN_Time
  {
    //! An enum determining how this behaviour should scale its time.
    public enum EE_TimeScaling
    {
      FullScaled, //!< Scaled by both World Scale and Object Scale
      WorldScaled, //!< Scaled by World Scale, ignores Object Scale
      SelfScaled, //!< Ignores World Scale, scaled by Object Scale
      Unscaled, //!< Returns UnscaledDeltaTime, with time scale 1.0f
    }

    //! An enum determining how all objects should be scaled with time.
    public enum EE_GlobalTimeMode
    {
      Normal, //!< Scale however the component/object is set to scale.
      ForceWorldScaled, //!< Force all components/objects to use Unity's time scale.
      ForceSelfScaled, //!< Force all components/objects to use their own time scale.
      ForceUnscaled, //!< Force all components/objects to be unscaled.
    }

    public const float time_MinTimeScale = -1.0f; //!< The minimum time scale allowed.
    public const float time_MaxTimeScale = 20.0f; //!< The maximum time scale allowed.

    public static EE_GlobalTimeMode time_GlobalTimeMode = EE_GlobalTimeMode.Normal; // The global time mode for this project. By default, it's at Normal.
  }
  /**********************************************************************************************************************/
}