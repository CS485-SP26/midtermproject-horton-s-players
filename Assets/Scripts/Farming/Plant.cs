using UnityEngine;

public class Plant : MonoBehaviour
{
    public enum Condition { Planted, Growing, Mature }

    [SerializeField] private Condition plantCondition = Condition.Planted;
    [SerializeField] private GameObject plantPrefabPlanted;
    [SerializeField] private GameObject plantPrefabGrowing;
    [SerializeField] private GameObject plantPrefabMature;
    private int daysSinceLastInteraction = 0;
    public Plant.Condition GetCondition { get { return plantCondition; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Assert(plantPrefabPlanted, "Plant needs a PrefabPlanted");
        Debug.Assert(plantPrefabGrowing, "Plant needs a PrefabGrowing");
        Debug.Assert(plantPrefabGrowing, "Plant needs a PrefabMature");
        plantPrefabPlanted.SetActive(false); // Start everything off. Planted state activated by interacting with a wet tile
        plantPrefabGrowing.SetActive(false);
        plantPrefabMature.SetActive(false);
    }

    private void UpdateVisual()
        {
            switch (plantCondition) // Logic: Makes sure only the prefab for the plant's growth state is active. Can probably be optimized but it's all here for testing purposes.
            {
                case Plant.Condition.Planted: 
                    plantPrefabPlanted.SetActive(true);
                    plantPrefabGrowing.SetActive(false);
                    plantPrefabMature.SetActive(false);
                    break;
                case Plant.Condition.Growing:
                    plantPrefabPlanted.SetActive(false);
                    plantPrefabGrowing.SetActive(true);
                    plantPrefabMature.SetActive(false);
                    break;
                case Plant.Condition.Mature:
                    plantPrefabPlanted.SetActive(false);
                    plantPrefabGrowing.SetActive(false);
                    plantPrefabMature.SetActive(true);
                    break;
            }
        }
    
    public void SetCondition(Condition condition)
        {
            plantCondition = condition;
            daysSinceLastInteraction = 0;
            UpdateVisual();
        }
    public void Growth()
        {
            daysSinceLastInteraction++;
            if(daysSinceLastInteraction >= 2)
            {
                if(plantCondition == Plant.Condition.Planted) plantCondition = Plant.Condition.Growing;
                else if(plantCondition == Plant.Condition.Growing) plantCondition = Plant.Condition.Mature;
            }
            UpdateVisual();
        }

    // Update is called once per frame
    void Update()
    {
        UpdateVisual(); // Testing purposes
    }
}
