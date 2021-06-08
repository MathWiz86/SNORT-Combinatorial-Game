/**************************************************************************************************/
/*!
\file   ExtensionKit_SpriteRenderer.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing a few extension functions for a Sprite Renderer.

\par References:
*/
/**************************************************************************************************/

using UnityEngine;

/**********************************************************************************************************************/
public static partial class ExtensionKit
{
  /// <summary>
  /// An extension function to quickly change the value of one specific Color Channel on a Sprite Renderer.
  /// </summary>
  /// <param name="srend">The Sprite Renderer who's color will be changed.</param>
  /// <param name="value">The value to set the channel to.</param>
  /// <param name="channel">The <see cref="ColorChannel"/> to change the value of.</param>
  public static void SetRendererColorChannel(this SpriteRenderer srend, float value, ColorChannel channel)
  {
    // Make sure the image is not null.
    if (srend != null)
    {
      Color current = srend.color; // Get the current color.

      // Swap the value, based on the specified channel.
      switch (channel)
      {
        // Change the Red Channel.
        case ColorChannel.Red:
          current.r = value;
          break;
        // Change the Green Channel.
        case ColorChannel.Green:
          current.g = value;
          break;
        // Change the Blue Channel.
        case ColorChannel.Blue:
          current.b = value;
          break;
        // By Default, change the Alpha Channel.
        default:
          current.a = value;
          break;
      }

      srend.color = current; // Set back the Color.
    }
  }
}
/**********************************************************************************************************************/