using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Farming
{
    public class PlantManager : MonoBehaviour
    {
        public static PlantManager Instance { get; private set; }

        [SerializeField] private Plant plantPrefab;
        [SerializeField] private DayController dayController;
        [SerializeField] private Vector3 plantSpawnOffset = new Vector3(0f, 0.5f, 0f);

        private readonly List<Plant> activePlants = new List<Plant>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple PlantManagers found. Keeping the first instance.");
                return;
            }

            Instance = this;
            if (!dayController)
            {
                dayController = FindAnyObjectByType<DayController>();
            }
        }

        void Start()
        {
            Debug.Assert(plantPrefab, "PlantManager requires a plantPrefab");
        }

        void OnEnable()
        {
            if (dayController)
            {
                dayController.dayPassedEvent.AddListener(OnDayPassed);
            }
        }

        void OnDisable()
        {
            if (dayController)
            {
                dayController.dayPassedEvent.RemoveListener(OnDayPassed);
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        public bool PlantOnTile(FarmTile tile)
        {
            if (!tile)
            {
                Debug.LogWarning("PlantOnTile failed: tile is null.");
                return false;
            }

            if (tile.GetCondition != FarmTile.Condition.Watered)
            {
                Debug.LogWarning("PlantOnTile failed: tile is not watered.", tile);
                return false;
            }

            if (!plantPrefab)
            {
                Debug.LogWarning("PlantOnTile failed: plantPrefab is not assigned.", this);
                return false;
            }

            if (tile.PlantedPlant)
            {
                tile.SetPlantedPlant(tile.PlantedPlant);
                RegisterPlant(tile.PlantedPlant);
                Debug.Log("PlantOnTile succeeded: reused existing plant.", tile);
                return true;
            }

            Plant plant = Instantiate(plantPrefab, tile.transform.position + plantSpawnOffset, Quaternion.identity, tile.transform);
            Vector3  s = tile.transform.lossyScale;
            plant.transform.localScale = new Vector3(1/s.x, 1/s.y, 1/s.z);
            RegisterPlant(plant);
            tile.SetPlantedPlant(plant);
            Debug.Log("PlantOnTile succeeded: spawned new plant.", tile);
            return true;
        }

        public void RegisterPlant(Plant plant)
        {
            if (!plant || activePlants.Contains(plant))
            {
                return;
            }

            activePlants.Add(plant);
        }

        private void OnDayPassed()
        {
            for (int i = activePlants.Count - 1; i >= 0; i--)
            {
                Plant plant = activePlants[i];
                if (!plant)
                {
                    activePlants.RemoveAt(i);
                    continue;
                }

                plant.Growth();
            }
        }
    }
}
