using UnityEngine;

[CreateAssetMenu(fileName = "NodeResource", menuName = "New Node Resource")]
public class ResourceConfig : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Texture2D Icon { get; private set; }
    
}
