using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "omfg", story: "Custom: [Agent] Moves to [Target]", category: "Action", id: "932c17a939fb6c71b056a875674a0a25")]
public partial class OmfgAction : Action
{
    public enum TargetPositionMode
    {
        ClosestPointOnAnyCollider,      // Use the closest point on any collider, including child objects
        ClosestPointOnTargetCollider,   // Use the closest point on the target's own collider only
        ExactTargetPosition             // Use the exact position of the target, ignoring colliders
    }

    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");

        // This will only be used in movement without a navigation agent.
        [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);
        [Tooltip("Defines how the target position is determined for navigation:" +
            "\n- ClosestPointOnAnyCollider: Use the closest point on any collider, including child objects" +
            "\n- ClosestPointOnTargetCollider: Use the closest point on the target's own collider only" +
            "\n- ExactTargetPosition: Use the exact position of the target, ignoring colliders. Default if no collider is found.")]
        [SerializeReference] public BlackboardVariable<TargetPositionMode> m_TargetPositionMode = new(TargetPositionMode.ClosestPointOnAnyCollider);

        private Animator m_Animator;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_ColliderAdjustedTargetPosition;
        [CreateProperty] private float m_OriginalStoppingDistance = -1f;
        [CreateProperty] private float m_OriginalSpeed = -1f;
        private float m_ColliderOffset;
        private float m_CurrentSpeed;

    protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            return Initialize();
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            // Check if the target position has changed.
            bool boolUpdateTargetPosition = !Mathf.Approximately(m_LastTargetPosition.x, Target.Value.transform.position.x)
                || !Mathf.Approximately(m_LastTargetPosition.y, Target.Value.transform.position.y);

            if (boolUpdateTargetPosition)
            {
                m_LastTargetPosition = Target.Value.transform.position;
                m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();
            }

            float distance = GetDistanceXZ();
            bool destinationReached = distance <= (DistanceThreshold + m_ColliderOffset);
// transform-based movement

            m_CurrentSpeed = CustomMove(Agent.Value.transform, m_ColliderAdjustedTargetPosition,
                Speed, distance, SlowDownDistance);


            return Status.Running;
        }

        protected override void OnEnd()
        {
            m_Animator = null;
        }

        protected override void OnDeserialize()
        {
            // If using a navigation mesh, we need to reset default value before Initialize

            Initialize();
        }

        private Status Initialize()
        {
            m_LastTargetPosition = Target.Value.transform.position;
            m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();

            // Add the extents of the colliders to the stopping distance.
            m_ColliderOffset = 0.0f;
            Collider agentCollider = Agent.Value.GetComponentInChildren<Collider>();
            if (agentCollider != null)
            {
                Vector3 colliderExtents = agentCollider.bounds.extents;
                m_ColliderOffset += Mathf.Max(colliderExtents.x, colliderExtents.z);
            }

            if (GetDistanceXZ() <= (DistanceThreshold + m_ColliderOffset))
            {
                return Status.Success;
            }

            // If using a navigation mesh, set target position for navigation mesh agent.

            m_Animator = Agent.Value.GetComponentInChildren<Animator>();

            return Status.Running;
        }

        private Vector3 GetPositionColliderAdjusted()
        {
            switch (m_TargetPositionMode.Value)
            {
                case TargetPositionMode.ClosestPointOnAnyCollider:
                    Collider anyCollider = Target.Value.GetComponentInChildren<Collider>(includeInactive: false);
                    if (anyCollider == null || anyCollider.enabled == false)
                        break;
                    Debug.Log("point1");
                    return anyCollider.ClosestPoint(Agent.Value.transform.position);
                case TargetPositionMode.ClosestPointOnTargetCollider:
                    Collider targetCollider = Target.Value.GetComponent<Collider>();
                    if (targetCollider == null || targetCollider.enabled == false)
                        break;
                    Debug.Log("point2");
                    return targetCollider.ClosestPoint(Agent.Value.transform.position);
            }

            // Default to target position.
            Debug.Log("point3");
            return Target.Value.transform.position;
        }

        private float GetDistanceXZ()
        {
            Vector3 agentPosition = new Vector3(Agent.Value.transform.position.x, Agent.Value.transform.position.y, m_ColliderAdjustedTargetPosition.z);
            return Vector3.Distance(agentPosition, m_ColliderAdjustedTargetPosition);
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
}

