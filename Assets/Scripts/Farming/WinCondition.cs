using UnityEngine;
using TMPro;
using UnityEditor;
using Environment;

namespace Farming
{
    public class WinCondition : MonoBehaviour
    {
        private bool fullyWatered = false;
        private bool rewarded = false;
        private int count;
        [SerializeField] private TMP_Text winText;
        [SerializeField] FarmTileManager farmTileManager;
        void Start()
        {
            Debug.Assert(farmTileManager, "WinCondition needs farmTileManager.");
            Debug.Assert(winText, "WinCondition needs winText.");
            winText.SetText("Congrats! You watered all the tiles!");
            winText.enabled = false;
        }
        public void countWateredTiles()
        {
            count = 0;
            foreach(FarmTile tile in farmTileManager.tiles)
            {
               if (tile.GetCondition == FarmTile.Condition.Watered)
                {
                    count++;
                } 
            }
            if(count == farmTileManager.tiles.Count)
            {
                Debug.Log("There are " + count + " Watered tiles.");
                fullyWatered = true;
                reward();
            }
        }
        void reward()
        {
            if (fullyWatered && !rewarded)
            {
                GameManager.Instance.AddFunds(10);
                rewarded = true;
                Debug.Log("Received reward.");
                winText.enabled = true;
            }
        }
    }
}