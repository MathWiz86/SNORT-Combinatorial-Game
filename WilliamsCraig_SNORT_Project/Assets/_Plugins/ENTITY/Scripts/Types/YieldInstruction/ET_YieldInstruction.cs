using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ET_YieldInstruction : CustomYieldInstruction
{
  public override bool keepWaiting => throw new System.NotImplementedException();
}
