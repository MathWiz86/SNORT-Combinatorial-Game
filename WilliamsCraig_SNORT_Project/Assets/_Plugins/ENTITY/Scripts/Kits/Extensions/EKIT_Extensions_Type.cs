/**************************************************************************************************/
/*!
\file   EKIT_Extensions_Type.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains functionality for extending a System.Type.

\par References:
*/
/**************************************************************************************************/
using System;
using System.Linq;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Extensions
  {
    /// <summary>
    /// A function which checks if a type is related to another class. This is just a shorthand
    /// for checking if a type is another type, or a subclass of that type.
    /// </summary>
    /// <param name="check_type">The type that is being checked.</param>
    /// <param name="type">The type being checked against.</param>
    /// <returns>Returns if the secondary type is the same as the calling type, or the calling type is a subclass.</returns>
    public static bool IsRelatedToClass(this Type check_type, Type type)
    {
      if (check_type != null && type != null)
        return check_type == type || check_type.IsSubclassOf(type); // Return if the class is a subclass, or the class itself.

      return false; // Something was null, so immediately return false.
    }

    /// <summary>
    /// A function for checking if a type contains an interface, or is the interface itself.
    /// </summary>
    /// <param name="check_type">The type that is being checked.</param>
    /// <param name="type">The type being checked against.</param>
    /// <returns>Returns if the checking type contains the second type as an interface, or if the two equal each other.</returns>
    public static bool IsRelatedToInterface(this Type check_type, Type type)
    {
      if (check_type != null && type != null)
        return check_type == type || check_type.GetInterfaces().Contains(type); // Return if the type is implemented, or is the interface.

      return false; // Something was null, so immediately return false.
    }

    public static bool IsRelatedToType(this Type check_type, Type type)
    {
      // Return if the types are the same, a subclass, or inherits the other type.
      if (check_type != null && type != null)
        return check_type == type || check_type.IsSubclassOf(type) || check_type.GetInterfaces().Contains(type);

      return false; // Something was null, so immediately return false.
    }
  }
  /**********************************************************************************************************************/
}