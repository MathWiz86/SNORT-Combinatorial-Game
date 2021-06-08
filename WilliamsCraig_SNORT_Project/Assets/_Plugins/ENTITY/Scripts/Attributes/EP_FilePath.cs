using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ENTITY
{
  /**********************************************************************************************************************/
  /// <summary>
  /// A PropertyAttribute for setting a string via selecting a folder.
  /// </summary>
  public class EP_FilePath : E_Attribute
  {
    /// <summary> The base current directory. If using this, the folder button will immediately open to this folder. The path can be relative to the project or exact. </summary>
    public string ParentFolder = System.IO.Directory.GetCurrentDirectory();
    /// <summary> If true, the string will only be the portion of the path after the parent folder. </summary>
    public bool RelativePath = false;
    /// <summary> If true, a warning will appear if the path provided does not exist. </summary>
    public bool AssertExistingPath = false;

    public string Extensions = "";
  }
  /**********************************************************************************************************************/

#if UNITY_EDITOR
  /**********************************************************************************************************************/
  /// <summary>
  /// The PropertyDrawer for EP_FolderPath. This handles drawing a button to open up a folder selection.
  /// </summary>
  [CustomPropertyDrawer(typeof(EP_FilePath))]
  public sealed class EPD_FilePath : E_PropertyDrawer
  {
    /// <summary> The default path to display if one is not provided by the user. </summary>
    private static string default_path = Directory.GetCurrentDirectory() + Path.AltDirectorySeparatorChar.ToString();
    /// <summary> If requested, the editor will show a warning if the path provided does not exist. This bool determines if the warning is on. </summary>
    private bool warning_shown = false;

    /// <summary>
    /// The static constructor for the attribute. This merely cleans up the default path.
    /// </summary>
    static EPD_FilePath()
    {
      EKIT_File.CleanupDirectoryPath(ref default_path); // Clean up the default path.
    }

    /// <summary>
    /// A helper function used to combine the parent folder provided by the player. This is used if the path is relative to the project.
    /// </summary>
    /// <param name="attribute">The FolderPath attribute being edited.</param>
    /// <returns>Returns the full folder path.</returns>
    private string CompletePath(EP_FilePath attribute)
    {
      string parent_folder = attribute.ParentFolder; // Get the parent folder.
      // If there is no ':', then the path is relative. We need to clean up the path.
      if (!parent_folder.Contains(":"))
      {
        EKIT_File.CleanupDirectoryPath(ref parent_folder); // Cleanup the parent folder provided.
        return Path.Combine(Directory.GetCurrentDirectory(), parent_folder); // Combine the paths and return it.
      }
      return parent_folder; // Return the full path. It is exact.
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // This will only work on String properties.
      if (property.propertyType == SerializedPropertyType.String)
      {
        EP_FilePath attribute = this.attribute as EP_FilePath; // Get the attribute being targeted.
        Rect default_position = position;
        EditorGUI.BeginProperty(position, label, property); // Begin the property.
        position.width -= 30; // Lower the width to make room for the button.
        position.height = 25; // Set the height to make room for the button.
        property.stringValue = EditorGUI.TextField(position, label, property.stringValue); // Create the base string field.
        position.x += position.width + 6; // Move the position a bit over.
        position.width = 25; // Set the width of the button.

        // Get the icon for the folder and create the Button.
        Texture folderIcon = (Texture)AssetDatabase.LoadAssetAtPath(ep_DefaultIconPath + "e_icon_folder.png", typeof(Texture));
        GUIContent folderGUI = new GUIContent(folderIcon, "Select File");
        string path = property.stringValue;
        // Check if the button was clicked.
        if (GUI.Button(position, folderGUI))
        {
          path = EditorUtility.OpenFilePanel("Select a File", CompletePath(attribute), attribute.Extensions); // Get the path from the folder panel.
          // If the path is valid, set the property's value as the path.
          if (path != null && path != string.Empty)
            property.stringValue = path;
          // If we are only using the relative path, remove the default path substring.
          if (attribute.RelativePath)
            property.stringValue = path.RemoveFirstSubstring(default_path);

          // Apply properties and exit the GUI, since this was a folder panel.
          property.serializedObject.ApplyModifiedProperties();
          GUIUtility.ExitGUI();
        }

        warning_shown = attribute.AssertExistingPath && !EKIT_File.CheckDirectory(ref path); // Check if the path exists, if we are asserting the path.
        // If the warning is shown, update the GUI height and display a help box.
        if (warning_shown)
        {
          default_position.height = 30;
          default_position.y += 30;
          EditorGUI.HelpBox(default_position, "This path does not exist!", MessageType.Warning);
        }

        EditorGUI.EndProperty();
      }
      else
        EditorGUI.PropertyField(position, property, label); // If not valid, just draw the property normally.

      property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      int extra_space = warning_shown ? 50 : 10; // If the warning is shown, extra space is increased.
      return base.GetPropertyHeight(property, label) + extra_space; // Increase the property height by the extra space.
    }
  }
  /**********************************************************************************************************************/
#endif
}
