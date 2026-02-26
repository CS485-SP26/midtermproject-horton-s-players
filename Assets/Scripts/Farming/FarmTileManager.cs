using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Environment;
using UnityEngine.SceneManagement;

namespace Farming
{
    public class FarmTileManager:MonoBehaviour
    {
        [SerializeField] private GameObject farmTilePrefab;

        [SerializeField] DayController dayController;
        [SerializeField] private int rows = 4;
        [SerializeField] private int cols = 4;
        [SerializeField] private float tileGap = 0.1f;
        public List<FarmTile> tiles = new List<FarmTile>();
        private string persistenceKey;
        
        void Start()
        {
            Debug.Assert(farmTilePrefab, "FarmTileManager requires a farmTilePrefab");
            Debug.Assert(dayController, "FarmTileManager requires a dayController");
            RebuildTilesListFromChildren();
            persistenceKey = BuildPersistenceKey();
            LoadTileStates();
        }

        void OnEnable()
        {
            if (dayController)
            {
                dayController.dayPassedEvent.AddListener(this.OnDayPassed);
            }
        }

        void OnDisable()
        {
            SaveTileStates();
            if (dayController)
            {
                dayController.dayPassedEvent.RemoveListener(this.OnDayPassed);
            }
        }

        public void OnDayPassed()
        {
            IncrementDays(1);
            SaveTileStates();
        }

        public void IncrementDays(int count)
        {
            while (count > 0)
            {
                foreach (FarmTile farmTile in tiles)
                {
                    farmTile.OnDayPassed();
                }
                count--;
            }
        }

        void InstantiateTiles()
        {
            Vector3 spawnPos = transform.position;
            int count = 0;
            GameObject clone = null; 

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    clone = Instantiate(farmTilePrefab, spawnPos, Quaternion.identity);
                    clone.name = "Farm Tile " + count++.ToString();
                    spawnPos.x += clone.transform.localScale.x + tileGap;
                    clone.transform.parent = transform; // build heirarchy
                    tiles.Add(clone.GetComponent<FarmTile>()); // for resize/delete
                }
                spawnPos.z += clone.transform.localScale.z + tileGap;
                spawnPos.x = transform.position.x;
            }
        }

        private void RebuildTilesListFromChildren()
        {
            tiles.Clear();
            foreach (Transform child in transform)
            {
                if (child.gameObject.TryGetComponent<FarmTile>(out var tile))
                {
                    tiles.Add(tile);
                }
            }
        }

        private void SaveTileStates()
        {
            if (tiles == null || tiles.Count == 0)
            {
                RebuildTilesListFromChildren();
            }

            if (tiles == null || tiles.Count == 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(persistenceKey))
            {
                persistenceKey = BuildPersistenceKey();
            }

            int[] states = new int[tiles.Count];
            for (int i = 0; i < tiles.Count; i++)
            {
                states[i] = (int)tiles[i].GetCondition;
            }

            GameManager.Instance.SaveFarmTileStates(persistenceKey, states);
        }

        private void LoadTileStates()
        {
            if (string.IsNullOrWhiteSpace(persistenceKey))
            {
                persistenceKey = BuildPersistenceKey();
            }

            if (!GameManager.Instance.TryGetFarmTileStates(persistenceKey, out int[] states))
            {
                return;
            }

            if (states == null || states.Length != tiles.Count)
            {
                return;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                FarmTile.Condition restoredCondition = (FarmTile.Condition)states[i];
                tiles[i].SetCondition(restoredCondition);
            }
        }

        private string BuildPersistenceKey()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            return sceneName + "/" + GetTransformPath(transform);
        }

        private string GetTransformPath(Transform current)
        {
            if (current == null) return string.Empty;

            string path = current.name;
            Transform parent = current.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        // ***************************************************************** //
        // Below this line is code to suppor the Unity Editor (Advanced)
        // Please feel free to disregard everything below this
        // ***************************************************************** //
        void OnValidate()
        {
            #if UNITY_EDITOR
            EditorApplication.delayCall += () => {
                if (this == null) return; // Guard against the object being deleted
                ValidateGrid();
            };
            #endif
        }

        void ValidateGrid() 
        {
            if (!farmTilePrefab) return;
            tiles.Clear();
            foreach (Transform child in transform)
            {
                if (child.gameObject.TryGetComponent<FarmTile>(out var tile))
                {
                    tiles.Add(tile);
                }
            }

            int newCount = rows * cols;

            if (tiles.Count != newCount)
            {
                DestroyTiles();
                InstantiateTiles();
            }
        }

        void DestroyTiles()
        {
            foreach (FarmTile tile in tiles)
            {
                #if UNITY_EDITOR
                DestroyImmediate(tile.gameObject);
                #else
                Destroy(tile.gameObject);
                #endif
            }
            tiles.Clear();
        }
    }
}