using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Extensions
  {
    public static void SetColorR(this TextMeshProUGUI tmp, float r)
    {
      Color color = tmp.color;
      color.r = r;
      tmp.color = color;
    }
  }
  /**********************************************************************************************************************/
}