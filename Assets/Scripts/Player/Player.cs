using System;
using System.Collections.Generic;
using Enums;
using Inventory;
using Items;
using Misc;
using SceneManagement;
using TimeSystem;
using UnityEditor.Build.Content;
using UnityEngine;


namespace _Player
{
    public class Player : SingletonMonoBehaviour<Player>
    {
        private AnimationOverrides animationOverrides;
        // List of character attribute that we parse in to animation override methods
        private List<CharacterAttribute> characterAttributeCustomizationList;
        [Tooltip("Shoud be papulated in the prefab with the equipped item sprite renderer")] [SerializeField]
        private SpriteRenderer equippedItemSpriteRenderer = null;

        //Player attributes that can be swapped
        private CharacterAttribute armsCharacterAttribute;
        private CharacterAttribute toolCharacterAttribute;

        private Camera mainCamera;

        //Movement Parameters
        #region Movement Variables
            private float xInput;
            private float yInput;
            private bool isWalking;
            private bool isRunning;
            private bool isIdle;
            private bool isCarrying = false;
            private bool axeRight;
            private bool axeLeft;
            private bool axeUp;
            private bool axeDown;
            private bool fishingRight;
            private bool fishingLeft;
            private bool fishingUp;
            private bool fishingDown;
            private bool miscRight;
            private bool miscLeft;
            private bool miscUp;
            private bool miscDown;
            private bool pickRight;
            private bool pickLeft;
            private bool pickUp;
            private bool pickDown;
            private bool sickleRight;
            private bool sickleLeft;
            private bool sickleUp;
            private bool sickleDown;
            private bool hammerRight;
            private bool hammerLeft;
            private bool hammerUp;
            private bool hammerDown;
            private bool shovelRight;
            private bool shovelLeft;
            private bool shovelUp;
            private bool shovelDown;
            private bool hoeRight;
            private bool hoeLeft;
            private bool hoeUp;
            private bool hoeDown;
            private bool idleUp;
            private bool idleDown;
            private bool idleLeft;
            private bool idleRight;
        #endregion

        private Rigidbody2D rigidbody2D;
#pragma warning disable 414
        private Direction playerDirection;
#pragma warning restoe 414
        private float movementSpeed;

        public Player()
        {
            PlayerInputIsDisabled = false;
        }

        public bool PlayerInputIsDisabled { get; set; }

        protected override void Awake()
        {
            base.Awake();
            rigidbody2D = GetComponent<Rigidbody2D>();

            // Get the reference to camera
            mainCamera=Camera.main;

            animationOverrides = GetComponentInChildren<AnimationOverrides>();
            // Initialize swappable character attribute
            armsCharacterAttribute=
                new CharacterAttribute(CharacterPartAnimator.Arms,PartVariantColour.None,PartVariantType.none);

            // Initialize character attribute list
            characterAttributeCustomizationList=new List<CharacterAttribute>();
        }

        private void Update()
        {
            #region PlayerInput

            if (!PlayerInputIsDisabled)
            {
                ResetAnimationTriggers();

                PlayerMovementInput();

                PlayerWalkInput();

                PlayerTestInput();

                // Send event to any listeners for player movement input
                Events.EventHandler.CallMovementEvent(
                    xInput,yInput,isWalking,isRunning,isIdle,isCarrying,axeRight,axeLeft,axeUp,
                    axeDown,fishingRight,fishingLeft,fishingUp,fishingDown,miscRight,
                    miscLeft,miscUp,miscDown,pickRight,pickLeft,pickUp,pickDown,sickleRight,
                    sickleLeft,sickleUp,sickleDown,hammerRight,hammerLeft,hammerUp,hammerDown,
                    shovelRight,shovelLeft,shovelUp,shovelDown,hoeRight,hoeLeft,hoeUp,hoeDown,
                    false,false,false,false
                );
            }

            #endregion
        }

        private void FixedUpdate()
        {
            PlayerMovement();
        }

        private void PlayerMovement()
        {
            Vector2 move=new Vector2(xInput * movementSpeed*Time.deltaTime,yInput*movementSpeed*Time.deltaTime);
            rigidbody2D.MovePosition(rigidbody2D.position + move);
        }

        // Switch between walk and run by shift key
        private void PlayerWalkInput()
        {
            if (Input.GetButton("Walk"))
            {
                isRunning = false;
                isWalking = true;
                isIdle = false;
                movementSpeed = Settings.walkingSpeed;
            } else
            {
                isRunning = true;
                isWalking = false;
                isIdle = false;
                movementSpeed = Settings.runningSpeed;
            }
        }

