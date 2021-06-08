/**************************************************************************************************/
/*!
\file   ExtensionKit_Image.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing a few extension functions for an Image.

\par References:
*/
/**************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

/**********************************************************************************************************************/
/// <summary>
/// An enum representing the 4 Color Channels of an Image or Texture.
/// </summary>
public enum ColorChannel
{
  /// <summary> The Red Color Channel. </summary>
  Red,
  /// <summary> The Green Color Channel. </summary>
  Green,
  /// <summary> The Blue Color Channel. </summary>
  Blue,
  /// <summary> The Alpha Color Channel. </summary>
  Alpha,
}
/**********************************************************************************************************************/
/**********************************************************************************************************************/
public static partial class ExtensionKit
{
  /// <summary>
  /// An extension function to quickly change the value of one specific Color Channel on an Image.
  /// </summary>
  /// <param name="img">The Image who's color will be changed.</param>
  /// <param name="value">The value to set the channel to.</param>
  /// <param name="channel">The <see cref="ColorChannel"/> to change the value of.</param>
  public static void SetImageColorChannel(this Image img, float value, ColorChannel channel)
  {
    // Make sure the image is not null.
    if (img != null)
    {
      Color current = img.color; // Get the current color.
      
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

      img.color = current; // Set back the Color.
    }
  }
}
/**********************************************************************************************************************/