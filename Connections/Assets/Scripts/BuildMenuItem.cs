using UnityEngine;

[CreateAssetMenu(fileName = "BuildMenuItem", menuName = "New Build Menu Item")]
public class BuildMenuItem : ScriptableObject
{
    [field: SerializeField] public Texture Icon { get; private set; }

    [field: SerializeField] public Texture IconBackground { get; private set; }
    [field: SerializeField] public Road ConnectionObjectPrefab { get; private set; }
    [field: SerializeField] public NodeConfig NodeConfig { get; private set; }

    public void OnBuildButtonPress()
    {
        Game.Player.EnterBuildMode(this);
    }

}