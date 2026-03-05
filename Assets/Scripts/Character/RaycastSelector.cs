using UnityEngine;
using Farming;
using Character;

public class RaycastSelector : TileSelector
{ 
    [SerializeField] private float rayDistance = 5f;
    LineRenderer rayLine;

    private bool TryGetFarmTile(RaycastHit hitInfo, out FarmTile tile)
    {
        tile = null;
        if (hitInfo.collider == null)
        {
            return false;
        }

        if (hitInfo.collider.TryGetComponent<FarmTile>(out tile))
        {
            return true;
        }

        tile = hitInfo.collider.GetComponentInParent<FarmTile>();
        return tile != null;
    }

    void Start()
    {
        rayLine = GetComponent<LineRenderer>();
        if(rayLine)
        {
            rayLine.positionCount = 2;
        }
    }
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance))
        {
            if (TryGetFarmTile(hitInfo, out FarmTile tile))
            {
                SetActiveTile(tile);
            }
            else
            {
                SetActiveTile(null);
            }
        } else //didnt hit anything
        {
            SetActiveTile(null);
        }

        #if DEBUG
        if (rayLine)
        {
            rayLine.SetPosition(0, transform.position);
            rayLine.SetPosition(1, transform.position + transform.forward * rayDistance);
        }
        #endif
    }
}
