using System;
using System.Collections;
using System.Collections.Generic;
using Crops;
using Enums;
using HelperClasses;
using Inventory;
using Items;
using Maps;
using Misc;
using SaveSystem;
using SceneManagement;
using TimeSystem;
using UI;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using VFX;
using Cursor = UI.Cursor;
using EventHandler = Events.EventHandler;

namespace _Player
{
    public class Player : SingletonMonoBehaviour<Player>,ISaveable
    {

        private WaitForSeconds afterUseHoeAnimationPause;
        private WaitForSeconds useHoeAnimationPause;
        private WaitForSeconds afterUseWateringCanAnimationPause;
        private WaitForSeconds afterPickAnimationPause;
        private WaitForSeconds afterChopAnimationPause;
        private WaitForSeconds afterBreakAnimationPause;

        private WaitForSeconds useWateringCanAnimationPause;
        private WaitForSeconds useToolAnimationPause;
        private WaitForSeconds pickAnimationPause;
        private WaitForSeconds chopAnimationPause;
        private WaitForSeconds breakAnimationPause;

        private bool playerToolUseDisabled = false;

        private AnimationOverrides animationOverrides;
        private GridCursor gridCursor;
        private Cursor cursor;
        // List of character attribute that we parse in to animation override methods
        private List<CharacterAttribute> characterAttributeCustomizationList;

        [Tooltip("Should be populated in the prefab with the equipped item sprite renderer")] [SerializeField]
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

            private bool harvestingRight;
            private bool harvestingLeft;
            private bool harvestingUp;
            private bool harvestingDown;

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

        private Rigidbody2D rBody2D;

        private Direction playerDirection;

        private float movementSpeed;

        public Player()
        {
            PlayerInputIsDisabled = false;
        }

        public bool PlayerInputIsDisabled { get; set; }

        public string ISaveableUniqueID { get; set; }
        public GameObjectSave GameObjectSave { get; set; }

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 30;

            rBody2D = GetComponent<Rigidbody2D>();

            // Get the reference to camera
            mainCamera=Camera.main;

            animationOverrides = GetComponentInChildren<AnimationOverrides>();
            // Initialize swappable character attribute
            armsCharacterAttribute=
                new CharacterAttribute(CharacterPartAnimator.Arms,PartVariantColour.None,PartVariantType.none);

            toolCharacterAttribute=
                new CharacterAttribute(CharacterPartAnimator.Tool,PartVariantColour.None,PartVariantType.Hoe);

            // Initialize character attribute list
            characterAttributeCustomizationList=new List<CharacterAttribute>();

            // Get Unique ID for gameobject and create save data object
            ISaveableUniqueID = GetComponent<GenerateGUID>().GUid;

            GameObjectSave=new GameObjectSave();
        }

        private void OnEnable()
        {
            ISaveableRegister();

            EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
            EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
        }

        private void OnDisable()
        {
            ISaveableDeregister();

            EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
            EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
        }

        private void Start()
        {
            gridCursor = FindObjectOfType<GridCursor>();
            cursor = FindObjectOfType<Cursor>();

            useHoeAnimationPause=new WaitForSeconds(Settings.useHoeAnimationPause);
            afterUseHoeAnimationPause=new WaitForSeconds(Settings.afterUseHoeAnimationPause);

            useWateringCanAnimationPause=new WaitForSeconds(Settings.useWateringCanAnimationPause);
            afterUseWateringCanAnimationPause=new WaitForSeconds(Settings.afterUseWateringCanAnimationPause);

            useToolAnimationPause=new WaitForSeconds(Settings.useToolAnimationPause);

            pickAnimationPause=new WaitForSeconds(Settings.pickAnimationPause);
            afterPickAnimationPause=new WaitForSeconds(Settings.afterPickAnimationPause);

            chopAnimationPause=new WaitForSeconds(Settings.chopAnimationPause);
            afterChopAnimationPause=new WaitForSeconds(Settings.afterChopAnimationPause);

            breakAnimationPause=new WaitForSeconds(Settings.breakAnimationPause);
            afterBreakAnimationPause=new WaitForSeconds(Settings.afterBreakAnimationPause);


        }

