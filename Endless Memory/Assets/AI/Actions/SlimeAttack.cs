using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class SlimeAttack : RAINAction
{
    public GameObject attack;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        GameObject player = ai.WorkingMemory.GetItem<GameObject>("playerGet");
        
        if(player != null)
        {
            Debug.Log(player);
            Vector3 pos = ai.Body.transform.position;
            GameObject.Find("ParticleManager").GetComponent<ParticleManager>().emitSlimeParticle(pos, player.transform.position);
        }
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}