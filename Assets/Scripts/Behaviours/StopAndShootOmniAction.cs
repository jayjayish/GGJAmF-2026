using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Entities;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Stop and Shoot Omni", story: "[Agent] stops and shoots Omni Directional", category: "Action", id: "1bf5932d07a5a16f0e17206d9ebfc2a0")]
public partial class StopAndShootOmniAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        for (int i = 0 ; i < 10; i++)
        {
            var ball = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.EnemyBasic, Agent.Value.transform.position, 0);
            ball.moveDirection = Quaternion.Euler(0, 0, 36.0f * i) * new Vector2(0, 1);
            ball.movementSpeed = 10;
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

