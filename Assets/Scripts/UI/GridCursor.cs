using System;
using System.Collections.Generic;
using Crops;
using Enums;
using Inventory;
using Items;
using Maps;
using Misc;
using UnityEngine;
using UnityEngine.UI;
using _Player;
using EventHandler = Events.EventHandler;
using HelperClasses;

namespace UI
{
    public class GridCursor : MonoBehaviour
    {
        private Canvas canvas;
        private Grid grid;
        private Camera mainCamera;

        [SerializeField] private Image cursorImage = null;
        [SerializeField] private RectTransform cursorRectTransform=null;
        [SerializeField] private Sprite greenCursorSprite=null;
        [SerializeField] private Sprite redCursorSprite=null;
        [SerializeField] private SO_CropDetailsList so_CropDetailsList=null;

        public bool CursorPositionIsValid { get; set; }
        public int ItemUserGridRadius { get; set; } = 0;
        public ItemType SelectedItemType { get; set; }
        public bool CursorIsEnabled { get; set; } = false;

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        }

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += SceneLoaded;
        }

        private void SceneLoaded()
        {
            grid = GameObject.FindObjectOfType<Grid>();
        }

        private void Start()
        {
            mainCamera=Camera.main;
            canvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            if (CursorIsEnabled)
            {
                DisplayCursor();
            }
        }

        private Vector3Int DisplayCursor()
        {
            if (grid!=null)
            {
                // Get grid position for cursor
                Vector3Int gridPosition = GetGridPositionForCursor();

                // Get grid position for player
                Vector3Int playerGridPosition = GetGridPositionForPlayer();

                // Set cursor sprite
                SetCursorValidity(gridPosition, playerGridPosition);

                // Get rect transform position for cursor
                cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

                return gridPosition;
            } else
            {
                return Vector3Int.zero;
            }
        }

        public void DisableCursor()
        {
            cursorImage.color = Color.clear;
            CursorIsEnabled = false;
        }

        public void EnableCursor()
        {
            cursorImage.color = new Color(1f,1f,1f,1f);
            CursorIsEnabled = true;
        }


        private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
        {
            SetCursorToValid();

            // Check item use radius is valid
            if(Mathf.Abs(cursorGridPosition.x- playerGridPosition.x)>ItemUserGridRadius ||
                Mathf.Abs(cursorGridPosition.y- playerGridPosition.y)>ItemUserGridRadius)
            {
                SetCursorToInvalid();
                return;
            }

            // Get selected item details
            ItemDetails itemDetails =
                InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
            if (itemDetails == null)
            {
                SetCursorToInvalid();
                return;
            }

            // Get grid property details at cursor position
            GridPropertyDetails gridPropertyDetails =
                GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
            if (gridPropertyDetails != null)
            {
                // Determine cursor validity based on inventory item selected and grid property details
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        if (!IsCursorValidForSeed(gridPropertyDetails))
                        {
                            SetCursorToInvalid();
                            return;
                        }
                        break;
                    case ItemType.Commodity:
                        if (!IsCursorValidForCommodity(gridPropertyDetails))
                        {
                            SetCursorToInvalid();
                            return;
                        }
                        break;
                    case ItemType.Chopping_Tool:
                    case ItemType.BreakingTool:
                    case ItemType.Reaping_Tool:
                    case ItemType.Collecting_Tool:
                    case ItemType.Watering_Tool:
                    case ItemType.HoeingTool:
                        if (!IsCursorValidForTool(gridPropertyDetails,itemDetails))
                        {
                            SetCursorToInvalid();
                            return;
                        }
                        break;

                    case ItemType.None:
                        break;
                    case ItemType.Count:
                        break;
                    default:
                        break;
                }
            } else
            {
                SetCursorToInvalid();
                return;
            }
        }

        /// <summary>
        /// Set the cursor as either valid or invalid for tool for target grid property details. return true if valid
        /// and false if invalid
        /// </summary>
        /// <param name="gridPropertyDetails"></param>
        /// <param name="itemDetails"></param>
        /// <returns></returns>
        private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
        {
            // Switch on tool
            switch (itemDetails.itemType)
            {
                case ItemType.Watering_Tool:
                    if (gridPropertyDetails.daySinceDug > -1 && gridPropertyDetails.daySinceWatered == -1)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                case ItemType.HoeingTool:
                    if (gridPropertyDetails.isDiggable && gridPropertyDetails.daySinceDug == -1)
                    {
                        #region Need to get any items at location so we can check if they are reapable
                        // Get world Position for cursor
                        Vector3 cursorWorldPosition=new Vector3(GetWorldPositionForCursor().x+0.5f,
                            GetWorldPositionForCursor().y+0.5f,0f);

                        // Get List of items at cursor location
                        List<Item> itemList=new List<Item>();

                        HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition,
                            Settings.cursorSize, 0f);
                        #endregion

                        // Loop through items found to see if any are reapable type - we are not going to let the player
                        // dig where there are reapable scenary items
                        bool foundReapable = false;
                        foreach (Item item in itemList)
                        {
                            if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType ==
                                ItemType.Reapable_scanary)
                            {
                                foundReapable = true;
                                break;
                            }
                        }

                        if (foundReapable)
                        {
                            return false;//not valid for dig
                        }

                        return true;// valid for dig
                    }
                    break;


                case ItemType.Reaping_Tool:
                    break;
                case ItemType.Chopping_Tool:
                case ItemType.Collecting_Tool:
                case ItemType.BreakingTool:

                    // Check if item can be harvested with item selected, check item is fully grown

                    // Check if seed planted
                    if (gridPropertyDetails.seedItemCode != -1)
                    {
                        // Get crop details for seed
                        CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                        // If crop details found
                        if (cropDetails != null)
                        {
                            // Check if crop fully grown
                            if (gridPropertyDetails.growthDays >=
                                                cropDetails.growthDays[cropDetails.growthDays.Length-1])
                            {
                                // Check if crop fully grown
                                return cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode);
                            } else
                            {
                                return false;
                            }
                        }
                    }

                    return false;
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

            return false;
        }

        private Vector3 GetWorldPositionForCursor()
        {
            return grid.CellToWorld(GetGridPositionForCursor());
        }

        private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
        {
            return gridPropertyDetails.canDropItem;
        }

        private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
        {
            return gridPropertyDetails.canDropItem;
        }

        private void SetCursorToInvalid()
        {
            cursorImage.sprite=redCursorSprite;
            CursorPositionIsValid = false;
        }

        private void SetCursorToValid()
        {
            cursorImage.sprite=greenCursorSprite;
            CursorPositionIsValid = true;
        }

        public Vector3Int GetGridPositionForPlayer()
        {
            return grid.WorldToCell(Player.Instance.transform.position);
        }

        public Vector3Int GetGridPositionForCursor()
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -mainCamera.transform.position.z));
            return grid.WorldToCell(worldPosition);
        }
        private Vector3 GetRectTransformPositionForCursor(Vector3Int gridPosition)
        {
            Vector3 gridWorldPosition = grid.WorldToCell(gridPosition);
            Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
            return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
        }


    }
}
