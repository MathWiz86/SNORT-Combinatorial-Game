using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: EKIT_Encryption</para>
  /// <summary>
  /// A toolkit for file management. This includes validating files, creating directories and files, and manipulating data within the files.
  /// </summary>
  public static partial class EKIT_Encryption
  {
    public static bool EncryptDecryptXOR(byte[] data, ref string key, out byte[] edited_data, bool remove_from_memory)
    {
      edited_data = null;

      if (data == null || data.Length <= 0 || key == null || key.Length <= 0)
        return false;

      edited_data = new byte[data.Length];

      for (int current = 0; current < data.Length; current++)
      {
        byte current_byte = data[current];
        current_byte ^= (byte)key[current % key.Length];
        edited_data[current] = current_byte;
      }

      if (remove_from_memory)
        RemoveKeyFromMemory(ref key);

      return true;
    }

    public static bool EncryptDecryptXOR(string data, ref string key, out string edited_data, bool remove_from_memory)
    {
      edited_data = null;

      if (data == null || data.Length <= 0 || key == null || key.Length <= 0)
        return false;

      StringBuilder builder = new StringBuilder();

      for (int current = 0; current < data.Length; current++)
        builder.Append((char)((int)data[current] ^ (uint)key[current % key.Length]));

      edited_data = builder.ToString();

      if (remove_from_memory)
        RemoveKeyFromMemory(ref key);

      return true;
    }
  }
  /**********************************************************************************************************************/
}