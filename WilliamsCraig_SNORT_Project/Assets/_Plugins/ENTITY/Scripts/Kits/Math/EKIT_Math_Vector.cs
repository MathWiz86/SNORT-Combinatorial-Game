/**************************************************************************************************/
/*!
\file   EKIT_Math_Vector.cs
\author Craig Williams
\par    Unity Version: 2019.3.5
\par    Updated:
\par    Copyright: Copyright 2019-2020 Craig Joseph Williams

\brief  
  A toolkit for Math functions. This file includes functions for handling and mainpulating
  Vector2, Vector3, Vector4, Vector2Int, and Vector3Int variables.

\par References:
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  /**********************************************************************************************************************/
  public static partial class EKIT_Math
  {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// VECTOR 2

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2 vector, float minValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minValue || vector.y < minValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2 vector, Vector2 minVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minVector.x || vector.y < minVector.y);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2 vector, float minValue, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2 vector, float minValue, Vector2 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2 vector, Vector2 minVector, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2 vector, Vector2 minVector, Vector2 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2 vector, float maxValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxValue || vector.y > maxValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2 vector, Vector2 maxVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxVector.x || vector.y > maxVector.y);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2 vector, float maxValue, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2 vector, float maxValue, Vector2 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2 vector, Vector2 maxVector, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2 vector, Vector2 maxVector, Vector2 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, float maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, float maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, Vector2 maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, Vector2 maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, float maxValue, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, float maxValue, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, Vector2 maxVector, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, float maxValue, Vector2 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, float maxValue, float setMinValue, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, Vector2 maxVector, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, float maxValue, Vector2 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, float maxValue, float setMinValue, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, Vector2 maxVector, Vector2 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, Vector2 maxVector, float setMinValue, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, float maxValue, Vector2 setMinVector, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, Vector2 maxVector, Vector2 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, Vector2 maxVector, float setMinValue, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, float maxValue, Vector2 setMinVector, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, float minValue, Vector2 maxVector, Vector2 setMinVector, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2 vector, Vector2 minVector, Vector2 maxVector, Vector2 setMinVector, Vector2 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// VECTOR 3

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3 vector, float minValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minValue || vector.y < minValue || vector.z < minValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3 vector, Vector3 minVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minVector.x || vector.y < minVector.y || vector.z < minVector.z);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3 vector, float minValue, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minValue)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3 vector, float minValue, Vector3 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minValue)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3 vector, Vector3 minVector, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minVector.z)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3 vector, Vector3 minVector, Vector3 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minVector.z)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3 vector, float maxValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxValue || vector.y > maxValue || vector.z > maxValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3 vector, Vector3 maxVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxVector.x || vector.y > maxVector.y || vector.z > maxVector.z);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3 vector, float maxValue, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxValue)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3 vector, float maxValue, Vector3 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxValue)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3 vector, Vector3 maxVector, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxVector.z)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3 vector, Vector3 maxVector, Vector3 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxVector.z)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, float maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, float maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, Vector3 maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, Vector3 maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, float maxValue, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, float maxValue, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, Vector3 maxVector, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, float maxValue, Vector3 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, float maxValue, float setMinValue, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, Vector3 maxVector, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, float maxValue, Vector3 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, float maxValue, float setMinValue, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, Vector3 maxVector, Vector3 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, Vector3 maxVector, float setMinValue, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, float maxValue, Vector3 setMinVector, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, Vector3 maxVector, Vector3 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, Vector3 maxVector, float setMinValue, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, float maxValue, Vector3 setMinVector, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, float minValue, Vector3 maxVector, Vector3 setMinVector, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3 vector, Vector3 minVector, Vector3 maxVector, Vector3 setMinVector, Vector3 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// VECTOR 4

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector4 vector, float minValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minValue || vector.y < minValue || vector.z < minValue || vector.w < minValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector4 vector, Vector4 minVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minVector.x || vector.y < minVector.y || vector.z < minVector.z || vector.w < minVector.w);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector4 vector, float minValue, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minValue)
      {
        vector.z = setValue;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w < minValue)
      {
        vector.w = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector4 vector, float minValue, Vector4 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minValue)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w < minValue)
      {
        vector.w = setVector.w;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector4 vector, Vector4 minVector, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minVector.z)
      {
        vector.z = setValue;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w < minVector.w)
      {
        vector.w = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector4 vector, Vector4 minVector, Vector4 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minVector.z)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w < minVector.w)
      {
        vector.w = setVector.w;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector4 vector, float maxValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxValue || vector.y > maxValue || vector.z > maxValue || vector.w > maxValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector4 vector, Vector4 maxVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxVector.x || vector.y > maxVector.y || vector.z > maxVector.z || vector.w > maxVector.w);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector4 vector, float maxValue, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxValue)
      {
        vector.z = setValue;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w > maxValue)
      {
        vector.w = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector4 vector, float maxValue, Vector4 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxValue)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w > maxValue)
      {
        vector.w = setVector.w;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector4 vector, Vector4 maxVector, float setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxVector.z)
      {
        vector.z = setValue;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w > maxVector.w)
      {
        vector.w = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector4 vector, Vector4 maxVector, Vector4 setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxVector.z)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      // Determine if the w axis is within boundaries.
      if (vector.w > maxVector.w)
      {
        vector.w = setVector.w;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, float maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, float maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, Vector4 maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, Vector4 maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, float maxValue, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, float maxValue, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, Vector4 maxVector, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, float maxValue, Vector4 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, float maxValue, float setMinValue, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, Vector4 maxVector, float setMinValue, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, float maxValue, Vector4 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, float maxValue, float setMinValue, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, Vector4 maxVector, Vector4 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, Vector4 maxVector, float setMinValue, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, float maxValue, Vector4 setMinVector, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, Vector4 maxVector, Vector4 setMinVector, float setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, Vector4 maxVector, float setMinValue, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, float maxValue, Vector4 setMinVector, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, float minValue, Vector4 maxVector, Vector4 setMinVector, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector4 vector, Vector4 minVector, Vector4 maxVector, Vector4 setMinVector, Vector4 setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// VECTOR 2 INT

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2Int vector, int minValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minValue || vector.y < minValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2Int vector, Vector2Int minVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minVector.x || vector.y < minVector.y);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2Int vector, int minValue, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2Int vector, int minValue, Vector2Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2Int vector, Vector2Int minVector, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector2Int vector, Vector2Int minVector, Vector2Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2Int vector, int maxValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxValue || vector.y > maxValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2Int vector, Vector2Int maxVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxVector.x || vector.y > maxVector.y);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2Int vector, int maxValue, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2Int vector, int maxValue, Vector2Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2Int vector, Vector2Int maxVector, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector2Int vector, Vector2Int maxVector, Vector2Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, int maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, int maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, Vector2Int maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, Vector2Int maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, int maxValue, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, int maxValue, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, Vector2Int maxVector, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, int maxValue, Vector2Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, int maxValue, int setMinValue, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, Vector2Int maxVector, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, int maxValue, Vector2Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, int maxValue, int setMinValue, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, Vector2Int maxVector, Vector2Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, Vector2Int maxVector, int setMinValue, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, int maxValue, Vector2Int setMinVector, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, Vector2Int maxVector, Vector2Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, Vector2Int maxVector, int setMinValue, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, int maxValue, Vector2Int setMinVector, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, int minValue, Vector2Int maxVector, Vector2Int setMinVector, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector2Int vector, Vector2Int minVector, Vector2Int maxVector, Vector2Int setMinVector, Vector2Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// VECTOR 3 INT

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3Int vector, int minValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minValue || vector.y < minValue || vector.z < minValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3Int vector, Vector3Int minVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x < minVector.x || vector.y < minVector.y || vector.z < minVector.z);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3Int vector, int minValue, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minValue)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3Int vector, int minValue, Vector3Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minValue)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3Int vector, Vector3Int minVector, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minVector.z)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are less than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMin(ref Vector3Int vector, Vector3Int minVector, Vector3Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x < minVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y < minVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z < minVector.z)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3Int vector, int maxValue)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxValue || vector.y > maxValue || vector.z > maxValue);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3Int vector, Vector3Int maxVector)
    {
      // Return false if any of the axes are not within boundaries.
      return !(vector.x > maxVector.x || vector.y > maxVector.y || vector.z > maxVector.z);
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3Int vector, int maxValue, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxValue)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3Int vector, int maxValue, Vector3Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxValue)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxValue)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxValue)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setValue">The value to set an axis to if it is outside the boundaries.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3Int vector, Vector3Int maxVector, int setValue)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setValue;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setValue;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxVector.z)
      {
        vector.z = setValue;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function determining if any axes of a given vector are more than a given boundary. The boundary is exclusive.
    /// If an axis is not within the boundary, the set value is applied to it.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setVector">The vector to set each value to. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all axes are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVectorMax(ref Vector3Int vector, Vector3Int maxVector, Vector3Int setVector)
    {
      bool isValid = true; // The return boolean determining if the vector is valid or not.

      // Determine if the x axis is within boundaries.
      if (vector.x > maxVector.x)
      {
        vector.x = setVector.x;
        isValid = false;
      }

      // Determine if the y axis is within boundaries.
      if (vector.y > maxVector.y)
      {
        vector.y = setVector.y;
        isValid = false;
      }

      // Determine if the z axis is within boundaries.
      if (vector.z > maxVector.z)
      {
        vector.z = setVector.z;
        isValid = false;
      }

      return isValid; // Return if the values were valid or not.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, int maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, int maxValue)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxValue);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, Vector3Int maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minValue) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, Vector3Int maxVector)
    {
      // Returns false if any of the axes are not within boundaries.
      return ValidateVectorMin(ref vector, minVector) || ValidateVectorMax(ref vector, maxVector);
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, int maxValue, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, int maxValue, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, Vector3Int maxVector, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, int maxValue, Vector3Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, int maxValue, int setMinValue, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, Vector3Int maxVector, int setMinValue, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, int maxValue, Vector3Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, int maxValue, int setMinValue, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, Vector3Int maxVector, Vector3Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, Vector3Int maxVector, int setMinValue, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, int maxValue, Vector3Int setMinVector, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxValue">The value to set an axis to if it is more than the maximum boundary.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, Vector3Int maxVector, Vector3Int setMinVector, int setMaxValue)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxValue) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinValue">The value to set an axis to if it is less than the minimum boundary.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, Vector3Int maxVector, int setMinValue, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinValue);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxValue">The maximum value any axis can be.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, int maxValue, Vector3Int setMinVector, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxValue, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minValue">The minimum value any axis can be.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, int minValue, Vector3Int maxVector, Vector3Int setMinVector, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minValue, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }

    /// <summary>
    /// A function which determines if all values within a Vector are within a given boundaries. The boundaries are exclusive.
    /// </summary>
    /// <param name="vector">The vector to validate.</param>
    /// <param name="minVector">The vector containing each minimum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="maxVector">The vector containing each maximum value for each axis. Each axis is compared to each axis.</param>
    /// <param name="setMinVector">The vector to set each value to if the value is less than the minimum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <param name="setMaxVector">The vector to set each value to if the value is more than the maximum. Each axis is set to each set axis if the axis isn't valid.</param>
    /// <returns>Returns true if all values are within boundaries. Returns false otherwise.</returns>
    public static bool ValidateVector(ref Vector3Int vector, Vector3Int minVector, Vector3Int maxVector, Vector3Int setMinVector, Vector3Int setMaxVector)
    {
      // Create a boolean to hold if the values are valid. It must be done this way to ensure both functions are called.
      bool isValid = ValidateVectorMin(ref vector, minVector, setMinVector);

      isValid = ValidateVectorMax(ref vector, maxVector, setMaxVector) && isValid;

      return isValid; // Return if the vector was valid.
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  }
  /**********************************************************************************************************************/
}