using UnityEngine;
using System;

[System.Serializable]
public struct NodeResource : IEquatable<NodeResource>
{
    public ResourceConfig Config;
    public int Amount;
    public NodeResource(ResourceConfig config, int amount)
    {
        Config = config;
        Amount = amount;
    }

    public bool Equals(NodeResource other)
    {
        return Config == other.Config && Amount == other.Amount;
    }

    public override bool Equals(object obj)
    {
        return obj is NodeResource other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (Config != null ? Config.GetHashCode() : 0);
            hash = hash * 31 + Amount;
            return hash;
        }
    }

    public static bool operator ==(NodeResource left, NodeResource right) => left.Equals(right);
    public static bool operator !=(NodeResource left, NodeResource right) => !left.Equals(right);
}
