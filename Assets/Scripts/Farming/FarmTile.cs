using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Farming 
{
    public class FarmTile : MonoBehaviour
    {
        public enum Condition { Grass, Tilled, Watered, Planted, Harvestable }

        [SerializeField] private Condition tileCondition = Condition.Grass; 
        [SerializeField] private Plant plantedPlant;
        private bool isSoilWatered;

        [Header("Visuals")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material tilledMaterial;
        [SerializeField] private Material wateredMaterial;
        MeshRenderer tileRenderer;

        [Header("Audio")]
        [SerializeField] private AudioSource stepAudio;
        [SerializeField] private AudioSource harvestAudio;
        [SerializeField] private AudioSource tillAudio;
        [SerializeField] private AudioSource waterAudio;

        List<Material> materials = new List<Material>();

        private int daysSinceLastInteraction = 0;
        public FarmTile.Condition GetCondition { get { return tileCondition; } } // TODO: Consider what the set would do?
        public Plant PlantedPlant { get { return plantedPlant; } }

        void Awake()
        {
            tileRenderer = GetComponent<MeshRenderer>();
        }

        void Start()
        {
            tileRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(tileRenderer, "FarmTile requires a MeshRenderer");

            foreach (Transform edge in transform)
            {
                materials.Add(edge.gameObject.GetComponent<MeshRenderer>().material);
            }
        }

        public void Interact()
        {
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled: Water(); break;
                case FarmTile.Condition.Watered: Plant(); break;
                case FarmTile.Condition.Planted: WaterPlant(); break; 
                case FarmTile.Condition.Harvestable: Harvest(); break;
            }
            daysSinceLastInteraction = 0;
        }

        public bool Plant()
        {
            if (tileCondition != FarmTile.Condition.Watered)
            {
                Debug.Log("Planting failed: tile is not watered.", this);
                return false;
            }

            if (GameManager.Instance.getSeeds() <= 0)
            {
                Debug.Log("Planting Failed: no seeds!", this);
                return false;
            }

            if (plantedPlant)
            {
                tileCondition = FarmTile.Condition.Planted;
                UpdateVisual();
                Debug.Log("Planting succeeded: existing plant assigned to tile.", this);
                return true;
            }

            if (!PlantManager.Instance)
            {
                Debug.LogWarning("PlantManager not found in scene.");
                return false;
            }

            bool planted = PlantManager.Instance.PlantOnTile(this);
            if(planted)
            {
                GameManager.Instance.AddSeeds(-1); // decrement seed count on planting
            }
            Debug.Log(planted ? "Planting succeeded." : "Planting failed.", this);
            return planted;
        }

        public void Till()
        {
            tileCondition = FarmTile.Condition.Tilled;
            UpdateVisual();
            tillAudio?.Play();
        }

        public void Water()
        {
            tileCondition = FarmTile.Condition.Watered; 
            isSoilWatered = true;
            UpdateVisual();
            waterAudio?.Play();
        }

        public bool WaterPlant()
        {
            if (tileCondition != Condition.Planted || !plantedPlant)
            {
                return false;
            }

            bool acceptedWater = plantedPlant.Water();
            if (acceptedWater)
            {
                waterAudio?.Play();
            }
            isSoilWatered = true;
            UpdateVisual();
            return acceptedWater;
        }

        public void Harvest()
        {
            if (tileCondition != Condition.Harvestable)
            {
                return;
            }

            if (!plantedPlant)
            {
                plantedPlant = GetComponentInChildren<Plant>();
            }

            if (!plantedPlant)
            {
                Debug.LogWarning("Harvest failed: tile is harvestable but has no plant reference.", this);
                return;
            }

            Destroy(plantedPlant.gameObject); // destroy the plant
            harvestAudio?.Play();
            plantedPlant = null; // set to null

            tileCondition = Condition.Tilled; // return the state of the tile to Tilled
            daysSinceLastInteraction = 0; // reset daysSinceLastInteraction
            UpdateVisual();

            GameManager.Instance.AddTomatoes(1); // increment tomato count (changing this later if we decide to add an inventory)
            Debug.Log("Added Tomato");
        }

        public bool HasWitheredPlant()
        {
            return tileCondition == Condition.Planted && plantedPlant && plantedPlant.IsWithered;
        }

        public bool TillWitheredPlantToDirt()
        {
            if (!HasWitheredPlant())
            {
                return false;
            }

            Destroy(plantedPlant.gameObject);
            plantedPlant = null;
            tileCondition = Condition.Tilled;
            daysSinceLastInteraction = 0;
            UpdateVisual();
            tillAudio?.Play();
            return true;
        }

        private void UpdateVisual()
        {
            if(tileRenderer == null) tileRenderer = GetComponent<MeshRenderer>();
            if(tileRenderer == null) return;
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: tileRenderer.material = grassMaterial; break;
                case FarmTile.Condition.Tilled: tileRenderer.material = tilledMaterial; break;
                case FarmTile.Condition.Watered: tileRenderer.material = wateredMaterial; break;
                case FarmTile.Condition.Planted: tileRenderer.material = isSoilWatered? wateredMaterial : tilledMaterial; break;
                case FarmTile.Condition.Harvestable: tileRenderer.material = wateredMaterial; break;
            }
        }

        public void SetCondition(Condition condition)
        {
            tileCondition = condition;
            if (tileCondition != Condition.Planted)
            {
                plantedPlant = null;
            }
            daysSinceLastInteraction = 0;
            UpdateVisual();
        }
        public void UpdateCondition()
        {
            if (!plantedPlant)
            {
                return;
            }
            else {
                Condition previousTileCondition = tileCondition;
                switch (plantedPlant.GetCondition)
                {
                    case Farming.Plant.Condition.Planted:
                        tileCondition = Condition.Planted;
                        break;
                    case Farming.Plant.Condition.Growing:
                        tileCondition = Condition.Planted;
                        break;
                    case Farming.Plant.Condition.Mature:
                        tileCondition = Condition.Harvestable;
                        break;
                    case Farming.Plant.Condition.Withered:
                        tileCondition = Condition.Planted;
                        break;
                }
                if (previousTileCondition != tileCondition)
                {
                    Debug.Log("Tile " + name + ": Plant state is now " + plantedPlant.GetCondition + ", tile condition = " + tileCondition);
                } 
                daysSinceLastInteraction = 0;
                UpdateVisual();
            }
        }
        public void SetPlantedPlant(Plant plant)
        {
            plantedPlant = plant;
            tileCondition = Condition.Planted;
            daysSinceLastInteraction = 0;
            plantedPlant?.Water();
            UpdateVisual();
        }

        public void SetHighlight(bool active)
        {
            foreach (Material m in materials)
            {
                if (active)
                {
                    m.EnableKeyword("_EMISSION");
                } 
                else 
                {
                    m.DisableKeyword("_EMISSION");
                }
            }
            if (active) stepAudio.Play();
        }

        public void OnDayPassed()
        {
            if (tileCondition == FarmTile.Condition.Planted)
            {
                if (!plantedPlant)
                {
                    tileCondition = FarmTile.Condition.Tilled;
                    UpdateVisual();
                    return;
                } 
                isSoilWatered = !isSoilWatered;
                UpdateCondition();
                UpdateVisual();
                return;
            }

            daysSinceLastInteraction++;
            
            if(daysSinceLastInteraction >= 2)
            {
                if(tileCondition == FarmTile.Condition.Watered) tileCondition = FarmTile.Condition.Tilled;
                else if(tileCondition == FarmTile.Condition.Tilled) tileCondition = FarmTile.Condition.Grass;
            }
            UpdateVisual();
        }
    }
}