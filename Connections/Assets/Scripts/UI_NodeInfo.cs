using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_NodeInfo : MonoBehaviour
{
    [SerializeField] TMP_Text nodeNameText;
    [SerializeField] GameObject resourceList;
    [SerializeField] UI_Subheading subheadingPrefab;
    [SerializeField] UI_ResourceItem resourceItemPrefab;
    [SerializeField] float MinHeight = 75f;

    List<GameObject> instantiatedItems = new List<GameObject>();
    float height = 0f;

    Node node;
    public Node Node => node;

    public void Set(Node node)
    {
        if (instantiatedItems.Count > 0)
        {
            foreach (var item in instantiatedItems)
            {
                Destroy(item);
            }
            instantiatedItems.Clear();
        }

        if (node == null) return;
        this.node = node;
        height = MinHeight;
        nodeNameText.text = node.Name;

        if (node.InResources != null && node.InResources.Count > 0)
        {
            var subheading = Instantiate(subheadingPrefab, resourceList.transform);
            subheading.Set("Requires");
            instantiatedItems.Add(subheading.gameObject);
            height += subheading.Height;
            foreach (var resource in node.InResources)
            {
                var item = Instantiate(resourceItemPrefab, resourceList.transform);
                instantiatedItems.Add(item.gameObject);
                item.Set(resource);
                height += item.Height;
            }
        }

        if (node.OutResources != null && node.OutResources.Count > 0)
        {
            var subheading = Instantiate(subheadingPrefab, resourceList.transform);
            subheading.Set("Produces");
            instantiatedItems.Add(subheading.gameObject);
            height += subheading.Height;
            foreach (var resource in node.OutResources)
            {
                var item = Instantiate(resourceItemPrefab, resourceList.transform);
                instantiatedItems.Add(item.gameObject);
                item.Set(resource);
                height += item.Height;
            }
        }

        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null) rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void Set(NodeConfig nodeConfig)
    {
        if (instantiatedItems.Count > 0)
        {
            foreach (var item in instantiatedItems)
            {
                Destroy(item);
            }
            instantiatedItems.Clear();
        }

        if (nodeConfig == null) return;
        height = MinHeight;
        nodeNameText.text = nodeConfig.Name;

        if (nodeConfig.InResources != null && nodeConfig.InResources.Count > 0)
        {
            var subheading = Instantiate(subheadingPrefab, resourceList.transform);
            subheading.Set("Requires");
            instantiatedItems.Add(subheading.gameObject);
            height += subheading.Height;
            foreach (var resource in nodeConfig.InResources)
            {
                var item = Instantiate(resourceItemPrefab, resourceList.transform);
                instantiatedItems.Add(item.gameObject);
                item.Set(resource);
                height += item.Height;
            }
        }

        if (nodeConfig.OutResources != null && nodeConfig.OutResources.Count > 0)
        {
            var subheading = Instantiate(subheadingPrefab, resourceList.transform);
            subheading.Set("Produces");
            instantiatedItems.Add(subheading.gameObject);
            height += subheading.Height;
            foreach (var resource in nodeConfig.OutResources)
            {
                var item = Instantiate(resourceItemPrefab, resourceList.transform);
                instantiatedItems.Add(item.gameObject);
                item.Set(resource);
                height += item.Height;
            }
        }

        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null) rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
