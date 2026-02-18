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
        }
        public void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            moveController.Move(inputVector);
        }

        public void OnJump(InputValue inputValue)
        {
            moveController.Jump();
        }

        public void OnInteract(InputValue value)
        {
            Debug.Log("Interact Pressed");
            FarmTile tile = tileSelector.GetSelectedTile();
            farmer.TryTileInteract(tile);
        }
        
    }
}