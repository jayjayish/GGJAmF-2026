using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using System.Linq;
using Entities;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Spawn Ads", story: "[Agent] Spawns [Ads] around [Target]", category: "Action", id: "4ddff27eac877299e8e24645d3b0a6b8")]
public partial class SpawnAdsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<List<GameObject>> AdSpawnLocations;

    protected override Status OnStart()
    {
        List<GameObject> positions = AdSpawnLocations.Value.ToList();
        for (int i = 0; i < 6; i++)
        {
            var obj = positions[i].transform.position;
            EnemyManager.SpawnEnemy(Data.GlobalTypes.EnemyTypes.Slime, new Vector2(obj.x, obj.y), i * 60);
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

