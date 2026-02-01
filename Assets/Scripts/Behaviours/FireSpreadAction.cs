using System;
using Unity.Behavior;
using UnityEngine;
using Entities;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FireSpread", story: "[Agent] fires 3 projectiles at [Target] at this [speed]", category: "Action", id: "0309bfd130909e2b9f242638e72d8abc")]
public partial class FireSpreadAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Speed;

    [SerializeReference] public BlackboardVariable<int> Repeats;

    protected override Status OnStart()
    {
        var ball1 = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.EnemyBasic, Agent.Value.transform.position, Agent.Value.GetComponent<Boss1>().ColorAngle);
        var ball2 = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.EnemyBasic, Agent.Value.transform.position, Agent.Value.GetComponent<Boss1>().ColorAngle);
        var ball3 = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.EnemyBasic, Agent.Value.transform.position, Agent.Value.GetComponent<Boss1>().ColorAngle);

        Vector3 directionToPlayer = (Target.Value.transform.position - Agent.Value.transform.position).normalized;
        ball1.moveDirection = (Vector2)directionToPlayer;
        Quaternion rotation2 = Quaternion.Euler(0, 0, 30.0f);
        Quaternion rotation3 = Quaternion.Euler(0, 0, -30.0f);
        ball2.moveDirection = rotation2 * directionToPlayer;
        ball3.moveDirection = rotation3 * directionToPlayer;

        ball1.movementSpeed = Speed;
        ball2.movementSpeed = Speed;
        ball3.movementSpeed = Speed;

        Repeats.Value+=1;

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

