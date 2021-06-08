/**************************************************************************************************/
/*!
\file   EKIT_File.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for file management. This file includes validating files and cleaning paths.

\par References:
   - https://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
   - https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-2000-server/cc938934(v=technet.10)?redirectedfrom=MSDN
*/
/**************************************************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_File
  {
    /// <summary> An array of reserved words which cannnot be used in directories or file names. These are removed from paths upon cleanup </summary>
    public static readonly string[] file_ReservedWords = {"CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2",
                                                          "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"};

    /// <summary>
    /// A function which takes a full filepath and attempts to break it up into the directory and filename.
    /// The filename will include its extension.
    /// </summary>
    /// <param name="filepath">The filepath to break up.</param>
    /// <param name="directory">The directory of the file path.</param>
    /// <param name="filename">The filename of the file path. Includes the extension.</param>
    public static void BreakupFilePath(string filepath, out string directory, out string filename)
    {
      // Initialize the strings.
      directory = string.Empty;
      filename = string.Empty;

      if (filepath == null)
        return;

      // Replace all slashes to use just a standard across all file platforms.
      filepath = filepath.Replace('/', Path.AltDirectorySeparatorChar);
      filepath = filepath.Replace('\\', Path.AltDirectorySeparatorChar);

      // If there is a slash at all, then there is a directory to separate.
      if (filepath.LastIndexOf(Path.AltDirectorySeparatorChar) >= 0)
      {
        directory = filepath.Substring(0, filepath.LastIndexOf(Path.AltDirectorySeparatorChar) + 1); // Break out the directory based on the last separator.
        filename = filepath.Substring(filepath.LastIndexOf(Path.AltDirectorySeparatorChar) + 1); // Break out the filename based on the last separator.
      }
      else
        filename = filepath; // Otherwise, assume the path is just a file name and extension.
    }

    /// <summary>
    /// A function which cleans up a directory path. Make sure it doesn't have a filename in it!.
    /// </summary>
    /// <param name="directory">The directory path to clean.</param>
    public static void CleanupDirectoryPath(ref string directory)
    {
      if (directory == null)
      {
        directory = string.Empty;
        return;
      }

      // Create an array of illegal characters, and format it into a REGEX. In a directory, ':', '/', and '\' are all valid characters.
      string illegalPath = Regex.Escape(new string((Path.GetInvalidFileNameChars().Where(c => c != ':' && c != '/' && c != '\\')).ToArray()));
      string illegalRegex = string.Format(@"[{0}]+", illegalPath);

      // Replace any slashes with the proper ones.
      directory = directory.Replace('/', Path.AltDirectorySeparatorChar);
      directory = directory.Replace('\\', Path.AltDirectorySeparatorChar);

      // Check if there is any colon at all. Please note that due to things like Volume Mount Points, the ':' is not necessarily at Index 1.
      if (directory.IndexOf(':') >= 0)
      {
        // Create a left and right substring. Only the right string needs to remove any colons.
        string left = directory.Substring(0, directory.IndexOf(':') + 1);
        string right = directory.Substring(directory.IndexOf(':') + 1);

        left = Regex.Replace(left, illegalRegex, ""); // Clean up the left side.
        right = Regex.Replace(right, illegalRegex, ""); // Clean up the right side.
        right = right.Replace(':'.ToString(), ""); // Remove additional colons from the right side.

        // Replace any reserved words.
        foreach(string word in file_ReservedWords)
        {
          string reservedRegex = string.Format("^{0}(\\.|$)", word);
          left = Regex.Replace(left, reservedRegex, "");
          right = Regex.Replace(right, reservedRegex, "");
        }

        // Stich the directory path together and finish by removing any extra slashes.
        directory = left + right;
        directory = Path.Combine(directory);

        if (directory.Last() != Path.AltDirectorySeparatorChar)
          directory += Path.AltDirectorySeparatorChar;
      }
      else
      {
        directory = Regex.Replace(directory, illegalRegex, ""); // Simply replace any bad characters.

        // Replace any reserved words.
        foreach (string word in file_ReservedWords)
        {
          string reservedRegex = string.Format("^{0}(\\.|$)", word);
          directory = Regex.Replace(directory, reservedRegex, "");
        }

        // Remove any extra slashes.
        directory = Path.Combine(directory);
      }
    }

    /// <summary>
    /// A function which cleans up a filename, plus extension. Make sure it doesn't have the directory in it!
    /// </summary>
    /// <param name="filename">The filename to clean.</param>
    public static void CleanupFileName(ref string filename)
    {
      if (filename == null)
      {
        filename = string.Empty;
        return;
      }

      // Create an array of illegal characters, and format it into a REGEX.
      string illegalPath = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
      string illegalRegex = string.Format(@"[{0}]+", illegalPath);

      filename = Regex.Replace(filename, illegalRegex, ""); // Simply replace any bad characters.

      // Replace any reserved words.
      foreach (string word in file_ReservedWords)
      {
        string reservedRegex = string.Format("^{0}(\\.|$)", word);
        filename = Regex.Replace(filename, reservedRegex, "$1");
      }
    }

    /// <summary>
    /// A function which cleans up a full file path, removing illegal and redundant characters.
    /// </summary>
    /// <param name="directory">The directory to clean up.</param>
    /// <param name="filename">The filename to clean up.</param>
    public static void CleanupFilePath(ref string directory, ref string filename)
    {
      CleanupDirectoryPath(ref directory); // Clean the directory.
      CleanupFileName(ref filename); // Clean the filename.

      string path = CreateFilePath(new string[] { directory, filename }); // Piece the two together.
      BreakupFilePath(path, out directory, out filename); // Break up the filepath again, now that a correct path has been made.
    }

    /// <summary>
    /// A function which cleans up a full file path, removing illegal and redundant characters.
    /// </summary>
    /// <param name="filepath">The filepath to clean up.</param>
    public static void CleanupFilePath(ref string filepath)
    {
      BreakupFilePath(filepath, out string directory, out string filename); // Break up the path.

      CleanupDirectoryPath(ref directory); // Clean the directory.
      CleanupFileName(ref filename); // Clean the filename.

      filepath = CreateFilePath(new string[] { directory, filename }); // Concatenate the path once more.
    }

    /// <summary>
    /// A function which cleans up a full file path, removing illegal and redundant characters.
    /// </summary>
    /// <param name="file">The file to edit and clean up.</param>
    public static void CleanupFilePath(ET_File file)
    {
      if (file != null)
        CleanupFilePath(ref file.directory, ref file.filename); // Clean up the file's directory and filename.
    }

    /// <summary>
    /// The internal function for attempting to check if a directory exists. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="directory">The path of the directory.</param>
    /// <param name="create_if_null">A bool determining if the directory should be created if it doesn't exist.</param>
    /// <returns>Returns if the directory exists or not.</returns>
    private static bool InternalCheckDirectory(ref string directory, bool create_if_null)
    {
      try
      {
        if (Directory.Exists(directory)) // If the directory exists, simply return true.
          return true;
        else if (create_if_null) // If it doesn't exist, but we can create it, return if the directory was created.
          return CreateDirectory(ref directory);
      }
      catch
      {
        // If any error happens, simply return false.
        return false;
      }

      return false; // Otherwise, the directory does not exist.
    }

    /// <summary>
    /// A function which checks if a directory exists.
    /// </summary>
    /// <param name="directory">The path of the directory.</param>
    /// <param name="create_if_null">A bool determining if the directory should be created if it doesn't exist.</param>
    /// <param name="cleanup">A bool determining if the directory string should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the directory exists or not.</returns>
    public static bool CheckDirectory(ref string directory, bool create_if_null = false, bool cleanup = false)
    {
      // Clean up the directory if necessary.
      if (cleanup)
        CleanupDirectoryPath(ref directory);

      return InternalCheckDirectory(ref directory, create_if_null);
    }

    /// <summary>
    /// A function which checks if a directory exists.
    /// </summary>
    /// <param name="file">The file to get the directory from.</param>
    /// <param name="create_if_null">A bool determining if the directory should be created if it doesn't exist.</param>
    /// <param name="cleanup">A bool determining if the entire file should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the directory exists or not.</returns>
    public static bool CheckDirectory(ET_File file, bool create_if_null = false, bool cleanup = false)
    {
      if (file == null)
        return false;

      // Cleanup the entire file.
      if (cleanup)
        CleanupFilePath(file);

      return CheckDirectory(ref file.directory, create_if_null, false); // Check the directory. We no longer need to clean.
    }

    /// <summary>
    /// The internal function for attempting to check if a file exists. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The full filepath to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it doesn't exist.</param>
    /// <returns>Returns if the file exists or not.</returns>
    private static bool InternalCheckFile(ref string filepath, bool create_if_null)
    {
      try
      {
        if (File.Exists(filepath)) // If the file exists, simply return true.
          return true;
        else if (create_if_null) // If it doesn't exist, but we can create it, return if the file was created.
          return CreateFile(ref filepath, true, true, false);
      }
      catch
      {
        // If any error happens, just return false.
        return false;
      }

      return false; // Otherwise, the file does not exist.
    }

    /// <summary>
    /// A function which checks if a file exists.
    /// </summary>
    /// <param name="filepath">The full filepath to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it doesn't exist.</param>
    /// <param name="cleanup">A bool determining if the filepath should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the file exists or not.</returns>
    public static bool CheckFile(ref string filepath, bool create_if_null = false, bool cleanup = false)
    {
      // If cleaning, clean up the whole file path.
      if (cleanup)
        CleanupFilePath(ref filepath);

      return InternalCheckFile(ref filepath, create_if_null);
    }

    /// <summary>
    /// A function which checks if a file exists.
    /// </summary>
    /// <param name="directory">The directory of the file to check.</param>
    /// <param name="filename">The name of the file to check.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it doesn't exist.</param>
    /// <param name="cleanup">A bool determining if the directory and filename should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the file exists or not.</returns>
    public static bool CheckFile(ref string directory, ref string filename, bool create_if_null = false, bool cleanup = false)
    {
      // If cleaning, clean up the whole file path.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      string path = CreateFilePath(new string[] { directory, filename }); // Create a placeholder path with everything combined.

      return InternalCheckFile(ref path, create_if_null); // Check the file.
    }

    /// <summary>
    /// A function which checks if a file exists.
    /// </summary>
    /// <param name="file">The file to check the internal file of.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it doesn't exist.</param>
    /// <param name="cleanup">A bool determining if the filepath should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the file exists or not.</returns>
    public static bool CheckFile(ET_File file, bool create_if_null = false, bool cleanup = false)
    {
      // Get the directory and filename, and check the file.
      return file != null ? CheckFile(ref file.directory, ref file.filename, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// A function which returns all the filepaths in a given directory. This does not return the names of sub-directories.
    /// </summary>
    /// <param name="directory">The directory to look into.</param>
    /// <param name="filepaths">The filepaths found in the directory. In the case of an error, a null array will be returned.</param>
    /// <param name="cleanup">A bool determining if the directory should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the files were successfully found or not.</returns>
    public static bool GetFileList(ref string directory, out string[] filepaths, bool cleanup = false)
    {
      filepaths = null; // Initialize the array.

      try
      {
        // Check if the directory exists.
        if (CheckDirectory(ref directory, false, cleanup))
        {
          filepaths = Directory.GetFiles(directory); // Get all the files.
          return filepaths != null; // Return if there were any fils.
        }

        return false; // No files were found.
      }
      catch
      {
        // In the event of an error, return false.
        filepaths = null;
        return false;
      }
    }

    /// <summary>
    /// A function which returns all the filepaths in a given directory. This does not return the names of sub-directories.
    /// </summary>
    /// <param name="directory">The directory to look into.</param>
    /// <param name="files">The files found in the directory. In the case of an error, a null array will be returned.</param>
    /// <param name="cleanup">A bool determining if the directory should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the files were successfully found or not.</returns>
    public static bool GetFileList(ref string directory, out ET_File[] files, bool cleanup = false)
    {
      files = null; // Initialize the array.

      // Attempt to get the file paths.
      if (GetFileList(ref directory, out string[] filepaths, cleanup))
      {
        files = new ET_File[filepaths.Length]; // Create a new array.

        // Create new ET_Files for each path.
        for (int i = 0; i < filepaths.Length; i++)
          files[i] = new ET_File(filepaths[i], cleanup);

        return true; // The files were found.
      }

      return false; // The files were not found.
    }

    /// <summary>
    /// A function which returns all the subdirectories in a given directory.
    /// </summary>
    /// <param name="directory">The directory to look into.</param>
    /// <param name="subdirectories">The subdirectories found in the directory. In the case of an error, a null array will be returned.</param>
    /// <param name="cleanup">A bool determining if the directory should be cleaned up. It is recommended to do so if you haven't previously.</param>
    /// <returns>Returns if the subdirectories were found or not.</returns>
    public static bool GetDirectoryList(ref string directory, out string[] subdirectories, bool cleanup = false)
    {
      subdirectories = null; // Initialize the array.

      try
      {
        // Check if the directory exists.
        if (CheckDirectory(ref directory, false, cleanup))
        {
          subdirectories = Directory.GetDirectories(directory); // Get all the files.
          return subdirectories != null; // Return if there were any fils.
        }

        return false; // No files were found.
      }
      catch
      {
        // In the event of an error, return false.
        subdirectories = null;
        return false;
      }
    }
  }
  /**********************************************************************************************************************/
}