        private void Update()
        {
            #region PlayerInput

            if (!PlayerInputIsDisabled)
            {
                ResetAnimationTriggers();

                PlayerMovementInput();

                PlayerWalkInput();

                PlayerClickInput();

                PlayerTestInput();

                // Send event to any listeners for player movement input
                Events.EventHandler.CallMovementEvent(
                    xInput,yInput,
                    isWalking,isRunning,isIdle,isCarrying,
                    axeRight,axeLeft,axeUp,axeDown,
                    fishingRight,fishingLeft,fishingUp,fishingDown,
                    miscRight,miscLeft,miscUp,miscDown,
                    harvestingRight,harvestingLeft,harvestingUp,harvestingDown,
                    pickRight,pickLeft,pickUp,pickDown,
                    sickleRight,sickleLeft,sickleUp,sickleDown,
                    hammerRight,hammerLeft,hammerUp,hammerDown,
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
            rBody2D.MovePosition(rBody2D.position + move);
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

        private void PlayerClickInput()
        {
            if (!playerToolUseDisabled)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
                    {
                        // Get cursor grid position
                        Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                        // Get player grid position
                        Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

                        ProcessPlayerClickInput(cursorGridPosition,playerGridPosition);
                    }
                }
            }

        }

        private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
        {
            ResetMovement();

            Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

            // Get grid property details at cursor position (the GridCursor validation routine
            // ensures that grid property details are not null).
            GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(
                cursorGridPosition.x, cursorGridPosition.y);

            // Get Selected item details
            ItemDetails itemDetails =
                InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);

            if (itemDetails != null)
            {
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        if (Input.GetMouseButton(0))
                        {
                            ProcessPlayerClickInputSeed(itemDetails,gridPropertyDetails);
                        }
                        break;
                    case ItemType.Commodity:
                        if (Input.GetMouseButton(0))
                        {
                            ProcessPlayerClickInputCommodity(itemDetails);
                        }
                        break;
                    case ItemType.Chopping_Tool:
                    case ItemType.Watering_Tool:
                    case ItemType.HoeingTool:
                    case ItemType.Reaping_Tool:
                    case ItemType.Collecting_Tool:
                    case ItemType.BreakingTool:
                        ProcessPlayerClickInputTool(gridPropertyDetails,itemDetails,playerDirection);
                        break;
                    case ItemType.Reapable_scanary:
                        break;
                    case ItemType.Furniture:
                        break;
                    case ItemType.None:
                        break;
                    case ItemType.Count:
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails,
            Vector3Int playerDirection)
        {
            // Switch on tool
            switch (itemDetails.itemType)
            {
                case ItemType.Watering_Tool:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        WateringGroundAtCursor(gridPropertyDetails, playerDirection);
                    }
                    break;
                case ItemType.HoeingTool:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                    }
                    break;
                case ItemType.Chopping_Tool:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                    }
                    break;
                case ItemType.BreakingTool:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        BreakingAtCursor(gridPropertyDetails, itemDetails, playerDirection);
                    }
                    break;
                case ItemType.Reaping_Tool:
                    if (cursor.CursorPositionIsValid)
                    {
                        playerDirection =
                            GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCenterPosition());
                        ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                    }
                    break;
                case ItemType.Collecting_Tool:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
        {
            if (cursorPosition.x>playerPosition.x
                &&
                cursorPosition.y<(playerPosition.y+cursor.ItemUseRadius*0.5f)
                &&
                cursorPosition.y>(playerPosition.y-cursor.ItemUseRadius*0.5f))
            {
                return Vector3Int.right;
            }
            else if (cursorPosition.x < playerPosition.x
                     &&
                     cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius * 0.5f)
                     &&
                     cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius * 0.5f))
            {
                return Vector3Int.left;
            }
            else if (cursorPosition.y > playerPosition.y)
            {
                return  Vector3Int.up;
            } else
            {
                return Vector3Int.down;
            }
        }

        private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
        {
            StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
        }

        private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
        {
            PlayerInputIsDisabled=true;
            playerToolUseDisabled = true;

            // Set tool animation to hoe in override animation
            /*toolCharacterAttribute.partVariantType = PartVariantType.Sickle;
            characterAttributeCustomizationList.Clear();
            characterAttributeCustomizationList.Add(toolCharacterAttribute);
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);*/

            // Reap in player direction
            UseToolInPlayerDirection(itemDetails, playerDirection);

            yield return useToolAnimationPause;

            PlayerInputIsDisabled = false;
            playerToolUseDisabled = false;
        }

        private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
        {

            if (Input.GetMouseButton(0))
            {
                switch (equippedItemDetails.itemType)
                {
                    case ItemType.Chopping_Tool:
                        break;
                    case ItemType.BreakingTool:
                        break;
                    case ItemType.Reaping_Tool:
                        if (playerDirection == Vector3Int.right)
                        {
                            sickleRight = true;
                        }
                        else if(playerDirection == Vector3Int.left)
                        {
                            sickleLeft = true;
                        }
                        else if(playerDirection == Vector3Int.up)
                        {
                            sickleUp= true;
                        }
                        else if(playerDirection == Vector3Int.down)
                        {
                            sickleDown = true;
                        }
                        break;
                    case ItemType.Collecting_Tool:
                        break;
                }

                // Define create point of square which will be used for collision testing
                Vector2 point =new Vector2(
                    GetPlayerCenterPosition().x + (playerDirection.x *(equippedItemDetails.itemUseRadius*0.5f)),
                    GetPlayerCenterPosition().y + (playerDirection.y *(equippedItemDetails.itemUseRadius*0.5f)));

                // Define size of the square which will be used for collision testing
                Vector2 size=new Vector2(equippedItemDetails.itemUseRadius,equippedItemDetails.itemUseRadius);

                // Get Item components with 2d collider located in the square at the centre point defined
                // ( 2d colliders tested limited to maxCollidersToTestPerReapSwing)
                Item[] itemArray =
                    HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(
                        Settings.maxCollidersToTestPerReapSwing,point, size, 0f);

                int reapableItemCount = 0;

                // Loop through all items retrieved
                for (int i = 0; i < itemArray.Length; i++)
                {
                    if (itemArray[i] != null)
                    {

                        // Destroy item game object if reapable
                        if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType ==
                            ItemType.Reapable_scanary)
                        {

                            // Effect position
                            Vector3 effectPosition=new Vector3(itemArray[i].transform.position.x,
                                itemArray[i].transform.position.y + Settings.gridCellSize*0.5f,itemArray[i].transform.position.z);

                            // Trigger the reaping effect
                            EventHandler.CallHarvestActionEffectEvent(effectPosition,HarvestActionEffect.Reaping);

                            Destroy(itemArray[i].gameObject);

                            reapableItemCount++;
                            if(reapableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                                break;

                        }

                    }
                }
            }
        }

        private void WateringGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
        {
           // Trigger Animation
           StartCoroutine(WateringGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
        }

        private IEnumerator WateringGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
        {
            PlayerInputIsDisabled = true;
            playerToolUseDisabled = true;

            // Set tool animation to hoe in override animation
            /*toolCharacterAttribute.partVariantType = PartVariantType.Watering;
            characterAttributeCustomizationList.Clear();
            characterAttributeCustomizationList.Add(toolCharacterAttribute);
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);*/

            if (playerDirection == Vector3Int.right)
            {
                miscRight = true;
            }
            else if(playerDirection == Vector3Int.left)
            {
                miscLeft = true;
            }
            else if(playerDirection == Vector3Int.up)
            {
                miscUp= true;
            }
            else if(playerDirection == Vector3Int.down)
            {
                miscDown = true;
            }

            yield return useWateringCanAnimationPause;

            // Set Grid Property Details for Dug ground
            if (gridPropertyDetails.daySinceWatered == -1)
            {
                gridPropertyDetails.daySinceWatered = 0;
            }

            // Set grid property to dug
            GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX,
                gridPropertyDetails.gridY,gridPropertyDetails);

            // Display watered grid tiles
            GridPropertiesManager.Instance.DisplayWateredgGround(gridPropertyDetails);

            // After animation pause
            yield return afterUseWateringCanAnimationPause;

            PlayerInputIsDisabled = false;
            playerToolUseDisabled = false;
        }


        private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
        {
            StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails,equippedItemDetails,playerDirection));
        }

        private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
        {
            PlayerInputIsDisabled = true;
            playerToolUseDisabled = true;

            // Set tool animation to hoe in override animation
            /*toolCharacterAttribute.partVariantType = PartVariantType.axe;
            characterAttributeCustomizationList.Clear();
            characterAttributeCustomizationList.Add(toolCharacterAttribute);
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);*/

            ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

            yield return chopAnimationPause;

            // After animation pause
            yield return afterChopAnimationPause;

            PlayerInputIsDisabled = false;
            playerToolUseDisabled = false;

        }

        private void BreakingAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
        {
            StartCoroutine(BreakingAtCursorRoutine(gridPropertyDetails,itemDetails,playerDirection));
        }

        private IEnumerator BreakingAtCursorRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
        {
            PlayerInputIsDisabled = true;
            playerToolUseDisabled = true;

            // Set tool animation to hoe in override animation
            /*toolCharacterAttribute.partVariantType = PartVariantType.pickaxe;
            characterAttributeCustomizationList.Clear();
            characterAttributeCustomizationList.Add(toolCharacterAttribute);
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);*/

            ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

            yield return breakAnimationPause;

            // After animation pause
            yield return afterBreakAnimationPause;

            PlayerInputIsDisabled = false;
            playerToolUseDisabled = false;
        }

        private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails,
                                              ItemDetails equippedItemDetails, Vector3Int playerDirection)
        {
            StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
        }



        private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
        {
            PlayerInputIsDisabled = true;
            playerToolUseDisabled = true;

            ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

            yield return pickAnimationPause;

            // After animation pause
            yield return afterPickAnimationPause;

            PlayerInputIsDisabled = false;
            playerToolUseDisabled = false;

        }

        private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection, ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
        {

            switch (equippedItemDetails.itemType)
            {
                case ItemType.Collecting_Tool:

                    if (playerDirection == Vector3Int.right)
                    {
                        harvestingRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        harvestingLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        harvestingUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        harvestingDown = true;
                    }
                    break;
                case ItemType.Chopping_Tool:
                    if (playerDirection == Vector3Int.right)
                    {
                        axeRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        axeLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        axeUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        axeDown = true;
                    }
                    break;

                case ItemType.BreakingTool :
                    if (playerDirection == Vector3Int.right)
                    {
                        pickRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        pickLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        pickUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        pickDown = true;
                    }
                    break;

                case ItemType.None:
                    break;
            }

            // Get crop at corner grid location
            Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

            // Execute Process tool action for crop
            if (crop != null)
            {
                switch (equippedItemDetails.itemType)
                {
                    case ItemType.Collecting_Tool:
                        crop.ProcessToolAction(equippedItemDetails,harvestingRight,harvestingLeft,harvestingUp,harvestingDown);
                        break;
                    case ItemType.Chopping_Tool:
                        crop.ProcessToolAction(equippedItemDetails,axeRight,axeLeft,axeUp,axeDown);
                        break;
                    case ItemType.BreakingTool:
                        crop.ProcessToolAction(equippedItemDetails,pickRight,pickLeft,pickUp,pickDown);
                        break;
                }
            }

        }

        private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
        {
            // Trigger animation
            StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
        }

        private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
        {
            PlayerInputIsDisabled = true;
            playerToolUseDisabled = true;

            // Set tool animation to hoe in override animation
            /*toolCharacterAttribute.partVariantType = PartVariantType.Hoe;
            characterAttributeCustomizationList.Clear();
            characterAttributeCustomizationList.Add(toolCharacterAttribute);
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomizationList);*/

            if (playerDirection == Vector3Int.right)
            {
                hoeRight = true;
            }
            else if(playerDirection == Vector3Int.left)
            {
                hoeLeft = true;
            }
            else if(playerDirection == Vector3Int.up)
            {
                hoeUp= true;
            }
            else if(playerDirection == Vector3Int.down)
            {
                hoeDown = true;
            }

            yield return useHoeAnimationPause;

            // Set Grid Property Details for Dug ground
            if (gridPropertyDetails.daySinceDug == -1)
            {
                gridPropertyDetails.daySinceDug = 0;
            }

            // Set grid property to dug
            GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX,
                gridPropertyDetails.gridY,gridPropertyDetails);

            // Display dug grid tiles
            GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

            // After animation pause
            yield return afterUseHoeAnimationPause;

            PlayerInputIsDisabled = false;
            playerToolUseDisabled = false;
        }

        private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
        {
            if (cursorGridPosition.x > playerGridPosition.x)
            {
                return Vector3Int.right;
            }
            else if (cursorGridPosition.x < playerGridPosition.x)
            {
                return Vector3Int.left;
            }
            else if (cursorGridPosition.y > playerGridPosition.y)
            {
                return Vector3Int.up;
            } else
            {
                return Vector3Int.down;
            }
        }

        private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
        {
            if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
            {
                EventHandler.CallDropSelectedItemEvent();
            }
        }

        private void ProcessPlayerClickInputSeed(ItemDetails itemDetails, GridPropertyDetails gridPropertyDetails)
        {
            if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid
                && gridPropertyDetails.daySinceDug>-1 && gridPropertyDetails.seedItemCode==-1)
            {
                PlantSeedAtCursor(gridPropertyDetails, itemDetails);
            }

            else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
            {
                EventHandler.CallDropSelectedItemEvent();
            }

        }

        private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
        {
            // Process if we have crop details for the seed
            if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
            {
                // Update Grid properties with seed details
                gridPropertyDetails.seedItemCode = itemDetails.itemCode;
                gridPropertyDetails.growthDays = 0;

                // Display planted crop at grid property details
                GridPropertiesManager.Instance.DisplayPlantedCrops(gridPropertyDetails);

                // Remove item from inventory
                EventHandler.CallRemoveSelectedItemFromInventoryEvent();
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

        public void ClearCarriedItem()
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
                xInput,yInput,
                isWalking,isRunning,isIdle,isCarrying,
                axeRight,axeLeft,axeUp,axeDown,
                fishingRight,fishingLeft,fishingUp,fishingDown,
                miscRight,miscLeft,miscUp,miscDown,
                harvestingRight,harvestingLeft,harvestingUp,harvestingDown,
                pickRight,pickLeft,pickUp,pickDown,
                sickleRight,sickleLeft,sickleUp,sickleDown,
                hammerRight,hammerLeft,hammerUp,hammerDown,
                shovelRight,shovelLeft,shovelUp,shovelDown,
                hoeRight,hoeLeft,hoeUp,hoeDown,
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

        public Vector3 GetPlayerCenterPosition()
        {
            return  new Vector3(transform.position.x,transform.position.y+ Settings.playerCenterYOffset,
                transform.position.z);
        }


        public void ISaveableRegister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Add(this);
        }

        public void ISaveableDeregister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
        }

        public GameObjectSave ISaveableSave()
        {
            // Delete saveScene for game object if it already exists
            GameObjectSave.sceneData.Remove(Settings.PersistentScene);

            // Create saveScene for game object
            SceneSave sceneSave = new SceneSave();

            // Create Vector3 Dictionary
            sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

            // Create String Dictionary
            sceneSave.stringDictionary = new Dictionary<string, string>();

            // Add Player position to Vector3 dictionary
            var position = transform.position;
            Vector3Serializable vector3Serializable = new Vector3Serializable(position.x, position.y, position.z);
            sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);

            // Add Current Scene Name to string dictionary
            sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

            // Add Player Direction to string dictionary
            sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());

            // Add sceneSave data for player game object
            GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

            return GameObjectSave;
        }

        public void ISaveableLoad(GameSave gameSave)
        {
            if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
            {
                // Get save data dictionary for scene
                if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
                {
                    // Get player position
                    if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                    {
                        transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                    }

                    // Get String dictionary
                    if (sceneSave.stringDictionary != null)
                    {
                        // Get player scene
                        if (sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                        {
                            SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                        }

                        // Get player direction
                        if (sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDir))
                        {
                            bool playerDirFound = Enum.TryParse<Direction>(playerDir, true, out Direction direction);

                            if (playerDirFound)
                            {
                                playerDirection = direction;
                                SetPlayerDirection(playerDirection);
                            }
                        }
                    }
                }
            }
        }

        public void ISaveableStoreScene(string sceneName)
        {
            // Nothing required here since the player is on a persistent scene;
        }

        public void ISaveableRestoreScene(string sceneName)
        {
            // Nothing required here since the player is on a persistent scene;
        }


        private void SetPlayerDirection(Direction playerDirection)
        {

            switch (playerDirection)
            {
                case Direction.Up:
                    // set idle up trigger
                    EventHandler.CallMovementEvent(0f, 0f, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false,
                        false, false, false, false, false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,
                        false,false,false,false,true,false,
                        false,false);

                    break;

                case Direction.Down:
                    // set idle down trigger
                    EventHandler.CallMovementEvent(0f, 0f, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false,
                        false, false, false, false, false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,true,
                        false,false);
                    break;

                case Direction.Left:
                    EventHandler.CallMovementEvent(0f, 0f, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false,
                        false, false, false, false, false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,false,
                        true,false);
                    break;

                case Direction.Right:
                    EventHandler.CallMovementEvent(0f, 0f, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false,
                        false, false, false, false, false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,false,
                        false,true);
                    break;

                default:
                    // set idle down trigger
                    EventHandler.CallMovementEvent(0f, 0f, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false, false,
                        false, false, false, false, false,
                        false, false, false, false, false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,
                        false,false,false,false,false,true,
                        false,false);

                    break;
            }
        }
    }
}
