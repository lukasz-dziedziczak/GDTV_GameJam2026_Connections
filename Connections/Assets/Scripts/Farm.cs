using System.Collections.Generic;
using UnityEngine;

public class Farm : NodeObject
{
    [SerializeField] GameObject[] plantPositions;

    List<GameObject> plants = new List<GameObject>();

    [System.Serializable]
    public class PlantProgress
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public float Progress { get; private set; }
    }

    [SerializeField] PlantProgress[] plantProgress;

    PlantProgress currentProgress;

    private PlantProgress GetPlantProgress
    {
        get
        {
            PlantProgress current = null;
            if (Node != null)
            {
                float progress = Node.ProductionProgress;
                if (progress == 0 && Node.Inventory.Count > 0) progress = 1;
                // return the first PlantProgress whose Progress is >= progress
                for (int i = 0; i < plantProgress.Length; i++)
                {
                    PlantProgress plant = plantProgress[i];
                    if (plant == null) continue;
                    if (progress <= plant.Progress)
                    {
                        current = plant;
                        break;
                    }
                }
                if (current == null && plantProgress.Length > 0) current = plantProgress[plantProgress.Length - 1];
            }
            return current;
        }
    }

    private void Update()
    {
        if (Node == null) return;

        if (currentProgress == null || Node.ProductionProgress > currentProgress.Progress || (currentProgress.Progress == 1 && Node.ProductionProgress < 1))
        {
            currentProgress = GetPlantProgress;
            UpdatePlantObjects();
        }
    }

    private void UpdatePlantObjects()
    {
        foreach(GameObject plant in plants)
        {
            if (plant != null) Destroy(plant);
        }
        plants.Clear();

        foreach (GameObject plantPosition in plantPositions)
        {
            if (plantPosition != null)
            {
                GameObject plant = Instantiate(currentProgress.Prefab, plantPosition.transform);
                plants.Add(plant);
            }
        }
    }
}
