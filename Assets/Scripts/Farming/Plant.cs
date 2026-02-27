using UnityEngine;

namespace Farming
{
    public class Plant : MonoBehaviour
    {
        public enum Condition { Planted, Growing, Mature }

        [SerializeField] private Condition plantCondition = Condition.Planted;
        [SerializeField] private GameObject plantPrefabPlanted;
        [SerializeField] private GameObject plantPrefabGrowing;
        [SerializeField] private GameObject plantPrefabMature;
        private int daysSinceLastInteraction = 0;
        public Plant.Condition GetCondition { get { return plantCondition; } }

        void Start()
        {
            Debug.Assert(plantPrefabPlanted, "Plant needs a PrefabPlanted");
            Debug.Assert(plantPrefabGrowing, "Plant needs a PrefabGrowing");
            Debug.Assert(plantPrefabMature, "Plant needs a PrefabMature");
            plantPrefabPlanted.SetActive(false);
            plantPrefabGrowing.SetActive(false);
            plantPrefabMature.SetActive(false);
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            switch (plantCondition)
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
            if (daysSinceLastInteraction >= 2)
            {
                if (plantCondition == Plant.Condition.Planted) plantCondition = Plant.Condition.Growing;
                else if (plantCondition == Plant.Condition.Growing) plantCondition = Plant.Condition.Mature;
            }
            UpdateVisual();
        }
    }
}
