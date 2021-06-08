/**************************************************************************************************/
/*!
\file   EC_Rigidbody.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  This file contains functionality for extending a 3D Rigidbody.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ENTITY
{
#if UNITY_EDITOR
  /**********************************************************************************************************************/
  /// <para>Class Name: EC_Rigidbody</para>
  /// <summary>
  /// An extension component for a 3D Rigidbody. Use this to add functionality to your rigidbody.
  /// </summary>
  [RequireComponent(typeof(Rigidbody))] [DisallowMultipleComponent] [CanEditMultipleObjects]
  public class EC_Rigidbody : E_Component
  {
    public Rigidbody rigidbody3D { get { return GetComponent<Rigidbody>(); } } //!< The attached rigidbody to this E_Object.
    [Range(-30, 30)] public float erigid_gravityScale = 1.0f; //!< The custom gravity scale for this rigidbody.
    [SerializeField] private bool erigid_useCustomGravity = false; //!< A bool determining if this rigidbody should use a custom gravity scale.
    [SerializeField] private Vector3 erigid_CustomGravity = new Vector3(0.0f, -9.81f, 0.0f); //!< The custom gravity scale for this object, if being used.

    private float lastTimeScale = 0.0f; //!< The time scale on the last frame.
    private Vector3 frozenVelocity = Vector3.zero; //!< The last velocity before an object was frozen to a time scale of 0.
    private RigidbodyConstraints frozenConstraints = RigidbodyConstraints.None; //The constraints set on this object before an object was frozen to a time scale of 0.

    private void Awake()
    {
      lastTimeScale = GetCalculatedTimeScale(); // Start off the last time scale.
      frozenConstraints = rigidbody3D.constraints; // Start off the frozen constraints.
    }

    private void FixedUpdate()
    {
      UpdateLastTimeScale(); // Update the time scale values.
      ApplyGravity(); // Apply the gravity force to this object.
    }

    /// <summary>
    /// A function that checks the last frame's time scale to this frame's time scale.
    /// This determines how the rigidbody should be updated before applying gravity.
    /// </summary>
    private void UpdateLastTimeScale()
    {
      float currentTimeScale = GetCalculatedTimeScale(); // Get the current time scale.

      // If the time scales do not match, we need to update some values.
      if (lastTimeScale != currentTimeScale)
      {
        // If the last time scale was 0, reset to the frozen velocity and constraints.
        if (lastTimeScale == 0)
        {
          rigidbody3D.velocity = frozenVelocity;
          rigidbody3D.constraints = frozenConstraints;
        }
        else
          rigidbody3D.velocity /= lastTimeScale; // Else, divide velocity by the last time scale to get the unscaled velocity.

        // If the current time scale is 0, we need to freeze values.
        if (currentTimeScale == 0)
        {
          frozenVelocity = rigidbody3D.velocity; // Get the frozen velocity.
          frozenConstraints = rigidbody3D.constraints; // Get the frozen constraints
          rigidbody3D.constraints = RigidbodyConstraints.FreezeAll; // Make sure the object can't move at all.
        }

        rigidbody3D.velocity *= currentTimeScale; // Rescale the velocity to the current time scale.
      }

      lastTimeScale = currentTimeScale; // Update the last time scale.
    }

    /// <summary>
    /// A function to apply gravity on our own to allow custom time scales to affect this object.
    /// </summary>
    private void ApplyGravity()
    {
      rigidbody3D.useGravity = false; // This rigidbody doesn't use gravity by normal means.

      Vector3 gravity; // The gravity to apply to rigidbody.

      // If using custom gravity, start with the custom gravity vector. Otherwise, use the overall gravity vector.
      if (erigid_useCustomGravity)
        gravity = erigid_CustomGravity;
      else
        gravity = Physics.gravity;

      // Multiply by the mass, gravity scale, and delta time. Then add that to the velocity.
      gravity = gravity * rigidbody3D.mass * erigid_gravityScale * GetCalculatedDeltaTime();
      if (GetCalculatedTimeScale() > 0)
        rigidbody3D.velocity += gravity;
      else
        rigidbody3D.velocity -= gravity;
    }

  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: EC_Rigidbody_GUI</para>
  /// <summary>
  /// The custom editor class for a EC_Rigidbody.
  /// </summary>
  [CanEditMultipleObjects, CustomEditor(typeof(EC_Rigidbody))]
  public class EC_Rigidbody_GUI : EGUI_Component
  {
    protected SerializedProperty sp_ERigidGravityScale; //!< The SerializedProperty for erigid_GravityScale.
    protected SerializedProperty sp_ERigidUseCustomGravity; //!< The SerializedProperty for erigid_UseCustomGravity.
    protected SerializedProperty sp_ERigidCustomGravity; //!< The SerializedProperty for erigid_CustomGravity.

    private bool rigidbodyExpanded = false; //!< A check for if the 'Rigidbody Properties' Foldout is expanded.

    /// <summary>
    /// A function containing all OnEnable functionality for EC_Rigidbody properties.
    /// </summary>
    protected void Enable_Rigidbody()
    {
      sp_ERigidGravityScale = serializedObject.FindProperty("erigid_gravityScale");
      sp_ERigidUseCustomGravity = serializedObject.FindProperty("erigid_useCustomGravity");
      sp_ERigidCustomGravity = serializedObject.FindProperty("erigid_CustomGravity");
    }

    /// <summary>
    /// A function containing all OnInspectorGUI functionality for EC_Rigidbody properties.
    /// </summary>
    protected void Inspector_Rigidbody()
    {
      rigidbodyExpanded = EditorGUILayout.Foldout(rigidbodyExpanded, "Rigidbody Properties", true); // Create a foldout for the Core Properties.

      if (rigidbodyExpanded)
      {
        int startIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        if (sp_ERigidGravityScale != null)
          EditorGUILayout.PropertyField(sp_ERigidGravityScale, new GUIContent("Gravity Scale", "The multiplier to this Rigidbody's gravity."));
        if (sp_ERigidUseCustomGravity != null)
          sp_ERigidUseCustomGravity.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Use Custom Gravity", "If true, this object uses the gravity set below. Otherwise, the one set in Physics is used."), sp_ERigidUseCustomGravity.boolValue);
        if (sp_ERigidCustomGravity != null)
        {
          bool guiEnabled = GUI.enabled;
          if (guiEnabled)
          {
            if (!sp_ERigidUseCustomGravity.boolValue)
              GUI.enabled = false;
          }
          EditorGUILayout.PropertyField(sp_ERigidCustomGravity, new GUIContent("Custom Gravity", "The custom gravity of this object, if the above bool is checked."));
          GUI.enabled = guiEnabled;

          EC_Rigidbody rigid = serializedObject.targetObject as EC_Rigidbody;
          if (rigid.GetCalculatedTimeScale() == 0)
            EditorGUILayout.LabelField("TIMESCALE 0: FREEZING ALL CONSTRAINTS", EditorStyles.boldLabel);
        }

        EditorGUI.indentLevel = startIndent;
      }
    }

    private void OnEnable()
    {
      Enable_Core();
      Enable_Component();
      Enable_Rigidbody();
    }

    public override void OnInspectorGUI()
    {
      Inspector_Core();
      Inspector_Component();
      Inspector_Rigidbody();
      serializedObject.ApplyModifiedProperties();
    }
  }
  /**********************************************************************************************************************/
#endif
}
