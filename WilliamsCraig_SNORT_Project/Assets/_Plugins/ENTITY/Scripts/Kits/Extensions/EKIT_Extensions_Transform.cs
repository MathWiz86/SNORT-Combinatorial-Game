/**************************************************************************************************/
/*!
\file   EKIT_Extensions_Transform.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for Component extended functions. This file includes functions for a Transform.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Extensions
  {
    /// <summary>
    /// A helper function for setting a transform's X position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the X position.</param>
    public static void SetPositionX(this Transform tr, float value)
    {
      Vector3 pos = tr.position;
      pos.x = value;
      tr.position = pos;
    }

    /// <summary>
    /// A helper function for setting a transform's Y position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Y position.</param>
    public static void SetPositionY(this Transform tr, float value)
    {
      Vector3 pos = tr.position;
      pos.y = value;
      tr.position = pos;
    }

    /// <summary>
    /// A helper function for setting a transform's Z position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Z position.</param>
    public static void SetPositionZ(this Transform tr, float value)
    {
      Vector3 pos = tr.position;
      pos.z = value;
      tr.position = pos;
    }

    /// <summary>
    /// A helper function for setting a transform's X local position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the X local position.</param>
    public static void SetLocalPositionX(this Transform tr, float value)
    {
      Vector3 pos = tr.localPosition;
      pos.x = value;
      tr.localPosition = pos;
    }

    /// <summary>
    /// A helper function for setting a transform's Y local position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Y local position.</param>
    public static void SetLocalPositionY(this Transform tr, float value)
    {
      Vector3 pos = tr.localPosition;
      pos.y = value;
      tr.localPosition = pos;
    }

    /// <summary>
    /// A helper function for setting a transform's Z local position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Z local position.</param>
    public static void SetLocalPositionZ(this Transform tr, float value)
    {
      Vector3 pos = tr.localPosition;
      pos.z = value;
      tr.localPosition = pos;
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion X rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion X rotation.</param>
    public static void SetRotationX(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.x = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion Y rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion Y rotation.</param>
    public static void SetRotationY(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.y = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion Z rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion Z rotation.</param>
    public static void SetRotationZ(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.z = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion W rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion W rotation.</param>
    public static void SetRotationW(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.w = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's Euler X rotation.
    /// This changes transform.rotation.eulerAngles, which is useful in rotating to the smallest/shortest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler X rotation.</param>
    public static void SetRotationEulerX(this Transform tr, float value)
    {
      Vector3 rot = tr.rotation.eulerAngles;
      rot.x = value;
      tr.rotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// A helper function for setting a transform's Euler Y rotation.
    /// This changes transform.rotation.eulerAngles, which is useful in rotating to the smallest/shortest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler Y rotation.</param>
    public static void SetRotationEulerY(this Transform tr, float value)
    {
      Vector3 rot = tr.rotation.eulerAngles;
      rot.y = value;
      tr.rotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// A helper function for setting a transform's Euler Z rotation.
    /// This changes transform.rotation.eulerAngles, which is useful in rotating to the smallest/shortest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler Z rotation.</param>
    public static void SetRotationEulerZ(this Transform tr, float value)
    {
      Vector3 rot = tr.rotation.eulerAngles;
      rot.z = value;
      tr.rotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion X local rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion X local rotation.</param>
    public static void SetLocalRotationX(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.x = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion Y local rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion Y local rotation.</param>
    public static void SetLocalRotationY(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.y = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion Z local rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion Z local rotation.</param>
    public static void SetLocalRotationZ(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.z = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's quaternion W local rotation.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the quaternion W local rotation.</param>
    public static void SetLocalRotationW(this Transform tr, float value)
    {
      Quaternion rot = tr.localRotation;
      rot.w = value;
      tr.localRotation = rot;
    }

    /// <summary>
    /// A helper function for setting a transform's Euler X local rotation.
    /// This changes transform.localRotation.eulerAngles, which is useful in rotating to the smallest/shortest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler X local rotation.</param>
    public static void SetLocalRotationEulerX(this Transform tr, float value)
    {
      Vector3 rot = tr.localRotation.eulerAngles;
      rot.x = value;
      tr.localRotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// A helper function for setting a transform's Euler Y local rotation.
    /// This changes transform.localRotation.eulerAngles, which is useful in rotating to the smallest/shortest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler Y local rotation.</param>
    public static void SetLocalRotationEulerY(this Transform tr, float value)
    {
      Vector3 rot = tr.localRotation.eulerAngles;
      rot.y = value;
      tr.localRotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// A helper function for setting a transform's Euler Z local rotation.
    /// This changes transform.localRotation.eulerAngles, which is useful in rotating to the smallest/shortest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler Z local rotation.</param>
    public static void SetLocalRotationEulerZ(this Transform tr, float value)
    {
      Vector3 rot = tr.localRotation.eulerAngles;
      rot.z = value;
      tr.localRotation = Quaternion.Euler(rot);
    }

    /// <summary>
    /// A helper function for setting a transform's Euler X.
    /// This changes transform.eulerAngles, which is useful in rotating to the given/longest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler X.</param>
    public static void SetTransformEulerX(this Transform tr, float value)
    {
      Vector3 euler = tr.eulerAngles;
      euler.x = value;
      tr.eulerAngles = euler;
    }

    /// <summary>
    /// A helper function for setting a transform's Euler Y.
    /// This changes transform.eulerAngles, which is useful in rotating to the given/longest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler Y.</param>
    public static void SetTransformEulerY(this Transform tr, float value)
    {
      Vector3 euler = tr.eulerAngles;
      euler.y = value;
      tr.eulerAngles = euler;
    }

    /// <summary>
    /// A helper function for setting a transform's Euler Z.
    /// This changes transform.eulerAngles, which is useful in rotating to the given/longest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Euler Z.</param>
    public static void SetTransformEulerZ(this Transform tr, float value)
    {
      Vector3 euler = tr.eulerAngles;
      euler.z = value;
      tr.eulerAngles = euler;
    }

    /// <summary>
    /// A helper function for setting a transform's Local Euler X.
    /// This changes transform.localEulerAngles, which is useful in rotating to the given/longest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Local Euler X.</param>
    public static void SetTransformLocalEulerX(this Transform tr, float value)
    {
      Vector3 euler = tr.localEulerAngles;
      euler.x = value;
      tr.localEulerAngles = euler;
    }

    /// <summary>
    /// A helper function for setting a transform's Local Euler Y.
    /// This changes transform.localEulerAngles, which is useful in rotating to the given/longest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Local Euler Y.</param>
    public static void SetTransformLocalEulerY(this Transform tr, float value)
    {
      Vector3 euler = tr.localEulerAngles;
      euler.y = value;
      tr.localEulerAngles = euler;
    }

    /// <summary>
    /// A helper function for setting a transform's Local Euler Z.
    /// This changes transform.localEulerAngles, which is useful in rotating to the given/longest value.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Local Euler Z.</param>
    public static void SetTransformLocalEulerZ(this Transform tr, float value)
    {
      Vector3 euler = tr.localEulerAngles;
      euler.z = value;
      tr.localEulerAngles = euler;
    }

    /// <summary>
    /// A helper function for setting a transform's X local scale.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the X local scale.</param>
    public static void SetLocalScaleX(this Transform tr, float value)
    {
      Vector3 scale = tr.localScale;
      scale.x = value;
      tr.localScale = scale;
    }

    /// <summary>
    /// A helper function for setting a transform's Y local scale.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Y local scale.</param>
    public static void SetLocalScaleY(this Transform tr, float value)
    {
      Vector3 scale = tr.localScale;
      scale.y = value;
      tr.localScale = scale;
    }

    /// <summary>
    /// A helper function for setting a transform's Z local scale.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Z local scale.</param>
    public static void SetLocalScaleZ(this Transform tr, float value)
    {
      Vector3 scale = tr.localScale;
      scale.z = value;
      tr.localScale = scale;
    }

    /// <summary>
    /// A helper function which resets a transform's position to 0,0,0.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    public static void ResetPosition(this Transform tr)
    {
      tr.position = Vector3.zero;
    }

    /// <summary>
    /// A helper function which resets a transform's local position to 0,0,0.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    public static void ResetLocalPosition(this Transform tr)
    {
      tr.localPosition = Vector3.zero;
    }

    /// <summary>
    /// A helper function which resets a transform's rotation to 0,0,0,0.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    public static void ResetRotation(this Transform tr)
    {
      tr.rotation = Quaternion.identity;
    }

    /// <summary>
    /// A helper function which resets a transform's local rotation to 0,0,0,0.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    public static void ResetLocalRotation(this Transform tr)
    {
      tr.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// A helper function which resets a transform's local scale to 0,0,0.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    public static void ResetLocalScale(this Transform tr)
    {
      tr.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// <summary>
    /// A helper function for setting a rect transform's X local anchored position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the X local position.</param>
    public static void SetAnchoredPositionX(this RectTransform tr, float value)
    {
      Vector3 pos = tr.anchoredPosition;
      pos.x = value;
      tr.anchoredPosition = pos;
    }

    /// <summary>
    /// A helper function for setting a rect transform's Y local anchored position.
    /// </summary>
    /// <param name="tr">The transform to change.</param>
    /// <param name="value">The new value of the Y local position.</param>
    public static void SetAnchoredPositionY(this RectTransform tr, float value)
    {
      Vector3 pos = tr.anchoredPosition;
      pos.y = value;
      tr.anchoredPosition = pos;
    }

    public static void SetSizeDeltaX(this RectTransform tr, float value)
    {
      Vector2 size = tr.sizeDelta;
      size.x = value;
      tr.sizeDelta = size;
    }

    public static void SetSizeDeltaY(this RectTransform tr, float value)
    {
      Vector2 size = tr.sizeDelta;
      size.y = value;
      tr.sizeDelta = size;
    }
  }
  /**********************************************************************************************************************/
}
