using UnityEngine;

namespace Farming
{
    [RequireComponent(typeof(Collider))]
    public class WaterRefillZone : MonoBehaviour
    {
        [SerializeField] private bool refillOnEnter = true;
        [SerializeField] private bool refillContinuouslyWhileInside = false;

        private void Reset()
        {
            Collider colliderComponent = GetComponent<Collider>();
            if (colliderComponent != null)
            {
                colliderComponent.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!refillOnEnter) return;
            TryRefill(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!refillContinuouslyWhileInside) return;
            TryRefill(other);
        }

        private void TryRefill(Collider other)
        {
            Farmer farmer = other.GetComponent<Farmer>();
            if (farmer == null)
            {
                farmer = other.GetComponentInParent<Farmer>();
            }

            if (farmer != null)
            {
                farmer.RefillWaterToFull();
            }
        }
    }
}