        private void PlayerMovementInput()
        {
            //Get input from player input
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            //Move player in diagonal direction when both x and y input pressed
            if (Math.Abs(xInput) > Mathf.Epsilon && Math.Abs(yInput) > Mathf.Epsilon )
            {
                xInput *= 0.71f;
                yInput *= 0.71f;
            }

            if (Math.Abs(xInput) > Mathf.Epsilon || Math.Abs(yInput) > Mathf.Epsilon)
            {
                isRunning = true;
                isWalking = false;
                isIdle = false;
                movementSpeed = Settings.runningSpeed;

                //Capture player direction for save game
                if (Mathf.Abs(xInput) < Mathf.Epsilon)
                {
                    playerDirection = Direction.Left;
                }
                else if (Mathf.Abs(xInput) > Mathf.Epsilon)
                {
                    playerDirection = Direction.Right;
                }
                else if (Mathf.Abs(yInput) < Mathf.Epsilon)
                {
                    playerDirection = Direction.Down;
                }
                else if (Mathf.Abs(yInput) > Mathf.Epsilon)
                {
                    playerDirection = Direction.Up;
                }
            }else if (xInput == 0 && yInput == 0)
            {
                isRunning = false;
                isWalking = false;
                isIdle = true;
            }
        }

        private void ResetAnimationTriggers()
        {
            axeRight = false;
            axeLeft = false;
            axeUp = false;
            axeDown = false;
            fishingRight = false;
            fishingLeft = false;
            fishingUp = false;
            fishingDown = false;
            miscRight = false;
            miscLeft = false;
            miscUp = false;
            miscDown = false;
            pickRight = false;
            pickLeft = false;
            pickUp = false;
            pickDown = false;
            sickleRight = false;
            sickleLeft = false;
            sickleUp = false;
            sickleDown = false;
            hammerRight = false;
            hammerLeft = false;
            hammerUp = false;
            hammerDown = false;
            shovelRight = false;
            shovelLeft = false;
            shovelUp = false;
            shovelDown = false;
            hoeRight = false;
            hoeLeft = false;
            hoeUp = false;
            hoeDown = false;
        }

        public void clearCarriedItem()
        {
            equippedItemSpriteRenderer.sprite = null;
            equippedItemSpriteRenderer.color=new Color(0f,0f,0f,0f);

            // Apply base character arms customization
            armsCharacterAttribute.partVariantType = PartVariantType.none;
            characterAttributeCustomizationList.Clear();
            characterAttributeCustomizationList.Add(armsCharacterAttribute);
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

            isCarrying = false;
        }
        public void ShowCarriedItem(int itemCode)
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
            if (itemDetails != null)
            {
                equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
                equippedItemSpriteRenderer.color=new Color(1f,1f,1f,1f);

                // Apply 'carry' character arms customization
                armsCharacterAttribute.partVariantType = PartVariantType.Carry;
                characterAttributeCustomizationList.Clear();
                characterAttributeCustomizationList.Add(armsCharacterAttribute);
                animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);

                isCarrying = true;
            }
        }
        public Vector3 GetPlayerViewPortPosition()
        {
            // Vector 3 viewport position for the player ((0,0) viewport bottom left,(1,1) viewport top right
            return mainCamera.WorldToViewportPoint(transform.position);
        }

        public void DisablePlayerInputAndResetMovement()
        {
            DisablePlayerInput();
            ResetMovement();

            // Send event to any listeners for player movement input
            Events.EventHandler.CallMovementEvent(
                xInput,yInput,isWalking,isRunning,isIdle,isCarrying,axeRight,axeLeft,axeUp,
                axeDown,fishingRight,fishingLeft,fishingUp,fishingDown,miscRight,
                miscLeft,miscUp,miscDown,pickRight,pickLeft,pickUp,pickDown,sickleRight,
                sickleLeft,sickleUp,sickleDown,hammerRight,hammerLeft,hammerUp,hammerDown,
                shovelRight,shovelLeft,shovelUp,shovelDown,hoeRight,hoeLeft,hoeUp,hoeDown,
                false,false,false,false
            );

        }

        // ToDo:Remove
        /// <summary>
        /// Temp routine for test input
        /// </summary>
        private void PlayerTestInput()
        {
            // Trigger Advance Time
            if (Input.GetKey(KeyCode.T))
            {
                TimeManager.Instance.TestAdvanceGameMinute();
            }

            // Trigger Advance Day
            if (Input.GetKey(KeyCode.G))
            {
                TimeManager.Instance.TestAdvanceGameDay();
            }

            // Reload current scene
            if (Input.GetKey(KeyCode.L))
            {
                SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
            }
        }

        private void ResetMovement()
        {
            // Reset Movement
            xInput = 0f;
            yInput = 0f;
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }

        public void DisablePlayerInput()
        {
            PlayerInputIsDisabled = true;
        }

        public void EnablePlayerInput()
        {
            PlayerInputIsDisabled = false;
        }
    }
}
