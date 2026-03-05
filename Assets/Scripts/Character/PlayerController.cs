using UnityEngine;
using UnityEngine.InputSystem;
using Farming;

namespace Character 
{
    [RequireComponent(typeof(PlayerInput))] // Input is required and we don't store a reference
    [RequireComponent(typeof(Farmer))] //Comtemplate if this should be a dependancy or should it send a message?
    
    public class PlayerController : MonoBehaviour
    { 
        [SerializeField] private TileSelector tileSelector;
        [SerializeField] private CameraFollow cameraFollow;
        MovementController moveController;      
        AnimatedController animatedController;
        Farmer farmer;
        void Start()
        {
            farmer = GetComponent<Farmer>();
            animatedController = GetComponent<AnimatedController>();
            moveController = GetComponent<MovementController>();
            Debug.Assert(animatedController, "Player requires an animatedController");
            Debug.Assert(tileSelector, "Player requires a TileSelector.");
            Debug.Assert(moveController, "PlayerController requires a MovementController");

            if (!cameraFollow)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera)
                {
                    cameraFollow = mainCamera.GetComponent<CameraFollow>();
                }
            }

            if (cameraFollow)
            {
                cameraFollow.SetPlayer(transform);
                moveController.SetMovementReference(cameraFollow.transform);
            }
            else if (Camera.main)
            {
                moveController.SetMovementReference(Camera.main.transform);
            }
        }
        public void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            farmer.SetIsMoving(inputVector.sqrMagnitude > 0.001f);
            moveController.Move(inputVector);
        }

        public void OnJump(InputValue inputValue)
        {
            moveController.Jump();
            Debug.Log("Jump Pressed");
        }

        public void OnInteract(InputValue value)
        {
            Debug.Log("Interact Pressed");
            FarmTile tile = tileSelector.GetSelectedTile();
            farmer.TryTileInteract(tile);
        }

        public void OnLook(InputValue inputValue)
        {
            if (!cameraFollow)
            {
                return;
            }

            cameraFollow.SetLookInput(inputValue.Get<Vector2>());
        }
        
    }
}