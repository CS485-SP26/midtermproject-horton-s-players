using UnityEngine;

namespace Farming
{
    public class Plant : MonoBehaviour
    {
        public enum Condition { Planted, Growing, Mature, Withered }

        [SerializeField] private Condition plantCondition = Condition.Planted;
        [SerializeField] private GameObject plantPrefabPlanted;
        [SerializeField] private GameObject plantPrefabGrowing;
        [SerializeField] private GameObject plantPrefabMature;
        [SerializeField] private GameObject plantPrefabWithered;
        private int wateredDays = 0;
        private int dryDays = 0;
        private bool receivedWaterToday = true;
        public Plant.Condition GetCondition { get { return plantCondition; } }
        public bool IsWithered { get { return plantCondition == Condition.Withered; } }

        void Start()
        {
            Debug.Assert(plantPrefabPlanted, "Plant needs a PrefabPlanted");
            Debug.Assert(plantPrefabGrowing, "Plant needs a PrefabGrowing");
            Debug.Assert(plantPrefabMature, "Plant needs a PrefabMature");
            plantPrefabPlanted.SetActive(false);
            plantPrefabGrowing.SetActive(false);
            plantPrefabMature.SetActive(false);
            plantPrefabWithered.SetActive(false);
            /*
            if (plantPrefabWithered)
            {
                plantPrefabWithered.SetActive(false);
            }
            */
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
                    plantPrefabWithered.SetActive(false);
                    break;
                case Plant.Condition.Growing:
                    plantPrefabPlanted.SetActive(false);
                    plantPrefabGrowing.SetActive(true);
                    plantPrefabMature.SetActive(false);
                    plantPrefabWithered.SetActive(false);
                    break;
                case Plant.Condition.Mature:
                    plantPrefabPlanted.SetActive(false);
                    plantPrefabGrowing.SetActive(false);
                    plantPrefabMature.SetActive(true);
                    plantPrefabWithered.SetActive(false);
                    break;
                case Plant.Condition.Withered:
                    plantPrefabPlanted.SetActive(false);
                    plantPrefabGrowing.SetActive(false);
                    plantPrefabMature.SetActive(false);
                    plantPrefabWithered.SetActive(true);
                    /*
                    if (plantPrefabWithered)
                    {
                        plantPrefabWithered.SetActive(true);
                    }
                    else
                    {
                        plantPrefabPlanted.SetActive(true);
                    }
                    */
                    break;
            }
        }

        public void SetCondition(Condition condition)
        {
            plantCondition = condition;
            wateredDays = 0;
            dryDays = 0;
            receivedWaterToday = condition != Condition.Withered;
            UpdateVisual();
        }

        public bool Water()
        {
            if (plantCondition == Condition.Withered)
            {
                return false;
            }

            receivedWaterToday = true;
            return true;
        }

        public void Growth()
        {
            if (plantCondition == Condition.Withered)
            {
                return;
            }

            if (receivedWaterToday)
            {
                dryDays = 0;
                wateredDays++;

                if (wateredDays >= 2)
                {
                    if (plantCondition == Plant.Condition.Planted)
                    {
                        plantCondition = Plant.Condition.Growing;
                        wateredDays = 0;
                    }
                    else if (plantCondition == Plant.Condition.Growing)
                    {
                        plantCondition = Plant.Condition.Mature;
                        wateredDays = 0;
                    }
                }
            }
            else
            {
                wateredDays = 0;
                dryDays++;
                if (dryDays >= 2)
                {
                    plantCondition = Plant.Condition.Withered;
                }
            }

            receivedWaterToday = false;
            UpdateVisual();
        }
    }
}
