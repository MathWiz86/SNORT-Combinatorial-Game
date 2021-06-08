using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Extensions
  {
    public static void SetMaterial(this Renderer renderer, Material material, int index)
    {
      List<Material> mats = renderer.materials.ToList();
      if (mats.IsValidIndex(index))
        mats[index] = material;
      else
        mats.Add(material);

      renderer.materials = mats.ToArray();
    }

    public static void SetMaterials(this Renderer renderer, IList<Material> materials)
    {
      renderer.materials = materials.ToArray();
    }

    public static void InitializeToNewMaterial(this Renderer renderer, int index)
    {
      if (renderer.materials.IsValidIndex(index))
        renderer.SetMaterial(new Material(renderer.materials[index]), index);
    }

    public static void InitializeToNewMaterials(this Renderer renderer)
    {
      Material[] mats = renderer.materials;

      for (int i = 0; i < mats.Length; i++)
        mats[i] = new Material(mats[i]);

      renderer.materials = mats;
    }
  }
  /**********************************************************************************************************************/
}