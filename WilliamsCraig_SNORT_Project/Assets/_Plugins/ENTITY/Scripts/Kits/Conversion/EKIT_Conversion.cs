/**************************************************************************************************/
/*!
\file   EKIT_Conversion.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for converting various types into other types. These are useful in a variety of
  situations.

\par References:
  - https://morgantechspace.com/2013/08/convert-object-to-byte-array-and-vice.html
*/
/**************************************************************************************************/

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: EKIT_Conversion</para>
  /// <summary>
  /// A toolkit for converting various types into other types.
  /// </summary>
  public static class EKIT_Conversion
  {
    /// <summary> An enum used for converting bytes to different sizes. This uses the 1024 byte standard. </summary>
    public enum EE_ByteConversion
    {
      // NOTE: If adding more sizes, make sure they are multiples of the 1024 standard, and assign the appropriate multiplier.
      Byte = 0,
      Kilobyte = 1,
      Megabyte = 2,
      Gigabyte = 3,
      Terabyte = 4,
      Petabyte = 5,
      Exabyte = 6,
      Zettabyte = 7,
      Yottabyte = 8,
    }

    /// <summary>
    /// A function which takes an array of bytes and converts it into an object of a given type. Make sure the type is able to
    /// be acquired from the byte array. If not sure, pass in T as an object.
    /// </summary>
    /// <typeparam name="T">The type of the object the bytes will be converted to.</typeparam>
    /// <param name="bytes">The array of bytes to convert.</param>
    /// <param name="obj">The object which is represented by the bytes.</param>
    /// <returns>Returns if the byte array was successfully converted. If not, 'obj' will be the default value of 'T'.</returns>
    public static bool ConvertFromSerializedBytes<T>(byte[] bytes, out T obj)
    {
      obj = default; // Initialize the object.

      // If the byte array does not exist, immediately return false.
      if (bytes == null || bytes.Length <= 0)
        return false;

      // Create a binary and memory stream to read in all of the bytes.
      BinaryFormatter bstream = new BinaryFormatter();
      using (MemoryStream mstream = new MemoryStream())
      {
        mstream.Write(bytes, 0, bytes.Length);
        mstream.Seek(0, SeekOrigin.Begin);

        // Attempt to deserialize the bytes and convert to type T. If successful, return true. If false (i.e. an invalid cast, return false.
        try
        {
          obj = (T)bstream.Deserialize(mstream);

          return true;
        }
        catch
        {
          return false;
        }
      }
    }

    /// <summary>
    /// A function which takes an object and converts it into an array of bytes. Make sure the object is serializable.
    /// To do this, make sure the struct/enum/class is marked with the 'System.Serializable' property attribute.
    /// </summary>
    /// <typeparam name="T">The type of the object to convert.</typeparam>
    /// <param name="obj">The object to convert to bytes.</param>
    /// <param name="bytes">The array of bytes representing the object.</param>
    /// <returns>Returns if the object was successfully converted or not.</returns>
    public static bool ConvertToSerializedBytes<T>(T obj, out byte[] bytes)
    {
      bytes = null; // Initialize the byte array.

      // Make sure the object is valid, and is serializable.
      if (obj != null && obj.GetType().IsSerializable)
      {
        // Create a binary stream and memory stream.
        BinaryFormatter bstream = new BinaryFormatter();
        using (MemoryStream mstream = new MemoryStream())
        {
          // Serialize the object into bytes.
          bstream.Serialize(mstream, obj);
          bytes = mstream.ToArray();
        }
        return true;
      }

      return false; // The object could not be serialized.
    }

    /// <summary>
    /// A function which converts a size from one size level to another. Recall that this will convert using the 1024 byte standard.
    /// </summary>
    /// <param name="size">The size to convert.</param>
    /// <param name="original_type">The original type of the size.</param>
    /// <param name="conversion_type">The new type of the size.</param>
    /// <returns>Returns the converted byte size..</returns>
    public static double ConvertFileSize(double size, EE_ByteConversion original_type, EE_ByteConversion conversion_type)
    {
      // First, convert directly to bytes. Each size type is a multiple of 1024.
      for (int i = 0; i < (int)original_type; i++)
        size *= 1024.0;

      // Then, convert directly to the wanted size type. Each size type is a multiple of 1024.
      for (int i = 0; i < (int)conversion_type; i++)
        size /= 1024.0;

      return size; // Return the converted size.
    }

    /// <summary>
    /// A function which converts a bool into an integer. If true, '1' is returned. If false, '0' is returned.
    /// </summary>
    /// <param name="statement">The bool statement to convert.</param>
    /// <returns>Returns '1' if the bool is true, and '0' if the bool is false.</returns>
    public static int ConvertBoolToInt(bool statement)
    {
      // If the statement is true, return 1. Otherwise, return 0.
      return statement ? 1 : 0;
    }

    /// <summary>
    /// A function which converts an integer into a bool. Positive numbers return true. Otherwise, they return false.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>Returns 'true' is the number is positive. Returns 'false' otherwise.</returns>
    public static bool ConvertNumberToBool(int number)
    {
      // Return if the number is positive.
      return number > 0;
    }

    /// <summary>
    /// A function which converts an float into a bool. Positive numbers return true. Otherwise, they return false.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>Returns 'true' is the number is positive. Returns 'false' otherwise.</returns>
    public static bool ConvertNumberToBool(float number)
    {
      // Return if the number is positive.
      return number > 0;
    }
  }
}