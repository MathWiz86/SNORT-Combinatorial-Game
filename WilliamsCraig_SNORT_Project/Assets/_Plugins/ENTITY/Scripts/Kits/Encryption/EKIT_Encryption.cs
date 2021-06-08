using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
// https://ourcodeworld.com/articles/read/471/how-to-encrypt-and-decrypt-files-using-the-aes-encryption-algorithm-in-c-sharp
namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: EKIT_Encryption</para>
  /// <summary>
  /// A toolkit for file management. This includes validating files, creating directories and files, and manipulating data within the files.
  /// </summary>
  public static partial class EKIT_Encryption
  {
    [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
    private static extern bool InternalRemoveKeyFromMemory(IntPtr position, int length);

    private static bool RemoveKeyFromMemory(ref string key)
    {
      GCHandle handle = GCHandle.Alloc(key, GCHandleType.Pinned);
      bool success = InternalRemoveKeyFromMemory(handle.AddrOfPinnedObject(), key.Length * 2);
      handle.Free();
      key = null;

      return success;
    }
  }
  /**********************************************************************************************************************/
}