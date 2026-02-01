using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ResetEverything", story: "[Agent] Resets", category: "Action", id: "0c9eef9a445992a44cfab52a1e0b9856")]
public partial class ResetEverythingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Repeats;

    [SerializeReference] public BlackboardVariable<float> AgentHealth;

    protected override Status OnStart()
    {
        Repeats.Value = 0;
        //Debug
        AgentHealth.Value = Agent.Value.GetComponent<Boss1>().Health;
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

