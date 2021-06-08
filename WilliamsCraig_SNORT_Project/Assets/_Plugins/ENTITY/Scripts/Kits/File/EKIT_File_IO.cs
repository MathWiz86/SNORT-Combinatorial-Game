/**************************************************************************************************/
/*!
\file   EKIT_File_IO.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for file management. This contains functionality for file input and output, as well
  as editing text files.

\par References:
  - https://docs.microsoft.com/en-us/dotnet/api/system.io.file.open?view=netcore-3.1
  - https://docs.microsoft.com/en-us/dotnet/api/system.io.file.create?view=netcore-3.1
  - https://stackoverflow.com/questions/1262965/how-do-i-read-a-specified-line-in-a-text-file
  - https://stackoverflow.com/questions/605685/how-to-both-read-and-write-a-file-in-c-sharp
  - https://stackoverflow.com/questions/1971008/edit-a-specific-line-of-a-text-file-in-c-sharp
*/
/**************************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Collections;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_File
  {
    /// <summary> A shortcut variable to the game's current directory. </summary>
    public static string file_CurrentDirectory { get { return System.IO.Directory.GetCurrentDirectory(); } }
    /// <summary> The maximum line that can be accesed. This is used to prevent accidentally creating extremely large files. Change with caution. </summary>
    private const long file_MaxLineAccess = 1073742000;
    /// <summary> The maximum times a standard loop can occur in a routine before yielding. </summary>
    private const int file_MaxLoopCount = 1000;
    /// <summary> The maximum times a line can be accessed in a file in a routine before yielding. </summary>
    private const int file_MaxAccessLoopCount = 10000;
    /// <summary> The maximum times a byte can be written to a file in a frame before yielding. Change this at your own risk. </summary>
    private const int file_MaxWriteLoopCount = 20000;

    /// <summary>
    /// A struct containing information used for manipulating text files. Use these when using any of ENTITY's file management functions.
    /// </summary>
    public struct ES_FileLine
    {
      /// <summary> The message for this line. </summary>
      public string line { get; private set; }
      /// <summary> The index of the line. This can never be less than 0. </summary>
      public long index { get; private set; }

      /// <summary>
      /// The constructor for a File Line.
      /// </summary>
      /// <param name="l">The string of this line. This is used for line insertion.</param>
      /// <param name="i">The number of this line. This is used for line insertion and reading.</param>
      public ES_FileLine(string l = "", long i = 0)
      {
        line = l != null ? l : string.Empty; // Enter either the string, or an empty string if none is provided.

        index = EKIT_Math.NoLessThan(i, 0); // Enter the index, which cannot be less than 0.
      }

      /// <summary>
      /// A function which will set the text of this line.
      /// </summary>
      /// <param name="l">The new text of this line. If this is null, an empty string will be used.</param>
      public void SetLine(string l)
      {
        line = l != null ? l : string.Empty; // Update the line. If it is null, use an empty string instead.
      }

      /// <summary>
      /// A function which will set the index of this line.
      /// </summary>
      /// <param name="i">The new index. If this is less than 0, it will be set to 0.</param>
      public void SetIndex(long i)
      {
        index = EKIT_Math.NoLessThan(i, 0); // Update the index. The index can never be less than 0.
      }
    }

    //private static int EKIT_Sort.CompareFileLinestLeastToGreatest(ES_FileLine a, ES_FileLine b)

    /// <summary>
    /// A function which combines several pieces of a full filepath together. If any pieces are illegal, nothing is returned.
    /// </summary>
    /// <param name="paths">The paths to combine together.</param>
    /// <returns>Returns the fixed up and combined path. If any pieces are illegal, nothing is returned.</returns>
    public static string CreateFilePath(string[] paths)
    {
      try
      {
        // Combine all the paths together and return it.
        return Path.Combine(paths);
      }
      catch
      {
        // If the paths are illegal, return the empty string.
        return string.Empty;
      }
    }

    /// <summary>
    /// A function which combines file information into a full filepath. If any pieces are illegal, nothing is returned.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <returns>Returns the fixed up and combined path. If any pieces are illegal, nothing is returned.</returns>
    public static string CreateFilePath(ET_File file)
    {
      // Create the file path with the given information.
      return file != null ? CreateFilePath(new string[] { file.directory, file.filename }) : string.Empty;
    }

    /// <summary>
    /// A function which attempts to create a temporary file.
    /// </summary>
    /// <param name="temp_path">The temporary file path. If an error occurs, this will be empty.</param>
    /// <returns>Returns if the temporary file was successfully created or not.</returns>
    public static bool CreateTempFilePath(out string temp_path)
    {
      try
      {
        // Attempt to get a temporary file path. One cannot be made if therea re already too many temp files.
        temp_path = Path.GetTempFileName();
        return true;
      }
      catch
      {
        // Upon any error, simply return an empty string.
        temp_path = string.Empty;
        return false;
      }
    }

    /// <summary>
    /// The internal function for attempting to create a directory. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="directory">The path of the directory to create. All subdirectories that don't exist are created as well.</param>
    /// <returns>Returns if the directory was successfully created or not.</returns>
    private static bool InternalCreateDirectory(ref string directory)
    {
      try
      {
        // Attempt to create the directory, and return if it was successfully created.
        DirectoryInfo info = Directory.CreateDirectory(directory);
        return info.Exists;
      }
      catch
      {
        // If an error occured, return false.
        return false;
      }
    }

    /// <summary>
    /// A function which will create a directory at the specified path.
    /// </summary>
    /// <param name="directory">The path of the directory to create. All subdirectories that don't exist are created as well.</param>
    /// <param name="cleanup">A bool determining if the path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the directory was successfully created or not.</returns>
    public static bool CreateDirectory(ref string directory, bool cleanup = false)
    {
      // Return false if there is no path.
      if (directory == string.Empty)
        return false;
      // Cleanup the path if requested.
      if (cleanup)
        CleanupDirectoryPath(ref directory);
      // Attempt to create the directory.
      return InternalCreateDirectory(ref directory);
    }

    /// <summary>
    /// A function which will create a directory at the specified path.
    /// </summary>
    /// <param name="file">The file to get the directory from.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the directory was successfully created or not.</returns>
    public static bool CreateDirectory(ET_File file, bool cleanup = false)
    {
      // Attempt to create the directory.
      return file != null ? CreateDirectory(ref file.directory, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to create a file at the specified path. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="directory">The directory of the file to create. This is used separately from the filepath.</param>
    /// <param name="filepath">The full path to the file.</param>
    /// <param name="create_directory">A bool determining if the directory should be created if it does not already exist.</param>
    /// <param name="overwrite">A bool determining if the file should be overwritten if it already exists.</param>
    /// <returns>Returns if the file was successfully created or not.</returns>
    private static bool InternalCreateFile(ref string directory, ref string filepath, bool create_directory, bool overwrite)
    {
      // If the directory does not exist, even after possibly creating it, return false immediately.
      if (!InternalCheckDirectory(ref directory, create_directory))
        return false;

      try
      {
        // If the file doesn't exist, or overwriting it, create the file.
        if (!File.Exists(filepath) || overwrite)
        {
          File.Create(filepath).Close(); // Close the file stream. We don't need it.
          return true; // The file was made.
        }
      }
      catch
      {
        // If any error occurs, return false.
        return false;
      }

      return false; // The file was not created.
    }

    /// <summary>
    /// A function which will create a file at the specified path.
    /// </summary>
    /// <param name="filepath">The full path to the file.</param>
    /// <param name="create_directory">A bool determining if the directory should be created if it does not already exist.</param>
    /// <param name="overwrite">A bool determining if the file should be overwritten if it already exists.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully created or not.</returns>
    public static bool CreateFile(ref string filepath, bool create_directory = false, bool overwrite = false, bool cleanup = false)
    {
      // If there is no file path, return false.
      if (filepath == string.Empty)
        return false;
      // Cleanup the file path if requested.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Break up the file path to get the directory.
      BreakupFilePath(filepath, out string dir, out string fi);
      // Attempt to create the file.
      return InternalCreateFile(ref dir, ref filepath, create_directory, overwrite);
    }

    /// <summary>
    /// A function which will create a file at the specified path.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file to create, extension included.</param>
    /// <param name="create_directory">A bool determining if the directory should be created if it does not already exist.</param>
    /// <param name="overwrite">A bool determining if the file should be overwritten if it already exists.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully created or not.</returns>
    public static bool CreateFile(ref string directory, ref string filename, bool create_directory = false, bool overwrite = false, bool cleanup = false)
    {
      // Cleanup the file path if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      try
      {
        // Concatenate the path together and attempt to create the file.
        string path = Path.Combine(directory, filename);
        return InternalCreateFile(ref directory, ref path, create_directory, overwrite);
      }
      catch
      {
        // If any error occurs, return false.
        return false;
      }
    }

    /// <summary>
    /// A function which will create a file at the specified path.
    /// </summary>
    /// <param name="file">The file information to use to create the file.</param>
    /// <param name="create_directory">A bool determining if the directory should be created if it does not already exist.</param>
    /// <param name="overwrite">A bool determining if the file should be overwritten if it already exists.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully created or not.</returns>
    public static bool CreateFile(ET_File file, bool create_directory = false, bool overwrite = false, bool cleanup = false)
    {
      // Attempt to create the file.
      return file != null ? CreateFile(ref file.directory, ref file.filename, create_directory, overwrite, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to delete a file at the specified path. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The path of the file to delete.</param>
    /// <returns>Returns if the file was successfully deleted or not.</returns>
    private static bool InternalDeleteFile(ref string filepath)
    {
      try
      {
        // If the file exists, delete it and return true.
        if (File.Exists(filepath))
        {
          File.Delete(filepath);
          return true;
        }
        return false; // The file does not exist.
      }
      catch
      {
        // If an error occurs, return false.
        return false;
      }
    }

    /// <summary>
    /// A function which will attempt to delete a file at the specified path.
    /// </summary>
    /// <param name="filepath">The path of the file to delete.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned up. Do so if you haven't in the past.</param>
    /// <returns>Returns if the file was successfully deleted or not.</returns>
    public static bool DeleteFile(ref string filepath, bool cleanup = false)
    {
      // Cleanup the filepath if requested.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to delete the file.
      return InternalDeleteFile(ref filepath);
    }

    /// <summary>
    /// A function which will attempt to delete a file at the specified path.
    /// </summary>
    /// <param name="directory">The directory of the file to delete.</param>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned up. Do so if you haven't in the past.</param>
    /// <returns>Returns if the file was successfully deleted or not.</returns>
    public static bool DeleteFile(ref string directory, ref string filename, bool cleanup = false)
    {
      // Cleanup the filepath if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Create a path and attempt to delete the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return DeleteFile(ref path, false);
    }

    /// <summary>
    /// A function which will attempt to delete a file at the specified path.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned up. Do so if you haven't in the past.</param>
    /// <returns>Returns if the file was successfully deleted or not.</returns>
    public static bool DeleteFile(ET_File file, bool cleanup = false)
    {
      // Attempt to delete the file.
      return file != null ? DeleteFile(ref file.directory, ref file.filename, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to get a file's size, in bytes. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    private static bool InternalGetFileSize(ref string filepath, out long size)
    {
      size = -1; // Initialize the size to -1.

      try
      {
        // Check if the file exists.
        if (InternalCheckFile(ref filepath, false))
        {
          // Create a file stream to get the size.
          using (FileStream filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
          {
            // Get the file's size and return true.
            size = filestream.Length;
            return true;
          }
        }
        return false; // The file does not exist.
      }
      catch
      {
        // Upon any error, return false.
        return false;
      }
    }

    /// <summary>
    /// A function which gets a file's size, in bytes.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileSize(ref string filepath, out long size, bool cleanup = false)
    {
      // Cleanup the path, if requested.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to get the file's size.
      return InternalGetFileSize(ref filepath, out size);
    }

    /// <summary>
    /// A function which gets a file's size, in bytes.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileSize(ref string directory, ref string filename, out long size, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to get the file size.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalGetFileSize(ref path, out size);
    }

    /// <summary>
    /// A function which gets a file's size, in bytes.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileSize(ET_File file, out long size, bool cleanup = false)
    {
      size = -1; // Initialize the size.

      // Attempt to get the file size.
      return file != null ? GetFileSize(ref file.directory, ref file.filename, out size, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to get a file's size and converts it into the requested size type.
    /// Note that all size conversions use the 1024 byte standard. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <param name="conversion">The conversion type for the size.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    private static bool InternalGetFileSize(ref string filepath, out double size, EKIT_Conversion.EE_ByteConversion conversion)
    {
      size = -1; // Initialize the size.
      // Attempt to get the byte size. If we can, convert to the requested size type and return true.
      if (InternalGetFileSize(ref filepath, out long bytes))
      {
        size = EKIT_Conversion.ConvertFileSize(bytes, EKIT_Conversion.EE_ByteConversion.Byte, conversion);
        return true;
      }
      // The size could not be obtained. Return false.
      return false;
    }

    /// <summary>
    /// A function which gets a file's size and converts it into the requested size type.
    /// Note that all size conversions use the 1024 byte standard.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <param name="conversion">The conversion type for the size.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileSize(ref string filepath, out double size, EKIT_Conversion.EE_ByteConversion conversion, bool cleanup = false)
    {
      // If cleaning up, clean up the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to get the size and convert to the wanted type.
      return InternalGetFileSize(ref filepath, out size, conversion);
    }

    /// <summary>
    /// A function which gets a file's size and converts it into the requested size type.
    /// Note that all size conversions use the 1024 byte standard.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <param name="conversion">The conversion type for the size.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileSize(ref string directory, ref string filename, out double size, EKIT_Conversion.EE_ByteConversion conversion, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to get the file size.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalGetFileSize(ref path, out size, conversion);
    }

    /// <summary>
    /// A function which gets a file's size and converts it into the requested size type.
    /// Note that all size conversions use the 1024 byte standard.
    /// </summary>
    /// <param name="file">The file path to go to.</param>
    /// <param name="size">The size of the file, in bytes. In the case of an error, this will return -1.</param>
    /// <param name="conversion">The conversion type for the size.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileSize(ET_File file, out double size, EKIT_Conversion.EE_ByteConversion conversion, bool cleanup = false)
    {
      size = -1.0; // Initialize the size.

      // Attempt to get the file size.
      return file != null ? GetFileSize(ref file.directory, ref file.filename, out size, conversion, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to get the number of lines stored in a file. A line is separated by a newline character. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="line_count">The number of lines in the file. An invalid file returns a -1.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    private static bool InternalGetFileLineCount(ref string filepath, out long line_count)
    {
      line_count = 0; // Initialize the size.
      // Check if the file exists.
      if (InternalCheckFile(ref filepath, false))
      {
        try
        {
          // Attempt to read every line one at a time. Once out of lines, we have the size.
          using (StreamReader stream_reader = new StreamReader(filepath))
          {
            while (stream_reader.ReadLine() != null)
              line_count++;
          }
          return true; // The file was successfully read.
        }
        catch
        {
          // In the event of an error, reset the count to -1 and return false.
          line_count = -1;
          return false;
        }
      }
      // The file does not exist. Set the count to -1 and return false.
      line_count = -1;
      return false;
    }

    /// <summary>
    /// A function which gets the number of lines stored in a file. A line is separated by a newline character.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="line_count">The number of lines in the file. An invalid file returns a -1.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileLineCount(ref string filepath, out long line_count, bool cleanup = false)
    {
      // If cleaning up, cleanup the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Get the line count of the file.
      return InternalGetFileLineCount(ref filepath, out line_count);
    }

    /// <summary>
    /// A function which gets the number of lines stored in a file. A line is separated by a newline character.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="line_count">The number of lines in the file. An invalid file returns a -1.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileLineCount(ref string directory, ref string filename, out long line_count, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to get the number of lines.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalGetFileLineCount(ref path, out line_count);
    }

    /// <summary>
    /// A function which gets the number of lines stored in a file. A line is separated by a newline character.
    /// </summary>
    /// <param name="file">The file path to go to.</param>
    /// <param name="line_count">The number of lines in the file. An invalid file returns a -1.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file exists, and was successfully accessed.</returns>
    public static bool GetFileLineCount(ET_File file, out long line_count, bool cleanup = false)
    {
      line_count = -1; // Initialize the size.
      // If the file information is valid, return the number of lines.
      return file != null ? GetFileLineCount(ref file.directory, ref file.filename, out line_count, cleanup) : false;
    }

    public static bool OpenFile(ref string filepath, out StreamReader stream_reader)
    {
      stream_reader = null;

      if (InternalCheckFile(ref filepath, false))
      {
        try
        {
          stream_reader = new StreamReader(filepath);
          return stream_reader != null;
        }
        catch
        {
          return false;
        }
        
      }

      return false;
    }

    /// <summary>
    /// The internal function for attempting to copy a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath_from">The filepath of the file that is being copied.</param>
    /// <param name="filepath_to">The filepath that the file will be copied to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <returns>Returns if the file was successfully copied over.</returns>
    private static bool InternalCopyFile(ref string filepath_from, ref string filepath_to, bool overwrite)
    {
      // Check if the original file already exists.
      if (InternalCheckFile(ref filepath_from, false))
      {
        try
        {
          // Attempt to copy the file.
          File.Copy(filepath_from, filepath_to, overwrite);
          return true;
        }
        catch
        {
          // In the event of an error, return false.
          return false;
        }
      }
      // The original file does not exist. Return false.
      return false;
    }

    /// <summary>
    /// A function which copies a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// </summary>
    /// <param name="filepath_from">The filepath of the file that is being copied.</param>
    /// <param name="filepath_to">The filepath that the file will be copied to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully copied over.</returns>
    public static bool CopyFile(ref string filepath_from, ref string filepath_to, bool overwrite = false, bool cleanup = false)
    {
      // If cleaning up, clean up the two file paths.
      if (cleanup)
      {
        CleanupFilePath(ref filepath_from);
        CleanupFilePath(ref filepath_to);
      }
      // Attempt to copy the file over.
      return InternalCopyFile(ref filepath_from, ref filepath_to, overwrite);
    }

    /// <summary>
    /// A function which copies a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// </summary>
    /// <param name="directory_from">The directory of the file that is being copied.</param>
    /// <param name="filename_from">The filename of the file that is being copied, extension included.</param>
    /// <param name="directory_to">The directory that the file will be copied to.</param>
    /// <param name="filename_to">The filename that the file will be copied to, extension included.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully copied over.</returns>
    public static bool CopyFile(ref string directory_from, ref string filename_from, ref string directory_to, ref string filename_to, bool overwrite = false, bool cleanup = false)
    {
      // If cleaning up, clean up the file paths.
      if (cleanup)
      {
        CleanupFilePath(ref directory_from, ref filename_from);
        CleanupFilePath(ref directory_to, ref filename_to);
      }
      // Create the full filepaths for the two locations.
      string filepath_from = CreateFilePath(new string[] { directory_from, filename_from });
      string filepath_to = CreateFilePath(new string[] { directory_to, filename_to });
      // Attempt to copy the file.
      return InternalCopyFile(ref filepath_from, ref filepath_to, overwrite);
    }

    /// <summary>
    /// A function which copies a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// </summary>
    /// <param name="file_from">The file information of the file that is being copied.</param>
    /// <param name="file_to">The file information that the file will be copied to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully copied over.</returns>
    public static bool CopyFile(ET_File file_from, ET_File file_to, bool overwrite = false, bool cleanup = false)
    {
      // If both file information are valid, attempt to copy the file over.
      if (file_from != null && file_to != null)
        return CopyFile(ref file_from.directory, ref file_from.filename, ref file_to.directory, ref file_to.filename, overwrite, cleanup);

      return false; // Something was null, so return false.
    }

    /// <summary>
    /// The internal function for attempting to copy a file from one filepath to another via a routine. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="file_from">The file information of the file that is being copied.</param>
    /// <param name="file_to">The file information that the file will be copied to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <returns>Returns an IEnumerator representing the function. If using an ET_Routine, returns a bool determining if the file was successfully copied.</returns>
    private static IEnumerator InternalCopyFileAsync(ET_File file_from, ET_File file_to, bool overwrite)
    {
      // Create the filepaths for the two files.
      string filepath_from = CreateFilePath(file_from);
      string filepath_to = CreateFilePath(file_to);

      // Check if the original file exists, and that the other file is freshly created.
      if (InternalCheckFile(ref filepath_from, false) && InternalCreateFile(ref file_to.directory, ref filepath_to, true, overwrite))
      {
        bool stream_made = true; // A bool determining if the file streams are correctly made.
        bool copy_good = true; // A bool determining if the copy is good.
        FileStream filestream_from = null; // The filestream for the 'From' file.

        // Attempt to create the filestream for the 'From' file. If unable to, yield false immediately.
        try
        {
          filestream_from = new FileStream(filepath_from, FileMode.Open, FileAccess.Read);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (filestream_from)
        {
          FileStream filestream_to = null; // The filestream for the 'To' file.

          // Attempt to create the filestream for the 'To' file. If unable to, yield false immediately.
          try
          {
            filestream_to = new FileStream(filepath_to, FileMode.Open, FileAccess.Write);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            filestream_from.Dispose();
            yield return false;
            yield break;
          }

          using (filestream_to)
          {
            long size = filestream_from.Length; // Get how many bytes are in the file.
            int loop_counter = 0; // The counter for appending bytes.
            for (long i = 0; i < size; i++)
            {
              // For every byte, write the byte from the original file to the new file.
              try
              {
                filestream_to.WriteByte((byte)filestream_from.ReadByte());
              }
              catch
              {
                copy_good = false; // If an error occurs, the copy is no longer good.
                break;
              }

              // If at the maximum loop count, yield the function.
              if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxWriteLoopCount))
                yield return null;
            }
          }
        }

        // If the copy wasn't good, delete the attempted copy file.
        if (!copy_good)
          InternalDeleteFile(ref filepath_to);

        yield return copy_good; // Return if the copy was made or not.
      }
      else
      {
        // If the original file does not exist, yield false.
        yield return false;
        yield break;
      }
    }

    /// <summary>
    /// A function which copies a file from one filepath to another via a routine.
    /// </summary>
    /// <param name="file_from">The file information of the file that is being copied.</param>
    /// <param name="file_to">The file information that the file will be copied to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator representing the function. If using an ET_Routine, returns a bool determining if the file was successfully copied.</returns>
    public static IEnumerator CopyFileAsync(ET_File file_from, ET_File file_to, bool overwrite = false, bool cleanup = false)
    {
      // If either of the file information are null, yield false immediately.
      if (file_from == null || file_to == null)
      {
        yield return false;
        yield break;
      }

      // If cleaning up, clean up the files.
      if (cleanup)
      {
        CleanupFilePath(file_from);
        CleanupFilePath(file_to);
      }
      // Create an internal routine to copy the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalCopyFileAsync(file_from, file_to, overwrite));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to move a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath_from">The filepath of the file that is being moved.</param>
    /// <param name="filepath_to">The filepath that the file will be moved to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <returns>Returns if the file was successfully moved over.</returns>
    private static bool InternalMoveFile(ref string filepath_from, ref string filepath_to, bool overwrite)
    {
      // Check if the original file already exists.
      if (InternalCheckFile(ref filepath_from, false))
      {
        // If a file already exists at the original location, check if we can overwrite.
        if (InternalCheckFile(ref filepath_to, false))
        {
          // If overwriting, delete the old file. Else, return false immediately.
          if (overwrite)
            InternalDeleteFile(ref filepath_to);
          else
            return false;
        }

        try
        {
          // Attempt to move the file.
          File.Move(filepath_from, filepath_to);
          return true;
        }
        catch
        {
          // In the event of an error, return false.
          return false;
        }
      }
      // The original file does not exist. Return false.
      return false;
    }

    /// <summary>
    /// A function which moves a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// </summary>
    /// <param name="filepath_from">The filepath of the file that is being moved.</param>
    /// <param name="filepath_to">The filepath that the file will be moved to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully moved over.</returns>
    public static bool MoveFile(ref string filepath_from, ref string filepath_to, bool overwrite = false, bool cleanup = false)
    {
      // If cleaning up, clean up the file paths.
      if (cleanup)
      {
        CleanupFilePath(ref filepath_from);
        CleanupFilePath(ref filepath_to);
      }
      return InternalMoveFile(ref filepath_from, ref filepath_to, overwrite); // Attempt to move the file.
    }

    /// <summary>
    /// A function which moves a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// </summary>
    /// <param name="directory_from">The directory of the file that is being moved.</param>
    /// <param name="filename_from">The filename of the file that is being moved, extension included.</param>
    /// <param name="directory_to">The directory that the file will be moved to.</param>
    /// <param name="filename_to">The filename that the file will be moved to, extension included.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully moved over.</returns>
    public static bool MoveFile(ref string directory_from, ref string filename_from, ref string directory_to, ref string filename_to, bool overwrite = false, bool cleanup = false)
    {
      // If cleaning up, clean up the file paths.
      if (cleanup)
      {
        CleanupFilePath(ref directory_from, ref filename_from);
        CleanupFilePath(ref directory_to, ref filename_to);
      }
      // Create the full filepaths for the two locations.
      string filepath_from = CreateFilePath(new string[] { directory_from, filename_from });
      string filepath_to = CreateFilePath(new string[] { directory_to, filename_to });
      // Attempt to move the file.
      return InternalMoveFile(ref filepath_from, ref filepath_to, overwrite);
    }

    /// <summary>
    /// A function which moves a file from one filepath to another. This simply does some extra checks the standard function does not.
    /// </summary>
    /// <param name="file_from">The file information of the file that is being moved.</param>
    /// <param name="file_to">The file information that the file will be moved to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully moved over.</returns>
    public static bool MoveFile(ET_File file_from, ET_File file_to, bool overwrite = false, bool cleanup = false)
    {
      // If both file information are valid, attempt to move the file over.
      if (file_from != null && file_to != null)
        return MoveFile(ref file_from.directory, ref file_from.filename, ref file_to.directory, ref file_to.filename, overwrite, cleanup);

      return false; // Something was null, so return false.
    }

    /// <summary>
    /// The internal function for attempting to move a file from one filepath to another via a routine. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="file_from">The file information of the file that is being moved.</param>
    /// <param name="file_to">The file information that the file will be moved to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <returns>Returns an IEnumerator representing the function. If using an ET_Routine, returns a bool determining if the file was successfully moved.</returns>
    private static IEnumerator InternalMoveFileAsync(ET_File file_from, ET_File file_to, bool overwrite)
    {
      // Create the filepaths for the two files.
      string filepath_from = CreateFilePath(file_from);
      string filepath_to = CreateFilePath(file_to);

      // Check if the original file exists. Also check that we can create the new file, or overwrite it.
      if (InternalCheckFile(ref filepath_from, false) && InternalCreateFile(ref file_to.directory, ref filepath_to, true, overwrite))
      {
        bool stream_made = true; // A bool determining if the file streams are correctly made.
        bool copy_good = true; // A bool determining if the copy is good.
        FileStream filestream_from = null; // The filestream for the 'From' file.

        // Attempt to create the filestream for the 'From' file. If unable to, yield false immediately.
        try
        {
          filestream_from = new FileStream(filepath_from, FileMode.Open, FileAccess.Read);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (filestream_from)
        {
          FileStream filestream_to = null; // The filestream for the 'To' file.
          // Attempt to create the filestream for the 'To' file. If unable to, yield false immediately.
          try
          {
            filestream_to = new FileStream(filepath_to, FileMode.Open, FileAccess.Write);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            filestream_from.Dispose();
            yield return false;
            yield break;
          }

          using (filestream_to)
          {
            long size = filestream_from.Length; // Get how many bytes are in the file.
            int loop_counter = 0; // The counter for appending bytes.
            for (long i = 0; i < size; i++)
            {
              // For every byte, write the byte from the original file to the new file.
              try
              {
                filestream_to.WriteByte((byte)filestream_from.ReadByte());
              }
              catch
              {
                copy_good = false; // If an error occurs, the copy is no longer good.
                break;
              }

              // If at the maximum loop count, yield the function.
              if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxWriteLoopCount))
                yield return null;
            }
          }
        }

        // If the copy was good, delete the old file. Else, delete the bad copied file.
        if (!copy_good || !InternalDeleteFile(ref filepath_from))
          InternalDeleteFile(ref filepath_to);
        yield return copy_good;
      }
      else
      {
        // If the original file does not exist, yield false.
        yield return false;
        yield break;
      }
    }

    /// <summary>
    /// A function which moves a file from one filepath to another via a routine.
    /// </summary>
    /// <param name="file_from">The file information of the file that is being moved.</param>
    /// <param name="file_to">The file information that the file will be moved to.</param>
    /// <param name="overwrite">A bool determining that, if a file already exists at the 'to' filepath, it will be overwritten.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator representing the function. If using an ET_Routine, returns a bool determining if the file was successfully moved.</returns>
    public static IEnumerator MoveFileAsync(ET_File file_from, ET_File file_to, bool overwrite = false, bool cleanup = false)
    {
      // If either of the file information are null, yield false immediately.
      if (file_from == null || file_to == null)
      {
        yield return false;
        yield break;
      }
      // If cleaning up, clean up the files.
      if (cleanup)
      {
        CleanupFilePath(file_from);
        CleanupFilePath(file_to);
      }
      // Create an internal routine to move the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(file_from, file_to, overwrite));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to read byte data from a given file. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="bytes">The byte array to send the data to.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    private static bool InternalReadBytesFromFile(ref string filepath, out byte[] bytes, int offset = 0, int data_count = 0)
    {
      bytes = null;

      try
      {
        // Make sure the file exists.
        if (InternalCheckFile(ref filepath, false))
        {
          // Using a file stream, read the bytes from the file.
          using (FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
          {
            // The stream needs to be more than 0 length or less than the max amount of data a list can read.
            if (stream.Length <= 0 || stream.Length > int.MaxValue || stream.Length - offset <= 0)
              return false;

            if (data_count == 0) // If the data count is 0, it signifies to read the whole file.
              data_count = (int)stream.Length;
            else if (data_count + offset > stream.Length) // If the data count plus the offset is greater than the file size, default to what data can be read.
              data_count = (int)stream.Length - offset;

            // Read in the data.
            bytes = new byte[data_count];
            stream.Read(bytes, offset, bytes.Length);
          }
          return true; // The file was successfully read.
        }
      }
      catch
      {
        // If any error occurs, return false.
        bytes = null;
        return false;
      }

      return false; // The file was not read.
    }

    /// <summary>
    /// A function which reads byte data from a given file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="bytes">The byte array to send the data to.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    public static bool ReadBytesFromFile(ref string filepath, out byte[] bytes, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      bytes = null; // Initialize the bytes array to be null.

      // If either the path is null, or the offsest or the data count are less than 0, this is an invalid read. Return false.
      if (filepath == null || offset < 0 || data_count < 0)
        return false;
      // If cleaning up, clean up the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to read the bytes from the file.
      return InternalReadBytesFromFile(ref filepath, out bytes, offset, data_count);
    }

    /// <summary>
    /// A function which reads byte data from a given file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="bytes">The byte array to send the data to.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    public static bool ReadBytesFromFile(ref string directory, ref string filename, out byte[] bytes, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to read the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalReadBytesFromFile(ref path, out bytes, offset, data_count);
    }

    /// <summary>
    /// A function which reads byte data from a given file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The byte array to send the data to.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    public static bool ReadBytesFromFile(ET_File file, out byte[] bytes, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      bytes = null; // Initialize the data.

      // Attempt to read the file.
      return file != null ? ReadBytesFromFile(ref file.directory, ref file.filename, out bytes, offset, data_count, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to read byte data from a given file via a routine. If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// All cleanup is handled in the public function.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The byte array to send the data to. i.e. (by => myBytes = by)</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    private static IEnumerator InternalReadBytesFromFileAsync(ET_File file, Action<byte[]> bytes, int offset = 0, int data_count = 0)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Check if the file exists.
      if (InternalCheckFile(ref filepath, false))
      {
        bool stream_made = true;
        FileStream file_stream = null; // Create a file stream and attemp to open the file.
        try
        {
          file_stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        }
        catch
        {
          // If the file cannot be opened, end and return false.
          bytes.Invoke(null);
          stream_made = false;
        }

        // If the stream was not made, return false.
        if (!stream_made)
        {
          yield return false;
          yield break;
        }
        // The stream needs to be more than 0 length or less than the max amount of data a list can read.
        if (file_stream.Length <= 0 || file_stream.Length > int.MaxValue || file_stream.Length - offset <= 0)
        {
          file_stream.Dispose();
          yield return false;
          yield break;
        }

        if (data_count == 0) // If the data count is 0, it signifies to read the whole file.
          data_count = (int)file_stream.Length;
        else if (data_count + offset > file_stream.Length) // If the data count plus the offset is greater than the file size, default to what data can be read.
          data_count = (int)file_stream.Length - offset;

        byte[] read_bytes = new byte[data_count]; // Make an array the size of the data count.

        using (file_stream)
        {
          bool read_valid = true; // A bool for making sure the try-catches are valid. Since this is a Coroutine, we have to separate every try-catch.
          int loop_counter = 0; // A loop counter for reading in bytes.

          // First, loop through the file's bytes up to the offset.
          for (int i = 0; i < offset; i++)
          {
            try
            {
              file_stream.ReadByte(); // Attempt to read a byte.
            }
            catch
            {
              // Upon an error, our read is bad.
              read_valid = false;
              break;
            }

            // Update the loop counter and check if we should yield.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
              yield return null;
          }

          // If no longer valid, return false.
          if (!read_valid)
          {
            bytes(null);
            file_stream.Dispose();
            yield return false;
            yield break;
          }

          loop_counter = 0; // Reset the loop counter.

          // Start reading in the wanted data.
          for (int i = 0; i < data_count; i++)
          {
            try
            {
              read_bytes[i] = (byte)file_stream.ReadByte(); // Attempt to read the bytes into the array.
            }
            catch
            {
              // Upon an error, our read is bad.
              read_valid = false;
              break;
            }

            // Update the loop counter and check if we should yield.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
              yield return null;
          }

          // If no longer valid, return false.
          if (!read_valid)
          {
            bytes(null);
            file_stream.Dispose();
            yield return false;
            yield break;
          }
          
        }

        // We are successful. Return the read bytes.
        bytes(read_bytes);
        yield return true;
        yield break;
      }
      else
      {
        bytes(null);
        yield return false;
      }
    }

    /// <summary>
    /// A function which reads byte data from a given file via a routine. If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The byte array to send the data to. i.e. (by => myBytes = by)</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    public static IEnumerator ReadBytesFromFileAsync(ET_File file, Action<byte[]> bytes, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      // If anything is null or incorrectly set, return false immediately.
      if (file == null || bytes == null || offset < 0 || data_count < 0)
      {
        bytes?.Invoke(null);
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to read the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalReadBytesFromFileAsync(file, bytes, offset, data_count));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A function which reads byte data from a given file via an ET_Routine. It is recommended to use the 'ReturnFirstAndBreak' return method.
    /// Please note that this version is built for ET_Routines, returning the bytes. Otherwise, use the alternate version which takes in an Action.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns the array of bytes representing the read data.</returns>
    public static IEnumerator ReadBytesFromFileAsync(ET_File file, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      byte[] bytes = null;
      ET_Routine routine = ET_Routine.CreateRoutine(ReadBytesFromFileAsync(file, by => bytes = by, offset, data_count, cleanup));
      yield return routine;
      yield return bytes;
    }

    /// <summary>
    /// The internal function for attempting to read byte data from a given file. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="obj">The object to send the data to.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    private static bool InternalReadBytesFromFile<T>(ref string filepath, out T obj, int offset = 0, int data_count = 0)
    {
      obj = default;
      // If we can read the bytes from the file, attempt to convert them into an object and return the object.
      if (InternalReadBytesFromFile(ref filepath, out byte[] bytes, offset, data_count))
        return EKIT_Conversion.ConvertFromSerializedBytes(bytes, out obj);

      return false; // The data could not be read.
    }

    /// <summary>
    /// A function which reads byte data from a given file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="obj">The object to send the data to.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    public static bool ReadBytesFromFile<T>(ref string filepath, out T obj, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      // If cleaning up, clean up the file path.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to read and convert the object.
      return InternalReadBytesFromFile(ref filepath, out obj, offset, data_count);
    }

    /// <summary>
    /// A function which reads byte data from a given file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="obj">The object to send the data to.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    public static bool ReadBytesFromFile<T>(ref string directory, ref string filename, out T obj, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to read the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalReadBytesFromFile(ref path, out obj, offset, data_count);
    }

    /// <summary>
    /// A function which reads byte data from a given file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The object to send the data to.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    public static bool ReadBytesFromFile<T>(ET_File file, out T obj, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      obj = default; // Initialize the object.

      // Attempt to read the file.
      return file != null ? ReadBytesFromFile(ref file.directory, ref file.filename, out obj, offset, data_count, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to read byte data from a given file via an ET_Routine, converting to an object. It is recommended to use the 'ReturnFirstAndBreak' return method.
    /// All cleanup is handled in the public function.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The Action to use to send the object. i.e. (ob => my_obj = ob)</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    private static IEnumerator InternalReadBytesFromFileAsync<T>(ET_File file, Action<T> obj, int offset = 0, int data_count = 0)
    {
      byte[] bytes = null; // The bytes to store.
      // Create a routine for reading the bytes and wait for it to finish.
      ET_Routine byte_routine = ET_Routine.CreateRoutine<bool>(InternalReadBytesFromFileAsync(file, by => bytes = by, offset, data_count));
      yield return byte_routine;
      Debug.Log(byte_routine.value);
      bool is_successful = false; // A bool determining if the conversion was successful.
      T byte_to_obj = default; // The object created from the byte array.

      // Since we don't know if the routine was used in a standard Coroutine or an ET_Routine, attempt the conversion regardless.
      is_successful = EKIT_Conversion.ConvertFromSerializedBytes(bytes, out byte_to_obj);

      // Send the object to the action and return if the conversion was successful.
      obj(byte_to_obj);
      yield return is_successful;
    }

    /// <summary>
    /// A function which reads byte data from a given file via an ET_Routine, converting to an object. It is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The Action to use to send the object. i.e. (ob => my_obj = ob)</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad offset or data count will result in a false return.</returns>
    public static IEnumerator ReadBytesFromFileAsync<T>(ET_File file, Action<T> obj, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      // Check to make sure the file and action are not null.
      if (file == null || obj == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to read the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalReadBytesFromFileAsync(file, obj, offset, data_count));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A function which reads byte data from a given file via an ET_Routine, converting to an object. It is recommended to use the 'ReturnFirstAndBreak' return method.
    /// Please note that this version is built for ET_Routines, returning the converted object. Otherwise, use the alternate version which takes in an Action.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="offset">The byte count offset to start reading the file from. If unsure, leave at 0 to read from the start.</param>
    /// <param name="data_count">The amount of bytes to read from the file. If unsure, leave at 0 to read in the whole file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns the converted object.</returns>
    public static IEnumerator ReadBytesFromFileAsync<T>(ET_File file, int offset = 0, int data_count = 0, bool cleanup = false)
    {
      T obj = default; // The object to convert.
      // Create a routine to read the bytes, converting to an object.
      ET_Routine routine = ET_Routine.CreateRoutine(ReadBytesFromFileAsync<T>(file, o => obj = o, offset, data_count, cleanup));
      yield return routine;
      yield return obj; // Return the converted object.
    }

    /// <summary>
    /// The internal function for attempting to read all text in a file to a single string. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="text">The string to send the data to. If there is no text, this will return a null string.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    private static bool InternalReadAllTextFromFile(ref string filepath, out string text)
    {
      text = null; // Initialize the string.

      try
      {
        // See if the file exists.
        if (InternalCheckFile(ref filepath, false))
        {
          // Create a stream reader for the file.
          using (StreamReader read = new StreamReader(filepath))
          {
            text = read.ReadToEnd(); // Read all of the text in the file.
          }
        }
        // Return if the text was set or not.
        return (text != null);
      }
      catch
      {
        // If any error occurs, return false.
        return false;
      }
    }

    /// <summary>
    /// A function which reads all text in a file to a single string.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="text">The string to send the data to. If there is no text, this will return a null string.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadAllTextFromFile(ref string filepath, out string text, bool cleanup = false)
    {
      text = null; // Initialize the string.

      // If the filepath is null, return false.
      if (filepath == null)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to read the entire file.
      return InternalReadAllTextFromFile(ref filepath, out text);
    }

    /// <summary>
    /// A function which reads all text in a file to a single string.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="text">The string to send the data to. If there is no text, this will return a null string.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadAllTextFromFile(ref string directory, ref string filename, out string text, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to read the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalReadAllTextFromFile(ref path, out text);
    }

    /// <summary>
    /// A function which reads all text in a file to a single string.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="text">The string to send the data to. If there is no text, this will return a null string.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadAllTextFromFile(ET_File file, out string text, bool cleanup = false)
    {
      text = string.Empty; // Initialize the string.

      // Attempt to read the file.
      return file != null ? ReadAllTextFromFile(ref file.directory, ref file.filename, out text, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to read a specified string line from a given file. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="message">The string to send the data to. If there is no line, this will return a null string.</param>
    /// <param name="line_number">The line number to read. This starts at 0.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    private static bool InternalReadStringFromFile(ref string filepath, out string message, long line_number)
    {
      message = null; // Initialize the string.

      try
      {
        // See if the file exists.
        if (InternalCheckFile(ref filepath, false))
        {
          // Create a stream reader for the file.
          using (StreamReader read = new StreamReader(filepath))
          {
            // Skip ahead to the right line number.
            for (long i = 0; i < line_number; i++)
            {
              if (read.ReadLine() == null)
                return false;
            }

            // Read the line we want. At this point, we know that the line exists.
            message = read.ReadLine();
            return (message != null);
          }
        }
      }
      catch
      {
        // If any error occurs, return false.
        return false;
      }

      return false; // The file was not successfully read.
    }

    /// <summary>
    /// A function which reads a specified string line from a given file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="message">The string to send the data to. If there is no line, this will return a null string.</param>
    /// <param name="line_number">The line number to read. This starts at 0.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadStringFromFile(ref string filepath, out string message, long line_number = 0, bool cleanup = false)
    {
      message = null; // Initialize the string.

      // If the filepath is null or the line number is bad, return false.
      if (filepath == null || line_number < 0 || line_number > file_MaxLineAccess)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to read the string.
      return InternalReadStringFromFile(ref filepath, out message, line_number);
    }

    /// <summary>
    /// A function which reads a specified string line from a given file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="message">The string to send the data to. If there is no line, this will return a null string.</param>
    /// <param name="line_number">The line number to read. This starts at 0.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadStringFromFile(ref string directory, ref string filename, out string message, long line_number = 0, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to read the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalReadStringFromFile(ref path, out message, line_number);
    }

    /// <summary>
    /// A function which reads a specified string line from a given file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The string to send the data to. If there is no line, this will return a null string.</param>
    /// <param name="line_number">The line number to read. This starts at 0.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadStringFromFile(ET_File file, out string message, long line_number = 0, bool cleanup = false)
    {
      message = string.Empty; // Initialize the string.

      // Attempt to read the file.
      return file != null ? ReadStringFromFile(ref file.directory, ref file.filename, out message, line_number, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to read a specified string line from a given file via a routine.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The Action to use to send the string. i.e. (me => my_string = me)</param>
    /// <param name="line_number">The line number to read. This starts at 0.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    private static IEnumerator InternalReadStringFromFileAsync(ET_File file, Action<string> message, long line_number = 0)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Make sure the file exists.
      if (InternalCheckFile(ref filepath, false))
      {
        // Attempt to make the FileStream.
        bool stream_made = true;
        string read_message = null; // The message that's read from the file.

        // Attempt to make the StreamReader.
        StreamReader stream_reader = null;
        try
        {
          stream_reader = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream failed to be made, return a null string and return false.
        if (!stream_made)
        {
          message(null);
          yield return false;
          yield break;
        }

        using (stream_reader)
        {
          int loop_counter = 0; // The counter for iterating through the file.
          bool read_good = true; // A bool determining if the file is being read successfully.
          // Up to the requested line, read each line from the file.
          for (long i = 0; i < line_number; i++)
          {
            try
            {
              // If a line is null, we have run out of lines to read. Leave the loop.
              if (stream_reader.ReadLine() == null)
              {
                read_good = false;
                break;
              }
            }
            catch
            {
              read_good = false;
              break;
            }

            // Update the loop counter and check if we should yield.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
              yield return null;
          }

          if (read_good)
            read_message = stream_reader.ReadLine(); // Attempt to read one more line.
        }

        // Return the read line, and return whether or not the line existed.
        message(read_message);
        yield return (read_message != null);
      }
      else
      {
        // If the file does not exist, return a null string and that the file was not read.
        message(null);
        yield return false;
      }
    }

    /// <summary>
    /// A function which reads a specified string line from a given file via a routine. If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The Action to use to send the string. i.e. (me => my_string = me)</param>
    /// <param name="line_number">The line number to read. This starts at 0.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    public static IEnumerator ReadStringFromFileAsync(ET_File file, Action<string> message, long line_number = 0, bool cleanup = false)
    {
      // If the filepath is null or the line number is bad, return false.
      if (file == null || message == null || line_number < 0 || line_number > file_MaxLineAccess)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to read the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalReadStringFromFileAsync(file, message, line_number));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A function which reads a specified string line from a given file via an ET_Routine, converting to an object. It is recommended to use the 'ReturnFirstAndBreak' return method.
    /// Please note that this version is built for ET_Routines, returning the converted object. Otherwise, use the alternate version which takes in an Action.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="line_number">The line number to read. This starts at 0.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns the read string.</returns>
    public static IEnumerator ReadStringFromFileAsync(ET_File file, long line_number = 0, bool cleanup = false)
    {
      string message = null; // The message read from the file.
      // If the filepath is null or the line number is bad, return false.
      ET_Routine string_routine = ET_Routine.CreateRoutine(ReadStringFromFileAsync(file, me => message = me, line_number, cleanup));
      yield return string_routine;
      yield return message;
    }

    /// <summary>
    /// The internal function for attempting to read in specified strings from a file into an array.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The path to the file to read.</param>
    /// <param name="messages">The array to send the read strings to.</param>
    /// <param name="line_numbers">The line numbers to read from the file. The messages are returned in the same order.</param>
    /// <returns>Returns if the strings were successfully read. An error results in a false return and a null array.</returns>
    private static bool InternalReadStringsFromFile(ref string filepath, out string[] messages, IList<long> line_numbers)
    {
      messages = null;
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, false))
        {
          // Sort the lines from least to greatest.
          IList<long> sorted_lines = line_numbers.OrderBy(num => num).ToList();

          // If we are nullifying on an invalid line, and there are invalid line numbers, we can immediately return now.
          if (sorted_lines.LastElement() > file_MaxLineAccess)
            return false;

          // Create a number array of indexes, sorted to match the indexes sorted line numbers.
          int[] indexes = EKIT_Math.GenerateNumberIList(0, line_numbers.Count()).OrderBy(num => line_numbers.ElementAt(num)).ToArray();

          // Initialize the array to the amount of lines to read.
          messages = new string[line_numbers.Count()];

          using (StreamReader stream_reader = new StreamReader(filepath))
          {
            long current_line = 0; // The current line number.
            long current_index = 0; // The current index in the 'indexes' array. This is the index of the unsorted line number.
            long last_line = -100; // The previous line index searched.

            // Go through every line to get.
            foreach (int i in sorted_lines)
            {
              // If the wanted line matches the last line, simply copy the line over. This prevents having to reset the stream.
              if (i == last_line)
                messages[indexes[current_index]] = messages[indexes[current_index - 1]];
              else
              {
                // Read lines until the wanted line is reached.
                while (current_line < i)
                {
                  current_line++; // Increment the current line.

                  // If there are not enough lines, quit the function immediately.
                  if (stream_reader.ReadLine() == null)
                  {
                    messages = null;
                    return false;
                  }
                }

                current_line++; // Increment one more time.
                string line; // A temp storage for the wanted line.

                // If the wanted line exists, set it into the messages array at the proper index. Otherwise, quit.
                if ((line = stream_reader.ReadLine()) != null)
                  messages[indexes[current_index]] = line;
                else
                {
                  messages = null;
                  return false;
                }
              }

              last_line = i; // Set the last line.
              current_index++; // Increment the index of the 'indexes' array.
            }
          }
          return true; // The file was read.
        }
      }
      catch
      {
        // If any error occurs, return false.
        messages = null;
        return false;
      }
      return false; // The file was not appended to.
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// </summary>
    /// <param name="filepath">The path to the file to read.</param>
    /// <param name="messages">The array to send the read strings to.</param>
    /// <param name="line_numbers">The line numbers to read from the file. The messages are returned in the same order.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the strings were successfully read. An error results in a false return and a null array.</returns>
    public static bool ReadStringsFromFile(ref string filepath, out string[] messages, IList<long> line_numbers, bool cleanup = false)
    {
      messages = null; // Initialize the array.

      // If the filepath is null or there are no line numbers, return false.
      if (filepath == null || line_numbers == null || line_numbers.Count <= 0)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to read all the strings from the file.
      return InternalReadStringsFromFile(ref filepath, out messages, line_numbers);
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="messages">The array to send the read strings to.</param>
    /// <param name="line_numbers">The line numbers to read from the file. The messages are returned in the same order.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the strings were successfully read. An error results in a false return and a null array.</returns>
    public static bool ReadStringsFromFile(ref string directory, ref string filename, out string[] messages, IList<long> line_numbers, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to read the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalReadStringsFromFile(ref path, out messages, line_numbers);
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The array to send the read strings to.</param>
    /// <param name="line_numbers">The line numbers to read from the file. The messages are returned in the same order.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the strings were successfully read. An error results in a false return and a null array.</returns>
    public static bool ReadStringsFromFile(ET_File file, out string[] messages, IList<long> line_numbers, bool cleanup = false)
    {
      messages = null; // Initialize the array.

      // Attempt to read the file.
      return file != null ? ReadStringsFromFile(ref file.directory, ref file.filename, out messages, line_numbers, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to read in specified strings from a file into an array via a routine. All cleanup is handled in the public function.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">An action used to set the array of strings read from the file. i.e.(me => myMessages = me)</param>
    /// <param name="line_numbers">The line numbers to read from the file. The messages are returned in the order of this variable.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    private static IEnumerator InternalReadStringsFromFileAsync(ET_File file, Action<string[]> messages, IList<long> line_numbers)
    {
      string filepath = CreateFilePath(file);

      // Check if the file exists.
      if (InternalCheckFile(ref filepath, false))
      {
        // This Func will compare and sort indexes based on their respective elements within the passed-in line numbers.
        Func<int, int, int> compare_indexes = (a, b) => line_numbers.ElementAt(a).CompareTo(line_numbers.ElementAt(b));

        // Copy and order the line numbers. We aren't calling 'OrderBy' in order to prevent a holdup on large lists.
        IList<long> sorted_lines = new List<long>(line_numbers); // Copy the list over so the original is untouched.
        int[] indexes = EKIT_Math.GenerateNumberArray(0, line_numbers.Count); // Create a number array of indexes for the number of line numbers.

        // Check if the list is already sorted. If so, we don't need to waste time.
        if (!sorted_lines.IsSorted(EKIT_Sort.CompareLeastToGreatest))
        {
          // Run two routines at the same time to sort the lines and the indexes to those lines.
          ET_Routine sorted_routine = ET_Routine.StartRoutine(EKIT_Sort.QuickSortAsync(sorted_lines, EKIT_Sort.CompareLeastToGreatest));
          yield return EKIT_Sort.QuickSortAsync(indexes, compare_indexes);
          yield return sorted_routine;
        }

        // If going past the max line access, and we nullify, quit immediately.
        if (line_numbers.LastElement() > file_MaxLineAccess)
        {
          messages(null);
          yield return false;
          yield break;
        }

        bool stream_made = true; // Attempt to make the FileStream.
        // Attempt to make the StreamReader.
        StreamReader stream_reader = null;
        try
        {
          stream_reader = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream failed to be made, return a null string and return false.
        if (!stream_made)
        {
          messages(null);
          yield return false;
          yield break;
        }

        using (stream_reader)
        {
          string[] read_messages = new string[line_numbers.Count]; // The strings read from the file.

          long current_line = 0; // The current line number.
          long current_index = 0; // The current index in the 'indexes' array. This is the index of the unsorted line number.
          long last_line = -100; // The previous line index searched.

          bool valid_read = true; // A bool determining if the read is still valid to continue.
          int loop_counter = 0; // The counter for looping through the lines.

          // Go through the entire list of lines.
          foreach (int i in sorted_lines)
          {
            // If the last line is the same as the current line, just input that same string.
            if (i == last_line)
              read_messages[indexes[current_index]] = read_messages[indexes[current_index - 1]];
            else
            {
              // Read through lines until reaching the right line.
              while (current_line < i)
              {
                try
                {
                  current_line++;
                  // If the line does not exist, quit the function immediately.
                  if (stream_reader.ReadLine() == null)
                  {
                    valid_read = false;
                    break;
                  }
                }
                catch
                {
                  // Upon an error, the read is no longer good.
                  valid_read = false;
                  break;
                }

                // Update the yield counter.
                if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                  yield return null;
              }

              // If still able to read, attempt to read the wanted line.
              if (valid_read)
              {
                current_line++; // Increment one more time.
                string line; // A temp storage for the wanted line.

                // If the wanted line exists, set it into the messages array at the proper index. Otherwise, quit.
                if ((line = stream_reader.ReadLine()) != null)
                  read_messages[indexes[current_index]] = line;
                else
                  valid_read = false; // If the line didn't exist, quit. There's no more lines.
              }

              // If the read is no longer good, return what we have and quit.
              if (!valid_read)
              {
                messages(null);
                stream_reader.Dispose();
                yield return false;
                yield break;
              }
            }

            last_line = i; // Set the last line.
            current_index++; // Increment the index of the 'indexes' array.
          }

          messages(read_messages); // Return the read messages.
          yield return true; // The file was read successfully.
        }
      }
      else
      {
        // The file was not read successfully.
        messages(null);
        yield return false;
      }
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array via a routine.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">An action used to set the array of strings read from the file. i.e.(me => myMessages = me)</param>
    /// <param name="line_numbers">The line numbers to read from the file. The messages are returned in the order of this variable.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    public static IEnumerator ReadStringsFromFileAsync(ET_File file, Action<string[]> messages, IList<long> line_numbers, bool cleanup = false)
    {
      // If any passed-in information does not exist, return false immediately.
      if (file == null || messages == null || line_numbers == null || line_numbers.Count <= 0)
      {
        yield return false;
        yield break;
      }
      // Cleanup and generate the filepath early, as it will be necessary later to create the FileStream.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to read the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalReadStringsFromFileAsync(file, messages, line_numbers));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array via a routine.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// Please note that this version is built for ET_Routines, returning the converted object. Otherwise, use the alternate version which takes in an Action.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="line_numbers">The line numbers to read from the file. The messages are returned in the order of this variable.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns the read strings.</returns>
    public static IEnumerator ReadStringsFromFileAsync(ET_File file, IList<long> line_numbers, bool cleanup = false)
    {
      string[] messages = null; // The messages read from the file.
      // Create an ET_Routine to read the file, and return what is found.
      ET_Routine string_routine = ET_Routine.CreateRoutine(ReadStringsFromFileAsync(file, me => messages = me, line_numbers, cleanup));
      yield return string_routine;
      yield return messages;
    }

    /// <summary>
    /// The internal function for attempting to read in specified strings from a file into an array.
    /// In the event of an error, such as the file being inaccessible, a null array is returned.
    /// All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The path to the file to read.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="line_start">The first line to start reading from the file. This is inclusive.</param>
    /// <param name="line_end">The last line to read from the file. This is exclusive.</param>
    /// <param name="allow_short_file">If true, if the file is shorter than 'line_end' lines, the array is shortened and returned normally.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    private static bool InternalReadStringsFromFile(ref string filepath, out string[] messages, int line_start, int line_end, bool allow_short_file = false)
    {
      messages = null;
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, false))
        {
          using (StreamReader stream_reader = new StreamReader(filepath))
          {
            // Read lines until reaching the start point.
            for (int i = 0; i < line_start; i++)
            {
              // If there aren't enough lines, return false.
              if (stream_reader.ReadLine() == null)
                return false;
            }

            messages = new string[line_end - line_start]; // Initialize the array, now that we're at a starting point.

            // Read every line from start to end.
            for (int i = line_start; i < line_end; i++)
            {
              string line;
              // If a line doesn't exist, the file is too short. Return false.
              if ((line = stream_reader.ReadLine()) != null)
                messages[i] = line;
              else
              {
                // If we allow a short file, resize the array to fit what was found.
                if (allow_short_file)
                  Array.Resize(ref messages, i - line_start);
                else
                  messages = null;

                return allow_short_file && messages.Length > 0; // Return the opposite of nullifying; if nullified, then nothing was read.
              }
            }
          }
          return true; // The file was successfully read.
        }
      }
      catch
      {
        // If any error occurs, return false.
        messages = null;
        return false;
      }
      // The file was not successfully read.
      return false;
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array.
    /// In the event of an error, such as the file being inaccessible, a null array is returned.
    /// </summary>
    /// <param name="filepath">The path to the file to read.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="line_start">The first line to start reading from the file. This is inclusive.</param>
    /// <param name="line_end">The last line to read from the file. This is exclusive.</param>
    /// <param name="allow_short_file">If true, if the file is shorter than 'line_end' lines, the array is shortened and returned normally.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadStringsFromFile(ref string filepath, out string[] messages, int line_start, int line_end, bool allow_short_file = false, bool cleanup = false)
    {
      messages = null;

      // If the filepath or the indexes are bad, return false.
      if (filepath == null || line_start < 0 || line_end < line_start || line_end > file_MaxLineAccess)
        return false;
      // If cleaning up, clean up the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to read the specified lines from the file.
      return InternalReadStringsFromFile(ref filepath, out messages, line_start, line_end, allow_short_file);
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array.
    /// In the event of an error, such as the file being inaccessible, a null array is returned.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="line_start">The first line to start reading from the file. This is inclusive.</param>
    /// <param name="line_end">The last line to read from the file. This is exclusive.</param>
    /// <param name="allow_short_file">If true, if the file is shorter than 'line_end' lines, the array is shortened and returned normally.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadStringsFromFile(ref string directory, ref string filename, out string[] messages, int line_start, int line_end, bool allow_short_file = false, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Concatenate the path and attempt to read the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalReadStringsFromFile(ref path, out messages, line_start, line_end, allow_short_file);
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array.
    /// In the event of an error, such as the file being inaccessible, a null array is returned.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="line_start">The first line to start reading from the file. This is inclusive.</param>
    /// <param name="line_end">The last line to read from the file. This is exclusive.</param>
    /// <param name="allow_short_file">If true, if the file is shorter than 'line_end' lines, the array is shortened and returned normally.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. A bad line number will result in a false return.</returns>
    public static bool ReadStringsFromFile(ET_File file, out string[] messages, int line_start, int line_end, bool allow_short_file = false, bool cleanup = false)
    {
      messages = null; // Initialize the array.

      // Attempt to read the file.
      return file != null ? ReadStringsFromFile(ref file.directory, ref file.filename, out messages, line_start, line_end, allow_short_file, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to read in specified strings from a file into an array via a routine. All cleanup is handled in the public function.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">An action used to set the array of strings read from the file. i.e.(me => myMessages = me)</param>
    /// <param name="line_start">The first line to start reading from the file. This is inclusive.</param>
    /// <param name="line_end">The last line to read from the file. This is exclusive.</param>
    /// <param name="allow_short_file">If true, if the file is shorter than 'line_end' lines, the array is shortened and returned normally.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    private static IEnumerator InternalReadStringsFromFileAsync(ET_File file, Action<string[]> messages, int line_start, int line_end, bool allow_short_file = false)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Make sure the file exists.
      if (InternalCheckFile(ref filepath, false))
      {
        // Attempt to make the FileStream.
        bool stream_made = true;
        // Attempt to make the StreamReader.
        StreamReader stream_reader = null;
        try
        {
          stream_reader = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream reader failed to be made, return a null string and return false.
        if (!stream_made)
        {
          messages(null);
          yield return false;
          yield break;
        }

        using (stream_reader)
        {
          bool reading_good = true; // A bool making sure that the read was good.
          try
          {
            // Attempt to go through through the lines, up to the first line to read.
            for (int i = 0; i < line_start; i++)
            {
              // If a line doesn't exist, then the file is too short.
              if (stream_reader.ReadLine() == null)
              {
                reading_good = false;
                break;
              }
            }
          }
          catch
          {
            // Upon an error, the read is now bad.
            reading_good = false;
          }

          // If the read is bad at this point, return a null array and return false.
          if (!reading_good)
          {
            messages(null);
            stream_reader.Dispose();
            yield return false;
            yield break;
          }

          string[] read_messages = new string[line_end - line_start]; // The array of strings read from the file.

          try
          {
            // Read for every single line.
            for (int i = line_start; i < line_end; i++)
            {
              // Attempt to read in a line. If it exists, add it to the array. Else, the read is no longer good.
              string line = null;
              if ((line = stream_reader.ReadLine()) != null)
                read_messages[i] = line;
              else
              {
                if (allow_short_file)
                  Array.Resize(ref read_messages, i - line_start);
                else
                  read_messages = null;

                reading_good = false;
                break;
              }
            }
          }
          catch
          {
            reading_good = false; // In the event of an error, the read is no longer good.
          }

          if (!reading_good)
          {
            // Return the messages, and return a bool based on if we nulled, and if not, if any lines were read.
            messages(read_messages);
            stream_reader.Dispose();
            yield return allow_short_file && read_messages.Length > 0; // Return the opposite of nullifying; if nullified, then nothing was read.
            yield break;
          }
          // All messages were read. Return them, and yield true.
          messages(read_messages);
        }
        // Return that the lines were read.
        yield return true;
      }
      else
      {
        // Nothing was read. Return null and yield false.
        messages(null);
        yield return false;
        yield break;
      }
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array via a routine.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">An action used to set the array of strings read from the file. i.e.(me => myMessages = me)</param>
    /// <param name="line_start">The first line to start reading from the file. This is inclusive.</param>
    /// <param name="line_end">The last line to read from the file. This is exclusive.</param>
    /// <param name="allow_short_file">If true, if the file is shorter than 'line_end' lines, the array is shortened and returned normally.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    public static IEnumerator ReadStringsFromFileAsync(ET_File file, Action<string[]> messages, int line_start, int line_end, bool allow_short_file = false, bool cleanup = false)
    {
      // If the passed in information is bad, return false immediately.
      if (file == null || messages == null || line_start < 0 || line_start > file_MaxLineAccess || line_end < line_start || line_end > file_MaxLineAccess)
      {
        messages?.Invoke(null);
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to read the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalReadStringsFromFileAsync(file, messages, line_start, line_end));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A function which reads in specified strings from a file into an array via a routine.
    /// In the event of an error, such as the file being too short or inaccessible, a null array is returned.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// Please note that this version is built for ET_Routines, returning the converted object. Otherwise, use the alternate version which takes in an Action.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="line_start">The first line to start reading from the file. This is inclusive.</param>
    /// <param name="line_end">The last line to read from the file. This is exclusive.</param>
    /// <param name="allow_short_file">If true, if the file is shorter than 'line_end' lines, the array is shortened and returned normally.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns the read strings.</returns>
    public static IEnumerator ReadStringsFromFileAsync(ET_File file, int line_start, int line_end, bool allow_short_file = false, bool cleanup = false)
    {
      string[] messages = null; // The messages read from the file.
      // Create an ET_Routine to read the file, and return what is found.
      ET_Routine string_routine = ET_Routine.CreateRoutine(ReadStringsFromFileAsync(file, me => messages = me, line_start, line_end, allow_short_file, cleanup));
      yield return string_routine;
      yield return messages;
    }

    /// <summary>
    /// The internal function for attempting to read an entire text file into a string array. All cleanup is handled in the public function.
    /// WARNING: Performing this takes longer the larger the file. If your file is large or has extremely long lines, a memory error will occur, resulting in a null return.
    /// If your file has more than 'file_MaxLineAccess' lines, the function will end early and return false.
    /// </summary>
    /// <param name="filepath">The path to the file to read.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <returns>Returns if the data was successfully read. Errors such as large file numbers will result in a false return.</returns>
    private static bool InternalReadAllStringsFromFile(ref string filepath, out string[] messages)
    {
      messages = null; // Initialize the array.

      try
      {
        // Check if the file exists.
        if (InternalCheckFile(ref filepath, false))
        {
          using (StreamReader stream = new StreamReader(filepath))
          {
            List<string> file_strings = new List<string>(); // The list of strings read from the file.

            // Read every line from start to end.
            while (true)
            {
              string line;
              // If a line doesn't exist, the file is too short. Return false.
              if ((line = stream.ReadLine()) != null)
              {
                // Make sure we didn't take in too much data. If clear, add the line to the list.
                if (file_strings.Count < (int)file_MaxLineAccess)
                  file_strings.Add(line);
                else
                  return false;
              }
              else
                break;
            }
            messages = file_strings.ToArray(); // After completion, convert the list into an array.
            return true; // The file was successfully read.
          }
        }
      }
      catch
      {
        return false; // In the event of an error, return false immediately.
      }

      return false; // The file was not successfully read.
    }

    /// <summary>
    /// A function which reads an entire text file into a string array.
    /// WARNING: Performing this takes longer the larger the file. If your file is large or has extremely long lines, a memory error will occur, resulting in a null return.
    /// If your file has more than 'file_MaxLineAccess' lines, the function will end early and return false.
    /// </summary>
    /// <param name="filepath">The path to the file to read.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. Errors such as large file numbers will result in a false return.</returns>
    public static bool ReadAllStringsFromFile(ref string filepath, out string[] messages, bool cleanup = false)
    {
      messages = null; // Initialize the array.
      // If the filepath doesn't exist, return false immediately.
      if (filepath == null)
        return false;
      // If cleaning up, clean up the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to read all the lines in the file.
      return InternalReadAllStringsFromFile(ref filepath, out messages);
    }

    /// <summary>
    /// A function which reads an entire text file into a string array.
    /// WARNING: Performing this takes longer the larger the file. If your file is large or has extremely long lines, a memory error will occur, resulting in a null return.
    /// If your file has more than 'file_MaxLineAccess' lines, the function will end early and return false.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. Errors such as large file numbers will result in a false return.</returns>
    public static bool ReadAllStringsFromFile(ref string directory, ref string filename, out string[] messages, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to read the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalReadAllStringsFromFile(ref path, out messages);
    }

    /// <summary>
    /// A function which reads an entire text file into a string array.
    /// WARNING: Performing this takes longer the larger the file. If your file is large or has extremely long lines, a memory error will occur, resulting in a null return.
    /// If your file has more than 'file_MaxLineAccess' lines, the function will end early and return false.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the data was successfully read. Errors such as large file numbers will result in a false return.</returns>
    public static bool ReadAllStringsFromFile(ET_File file, out string[] messages, bool cleanup = false)
    {
      messages = null; // Initialize the array.

      // Attempt to read the file.
      return file != null ? ReadAllStringsFromFile(ref file.directory, ref file.filename, out messages, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to read an entire text file into a string array via a routine.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method. All cleanup is handled in the public function.
    /// WARNING: Performing this takes longer the larger the file. If your file is large or has extremely long lines, a memory error will occur, resulting in a null return.
    /// If your file has more than 'file_MaxLineAccess' lines, the function will end early and return false.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    private static IEnumerator InternalReadAllStringsFromFileAsync(ET_File file, Action<string[]> messages)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Make sure the file exists.
      if (InternalCheckFile(ref filepath, false))
      {
        List<string> file_strings = new List<string>(); // The list containing all the file strings.

        // Attempt to make the FileStream.
        bool stream_made = true;
        // Attempt to make the StreamReader.
        StreamReader stream_reader = null;
        try
        {
          stream_reader = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream reader failed to be made, return a null string and return false.
        if (!stream_made)
        {
          messages(null);
          yield return false;
          yield break;
        }

        using (stream_reader)
        {
          bool reading_good = true; // A bool determining if the read is still good.
          int loop_counter = 0; // A loop counte for the file.

          // Loop while there are still lines to read.
          while (true)
          {
            try
            {
              string line = null; // The current line to read.
              if ((line = stream_reader.ReadLine()) != null)
              {
                // If there is still space, add the line to the list. WATCH YOUR MEMORY.
                if (file_strings.Count < (int)file_MaxLineAccess)
                  file_strings.Add(line);
                // Otherwise, we cannot read the whole file.
                else
                {
                  reading_good = false;
                  break;
                }
              }
              else
              {
                break;
              }
            }
            catch
            {
              // In the event of an error, the read is no longer good.
              reading_good = false;
              break;
            }
            // Yield if the counter is hit.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
              yield return null;
          }

          // If the read failed, return a null array and yield false.
          if (!reading_good)
          {
            messages(null);
            stream_reader.Dispose();
            yield return false;
            yield break;
          }
        }

        // Return the successful array.
        messages(file_strings.ToArray());
        yield return true;
      }
      else
      {
        // The file wasn't found. Return false.
        messages(null);
        yield return false;
      }
    }

    /// <summary>
    /// A function which reads an entire text file into a string array via a routine.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// WARNING: Performing this takes longer the larger the file. If your file is large or has extremely long lines, a memory error will occur, resulting in a null return.
    /// If your file has more than 'file_MaxLineAccess' lines, the function will end early and return false.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The array of strings to send the data to.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was read or not.</returns>
    public static IEnumerator ReadAllStringsFromFileAsync(ET_File file, Action<string[]> messages, bool cleanup = false)
    {
      // If the filepath or action is null, return false.
      if (file == null || messages == null)
      {
        messages?.Invoke(null);
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to read the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalReadAllStringsFromFileAsync(file, messages));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A function which reads an entire text file into a string array via a routine.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// Please note that this version is built for ET_Routines, returning the converted object. Otherwise, use the alternate version which takes in an Action.
    /// WARNING: Performing this takes longer the larger the file. If your file is large or has extremely long lines, a memory error will occur, resulting in a null return.
    /// If your file has more than 'file_MaxLineAccess' lines, the function will end early and return false.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns the read strings.</returns>
    public static IEnumerator ReadAllStringsFromFileAsync(ET_File file, bool cleanup = false)
    {
      string[] messages = null; // The messages read from the file.
      // Create an ET_Routine to read the file, and return what is found.
      ET_Routine string_routine = ET_Routine.CreateRoutine(ReadAllStringsFromFileAsync(file, me => messages = me, cleanup));
      yield return string_routine;
      yield return messages;
    }

    /// <summary>
    /// The internal function for attempting to append an array of bytes to a given file. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendBytesToFile(ref string filepath, byte[] bytes, bool create_if_null = false)
    {
      try
      {
        // Make sure the file exists, or at least was created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Using a file stream, append the bytes to the file.
          using (FileStream stream = new FileStream(filepath, FileMode.Append, FileAccess.Write))
            stream.Write(bytes, 0, bytes.Length);

          return true; // The file was successfully appended to.
        }
      }
      catch
      {
        // If any error occurs, return false.
        return false;
      }

      return false; // The file was not appended to.
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFile(ref string filepath, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // If any of the passed-in information is invalid, return false immediately.
      if (filepath == null || bytes == null || bytes.Length <= 0)
        return false;
      // If cleaning up, clean up the file path.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to append to the file.
      return InternalAppendBytesToFile(ref filepath, bytes, create_if_null);
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFile(ref string directory, ref string filename, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // Cleanup the path if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Concatenate the path and attempt to append the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendBytesToFile(ref path, bytes, create_if_null);
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFile(ET_File file, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the bytes to the file.
      return file != null ? AppendBytesToFile(ref file.directory, ref file.filename, bytes, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append an array of bytes to a given file via a routine. All cleanup is handled in the public function.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    private static IEnumerator InternalAppendBytesToFileAsync(ET_File file, byte[] bytes, bool create_if_null = false)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Check if the file exists, or does so after creation.
      if (InternalCheckFile(ref filepath, create_if_null))
      {
        // Attempt to make the FileStream.
        bool stream_made = true;
        FileStream file_stream = null;
        try
        {
          file_stream = new FileStream(filepath, FileMode.Append, FileAccess.Write);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream failed to be made, return a null string and return false.
        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        bool write_good = true; // A bool determining if the write is still valid.

        using (file_stream)
        {
          int loop_counter = 0; // A counter for looping how many times a byte is written.

          // Write each byte individually into the file.
          for (int i = 0; i < bytes.Length; i++)
          {
            // Attempt to append every byte. In the event of an error, return false immediately.
            try
            {
              file_stream.WriteByte(bytes[i]);
            }
            catch
            {
              write_good = false;
              break;
            }

            // If reaching the maximum loop, yield the routine.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxAccessLoopCount))
              yield return null;
          }
        }
        yield return write_good; // Return if the write was good.
      }
      else
      {
        yield return false;
      }
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file via a routine. If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    public static IEnumerator AppendBytesToFileAsync(ET_File file, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null || bytes == null || bytes.Length <= 0)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalAppendBytesToFileAsync(file, bytes, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to append an array of bytes to a given file. All cleanup is handled in the public function.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendBytesToFileSafely(ref string filepath, byte[] bytes, bool create_if_null = false)
    {
      // If a temporary file cannot be made, return false immediately.
      if (!CreateTempFilePath(out string temp_path))
        return false;

      try
      {
        // Make sure the file exists, or at least was created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Attempt to copy the file to the temporary file.
          if (InternalCopyFile(ref filepath, ref temp_path, true))
          {
            // Using a file stream, append the bytes to the file.
            using (FileStream filestream_temp = new FileStream(temp_path, FileMode.Append, FileAccess.Write))
              filestream_temp.Write(bytes, 0, bytes.Length);

            return InternalMoveFile(ref temp_path, ref filepath, true); // Move the file from the temporary location to the original location.
          }

          return false; // The file could not be copied.
        }
      }
      catch
      {
        // If any error occurs, return false.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false; // The file could not be appended to.
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFileSafely(ref string filepath, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // If the filepath or bytes do not exist, return false immediately.
      if (filepath == null || bytes == null || bytes.Length <= 0)
        return false;
      // If cleaning up, clean up the file path.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to safely append the bytes to the file.
      return InternalAppendBytesToFileSafely(ref filepath, bytes, create_if_null);
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFileSafely(ref string directory, ref string filename, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // Cleanup the path if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to append the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendBytesToFileSafely(ref path, bytes, create_if_null);
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFileSafely(ET_File file, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the bytes to the file.
      return file != null ? AppendBytesToFileSafely(ref file.directory, ref file.filename, bytes, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append an array of bytes to a given file via a routine. All cleanup is handled in the public function.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    private static IEnumerator InternalAppendBytesToFileSafelyAsync(ET_File file, byte[] bytes, bool create_if_null = false)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);
      
      // Check if the file exists, or does so after creation.
      if (InternalCheckFile(ref filepath, create_if_null))
      {
        if (!CreateTempFilePath(out string temp_path))
        {
          yield return false;
          yield break;
        }
        ET_File temp_file = new ET_File(temp_path, false);

        // Copy the file over to the temp file.
        ET_Routine copy_routine = ET_Routine.CreateRoutine<bool>(InternalCopyFileAsync(file, temp_file, true));
        yield return copy_routine;
        if (copy_routine.value is bool && !(bool)copy_routine.value)
        {
          yield return false;
          yield break;
        }
        bool stream_made = true; // A bool determining that the stream was correctly made.
        bool write_good = true; // A bool determining if the write is still valid.
        // Attempt to make the FileStream for the temporary path.
        FileStream filestream_temp = null;
        try
        {
          filestream_temp = new FileStream(temp_path, FileMode.Append, FileAccess.Write);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream failed to be made, return a null string and return false.
        if (!stream_made)
        {

          yield return false;
          yield break;
        }
        
        using (filestream_temp)
        {
          int loop_counter = 0; // A counter for looping how many times a byte is written.

          // Write each byte individually into the file. In the event of an error, return false.
          for (int i = 0; i < bytes.Length; i++)
          {
            try
            {
              filestream_temp.WriteByte(bytes[i]);
            }
            catch
            {
              
              write_good = false;
              break;
            }

            // If reaching the maximum loop, yield the routine.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxAccessLoopCount))
              yield return null;
          }
        }
        
        // If the write was good, move the temporary file to overwrite the old file.
        if (write_good)
        {
          ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
          yield return move_routine;
          yield return move_routine.value;
        }
        else
        {
          // If the write was bad, just delete the old path.
          InternalDeleteFile(ref temp_path);
          yield return false;
        }
      }
      else
      {
        yield return false; // The file does not exist. Return false immediately.
      }
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file via a routine.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="bytes">The bytes to append to the file.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    public static IEnumerator AppendBytesToFileSafelyAsync(ET_File file, byte[] bytes, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null || bytes == null || bytes.Length <= 0)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalAppendBytesToFileSafelyAsync(file, bytes, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to convert an object into an array of bytes before appending it to a given file. All cleanup is handled in the public function.
    /// The object MUST be fully serializable for this to work.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendBytesToFile<T>(ref string filepath, T obj, bool create_if_null = false)
    {
      // Check that the filepath is valid and attempt to convert the object to bytes.
      if (!EKIT_Conversion.ConvertToSerializedBytes(obj, out byte[] bytes))
        return false;
      // Attempt to append the byte array to the file.
      return InternalAppendBytesToFile(ref filepath, bytes, create_if_null);
    }

    /// <summary>
    /// A function which converts an object into an array of bytes before appending it to a given file.
    /// The object MUST be fully serializable for this to work.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFile<T>(ref string filepath, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // Check that the filepath and object are valid.
      if (filepath == null)
        return false;
      // If cleaning up, clean up the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to append the object.
      return InternalAppendBytesToFile(ref filepath, obj, create_if_null);
    }

    /// <summary>
    /// A function which converts an object into an array of bytes before appending it to a given file.
    /// The object MUST be fully serializable for this to work.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFile<T>(ref string directory, ref string filename, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // Cleanup the path if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to append the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendBytesToFile(ref path, obj, create_if_null);
    }

    /// <summary>
    /// A function which converts an object into an array of bytes before appending it to a given file.
    /// The object MUST be fully serializable for this to work.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFile<T>(ET_File file, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the bytes to the file.
      return file != null ? AppendBytesToFile(ref file.directory, ref file.filename, obj, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to convert an object into an array of bytes before appending it to a given file via a routine.
    /// All cleanup is handled in the public function.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    private static IEnumerator InternalAppendBytesToFileAsync<T>(ET_File file, T obj, bool create_if_null = false)
    {
      // Attempt to convert the bytes. If it fails, return false immediately.
      if (!EKIT_Conversion.ConvertToSerializedBytes(obj, out byte[] bytes))
      {
        yield return false;
        yield break;
      }
      // Attempt to append the bytes to the file.
      ET_Routine append_routine = ET_Routine.CreateRoutine<bool>(InternalAppendBytesToFileAsync(file, bytes, create_if_null));
      yield return append_routine;
      yield return append_routine.value;
    }

    /// <summary>
    /// A function which appends an array of bytes to a given file via a routine. If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    public static IEnumerator AppendBytesToFileAsync<T>(ET_File file, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // If the file is null, return false immediately.
      if (file == null)
      {
        yield return false;
        yield break;
      }
      // If cleaning up, clean up the file information.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine append_routine = ET_Routine.CreateRoutine<bool>(InternalAppendBytesToFileAsync(file, obj, create_if_null));
      yield return append_routine;
      yield return append_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to convert an object an object into an array of bytes before appending it to a given file. All cleanup is handled in the public function.
    /// The object MUST be fully serializable for this to work.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendBytesToFileSafely<T>(ref string filepath, T obj, bool create_if_null = false)
    {
      // Check that the filepath is valid and attempt to convert the object to bytes.
      if (!EKIT_Conversion.ConvertToSerializedBytes(obj, out byte[] bytes))
        return false;
      // Attempt to append the bytes.
      return InternalAppendBytesToFileSafely(ref filepath, bytes, create_if_null);
    }

    /// <summary>
    /// A function which converts an object into an array of bytes before appending it to a given file.
    /// The object MUST be fully serializable for this to work.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFileSafely<T>(ref string filepath, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // Check that the filepath is valid and attempt to convert the object to bytes.
      if (filepath == null)
        return false;
      // If cleaning up, clean up the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to append the object to the file.
      return InternalAppendBytesToFileSafely(ref filepath, obj, create_if_null);
    }

    /// <summary>
    /// A function which converts an object into an array of bytes before appending it to a given file.
    /// The object MUST be fully serializable for this to work.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFileSafely<T>(ref string directory, ref string filename, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // Cleanup the path if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Concatenate the path and attempt to append the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendBytesToFileSafely(ref path, obj, create_if_null);
    }

    /// <summary>
    /// A function which converts an object into an array of bytes before appending it to a given file.
    /// The object MUST be fully serializable for this to work.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendBytesToFileSafely<T>(ET_File file, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the bytes to the file.
      return file != null ? AppendBytesToFileSafely(ref file.directory, ref file.filename, obj, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to convert an object an object into an array of bytes before appending it to a given file via a routine.
    /// All cleanup is handled in the public function.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    private static IEnumerator InternalAppendBytesToFileSafelyAsync<T>(ET_File file, T obj, bool create_if_null = false)
    {
      // Check that the file information is valid and the object can be converted to bytes.
      if (!EKIT_Conversion.ConvertToSerializedBytes(obj, out byte[] bytes))
      {
        yield return false;
        yield break;
      }
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine append_routine = ET_Routine.CreateRoutine<bool>(InternalAppendBytesToFileSafelyAsync(file, bytes, create_if_null));
      yield return append_routine;
      yield return append_routine.value;
    }

    /// <summary>
    /// A function which converts an object into an array of bytes before appending it to a given file via a routine.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="obj">The object to append. This object must be serializable.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    public static IEnumerator AppendBytesToFileSafelyAsync<T>(ET_File file, T obj, bool create_if_null = false, bool cleanup = false)
    {
      // Check that the file information is valid and the object can be converted to bytes.
      if (file == null)
      {
        yield return false;
        yield break;
      }
      // If cleaning up, clean up the file information.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine append_routine = ET_Routine.CreateRoutine<bool>(InternalAppendBytesToFileSafelyAsync(file, obj, create_if_null));
      yield return append_routine;
      yield return append_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to append a string to a given file. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendStringToFile(ref string filepath, string message, bool append_newline = true, bool create_if_null = false)
    {
      try
      {
        // See if the file exsists, or does so after being created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Using a StreamWriter, write to the file. 'WriteLine' automatically adds the newline.
          using (StreamWriter stream = new StreamWriter(filepath, true))
          {
            if (append_newline)
              stream.WriteLine(message);
            else
              stream.Write(message);
          }
          // The message was successfully appended to.
          return true;
        }
      }
      catch
      {
        // If any error occurs, return false.
        return false;
      }
      // The file was not successfully appended to.
      return false;
    }

    /// <summary>
    /// A function which appends a string to a given file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringToFile(ref string filepath, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // If the filepath or message are null, return false immediately.
      if (filepath == null || message == null)
        return false;
      // If cleaning up, clean up the file path.
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Attempt to append the string to the file.
      return InternalAppendStringToFile(ref filepath, message, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends a string to a given file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringToFile(ref string directory, ref string filename, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      if (directory == null || filename == null || message == null)
        return false;
      // Cleanup the file path if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Concatenate the path and attempt to append the string to the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendStringToFile(ref path, message, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends a string to a given file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringToFile(ET_File file, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the string to the file.
      return file != null ? AppendStringToFile(ref file.directory, ref file.filename, message, append_newline, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append a string to a given file via a routine. All cleanup is handled in the public function.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static IEnumerator InternalAppendStringToFileAsync(ET_File file, string message, bool append_newline = true, bool create_if_null = false)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Check if the file exists, or does so after being created.
      if (InternalCheckFile(ref filepath, create_if_null))
      {
        bool stream_made = true; // A bool determining if the StreamWriter was correctly made.
        StreamWriter stream_writer = null; // The StreamWriter for the target file.
        // Attempt to create the StreamWriter. If it fails, return false immediately.
        try
        {
          stream_writer = new StreamWriter(filepath, true);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        bool write_good = true; // A bool determining if the file is successfully appended to.
        using (stream_writer)
        {
          try
          {
            // If appending a newline, use WriteLine. Otherwise, just use Write.
            if (append_newline)
              stream_writer.WriteLine(message);
            else
              stream_writer.Write(message);
          }
          catch
          {
            write_good = false; // If an error occurs, the write is no longer good.
          }
        }

        yield return write_good; // Return if the write was good.
      }
      else
      {
        yield return false; // The file did not exist.
      }
    }

    /// <summary>
    /// A function which appends a string to a given file via a routine.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static IEnumerator AppendStringToFileAsync(ET_File file, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null || message == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalAppendStringToFileAsync(file, message, append_newline, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to append a string to a given file. All cleanup is handled in the public function.
    /// This version will first create a temporary file and attempt to append the string before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendStringToFileSafely(ref string filepath, string message, bool append_newline = true, bool create_if_null = false)
    {
      // If a temporary file cannot be made, return false immediately.
      if (!CreateTempFilePath(out string temp_path))
        return false;

      try
      {
        // Make sure the file exists, or at least was created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Attempt to copy the file to the temporary file.
          if (InternalCopyFile(ref filepath, ref temp_path, true))
          {
            // Using a file stream, append the string to the file.
            using (StreamWriter streamwriter_temp = new StreamWriter(temp_path, true))
            {
              // If appending a newline, use WriteLine. Otherwise, just use Write.
              if (append_newline)
                streamwriter_temp.WriteLine(message);
              else
                streamwriter_temp.Write(message);
            }

            return InternalMoveFile(ref temp_path, ref filepath, true); // Move the file from the temporary location to the original location.
          }

          return false; // The file could not be copied.
        }
      }
      catch
      {
        // If any error occurs, return false.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false; // The file could not be appended to.
    }

    /// <summary>
    /// A function which appends a string to a given file.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringToFileSafely(ref string filepath, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // If the filepath or message do not exist, return false immediately.
      if (filepath == null || message == null)
        return false;
      // If cleaning up, clean up the filepath.
      if (cleanup)
        CleanupFilePath(ref filepath);
      return InternalAppendStringToFileSafely(ref filepath, message, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends a string to a given file.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringToFileSafely(ref string directory, ref string filename, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // Cleanup the file path if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to append the string to the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendStringToFileSafely(ref path, message, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends a string to a given file.
    /// This version will first create a temporary file and attempt to append the bytes before overwriting the original file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringToFileSafely(ET_File file, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the string to the file.
      return file != null ? AppendStringToFileSafely(ref file.directory, ref file.filename, message, append_newline, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append a string to a given file via a routine. All cleanup is handled in the public function.
    /// This version will first create a temporary file and attempt to append the string before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    private static IEnumerator InternalAppendStringToFileSafelyAsync(ET_File file, string message, bool append_newline = true, bool create_if_null = false)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Check if the file exists, or does so after creation.
      if (InternalCheckFile(ref filepath, create_if_null))
      {
        if (!CreateTempFilePath(out string temp_path))
        {
          yield return false;
          yield break;
        }
        ET_File temp_file = new ET_File(temp_path, false);

        // Copy the file over to the temp file.
        ET_Routine copy_routine = ET_Routine.CreateRoutine<bool>(InternalCopyFileAsync(file, temp_file, true));
        yield return copy_routine;
        if (copy_routine.value is bool && !(bool)copy_routine.value)
        {
          yield return false;
          yield break;
        }

        bool stream_made = true; // A bool determining if the stream was successfully made.
        bool write_good = true; // A bool determining if the write is still valid.

        // Attempt to make the StreamWriter for the temporary path.
        StreamWriter streamwriter_temp = null;
        try
        {
          streamwriter_temp = new StreamWriter(temp_path, true);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream failed to be made, return a null string and return false.
        if (!stream_made)
        {

          yield return false;
          yield break;
        }

        using (streamwriter_temp)
        {
          try
          {
            // If appending a newline, use WriteLine. Otherwise, just use Write.
            if (append_newline)
              streamwriter_temp.WriteLine(message);
            else
              streamwriter_temp.Write(message);
          }
          catch
          {
            write_good = false; // If an error occurs, the write is no longer good.
          }
        }

        // If the write was good, move the temporary file to overwrite the old file.
        if (write_good)
        {
          ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
          yield return move_routine;
          yield return move_routine.value;
        }
        else
        {
          // If the write was bad, just delete the old path.
          InternalDeleteFile(ref filepath);
          yield return false;
        }
      }
      else
      {
        yield return false; // The file does not exist. Return false immediately.
      }
    }

    /// <summary>
    /// A function which appends a string to a given file to a given file via a routine.
    /// This version will first create a temporary file and attempt to append the string before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="message">The message to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    public static IEnumerator AppendStringToFileSafelyAsync(ET_File file, string message, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null || message == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalAppendStringToFileSafelyAsync(file, message, append_newline, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to append several strings to a given file. All cleanup is handled in the public function.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendStringsToFile(ref string filepath, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false)
    {
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Using a stream writer, write each individual line to the file. 'WriteLine' appends a newline automatically.
          using (StreamWriter stream = new StreamWriter(filepath, true))
          {
            foreach (string m in messages)
            {
              if (m != string.Empty)
              {
                if (append_newline)
                  stream.WriteLine(m);
                else
                  stream.Write(m);
              }
            }
          }

          return true; // The file was appended to.
        }
      }
      catch
      {
        // If any error occurs, return false.
        return false;
      }

      return false; // The file was not appended to.
    }

    /// <summary>
    /// A function which appends several strings to a given file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringsToFile(ref string filepath, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      if (filepath == null || messages == null)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      return InternalAppendStringsToFile(ref filepath, messages, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends several strings to a given file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringsToFile(ref string directory, ref string filename, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Concatenate the path and attempt to append to the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendStringsToFile(ref path, messages, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends several strings to a given file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringsToFile(ET_File file, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the strings to the file.
      return file != null ? AppendStringsToFile(ref file.directory, ref file.filename, messages, append_newline, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append a given group of strings to a given file via a routine. All cleanup is handled in the public function.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static IEnumerator InternalAppendStringsToFileAsync(ET_File file, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Check if the file exists, or does so after being created.
      if (InternalCheckFile(ref filepath, create_if_null))
      {
        bool stream_made = true; // A bool determining if the StreamWriter was correctly made.
        StreamWriter stream_writer = null; // The StreamWriter for the target file.
        // Attempt to create the StreamWriter. If it fails, return false immediately.
        try
        {
          stream_writer = new StreamWriter(filepath, true);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        bool write_good = true; // A bool determining if the file is successfully appended to.
        int loop_counter = 0; // A counter for the number of appends made before yielding.
        using (stream_writer)
        {
          // Append each string individually.
          foreach (string s in messages)
          {
            try
            {
              // If appending a newline, use WriteLine. Otherwise, just use Write.
              if (append_newline)
                stream_writer.WriteLine(s);
              else
                stream_writer.Write(s);
            }
            catch
            {
              write_good = false; // If an error occurs, the write is no longer good.
              break;
            }
            // If we've hit the max number of loops, yield the routine.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxAccessLoopCount))
              yield return null;
          }
        }
        yield return write_good; // Return if the write was good.
      }
      else
      {
        yield return false; // The file did not exist.
      }
    }

    /// <summary>
    /// A function which appends a given group of strings to a given file via a routine.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static IEnumerator AppendStringsToFileAsync(ET_File file, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null || messages == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalAppendStringsToFileAsync(file, messages, append_newline, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to append several strings to a given file. All cleanup is handled in the public function.
    /// This version will first create a temporary file and attempt to append the strings before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    private static bool InternalAppendStringsToFileSafely(ref string filepath, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false)
    {
      // If a temporary file cannot be made, return false immediately.
      if (!CreateTempFilePath(out string temp_path))
        return false;

      try
      {
        // Make sure the file exists, or at least was created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Attempt to copy the file to the temporary file.
          if (InternalCopyFile(ref filepath, ref temp_path, true))
          {
            // Using a file stream, append the strings to the file.
            using (StreamWriter streamwriter_temp = new StreamWriter(temp_path, true))
            {
              foreach (string m in messages)
              {
                if (m != string.Empty)
                {
                  if (append_newline)
                    streamwriter_temp.WriteLine(m);
                  else
                    streamwriter_temp.Write(m);
                }
              }
            }

            return InternalMoveFile(ref temp_path, ref filepath, true); // Move the file from the temporary location to the original location.
          }

          return false; // The file could not be copied.
        }
      }
      catch
      {
        // If any error occurs, return false.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false; // The file could not be appended to.
    }

    /// <summary>
    /// A function which appends several strings to a given file.
    /// This version will first create a temporary file and attempt to append the strings before overwriting the original file.
    /// </summary>
    /// <param name="filepath">The file path to go to.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringsToFileSafely(ref string filepath, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      if (filepath == null || messages == null)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      return InternalAppendStringsToFileSafely(ref filepath, messages, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends several strings to a given file.
    /// This version will first create a temporary file and attempt to append the strings before overwriting the original file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringsToFileSafely(ref string directory, ref string filename, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Concatenate the path and attempt to append to the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalAppendStringsToFileSafely(ref path, messages, append_newline, create_if_null);
    }

    /// <summary>
    /// A function which appends several strings to a given file.
    /// This version will first create a temporary file and attempt to append the strings before overwriting the original file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully appended to.</returns>
    public static bool AppendStringsToFileSafely(ET_File file, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // Attempt to append the strings to the file.
      return file != null ? AppendStringsToFileSafely(ref file.directory, ref file.filename, messages, append_newline, create_if_null, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append several strings to a given file via a routine. All cleanup is handled in the public function.
    /// This version will first create a temporary file and attempt to append the strings before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    private static IEnumerator InternalAppendStringsToFileSafelyAsync(ET_File file, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false)
    {
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Check if the file exists, or does so after creation.
      if (InternalCheckFile(ref filepath, create_if_null))
      {
        if (!CreateTempFilePath(out string temp_path))
        {
          yield return false;
          yield break;
        }
        ET_File temp_file = new ET_File(temp_path, false);

        // Copy the file over to the temp file.
        ET_Routine copy_routine = ET_Routine.CreateRoutine<bool>(InternalCopyFileAsync(file, temp_file, true));
        yield return copy_routine;
        if (copy_routine.value is bool && !(bool)copy_routine.value)
        {
          yield return false;
          yield break;
        }

        bool stream_made = true; // A bool determining if the stream was successfully made.
        bool write_good = true; // A bool determining if the write is still valid.

        // Attempt to make the StreamWriter for the temporary path.
        StreamWriter streamwriter_temp = null;
        try
        {
          streamwriter_temp = new StreamWriter(temp_path, true);
        }
        catch
        {
          stream_made = false;
        }

        // If the stream failed to be made, return a null string and return false.
        if (!stream_made)
        {

          yield return false;
          yield break;
        }

        using (streamwriter_temp)
        {
          int loop_counter = 0;
          // Append each string individually.
          foreach (string s in messages)
          {
            try
            {
              // If appending a newline, use WriteLine. Otherwise, just use Write.
              if (append_newline)
                streamwriter_temp.WriteLine(s);
              else
                streamwriter_temp.Write(s);
            }
            catch
            {
              write_good = false; // If an error occurs, the write is no longer good.
              break;
            }
            // If we've hit the max number of loops, yield the routine.
            if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxAccessLoopCount))
              yield return null;
          }
        }
        // If the write was good, move the temporary file to overwrite the old file.
        if (write_good)
        {
          ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
          yield return move_routine;
          yield return move_routine.value;
        }
        else
        {
          // If the write was bad, just delete the old path.
          InternalDeleteFile(ref temp_path);
          yield return false;
        }
      }
      else
      {
        yield return false; // The file does not exist. Return false immediately.
      }
    }

    /// <summary>
    /// A function which appends several strings to a given file via a routine.
    /// This version will first create a temporary file and attempt to append the strings before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="messages">The messages to append to the file.</param>
    /// <param name="append_newline">A bool determining if a terminating newline should be added to the message.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was appended to or not.</returns>
    public static IEnumerator AppendStringsToFileSafelyAsync(ET_File file, IEnumerable<string> messages, bool append_newline = true, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null || messages == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to append to the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalAppendStringsToFileSafelyAsync(file, messages, append_newline, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A helper function which inserts a line into a file at a designated place.
    /// This function creates a temporary file to insert the line.
    /// This version will create empty lines in the new file if the insertion is for a line that doesn't exist.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static bool InternalInsertStringToFileNull(ref string filepath, ES_FileLine insertion, bool create_if_null = false)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
        return false;
      BreakupFilePath(temp_path, out string temp_dir, out string temp_fname);
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Attempt to create the temporary file.
          if (InternalCreateFile(ref temp_dir, ref temp_path, true, true))
          {

            using (StreamReader streamreader_original = new StreamReader(filepath)) // The reader for the original file.
            {
              using (StreamWriter streamreader_temp = new StreamWriter(temp_path)) // The writer for the temp file.
              {
                // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, just write an empty line.
                for (long i = 0; i < insertion.index; i++)
                {
                  string line = string.Empty;
                  if ((line = streamreader_original.ReadLine()) != null)
                    streamreader_temp.WriteLine(line);
                  else
                    streamreader_temp.WriteLine(string.Empty);
                }

                // Insert the new line.
                streamreader_temp.WriteLine(insertion.line);

                // Write in all remaining lines.
                while (!streamreader_original.EndOfStream)
                {
                  streamreader_temp.WriteLine(streamreader_original.ReadLine());
                }
              }
            }

            // Delete the old file, and move the temp file in as the old file.
            InternalMoveFile(ref temp_path, ref filepath, true);
            return true;
          }
        }
      }
      catch
      {
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false;
    }

    /// <summary>
    /// A helper function which inserts a line into a file at a designated place.
    /// This function creates a temporary file to insert the line.
    /// This version will not insert if the line number does not exist in the original file.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static bool InternalInsertStringToFileNoNull(ref string filepath, ES_FileLine insertion, bool create_if_null = false)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
        return false;
      BreakupFilePath(temp_path, out string temp_dir, out string temp_fname);
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Attempt to create the temporary file.
          if (InternalCreateFile(ref temp_dir, ref temp_path, true, true))
          {
            using (StreamReader soriginal = new StreamReader(filepath)) // The reader for the original file.
            {
              bool temp_good = true; // A bool determining if the temp can be moved over, or if it should be deleted.

              using (StreamWriter stemp = new StreamWriter(temp_path)) // The writer for the temp file.
              {
                // Read up to the wanted index. Write all lines to the temp file.
                for (long i = 0; i < insertion.index; i++)
                {
                  // If the current line is not null, add it to the temp file. Otherwise, this means we cannot insert the line.
                  string line = string.Empty;
                  if ((line = soriginal.ReadLine()) != null)
                    stemp.WriteLine(line);
                  else
                  {
                    temp_good = false;
                    break;
                  }
                }

                if (temp_good)
                {
                  // Write the inserted line.
                  stemp.WriteLine(insertion.line);

                  // Write the rest of the file to the temporary file.
                  while (!soriginal.EndOfStream)
                  {
                    stemp.WriteLine(soriginal.ReadLine());
                  }
                }
              }

              if (!temp_good)
              {
                InternalDeleteFile(ref temp_path);
                return false;
              }
            }

            // Delete the old file and move in the new file.
            return InternalMoveFile(ref temp_path, ref filepath, true);
          }
        }
      }
      catch
      {
        // If any error occurs, delete the temp file if it was made and return false.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false; // The file was not successfully inserted into.
    }

    /// <summary>
    /// A function which inserts a given line into a file. This function creates a temporary file to insert the line.
    /// When inserting, the function will make the given sentence as the given index.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool InsertStringToFile(ref string filepath, ES_FileLine insertion, bool create_if_null = false, bool insert_if_null = false, bool cleanup = false)
    {
      if (filepath == null)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      // Insert based on whether or not we can insert into the file while null lines are found.
      if (insert_if_null)
        return InternalInsertStringToFileNull(ref filepath, insertion, create_if_null);
      else
        return InternalInsertStringToFileNoNull(ref filepath, insertion, create_if_null);
    }

    /// <summary>
    /// A function which inserts a given line into a file. This function creates a temporary file to insert the line.
    /// When inserting, the function will make the given sentence as the given index.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool InsertStringToFile(ref string directory, ref string filename, ES_FileLine insertion, bool create_if_null = false, bool insert_if_null = false, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to append to the file.
      string path = CreateFilePath(new string[] { directory, filename });
      // Insert based on whether or not we can insert into the file while null lines are found.
      if (insert_if_null)
        return InternalInsertStringToFileNull(ref path, insertion, create_if_null);
      else
        return InternalInsertStringToFileNoNull(ref path, insertion, create_if_null);
    }

    /// <summary>
    /// A function which inserts a given line into a file. This function creates a temporary file to insert the line.
    /// When inserting, the function will make the given sentence as the given index.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool InsertStringToFile(ET_File file, ES_FileLine insertion, bool create_if_null = false, bool insert_if_null = false, bool cleanup = false)
    {
      // Attempt to insert a line into the file.
      return file != null ? InsertStringToFile(ref file.directory, ref file.filename, insertion, create_if_null, insert_if_null, cleanup) : false;
    }

    /// <summary>
    /// A helper function which inserts a line into a file at a designated place via a routine.
    /// This function creates a temporary file to insert the line.
    /// This version will create empty lines in the new file if the insertion is for a line that doesn't exist.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was successfully edited.</returns>
    private static IEnumerator InternalInsertStringToFileAsyncNull(ET_File file, ES_FileLine insertion, bool create_if_null = false)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
      {
        yield return false;
        yield break;
      }
      ET_File temp_file = new ET_File(temp_path, false); // Create a new file info item for later.
      string filepath = CreateFilePath(file); // Concatenate the path and attempt to read the file.

      // Check if the file exists, or does after being created.
      if (InternalCheckFile(ref filepath, create_if_null) && InternalCreateFile(ref temp_file.directory, ref temp_path, true, true))
      {
        bool stream_made = true; // A bool determining if the stream reader was successfully made.
        bool insertion_good = true; // A bool determining if the string insertion is still good.
        StreamReader streamreader_original = null; // The stream reader for the original file.
        // Attempt to create the stream reader. If not successful, yield false and end the function.
        try
        {
          streamreader_original = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (streamreader_original)
        {
          StreamWriter streamwriter_temp = null; // The stream writer for the temporary file.
          // Attempt to create the stream writer. If not successful, yield false and end the function.
          try
          {
            streamwriter_temp = new StreamWriter(temp_path);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            streamreader_original.Dispose(); // Make sure to dispose of the original stream reader!
            yield return false;
            yield break;
          }

          using (streamwriter_temp)
          {
            int loop_counter = 0; // The loop counter for the insertion loop.
            // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, just write an empty line.
            for (long i = 0; i < insertion.index; i++)
            {
              try
              {
                string line; // The currently read line.
                // Write an empty line if the line is null.
                if ((line = streamreader_original.ReadLine()) != null)
                  streamwriter_temp.WriteLine(line);
                else
                  streamwriter_temp.WriteLine(string.Empty);
              }
              catch
              {
                // If an error occurs, the insertion is no longer good.
                insertion_good = false;
                break;
              }
              // Yield the loop if the counter reaches max.
              if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                yield return null;
            }

            // If the insertion is still valid, insert the new line.
            if (insertion_good)
            {
              try
              {
                // Insert the new line.
                streamwriter_temp.WriteLine(insertion.line);
              }
              catch
              {
                // If an error occurs, the insertion is no longer valid.
                insertion_good = false;
              }
            }
            
            // If the insertion is still valid, write the remaining lines to the temporary file.
            if (insertion_good)
            {
              loop_counter = 0; // Reset the loop counter.
              // Write in all remaining lines.
              while (!streamreader_original.EndOfStream)
              {
                try
                {
                  streamwriter_temp.WriteLine(streamreader_original.ReadLine());
                }
                catch
                {
                  // If an error occurs, the insertion is no longer valid.
                  insertion_good = false;
                  break;
                }
                // Yield the loop if the counter reaches max.
                if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                  yield return null;
              }
            }
          }
        }

        // If the insertion is no longer valid, delete the temporary file and yield false.
        if (!insertion_good)
        {
          InternalDeleteFile(ref temp_path);
          yield return false;
          yield break;
        }

        // If the write was good, move the temporary file to overwrite the old file.
        if (insertion_good)
        {
          ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
          yield return move_routine;
          yield return move_routine.value;
        }
      }
      else
      {
        // The file was not found. Yield false.
        yield return false;
      }
    }

    /// <summary>
    /// A helper function which inserts a line into a file at a designated place via a routine.
    /// This function creates a temporary file to insert the line.
    /// This version will not insert if the line number does not exist in the original file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was successfully edited.</returns>
    private static IEnumerator InternalInsertStringToFileAsyncNoNull(ET_File file, ES_FileLine insertion, bool create_if_null = false)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
      {
        yield return false;
        yield break;
      }
      ET_File temp_file = new ET_File(temp_path, false); // Create a new file info item for later.
      // Concatenate the path and attempt to read the file.
      string filepath = CreateFilePath(file);

      // Check if the file exists, or does after being created.
      if (InternalCheckFile(ref filepath, create_if_null) && InternalCreateFile(ref temp_file.directory, ref temp_path, true, true))
      {
        bool stream_made = true; // A bool determining if the stream reader was successfully made.
        bool insertion_good = true; // A bool determining if the string insertion is still good.
        StreamReader streamreader_original = null; // The stream reader for the original file.
        // Attempt to create the stream reader. If not successful, yield false and end the function.
        try
        {
          streamreader_original = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (streamreader_original)
        {
          StreamWriter streamwriter_temp = null; // The stream writer for the temporary file.
          // Attempt to create the stream writer. If not successful, yield false and end the function.
          try
          {
            streamwriter_temp = new StreamWriter(temp_path);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            streamreader_original.Dispose(); // Make sure to dispose of the original stream reader!
            yield return false;
            yield break;
          }

          using (streamwriter_temp)
          {
            int loop_counter = 0; // The loop counter for the insertion loop.
            // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, quit immediately.
            for (long i = 0; i < insertion.index; i++)
            {
              try
              {
                string line; // The currently read line.
                // If the line is null, the file is too short. The insertion is no longer valid.
                if ((line = streamreader_original.ReadLine()) != null)
                  streamwriter_temp.WriteLine(line);
                else
                {
                  insertion_good = false;
                  break;
                }
              }
              catch
              {
                // If an error occurs, the insertion is no longer good.
                insertion_good = false;
                break;
              }
              // Yield the loop if the counter reaches max.
              if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                yield return null;
            }

            // If the insertion is still valid, insert the new line.
            if (insertion_good)
            {
              try
              {
                // Insert the new line.
                streamwriter_temp.WriteLine(insertion.line);
              }
              catch
              {
                // If an error occurs, the insertion is no longer valid.
                insertion_good = false;
              }
            }

            // If the insertion is still valid, write the remaining lines to the temporary file.
            if (insertion_good)
            {
              loop_counter = 0; // Reset the loop counter.
              // Write in all remaining lines.
              while (!streamreader_original.EndOfStream)
              {
                try
                {
                  streamwriter_temp.WriteLine(streamreader_original.ReadLine());
                }
                catch
                {
                  // If an error occurs, the insertion is no longer valid.
                  insertion_good = false;
                  break;
                }
                // Yield the loop if the counter reaches max.
                if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                  yield return null;
              }
            }
          }
        }

        // If the insertion is no longer valid, delete the temporary file and yield false.
        if (!insertion_good)
        {
          InternalDeleteFile(ref temp_path);
          yield return false;
          yield break;
        }

        // If the write was good, move the temporary file to overwrite the old file.
        if (insertion_good)
        {
          ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
          yield return move_routine;
          yield return move_routine.value;
        }
      }
      else
      {
        // The file was not found. Yield false.
        yield return false;
      }
    }

    /// <summary>
    /// A function which inserts a given line into a file. This function creates a temporary file to insert the line via a routine.
    /// When inserting, the function will make the given sentence as the given index.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was successfully edited.</returns>
    public static IEnumerator InsertStringToFileAsync(ET_File file, ES_FileLine insertion, bool insert_if_null = false, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to insert into the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine;
      if (insert_if_null)
        internal_routine = ET_Routine.CreateRoutine<bool>(InternalInsertStringToFileAsyncNull(file, insertion, create_if_null));
      else
        internal_routine = ET_Routine.CreateRoutine<bool>(InternalInsertStringToFileAsyncNoNull(file, insertion, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A helper function which inserts several lines into a file at a designated place.
    /// This function creates a temporary file to insert the line.
    /// This version will create empty lines in the new file if the insertion is for a line that doesn't exist.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="insertion">The lines and placements to insert into the file. This will insert the lines at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static bool InternalInsertStringsToFileNull(ref string filepath, IList<ES_FileLine> insertion, bool create_if_null = false)
    {
      // If we cannot create a temp file, return false.
      if (!CreateTempFilePath(out string temp_path))
        return false;
      BreakupFilePath(temp_path, out string temp_dir, out string temp_fname);
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Attempt to create the temporary file.
          if (InternalCreateFile(ref temp_dir, ref temp_path, true, true))
          {
            // Sort the lines from least to greatest.
            IEnumerable<ES_FileLine> sorted_lines = insertion.OrderBy(line => line.index);

            // We do not want to process this if trying to insert past the maximum file line.
            if (sorted_lines.Last().index > file_MaxLineAccess)
              return false;

            using (StreamReader soriginal = new StreamReader(filepath)) // The reader for the original file.
            {
              using (StreamWriter stemp = new StreamWriter(temp_path)) // The writer for the temp file.
              {
                long current_line = 0; // The current line number.
                long last_line = -100; // The previous line index searched.

                foreach (ES_FileLine line in sorted_lines)
                {
                  // Simply insert the line again if we already have the current index.
                  if (last_line == line.index)
                    stemp.WriteLine(line.line);
                  else
                  {
                    // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, just write an empty line.
                    while (current_line < line.index)
                    {
                      string message = string.Empty;
                      if ((message = soriginal.ReadLine()) != null)
                        stemp.WriteLine(message);
                      else
                        stemp.WriteLine(string.Empty);

                      current_line++;
                    }

                    stemp.WriteLine(line.line);
                    last_line = line.index;
                  }
                }

                // Write in all remaining lines.
                while (!soriginal.EndOfStream)
                {
                  string message = string.Empty;
                  if ((message = soriginal.ReadLine()) != null)
                    stemp.WriteLine(message);
                }
              }
            }

            // Delete the old file, and move the temp file in as the old file.
            return InternalMoveFile(ref temp_path, ref filepath, true);
          }
        }
      }
      catch
      {
        // Delete the temporary file if it existed.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false;
    }

    /// <summary>
    /// A helper function which inserts several lines into a file at a designated place.
    /// This function creates a temporary file to insert the line.
    /// This version will not insert if the line number does not exist in the original file.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="insertion">The lines and placements to insert into the file. This will insert the lines at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static bool InternalInsertStringsToFileNoNull(ref string filepath, IList<ES_FileLine> insertion, bool create_if_null = false)
    {
      // If we cannot create a temp file, return false.
      if (!CreateTempFilePath(out string temp_path))
        return false;
      BreakupFilePath(temp_path, out string temp_dir, out string temp_fname);
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, create_if_null))
        {
          // Attempt to create the temporary file.
          if (InternalCreateFile(ref temp_dir, ref temp_path, true, true))
          {
            // Sort the lines in order from least to greatest.
            IEnumerable<ES_FileLine> sorted_lines = insertion.OrderBy(line => line.index);

            // We don't want to go past the max access point, just in case. Return false if the attempt is made.
            if (sorted_lines.Last().index > file_MaxLineAccess)
              return false;

            using (StreamReader soriginal = new StreamReader(filepath)) // The reader for the original file.
            {
              bool temp_good = true; // A bool determining if the temp can be moved over, or if it should be deleted.

              using (StreamWriter stemp = new StreamWriter(temp_path)) // The writer for the temp file.
              {
                long current_line = 0; // The current line number.
                long last_line = -100; // The previous line index searched.

                foreach (ES_FileLine line in sorted_lines)
                {
                  // Simply insert the line again if we already have the current index.
                  if (last_line == line.index)
                    stemp.WriteLine(line.line);
                  else
                  {
                    // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, we want to quit this insertion.
                    while (current_line < line.index)
                    {
                      string message = string.Empty;
                      if ((message = soriginal.ReadLine()) != null)
                        stemp.WriteLine(message);
                      else
                      {
                        temp_good = false;
                        break;
                      }

                      current_line++; // Increment the line count.
                    }

                    if (!temp_good)
                      break;

                    stemp.WriteLine(line.line);
                    last_line = line.index;
                  }
                }

                if (temp_good)
                {
                  // Write in all remaining lines.
                  while (!soriginal.EndOfStream)
                  {
                    string message = string.Empty;
                    if ((message = soriginal.ReadLine()) != null)
                      stemp.WriteLine(message);
                  }
                }
              }

              if (!temp_good)
              {
                InternalDeleteFile(ref temp_path);
                return false;
              }
            }

            // Delete the old file, and move the temp file in as the old file.
            InternalMoveFile(ref temp_path, ref filepath, true);
            return true;
          }
        }
      }
      catch
      {
        // Delete the temporary file if it existed.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false;
    }

    /// <summary>
    /// A function which inserts several lines into a file at a designated place.
    /// This function creates a temporary file to insert the line.
    /// When inserting, the function will insert at the indexes of the UNEDITED file.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="insertion">The lines and placements to insert into the file. This will insert the lines at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool InsertStringsToFile(ref string filepath, IList<ES_FileLine> insertion, bool create_if_null = false, bool insert_if_null = false, bool cleanup = false)
    {
      if (filepath == null)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      if (insert_if_null)
        return InternalInsertStringsToFileNull(ref filepath, insertion, create_if_null);
      else
        return InternalInsertStringsToFileNoNull(ref filepath, insertion, create_if_null);
    }

    /// <summary>
    /// A function which inserts several lines into a file at a designated place.
    /// This function creates a temporary file to insert the line.
    /// When inserting, the function will insert at the indexes of the UNEDITED file.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="insertion">The lines and placements to insert into the file. This will insert the lines at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool InsertStringsToFile(ref string directory, ref string filename, IList<ES_FileLine> insertion, bool create_if_null = false, bool insert_if_null = false, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);
      // Concatenate the path and attempt to insert into the file.
      string path = CreateFilePath(new string[] { directory, filename });
      if (insert_if_null)
        return InternalInsertStringsToFileNull(ref path, insertion, create_if_null);
      else
        return InternalInsertStringsToFileNoNull(ref path, insertion, create_if_null);
    }

    /// <summary>
    /// A function which inserts several lines into a file at a designated place.
    /// This function creates a temporary file to insert the line.
    /// When inserting, the function will insert at the indexes of the UNEDITED file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The lines and placements to insert into the file. This will insert the lines at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool InsertStringsToFile(ET_File file, IList<ES_FileLine> insertion, bool create_if_null = false, bool insert_if_null = false, bool cleanup = false)
    {
      return file != null ? InsertStringsToFile(ref file.directory, ref file.filename, insertion, create_if_null, insert_if_null, cleanup) : false;
    }

    /// <summary>
    /// A helper function which inserts several lines into a file at a designated place via a routine.
    /// This function creates a temporary file to insert the lines.
    /// This version will create empty lines in the new file if the insertion is for a line that doesn't exist.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static IEnumerator InternalInsertStringsToFileAsyncNull(ET_File file, IList<ES_FileLine> insertion, bool create_if_null = false)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
      {
        yield return false;
        yield break;
      }
      ET_File temp_file = new ET_File(temp_path, false); // Create a file info item for later.
      string filepath = CreateFilePath(file); // Concatenate the path and attempt to read the file.

      // Check if the file exists, or does after being created.
      if (InternalCheckFile(ref filepath, create_if_null) && InternalCreateFile(ref temp_file.directory, ref temp_path, true, true))
      {
        bool stream_made = true; // A bool determining if the stream reader was successfully made.
        bool insertion_good = true; // A bool determining if the string insertion is still good.
        IList<ES_FileLine> sorted_lines = new List<ES_FileLine>(insertion); // The list of lines, duplicated as to not affect the original.
        // If the lines are not already sorted, sort the lines using Quick Sort.
        if (!sorted_lines.IsSorted(EKIT_Sort.CompareFileLinestLeastToGreatest))
          yield return EKIT_Sort.QuickSortAsync(sorted_lines, EKIT_Sort.CompareFileLinestLeastToGreatest);

        StreamReader streamreader_original = null; // The stream reader for the original file.
        // Attempt to create the stream reader. If not successful, yield false and end the function.
        try
        {
          streamreader_original = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (streamreader_original)
        {
          StreamWriter streamwriter_temp = null; // The stream writer for the temporary file.
          // Attempt to create the stream writer. If not successful, yield false and end the function.
          try
          {
            streamwriter_temp = new StreamWriter(temp_path);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            streamreader_original.Dispose(); // Make sure to dispose of the original stream reader!
            yield return false;
            yield break;
          }

          using (streamwriter_temp)
          {
            long current_line = 0; // The current line number.
            long last_line = -100; // The previous line index searched.
            int loop_counter = 0; // The loop counter for the insertion loop.

            // Go through every line in order of the sort.
            foreach (ES_FileLine line in sorted_lines)
            {
              // Simply insert the line again if we already have the current index.
              if (last_line == line.index)
                streamwriter_temp.WriteLine(line.line);
              else
              {
                // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, just write an empty line.
                while (current_line < line.index)
                {
                  try
                  {
                    string message; // The currently read line.
                    // Write an empty line if the line is null.
                    if ((message = streamreader_original.ReadLine()) != null)
                      streamwriter_temp.WriteLine(message);
                    else
                      streamwriter_temp.WriteLine(string.Empty);
                  }
                  catch
                  {
                    // If an error occurs, the insertion is no longer good.
                    insertion_good = false;
                    break;
                  }
                  current_line++; // Increment the current line index.
                  // Yield the loop if the counter reaches max.
                  if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                    yield return null;
                }
                // If the insertion is still valid, insert the new line.
                if (insertion_good)
                {
                  try
                  {
                    streamwriter_temp.WriteLine(line.line); // Insert the new line.
                    last_line = line.index; // Update the last line index we inserted into.
                  }
                  catch
                  {
                    // If an error occurs, the insertion is no longer valid.
                    insertion_good = false;
                  }
                }
                // If the insertion is no longer valid, break out of the loop.
                if (!insertion_good)
                  break;
              }
            }

            // If the insertion is still valid, write the remaining lines to the temporary file.
            if (insertion_good)
            {
              loop_counter = 0; // Reset the loop counter.
              // Write in all remaining lines.
              while (!streamreader_original.EndOfStream)
              {
                try
                {
                  streamwriter_temp.WriteLine(streamreader_original.ReadLine());
                }
                catch
                {
                  // If an error occurs, the insertion is no longer valid.
                  insertion_good = false;
                  break;
                }
                // Yield the loop if the counter reaches max.
                if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                  yield return null;
              }
            }
          }
        }

        // If the insertion is no longer valid, delete the temporary file and yield false.
        if (!insertion_good)
        {
          InternalDeleteFile(ref temp_path);
          yield return false;
          yield break;
        }

        // Delete the old file, and move the temp file in as the old file.
        ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
        yield return move_routine;
        yield return move_routine.value;
      }
      else
      {
        // The file was not found. Yield false.
        yield return false;
      }
    }

    /// <summary>
    /// A helper function which inserts several lines into a file at a designated place via a routine.
    /// This function creates a temporary file to insert the lines.
    /// This version will not insert if the line number does not exist in the original file.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static IEnumerator InternalInsertStringsToFileAsyncNoNull(ET_File file, IList<ES_FileLine> insertion, bool create_if_null = false)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
      {
        yield return false;
        yield break;
      }
      ET_File temp_file = new ET_File(temp_path, false); // Create a file info item for later.
      string filepath = CreateFilePath(file); // Concatenate the path and attempt to read the file.

      // Check if the file exists, or does after being created.
      if (InternalCheckFile(ref filepath, create_if_null) && InternalCreateFile(ref temp_file.directory, ref temp_path, true, true))
      {
        bool stream_made = true; // A bool determining if the stream reader was successfully made.
        bool insertion_good = true; // A bool determining if the string insertion is still good.
        IList<ES_FileLine> sorted_lines = new List<ES_FileLine>(insertion); // The list of lines, duplicated as to not affect the original.
        // If the lines are not already sorted, sort the lines using Quick Sort.
        if (!sorted_lines.IsSorted(EKIT_Sort.CompareFileLinestLeastToGreatest))
          yield return EKIT_Sort.QuickSortAsync(sorted_lines, EKIT_Sort.CompareFileLinestLeastToGreatest);

        StreamReader streamreader_original = null; // The stream reader for the original file.
        // Attempt to create the stream reader. If not successful, yield false and end the function.
        try
        {
          streamreader_original = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (streamreader_original)
        {
          StreamWriter streamwriter_temp = null; // The stream writer for the temporary file.
          // Attempt to create the stream writer. If not successful, yield false and end the function.
          try
          {
            streamwriter_temp = new StreamWriter(temp_path);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            streamreader_original.Dispose(); // Make sure to dispose of the original stream reader!
            yield return false;
            yield break;
          }

          using (streamwriter_temp)
          {
            long current_line = 0; // The current line number.
            long last_line = -100; // The previous line index searched.
            int loop_counter = 0; // The loop counter for the insertion loop.

            // Go through every line in order of the sort
            foreach (ES_FileLine line in sorted_lines)
            {
              // Simply insert the line again if we already have the current index.
              if (last_line == line.index)
                streamwriter_temp.WriteLine(line.line);
              else
              {
                // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, quit immediately.
                while (current_line < line.index)
                {
                  try
                  {
                    string message; // The currently read line.
                    // If the line is null, the insertion is no longer valid.
                    if ((message = streamreader_original.ReadLine()) != null)
                      streamwriter_temp.WriteLine(message);
                    else
                    {
                      insertion_good = false;
                      break;
                    }
                  }
                  catch
                  {
                    // If an error occurs, the insertion is no longer good.
                    insertion_good = false;
                    break;
                  }
                  current_line++; // Increment the current line index.
                  // Yield the loop if the counter reaches max.
                  if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                    yield return null;
                }

                // If the insertion is still valid, insert the new line.
                if (insertion_good)
                {
                  try
                  {
                    streamwriter_temp.WriteLine(line.line); // Insert the new line.
                    last_line = line.index; // Update the last line index we inserted into.
                  }
                  catch
                  {
                    // If an error occurs, the insertion is no longer valid.
                    insertion_good = false;
                  }
                }
                // If the insertion is no longer valid, break out of the loop.
                if (!insertion_good)
                  break;
              }
            }

            // If the insertion is still valid, write the remaining lines to the temporary file.
            if (insertion_good)
            {
              loop_counter = 0; // Reset the loop counter.
              // Write in all remaining lines.
              while (!streamreader_original.EndOfStream)
              {
                try
                {
                  streamwriter_temp.WriteLine(streamreader_original.ReadLine());
                }
                catch
                {
                  // If an error occurs, the insertion is no longer valid.
                  insertion_good = false;
                  break;
                }
                // Yield the loop if the counter reaches max.
                if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                  yield return null;
              }
            }
          }
        }

        // If the insertion is no longer valid, delete the temporary file and yield false.
        if (!insertion_good)
        {
          InternalDeleteFile(ref temp_path);
          yield return false;
          yield break;
        }

        // If the write was good, move the temporary file to overwrite the old file.
        ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
        yield return move_routine;
        yield return move_routine.value;
      }
      else
      {
        // The file was not found. Yield false.
        yield return false;
      }
    }

    /// <summary>
    /// A function which inserts several lines into a file at a designated place via a routine. This function creates a temporary file to insert the line.
    /// When inserting, the function will make the given sentence as the given index.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="insertion">The line and placement to insert into the file. This will insert the line at the index listed.</param>
    /// <param name="create_if_null">A bool determining if the file should be created if it does not already exist.</param>
    /// <param name="insert_if_null">A bool determining if the line should still be inserted if the old file is not long enough. If true, empty lines will be added
    /// until reaching the desired line.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static IEnumerator InsertStringsToFileAsync(ET_File file, IList<ES_FileLine> insertion, bool insert_if_null = false, bool create_if_null = false, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to insert into the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine;
      if (insert_if_null)
        internal_routine = ET_Routine.CreateRoutine<bool>(InternalInsertStringsToFileAsyncNull(file, insertion, create_if_null));
      else
        internal_routine = ET_Routine.CreateRoutine<bool>(InternalInsertStringsToFileAsyncNoNull(file, insertion, create_if_null));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// The internal function for attempting to edit a line in a file. All cleanup is handled in the public function.
    /// This function creates a temporary file to edit the line.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="edited_line">The index of the line and what to replace it with.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static bool InternalEditStringInFile(ref string filepath, ES_FileLine edited_line)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
        return false;
      BreakupFilePath(temp_path, out string temp_dir, out string temp_fname);
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, false))
        {
          // Attempt to create the temporary file.
          if (InternalCreateFile(ref temp_dir, ref temp_path, true, true))
          {
            using (StreamReader soriginal = new StreamReader(filepath)) // The reader for the original file.
            {
              bool temp_good = true; // A bool determining if the temp can be moved over, or if it should be deleted.

              using (StreamWriter stemp = new StreamWriter(temp_path)) // The writer for the temp file.
              {
                // Read up to the wanted index. Write all lines to the temp file.
                for (long i = 0; i < edited_line.index; i++)
                {
                  // If the current line is not null, add it to the temp file. Otherwise, this means we cannot insert the line.
                  string line = string.Empty;
                  if ((line = soriginal.ReadLine()) != null)
                    stemp.WriteLine(line);
                  else
                  {
                    temp_good = false;
                    break;
                  }
                }

                // If the temporary file is still valid, write the edited line.
                if (temp_good)
                {
                  
                  stemp.WriteLine(edited_line.line); // Write the edited line.
                  soriginal.ReadLine(); // Read ahead once to skip the overwritten line.
                  // Write the rest of the file to the temporary file.
                  while (!soriginal.EndOfStream)
                    stemp.WriteLine(soriginal.ReadLine());
                }
              }

              // If the temporary file is not valid, delete the temporary file.
              if (!temp_good)
              {
                InternalDeleteFile(ref temp_path);
                return false;
              }
            }

            // Delete the old file and move in the new file.
            return InternalMoveFile(ref temp_path, ref filepath, true);
          }
        }
      }
      catch
      {
        // If any error occurs, delete the temp file if it was made and return false.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false; // The file was not successfully inserted into.
    }

    /// <summary>
    /// A function which edits a line in a file.
    /// This function creates a temporary file to edit the line.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="edited_line">The index of the line and what to replace it with.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool EditStringInFile(ref string filepath, ES_FileLine edited_line, bool cleanup = false)
    {
      // If the filepath or index is bad, return false.
      if (filepath == null || edited_line.index < 0 || edited_line.index > file_MaxLineAccess)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      return InternalEditStringInFile(ref filepath, edited_line);
    }

    /// <summary>
    /// A function which edits a line in a file.
    /// This function creates a temporary file to edit the line.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="edited_line">The index of the line and what to replace it with.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool EditStringInFile(ref string directory, ref string filename, ES_FileLine edited_line, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to edit the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalEditStringInFile(ref path, edited_line);
    }

    /// <summary>
    /// A function which edits a line in a file.
    /// This function creates a temporary file to edit the line.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="edited_line">The index of the line and what to replace it with.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool EditStringInFile(ET_File file, ES_FileLine edited_line, bool cleanup = false)
    {
      // Attempt to edit the file.
      return file != null ? EditStringInFile(ref file.directory, ref file.filename, edited_line, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append a edit a line in a given file via a routine. All cleanup is handled in the public function.
    /// This will first create a temporary file and attempt to append the string before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="edited_line">The index of the line and what to replace it with.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was successfully edited.</returns>
    private static IEnumerator InternalEditStringInFileAsync(ET_File file, ES_FileLine edited_line)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
      {
        yield return false;
        yield break;
      }
      ET_File temp_file = new ET_File(temp_path, false); // Create a file info item for later.
      string filepath = CreateFilePath(file); // Concatenate the path and attempt to read the file.

      // Check if the file exists, or does after being created.
      if (InternalCheckFile(ref filepath, false) && InternalCreateFile(ref temp_file.directory, ref temp_path, true, true))
      {
        bool stream_made = true; // A bool determining if the stream reader/writer was successfully made.
        bool edit_good = true; // A bool determining if the string edit is still good.
        StreamReader streamreader_original = null; // The stream reader for the original file.
        // Attempt to create the stream reader. If not successful, yield false and end the function.
        try
        {
          streamreader_original = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (streamreader_original)
        {
          StreamWriter streamwriter_temp = null; // The stream reader for the temporary file.
          // Attempt to create the stream reader. If not successful, yield false and end the function.
          try
          {
            streamwriter_temp = new StreamWriter(temp_path);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            streamreader_original.Dispose(); // Make sure to dispose of the original stream reader!
            yield return false;
            yield break;
          }

          using (streamwriter_temp)
          {
            int loop_counter = 0; // The loop counter for the insertion loop.
            // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, quit immediately.
            for (long i = 0; i < edited_line.index; i++)
            {
              try
              {
                string line; // The currently read line.
                // If the line is null, the edit is no longer valid.
                if ((line = streamreader_original.ReadLine()) != null)
                  streamwriter_temp.WriteLine(line);
                else
                {
                  edit_good = false;
                  break;
                }
              }
              catch
              {
                // If an error occurs, the edit is no longer good.
                edit_good = false;
                break;
              }
              // Yield the loop if the counter reaches max.
              if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                yield return null;
            }

            // If the edit is still valid, insert the new line.
            if (edit_good)
            {
              try
              {
                streamwriter_temp.WriteLine(edited_line.line); // Insert the new line.
                streamreader_original.ReadLine(); // Read ahead to skip the overwritten line.
              }
              catch
              {
                // If an error occurs, the edit is no longer valid.
                edit_good = false;
              }
            }

            // If the edit is still valid, write the remaining lines to the temporary file.
            if (edit_good)
            {
              loop_counter = 0; // Reset the loop counter.
              // Write in all remaining lines.
              while (!streamreader_original.EndOfStream)
              {
                try
                {
                  streamwriter_temp.WriteLine(streamreader_original.ReadLine());
                }
                catch
                {
                  // If an error occurs, the edit is no longer valid.
                  edit_good = false;
                  break;
                }
                // Yield the loop if the counter reaches max.
                if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                  yield return null;
              }
            }
          }
        }

        // If the edit is no longer valid, delete the temporary file and yield false.
        if (!edit_good)
        {
          InternalDeleteFile(ref temp_path);
          yield return false;
          yield break;
        }

        // If the write was good, move the temporary file to overwrite the old file.
        ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
        yield return move_routine;
        yield return move_routine.value;
      }
      else
      {
        // The file was not found. Yield false.
        yield return false;
      }
    }

    /// <summary>
    /// A function which edits a line in a file via a routine. This function creates a temporary file to edit the line.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="edited_line">The index of the line and what to replace it with.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was successfully edited.</returns>
    public static IEnumerator EditStringInFileAsync(ET_File file, ES_FileLine edited_line, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to edit the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalEditStringInFileAsync(file, edited_line));
      yield return internal_routine;
      yield return internal_routine.value;
    }

    /// <summary>
    /// A function which edits several lines in a file.
    /// This function creates a temporary file to edit the lines.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="edited_lines">The lines to be edited. If multiple lines have the same index, the function will only use the first version of the edit.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    private static bool InternalEditStringsInFile(ref string filepath, IEnumerable<ES_FileLine> edited_lines)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
        return false;
      BreakupFilePath(temp_path, out string temp_dir, out string temp_fname);
      try
      {
        // Check if the file exists, or does after being created.
        if (InternalCheckFile(ref filepath, false))
        {
          // Attempt to create the temporary file.
          if (InternalCreateFile(ref temp_dir, ref temp_path, true, true))
          {
            IEnumerable<ES_FileLine> sorted_lines = edited_lines.OrderBy(line => line.index); // Sort the lines.

            // We don't want to go past the max access point, just in case. Return false if the attempt is made.
            if (sorted_lines.Last().index > file_MaxLineAccess)
              return false;

            using (StreamReader soriginal = new StreamReader(filepath)) // The reader for the original file.
            {
              bool temp_good = true; // A bool determining if the temp can be moved over, or if it should be deleted.

              using (StreamWriter stemp = new StreamWriter(temp_path)) // The writer for the temp file.
              {
                long current_line = 0; // The current line number.
                long last_line = -100; // The previous line index searched. Start this at the first editing line.
                string edit = string.Empty;

                foreach (ES_FileLine line in sorted_lines)
                {
                  // If already at the end of stream, we just want to stop here.
                  if (soriginal.EndOfStream)
                  {
                    temp_good = false;
                    break;
                  }

                  if (line.index != last_line)
                  {
                    // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, we are at the end of the file.
                    while (current_line < line.index)
                    {
                      string message = string.Empty;
                      if ((message = soriginal.ReadLine()) != null)
                        stemp.WriteLine(message);
                      else
                      {
                        temp_good = false;
                        break;
                      }

                      current_line++;
                    }

                    if (!temp_good)
                      break;


                    // Write up the new line to the temp file instead, and read forward one line.
                    stemp.WriteLine(line.line);
                    soriginal.ReadLine();
                    current_line++;
                    last_line = line.index;
                  }
                }


                if (temp_good)
                {
                  // Write in all remaining lines.
                  while (!soriginal.EndOfStream)
                    stemp.WriteLine(soriginal.ReadLine());
                }
              }

              // If the temporary file failed, and we want to nullify changes, delete the temp file.
              if (!temp_good)
              {
                InternalDeleteFile(ref temp_path);
                return false;
              }
            }

            // Delete the old file and move in the new file.
            return InternalMoveFile(ref temp_path, ref filepath, true);
          }
        }
      }
      catch
      {
        // If any error occurs, delete the temp file if it was made and return false.
        InternalDeleteFile(ref temp_path);
        return false;
      }

      return false; // The file was not successfully inserted into.
    }

    /// <summary>
    /// A function which edits several lines in a file.
    /// This function creates a temporary file to edit the lines.
    /// </summary>
    /// <param name="filepath">The filepath of the file to edit.</param>
    /// <param name="edited_lines">The lines to be edited. If multiple lines have the same index, the function will only use the first version of the edit.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool EditStringsInFile(ref string filepath, IEnumerable<ES_FileLine> edited_lines, bool cleanup = false)
    {
      // If the lines do not exist, return false immediately.
      if (filepath == null || edited_lines == null || edited_lines.Count() <= 0)
        return false;
      if (cleanup)
        CleanupFilePath(ref filepath);
      return InternalEditStringsInFile(ref filepath, edited_lines);
    }

    /// <summary>
    /// A function which edits several lines in a file.
    /// This function creates a temporary file to edit the lines.
    /// </summary>
    /// <param name="directory">The directory of the file.</param>
    /// <param name="filename">The name of the file, extension included.</param>
    /// <param name="edited_lines">The lines to be edited. If multiple lines have the same index, the function will only use the first version of the edit.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool EditStringsInFile(ref string directory, ref string filename, IEnumerable<ES_FileLine> edited_lines, bool cleanup = false)
    {
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(ref directory, ref filename);

      // Concatenate the path and attempt to edit the file.
      string path = CreateFilePath(new string[] { directory, filename });
      return InternalEditStringsInFile(ref path, edited_lines);
    }

    /// <summary>
    /// A function which edits several lines in a file.
    /// This function creates a temporary file to edit the lines.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="edited_lines">The lines to be edited. If multiple lines have the same index, the function will only use the first version of the edit.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns if the file was successfully edited.</returns>
    public static bool EditStringsInFile(ET_File file, IEnumerable<ES_FileLine> edited_lines, bool cleanup = false)
    {
      // Attempt to edit the file.
      return file != null ? EditStringsInFile(ref file.directory, ref file.filename, edited_lines, cleanup) : false;
    }

    /// <summary>
    /// The internal function for attempting to append a edit several lines in a given file via a routine. All cleanup is handled in the public function.
    /// This will first create a temporary file and attempt to append the string before overwriting the original file.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="edited_lines">The lines to be edited. If multiple lines have the same index, the function will only use the first version of the edit.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was successfully edited.</returns>
    private static IEnumerator InternalEditStringsInFileAsync(ET_File file, IList<ES_FileLine> edited_lines)
    {
      // Create a temporary file. If something goes wrong, we don't edit the first file.
      if (!CreateTempFilePath(out string temp_path))
      {
        yield return false;
        yield break;
      }
      ET_File temp_file = new ET_File(temp_path, false); // Create a file info item for later.
      string filepath = CreateFilePath(file); // Concatenate the path and attempt to read the file.

      // Check if the file exists, or does after being created.
      if (InternalCheckFile(ref filepath, false) && InternalCreateFile(ref temp_file.directory, ref temp_path, true, true))
      {
        bool stream_made = true; // A bool determining if the stream reader/writer was successfully made.
        bool edit_good = true; // A bool determining if the string edit is still good.
        IList<ES_FileLine> sorted_lines = new List<ES_FileLine>(edited_lines); // The list of lines, duplicated as to not affect the original.
        // If the lines are not already sorted, sort the lines using Quick Sort.
        if (!sorted_lines.IsSorted(EKIT_Sort.CompareFileLinestLeastToGreatest))
          yield return EKIT_Sort.QuickSortAsync(sorted_lines, EKIT_Sort.CompareFileLinestLeastToGreatest);

        StreamReader streamreader_original = null; // The stream reader for the original file.
        // Attempt to create the stream reader. If not successful, yield false and end the function.
        try
        {
          streamreader_original = new StreamReader(filepath);
        }
        catch
        {
          stream_made = false;
        }

        if (!stream_made)
        {
          yield return false;
          yield break;
        }

        using (streamreader_original)
        {
          StreamWriter streamwriter_temp = null; // The stream reader for the temporary file.
          // Attempt to create the stream writer. If not successful, yield false and end the function.
          try
          {
            streamwriter_temp = new StreamWriter(temp_path);
          }
          catch
          {
            stream_made = false;
          }

          if (!stream_made)
          {
            streamreader_original.Dispose(); // Make sure to dispose of the original stream reader!
            yield return false;
            yield break;
          }

          using (streamwriter_temp)
          {
            long current_line = 0; // The current line number.
            long last_line = -100; // The previous line index searched.
            int loop_counter = 0; // The loop counter for the insertion loop.
            // Go through every line in order of the sort.
            foreach (ES_FileLine line in sorted_lines)
            {
              // Simply edit the line again if we already have the current index.
              if (last_line == line.index)
                streamwriter_temp.WriteLine(line.line);
              else
              {
                // Read up to the wanted index. Write all lines to the temp file. If a line doesn't exist, just write an empty line.
                while (current_line < line.index)
                {
                  try
                  {
                    string message; // The currently read line.
                    // If the line is null, the edit is no longer valid.
                    if ((message = streamreader_original.ReadLine()) != null)
                      streamwriter_temp.WriteLine(message);
                    else
                    {
                      edit_good = false;
                      break;
                    }
                  }
                  catch
                  {
                    // If an error occurs, the edit is no longer good.
                    edit_good = false;
                    break;
                  }
                  current_line++; // Increment the current line index.
                  // Yield the loop if the counter reaches max.
                  if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                    yield return null;
                }

                // If the edit is still valid, insert the new line.
                if (edit_good)
                {
                  try
                  {
                    streamwriter_temp.WriteLine(line.line); // Insert the new line.
                    streamreader_original.ReadLine(); // Read ahead to skip the overwritten line.
                    current_line++; // Increment the line index.
                    last_line = line.index; // Update the last line index we inserted into.
                  }
                  catch
                  {
                    // If an error occurs, the edit is no longer valid.
                    edit_good = false;
                  }
                }

                // If the edit is no longer good, exit the loop.
                if (!edit_good)
                  break;
              }
            }

            // If the edit is still valid, write the remaining lines to the temporary file.
            if (edit_good)
            {
              loop_counter = 0; // Reset the loop counter.
              // Write in all remaining lines.
              while (!streamreader_original.EndOfStream)
              {
                try
                {
                  streamwriter_temp.WriteLine(streamreader_original.ReadLine());
                }
                catch
                {
                  // If an error occurs, the edit is no longer valid.
                  edit_good = false;
                  break;
                }
                // Yield the loop if the counter reaches max.
                if (EKIT_Counter.IncrementCounter(ref loop_counter, file_MaxLoopCount))
                  yield return null;
              }
            }
          }
        }

        // If the edit is no longer valid, delete the temporary file and yield false.
        if (!edit_good)
        {
          InternalDeleteFile(ref temp_path);
          yield return false;
          yield break;
        }

        // If the write was good, move the temporary file to overwrite the old file.
        ET_Routine move_routine = ET_Routine.CreateRoutine<bool>(InternalMoveFileAsync(temp_file, file, true));
        yield return move_routine;
        yield return move_routine.value;
      }
      else
      {
        // The file was not found. Yield false.
        yield return false;
      }
    }

    /// <summary>
    /// A function which edit several lines in a file via a routine. This function creates a temporary file to edit the line.
    /// If using an ET_Routine, it is recommended to use the 'ReturnFirstAndBreak' return method.
    /// </summary>
    /// <param name="file">The file information to use.</param>
    /// <param name="edited_lines">The lines to be edited. If multiple lines have the same index, the function will only use the first version of the edit.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned. If you haven't before, you should now.</param>
    /// <returns>Returns an IEnumerator to use in an routine. If using an ET_Routine, returns a bool determining if the file was successfully edited.</returns>
    public static IEnumerator EditStringsInFileAsync(ET_File file, IList<ES_FileLine> edited_lines, bool cleanup = false)
    {
      // If any passed-in information is invalid, return false immediately.
      if (file == null)
      {
        yield return false;
        yield break;
      }
      // Clean up the file path, if requested.
      if (cleanup)
        CleanupFilePath(file);
      // Create an internal routine to edit the file. Doing it this way ensures this function can be used using ET_Routine or standard Unity Coroutines.
      ET_Routine internal_routine = ET_Routine.CreateRoutine<bool>(InternalEditStringsInFileAsync(file, edited_lines));
      yield return internal_routine;
      yield return internal_routine.value;
    }
  }
  /**********************************************************************************************************************/
  /**********************************************************************************************************************/
  /// <para>Class Name: ET_File</para>
  /// <summary>
  /// A type which contains information pointing to a file.
  /// </summary>
  [System.Serializable]
  public class ET_File
  {
    /// <summary> The directory of the file. This does not include the file's name and extension. </summary>
    public string directory;
    /// <summary> The file's name, including the extension. </summary>
    public string filename;

    /// <summary>
    /// A constructor for an ET_File. This passes in a full file path to use, which is broken up.
    /// </summary>
    /// <param name="filepath">The full filepath of the wanted file. This is broken up into its directory and filename.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned up. It is highly recommended to do so.</param>
    public ET_File(string filepath, bool cleanup = true)
    {
      EKIT_File.BreakupFilePath(filepath, out string dir, out string fi); // Break up the file path.

      // If cleaning up, clean up the file path.
      if (cleanup)
        EKIT_File.CleanupFilePath(ref dir, ref fi);

      // Set the information.
      directory = dir;
      filename = fi;
    }

    /// <summary>
    /// A constructor for an ET_File. This passes in the separated directory and filepath.
    /// </summary>
    /// <param name="d">The directory of the file.</param>
    /// <param name="f">The name and extension of the file.</param>
    /// <param name="cleanup">A bool determining if the file path should be cleaned up. It is highly recommended to do so.</param>
    public ET_File(string d, string f, bool cleanup = true)
    {
      // If cleaning up, clean up the file path.
      if (cleanup)
        EKIT_File.CleanupFilePath(ref d, ref f);

      // Set the information.
      directory = d;
      filename = f;
    }

    /// <summary>
    /// A function which converts information within the class into a string.
    /// </summary>
    /// <returns>Returns a representative string of the class's information.</returns>
    public override string ToString()
    {
      // Return a custom string representing the information inside.
      return "{ Directory: " + directory + ", File Name: " + filename + "}";
    }
  }
  /**********************************************************************************************************************/
}