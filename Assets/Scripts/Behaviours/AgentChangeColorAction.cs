using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Agent change color", story: "[Agent] changes color", category: "Action", id: "909fee92529bff816bbce21d3415320a")]
public partial class AgentChangeColorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    protected override Status OnStart()
    {
        Agent.Value.GetComponent<Boss1>().ColorAngle = UnityEngine.Random.Range(0, 6) * 60;
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

