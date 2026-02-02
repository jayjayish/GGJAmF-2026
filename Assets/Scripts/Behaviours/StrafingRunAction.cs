using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Entities;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StrafingRun", story: "[Target] Locks on to [Players] [Location] And Zips", category: "Action", id: "48c862225b9a20fb56b290a8774e280d")]
public partial class StrafingRunAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Players;
    [SerializeReference] public BlackboardVariable<Transform> Location;

    public float distanceThreshold = 2.0f;
    public Vector2 direction;
    public int overshoot;
    public Vector3 newPos;
    public float shotFrequency;
    public float deltaTime = 0;
    private const float maxDuration = 5f;


    protected override Status OnStart()
    {
        direction = (Location.Value.position - Target.Value.transform.position).normalized;
        overshoot = 5;
        newPos = Location.Value.position + new Vector3(direction.x * overshoot, direction.y * overshoot, 0.0f);
        shotFrequency = 0.1f;
        deltaTime = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float distance = GetDistanceXY(Target.Value.transform.position, newPos);
        CustomMove(Target.Value.transform, newPos,
            10f, distance, 0.0f);

        bool destinationReached = distance <= distanceThreshold;

        deltaTime += Time.deltaTime;
        if (deltaTime > maxDuration)
        {
            return Status.Success;
        }
        if (deltaTime > shotFrequency)
        {
            deltaTime = 0.0f;
            Airstrike();
        }
        
        if (destinationReached)
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

    private float GetDistanceXY(Vector3 obj1, Vector3 obj2)
    {
        return Vector2.Distance(obj1, obj2);
    }

    private void Airstrike()
    {
        Debug.Log("Airstriking");
        var ball1 = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.EnemyBasic, Target.Value.transform.position, Target.Value.GetComponent<Boss1>().ColorAngle);
        ball1.moveDirection = Quaternion.Euler(0, 0, 90.0f) * direction;
        ball1.movementSpeed = 7;

        var ball2 = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.EnemyBasic, Target.Value.transform.position, Target.Value.GetComponent<Boss1>().ColorAngle);
        ball2.moveDirection = Quaternion.Euler(0, 0, -90.0f) * direction;
        ball2.movementSpeed = 7;
    }
}

