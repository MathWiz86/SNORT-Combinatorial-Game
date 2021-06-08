using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ENTITY
{
  public static partial class EEDIT_MenuItems
  {
    private const string name_EObject = "ENTITY Object";
    private const string name_ECube = "ENTITY Cube";
    private const string name_ESphere = "ENTITY Sphere";
    private const string name_ECapsule = "ENTITY Capsule";
    private const string name_ECylinder = "ENTITY Cylinder";
    private const string name_EPlane = "ENTITY Plane";
    private const string name_EQuad = "ENTITY Quad";

    /// <summary>
    /// A menu function for spawning an empty GameObject with the 'E_Object' component.
    /// </summary>
    /// <param name="command">The menu command that activated this function.</param>
    [MenuItem("GameObject/ENTITY/Objects/Create ENTITY Object", false, -2)]
    [MenuItem("ENTITY/Objects/Create ENTITY Object %e", false, -2)]
    private static void Spawn_EObject(MenuCommand command)
    {
      GameObject eobj = new GameObject(name_EObject); // Create a new GameObject.
      eobj.AddComponent<E_Object>(); // Add the 'E_Object' component.

      // If an object is being selected, parent the new object to it. Otherwise, spawn it as it's own object.
      if (Selection.activeObject as GameObject != null)
        eobj.transform.parent = (Selection.activeObject as GameObject).transform;
      else
        GameObjectUtility.SetParentAndAlign(eobj, command.context as GameObject);

      Undo.RegisterCreatedObjectUndo(eobj, "Create " + eobj.name); // Allow the creation to be undone.
      Selection.activeObject = eobj; // Move context to the new object.
    }

    /// <summary>
    /// A menu function for spawning a Cube GameObject with the 'E_Object' component.
    /// </summary>
    /// <param name="command">The menu command that activated this function.</param>
    [MenuItem("GameObject/ENTITY/Objects/3D Objects/Create ENTITY Cube", false, 0)]
    [MenuItem("ENTITY/Objects/3D Objects/Create ENTITY Cube", false, 0)]
    private static void Spawn_CubeEObject(MenuCommand command)
    {
      GameObject eobj = GameObject.CreatePrimitive(PrimitiveType.Cube); // Create a new Cube GameObject.
      eobj.name = name_ECube;
      eobj.AddComponent<E_Object>(); // Add the 'E_Object' component.

      // If an object is being selected, parent the new object to it. Otherwise, spawn it as it's own object.
      if (Selection.activeObject as GameObject != null)
        eobj.transform.parent = (Selection.activeObject as GameObject).transform;
      else
        GameObjectUtility.SetParentAndAlign(eobj, command.context as GameObject);

      Undo.RegisterCreatedObjectUndo(eobj, "Create " + eobj.name); // Allow the creation to be undone.
      Selection.activeObject = eobj; // Move context to the new object.
    }

    /// <summary>
    /// A menu function for spawning a Sphere GameObject with the 'E_Object' component.
    /// </summary>
    /// <param name="command">The menu command that activated this function.</param>
    [MenuItem("GameObject/ENTITY/Objects/3D Objects/Create ENTITY Sphere", false, 1)]
    [MenuItem("ENTITY/Objects/3D Objects/Create ENTITY Sphere", false, 1)]
    private static void Spawn_SphereEObject(MenuCommand command)
    {
      GameObject eobj = GameObject.CreatePrimitive(PrimitiveType.Sphere); // Create a new Cube GameObject.
      eobj.name = name_ESphere;
      eobj.AddComponent<E_Object>(); // Add the 'E_Object' component.

      // If an object is being selected, parent the new object to it. Otherwise, spawn it as it's own object.
      if (Selection.activeObject as GameObject != null)
        eobj.transform.parent = (Selection.activeObject as GameObject).transform;
      else
        GameObjectUtility.SetParentAndAlign(eobj, command.context as GameObject);

      Undo.RegisterCreatedObjectUndo(eobj, "Create " + eobj.name); // Allow the creation to be undone.
      Selection.activeObject = eobj; // Move context to the new object.
    }

    /// <summary>
    /// A menu function for spawning a Capsule GameObject with the 'E_Object' component.
    /// </summary>
    /// <param name="command">The menu command that activated this function.</param>
    [MenuItem("GameObject/ENTITY/Objects/3D Objects/Create ENTITY Capsule", false, 2)]
    [MenuItem("ENTITY/Objects/3D Objects/Create ENTITY Capsule", false, 2)]
    private static void Spawn_CapsuleEObject(MenuCommand command)
    {
      GameObject eobj = GameObject.CreatePrimitive(PrimitiveType.Capsule); // Create a new Cube GameObject.
      eobj.name = name_ECapsule;
      eobj.AddComponent<E_Object>(); // Add the 'E_Object' component.

      // If an object is being selected, parent the new object to it. Otherwise, spawn it as it's own object.
      if (Selection.activeObject as GameObject != null)
        eobj.transform.parent = (Selection.activeObject as GameObject).transform;
      else
        GameObjectUtility.SetParentAndAlign(eobj, command.context as GameObject);

      Undo.RegisterCreatedObjectUndo(eobj, "Create " + eobj.name); // Allow the creation to be undone.
      Selection.activeObject = eobj; // Move context to the new object.
    }

    /// <summary>
    /// A menu function for spawning a Cylinder GameObject with the 'E_Object' component.
    /// </summary>
    /// <param name="command">The menu command that activated this function.</param>
    [MenuItem("GameObject/ENTITY/Objects/3D Objects/Create ENTITY Cylinder", false, 3)]
    [MenuItem("ENTITY/Objects/3D Objects/Create ENTITY Cylinder", false, 3)]
    private static void Spawn_CylinderEObject(MenuCommand command)
    {
      GameObject eobj = GameObject.CreatePrimitive(PrimitiveType.Cylinder); // Create a new Cube GameObject.
      eobj.name = name_ECylinder;
      eobj.AddComponent<E_Object>(); // Add the 'E_Object' component.

      // If an object is being selected, parent the new object to it. Otherwise, spawn it as it's own object.
      if (Selection.activeObject as GameObject != null)
        eobj.transform.parent = (Selection.activeObject as GameObject).transform;
      else
        GameObjectUtility.SetParentAndAlign(eobj, command.context as GameObject);

      Undo.RegisterCreatedObjectUndo(eobj, "Create " + eobj.name); // Allow the creation to be undone.
      Selection.activeObject = eobj; // Move context to the new object.
    }

    /// <summary>
    /// A menu function for spawning a Plane GameObject with the 'E_Object' component.
    /// </summary>
    /// <param name="command">The menu command that activated this function.</param>
    [MenuItem("GameObject/ENTITY/Objects/3D Objects/Create ENTITY Plane", false, 4)]
    [MenuItem("ENTITY/Objects/3D Objects/Create ENTITY Plane", false, 4)]
    private static void Spawn_PlaneEObject(MenuCommand command)
    {
      GameObject eobj = GameObject.CreatePrimitive(PrimitiveType.Plane); // Create a new Cube GameObject.
      eobj.name = name_EPlane;
      eobj.AddComponent<E_Object>(); // Add the 'E_Object' component.

      // If an object is being selected, parent the new object to it. Otherwise, spawn it as it's own object.
      if (Selection.activeObject as GameObject != null)
        eobj.transform.parent = (Selection.activeObject as GameObject).transform;
      else
        GameObjectUtility.SetParentAndAlign(eobj, command.context as GameObject);

      Undo.RegisterCreatedObjectUndo(eobj, "Create " + eobj.name); // Allow the creation to be undone.
      Selection.activeObject = eobj; // Move context to the new object.
    }

    /// <summary>
    /// A menu function for spawning a Quad GameObject with the 'E_Object' component.
    /// </summary>
    /// <param name="command">The menu command that activated this function.</param>
    [MenuItem("GameObject/ENTITY/Objects/3D Objects/Create ENTITY Quad", false, 5)]
    [MenuItem("ENTITY/Objects/3D Objects/Create ENTITY Quad", false, 5)]
    private static void Spawn_QuadEObject(MenuCommand command)
    {
      GameObject eobj = GameObject.CreatePrimitive(PrimitiveType.Quad); // Create a new Cube GameObject.
      eobj.name = name_EQuad;
      eobj.AddComponent<E_Object>(); // Add the 'E_Object' component.

      // If an object is being selected, parent the new object to it. Otherwise, spawn it as it's own object.
      if (Selection.activeObject as GameObject != null)
        eobj.transform.parent = (Selection.activeObject as GameObject).transform;
      else
        GameObjectUtility.SetParentAndAlign(eobj, command.context as GameObject);

      Undo.RegisterCreatedObjectUndo(eobj, "Create " + eobj.name); // Allow the creation to be undone.
      Selection.activeObject = eobj; // Move context to the new object.
    }
  }
}
