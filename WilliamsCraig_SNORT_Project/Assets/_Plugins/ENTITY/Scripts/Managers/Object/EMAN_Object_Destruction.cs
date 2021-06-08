/**************************************************************************************************/
/*!
\file   EMAN_Object_Destruction.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A piece of the Object Manager. This handles the Destruction of various objects. In particular,
  it handles managing all 'DontDestroyOnLoad' objects to allow them to be destroyed anywhere.

\par References:
  - https://gamedev.stackexchange.com/questions/140014/how-can-i-get-all-dontdestroyonload-gameobjects
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EMAN_Object
  {
    private static List<Object> objs_DontDestroyOnLoad = new List<Object>(); //!< A list of Objects set to not be destroyed upon loading a scene.

    /// <summary>
    /// A function that adds an Object to not be destroyed upon loading a scene.
    /// </summary>
    /// <param name="obj">The object to not destroy.</param>
    public static void DontDestroyObjectOnLoad(Object obj)
    {
      // Check if the object is not null.
      if (obj != null)
      {
        CleanupDontDestroyOnLoadObjectList(); // Cleanup the list.
        objs_DontDestroyOnLoad.Add(obj); // Add the Object to the list.
        Object.DontDestroyOnLoad(obj); // Set the object to not be destroyed.
      }
    }

    /// <summary>
    /// A function that destroys all DontDestroyOnLoad Objects in this manager's list.
    /// </summary>
    public static void DestroyAllDontDestroyObjects()
    {
      // For each Object in the list, destroy the Object.
      foreach (Object o in objs_DontDestroyOnLoad)
        Object.Destroy(o);

      objs_DontDestroyOnLoad.Clear(); // Clear the list.
    }

    /// <summary>
    /// A function that cleans up the list of DontDestroyOnLoad Objects, removing null Objects.
    /// </summary>
    private static void CleanupDontDestroyOnLoadObjectList()
    {
      // For each Object in the list, if the Object is null, remove it from the list.
      //foreach(Object o in objs_DontDestroyOnLoad)
      for (int i = 0; i < objs_DontDestroyOnLoad.Count; i++)
      {
        Object o = objs_DontDestroyOnLoad[i];
        if (o == null)
          objs_DontDestroyOnLoad.Remove(o);
      }
    }
  }
  /**********************************************************************************************************************/
}

