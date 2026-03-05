using UnityEngine;
using Farming;

namespace Character
{
    public class PointSelector : TileSelector
    {
        private bool TryGetFarmTile(Collider collider, out FarmTile tile)
        {
            tile = null;
            if (!collider)
            {
                return false;
            }

            if (collider.TryGetComponent<FarmTile>(out tile))
            {
                return true;
            }

            tile = collider.GetComponentInParent<FarmTile>();
            return tile != null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (TryGetFarmTile(other, out FarmTile tile))
            {
                SetActiveTile(tile);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!TryGetFarmTile(other, out var tile))
            {
                return;
            }

            if (activeTile == tile)
            {
                SetActiveTile(null);
            }
        }
    }
}