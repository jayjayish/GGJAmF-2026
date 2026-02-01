using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Entities;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Run Away", story: "Agent runs away from [Player] and shoots wildly", category: "Action", id: "64df9f6669f2ba95c07450181e8bf4c6")]
public partial class RunAwayAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public Vector2 direction;
    public float shotFrequency = 0.5f;
    public float deltaTime = 0;
    protected override Status OnStart()
    {
        direction = calcDirection();
        shotFrequency = 0.5f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        direction = calcDirection();
        CustomMove(Agent.Value.transform, Agent.Value.transform.position + (Vector3)direction,
            20, 30, 0.0f);


        shotFrequency += Time.deltaTime;
        if (deltaTime > shotFrequency)
        {
            deltaTime = 0.0f;
            var ball = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.EnemyBasic, Agent.Value.transform.position, Agent.Value.GetComponent<Boss1>().ColorAngle);
            ball.moveDirection = (Player.Value.transform.position - Agent.Value.transform.position).normalized;
            ball.movementSpeed = 20;

        }

        if (Agent.Value.GetComponent<Boss1>().isDead)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    private float CustomMove(Transform agentTransform, Vector3 targetLocation, float speed, float distance, float slowDownDistance = 0f, float minSpeedRatio = 0.1f)
    {
        if (agentTransform == null)
        {
            return 0f;
        }

        Vector3 position = agentTransform.position;
        float num = speed;
        if (slowDownDistance > 0f && distance < slowDownDistance)
        {
            float num2 = distance / slowDownDistance;
            num = Mathf.Max(speed * minSpeedRatio, speed * num2);
        }

        Vector3 vector = targetLocation - position;
        vector.z = 0f;
        if (vector.sqrMagnitude > 0.0001f)
        {
            vector.Normalize();
            position += vector * (num * Time.deltaTime);
            agentTransform.position = position;
        }

        return num;
    }

    private Vector2 calcDirection()
    {
        Vector2 randomDir = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized;
        Vector2 awayFromPlayerDir = (Vector2)(Agent.Value.transform.position - Player.Value.transform.position).normalized;
        Vector2 netDir = ((randomDir * 65) + (awayFromPlayerDir * 35)).normalized;

        return netDir;
    }
}

