using UnityEngine;
using Farming;
using Character;

public class RaycastSelector : TileSelector
{ 
    [SerializeField] private float rayDistance = 5f;
    LineRenderer rayLine;
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
            if (hitInfo.collider.TryGetComponent<FarmTile>(out FarmTile tile))
            {
                SetActiveTile(tile);
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
