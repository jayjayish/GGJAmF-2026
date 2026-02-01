using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Entities;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ErasePlayerProjectiles", story: "[Agent] erases all Player Projectiles", category: "Action", id: "60452ebae10a18f4c4d8e9a502fdb6ba")]
public partial class ErasePlayerProjectilesAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    public int allowableProjectiles = 2;

    protected override Status OnStart()
    {
        if (ProjectileManager.GetPlayerProjectiles().Count >= allowableProjectiles)
        {
            foreach (var ball in ProjectileManager.GetPlayerProjectiles())
            {
                ProjectileManager.ReturnProjectile(ball.Type, ball);
            }
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

