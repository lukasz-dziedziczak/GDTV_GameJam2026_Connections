using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Objectives : MonoBehaviour
{
    [SerializeField] UI_Objective objectivePrefab;
    [SerializeField] GameObject objBox;
    [SerializeField] float minWidth = 20f;

    List<UI_Objective> objectives;
    float spacing = 0;
    float width = 0f;

    private void Start()
    {
        spacing = objBox.GetComponent<HorizontalLayoutGroup>().spacing;
        objectives = new List<UI_Objective>();
        width = minWidth;

        List<List<NodeResource>> consumerReqs = new List<List<NodeResource>>();
        var gate = Game.GateNode;
        if (gate != null && gate.InResources != null)
        {
            var reqList = new List<NodeResource>();
            foreach (var r in gate.InResources)
            {
                if (r == null || r.Config == null) continue;
                reqList.Add(new NodeResource(r.Config, r.Amount));
            }
            if (reqList.Count > 0) consumerReqs.Add(reqList);
        }

        foreach (var reqList in consumerReqs)
        {
            foreach (var res in reqList)
            {
                var ui = Instantiate(objectivePrefab, objBox.transform);
                ui.Set(res);
                objectives.Add(ui);
                width += ui.Width + spacing;
            }
        }

        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null) rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public void UpdateObjectives()
    {
        if (objectives == null || objectives.Count == 0) return;

        Dictionary<ResourceConfig, int> totals = new Dictionary<ResourceConfig, int>();
        var gate = Game.GateNode;
        if (gate != null && gate.Inventory != null)
        {
            foreach (NodeResource item in gate.Inventory)
            {
                if (item.Config == null) continue;
                totals.TryGetValue(item.Config, out int cur);
                totals[item.Config] = cur + item.Amount;
            }
        }

        foreach (UI_Objective ui in objectives)
        {
            if (ui == null || ui.Resource == null) continue;
            totals.TryGetValue(ui.Resource, out int amt);
            ui.UpdateAmount(amt);
        }
    }
}
