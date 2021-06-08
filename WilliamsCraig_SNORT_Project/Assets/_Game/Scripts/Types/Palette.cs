/**************************************************************************************************/
/*!
\file   Palette.cs
\author Craig Williams
\par    Unity Version: 2020.1.0f
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A file containing implementation of a Palette. A Palette is locked to an enum type, making them
  perfect for keeping a list perfectly sized.

\par References:
*/
/**************************************************************************************************/

using ENTITY;
using System;
using System.Collections;
using System.Collections.Generic;
using TypeReferences;
using UnityEditor;
using UnityEngine;

/**********************************************************************************************************************/
/// <summary>
/// A special version of an array which can be enum locked. This will be the same size as the enum set to it.
/// </summary>
/// <typeparam name="TEnum">The type of the enum to use as a key.</typeparam>
/// <typeparam name="TValue">The type stored in the Palette.</typeparam>
[System.Serializable]
public class Palette<TEnum, TValue> : IList<TValue> where TEnum : System.Enum
{
  /// <summary> The values of the palette. This array is sized to the size of the enum. </summary>
  [SerializeField] protected TValue[] values = null;

  /// <summary> The number of values in the Palette. </summary>
  public int Count { get { return values.Length; } }
  /// <summary> A bool determining if the Palette is read only. Should always return false. </summary>
  public bool IsReadOnly { get { return values.IsReadOnly; } }

  /// <summary>
  /// The default constructor for a Palette.
  /// </summary>
  public Palette()
  {
    values = new TValue[KIT_Enum.GetEnumValueCount<TEnum>()];
  }

  /// <summary>
  /// An overload for the [] operator. Returns whatever is at the index of values.
  /// </summary>
  /// <param name="index">The index to use.</param>
  /// <returns>Returns the item at the index.</returns>
  public TValue this[int index] { get { return values[index]; } set { values[index] = value; } }

  /// <summary>
  /// An overload for the [] operator. Returns whatever is at the index of values.
  /// </summary>
  /// <param name="index">The index to use. This can be an enum. Be warned not necessarily the enum lock.</param>
  /// <returns>Returns the item at the index.</returns>
  public TValue this[TEnum index] { get { return values[Convert.ToInt32(index)]; } set { values[Convert.ToInt32(index)] = value; } }

  /// <summary>
  /// A function for getting the index of a value.
  /// </summary>
  /// <param name="item">The item to get the index of.</param>
  /// <returns>Returns the index of the item. Returns -1 if the item is not in the Palette.</returns>
  public int IndexOf(TValue item)
  {
    return Array.IndexOf(values, item);
  }

  /// <summary>
  /// A function that checks if the Palette contains a specific item.
  /// </summary>
  /// <param name="item">The item to look for.</param>
  /// <returns>Returns true if the item is in the Palette. Returns false otherwise.</returns>
  public bool Contains(TValue item)
  {
    return Array.IndexOf(values, item) != -1;
  }

  /// <summary>
  /// A function that copies a value at a given index into another array.
  /// </summary>
  /// <param name="array">The array to copy into.</param>
  /// <param name="arrayIndex">The index of the value to copy.</param>
  public void CopyTo(TValue[] array, int arrayIndex)
  {
    values.CopyTo(array, arrayIndex);
  }

  /// <summary>
  /// An enumeration for the values of the Palette.
  /// </summary>
  /// <returns>Returns an IEnumerable for the values.</returns>
  protected IEnumerable<TValue> Enumeration()
  {
    foreach (TValue t in values)
      yield return t;
  }

  /// <summary>
  /// An enumerator for the values of the Palette.
  /// </summary>
  /// <returns>Returns a routine that can be enumerated (iterated) through.</returns>
  public IEnumerator<TValue> GetEnumerator()
  {
    return Enumeration().GetEnumerator();
  }

  /// <summary>
  /// An enumerator for the values of the Palette's values.
  /// </summary>
  /// <returns>Returns a routine that can be enumerated (iterated) through.</returns>
  IEnumerator IEnumerable.GetEnumerator()
  {
    return values.GetEnumerator();
  }

  /// <summary>
  /// NOT IMPLEMENTED. DO NOT USE.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="item"></param>
  public void Insert(int index, TValue item)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// NOT IMPLEMENTED. DO NOT USE.
  /// </summary>
  /// <param name="index"></param>
  public void RemoveAt(int index)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// NOT IMPLEMENTED. DO NOT USE.
  /// </summary>
  /// <param name="item"></param>
  public void Add(TValue item)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// NOT IMPLEMENTED. DO NOT USE.
  /// </summary>
  public void Clear()
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// NOT IMPLEMENTED. DO NOT USE.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public bool Remove(TValue item)
  {
    throw new NotImplementedException();
  }
}
/**********************************************************************************************************************/
#if UNITY_EDITOR
/**********************************************************************************************************************/
[CustomPropertyDrawer(typeof(Palette<,>))]
public class PaletteDrawer : E_PropertyDrawer
{
  protected SerializedProperty sp_Values;
  bool isExpanded = false;
  float height = 15.0f;
  float heightChange = 22.0f;

  protected void DisplayPalette(Rect position, SerializedProperty property, GUIContent label)
  {
    sp_Values = property.FindPropertyRelative("values");
    height = 15.0f;
    position.height = height;

    EditorGUI.BeginProperty(position, label, property);
    isExpanded = EditorGUI.Foldout(position, isExpanded, label, true);

    if (isExpanded)
    {
      EditorGUI.indentLevel++;
      height += heightChange;
      position.y += heightChange;

      object palette = EKIT_Reflection.GetSerializedValue<object>(property);

      position.y += 5.0f;
      height += 5.0f;
      EKIT_EditorGUI.DrawEnumArray(palette.GetType().GenericTypeArguments[0], position, ref height, sp_Values, "", "", "Palette Values");
      height += heightChange;

      EditorGUI.indentLevel--;
    }

    EditorGUI.EndProperty();
  }

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    DisplayPalette(position, property, label);
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  {
    return height;
  }
}
/**********************************************************************************************************************/
#endif