using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "New Level Config")]
public class LevelConfig : ScriptableObject
{
    [field: SerializeField] public List<NodeResource> RequiredResources { get; private set; }

}
