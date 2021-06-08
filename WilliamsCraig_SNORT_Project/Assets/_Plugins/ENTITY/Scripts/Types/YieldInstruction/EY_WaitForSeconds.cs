using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
  public class EY_WaitForSeconds : ET_YieldInstruction
  {
    private float time_ToWait = 0;
    private float time_Passed = 0;
    private ENTITY entity = null;
    private bool isComplete = true;

    public override bool keepWaiting
    {
      get
      {
        if (entity)
        {
          time_Passed += entity.GetCalculatedDeltaTime();
          if (time_Passed >= time_ToWait)
            isComplete = false;
        }
        else
          isComplete = false;

        return isComplete;
      }
    }

    public EY_WaitForSeconds(float time, ENTITY e)
    {
      time_ToWait = time;
      entity = e;
    }
  }
}