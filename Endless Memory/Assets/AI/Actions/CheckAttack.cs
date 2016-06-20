using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class CheckAttack : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        GameObject player = ai.WorkingMemory.GetItem<GameObject>("playerGet");
        if(player != null)
        {
            float dis = Vector3.Distance(player.transform.position, ai.Body.transform.position);
            if (dis <= 1.5)
                ai.WorkingMemory.SetItem<bool>("Attackable", true);
            else
            {
                ai.WorkingMemory.SetItem<bool>("Attackable", false);
            }
        }
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}