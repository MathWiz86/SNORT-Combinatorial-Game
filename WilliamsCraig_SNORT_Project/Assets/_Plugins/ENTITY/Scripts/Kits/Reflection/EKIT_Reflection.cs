/**************************************************************************************************/
/*!
\file   EKIT_Reflection.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains the declaration of a kit for getting values from a class in various ways. This includes
  from a SerializedProperty, a given object, a Field, or a Property. A Field is a standard variable.
  A Property is a { get; set; } variable.

\par References:
  - https://docs.microsoft.com/en-us/dotnet/api/system.reflection.bindingflags?view=netframework-4.8
  - https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo.getvalue?view=netframework-4.8
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <para>Class Name: EKIT_Reflection</para>
  /// <summary>
  /// A set of functions for getting values via C#'s Reflection System. These functions are expensive, so only use sparingly.
  /// </summary>
  public static partial class EKIT_Reflection { }
  /**********************************************************************************************************************/
}