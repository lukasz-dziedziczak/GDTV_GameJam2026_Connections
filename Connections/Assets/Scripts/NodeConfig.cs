using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeConfig", menuName = "New Node Config")]
public class NodeConfig : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public ENodeType Type { get; private set; }
    [field: SerializeField] public List<NodeResource> InResources { get; private set; }
    [field: SerializeField] public List<NodeResource> OutResources { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = new Vector2Int(1,1);
    [field: SerializeField] public NodeObject ObjectPrefab { get; private set; }
    [field: SerializeField] public float ProductionTime { get; private set; } = 1f;

}
