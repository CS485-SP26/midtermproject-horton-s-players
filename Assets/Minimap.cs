using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Minimap : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y; // Keep the minimap at a fixed height
            transform.position = newPosition; // Scale the position for the minimap

            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f); // Rotate the minimap to match the player's orientation
        }
    }
}
