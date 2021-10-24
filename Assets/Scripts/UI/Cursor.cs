using System;
using System.Collections.Generic;
using Enums;
using HelperClasses;
using Inventory;
using Items;
using UnityEngine;
using UnityEngine.UI;
using _Player;

namespace UI
{
    public class Cursor : MonoBehaviour
    {
        private Canvas canvas;
        private Camera mainCamera;

        [SerializeField] private Image cursorImage = null;
        [SerializeField] private RectTransform cursorRectTransform = null;
        [SerializeField] private Sprite greenCursorSprite = null;
        [SerializeField] private Sprite transparentCursorSprite = null;
        [SerializeField] private GridCursor gridCursor = null;

        public bool CursorIsEnabled { get; set; } = false;
        public bool CursorPositionIsValid { get; set; } = false;
        public ItemType SelectedItemType { get; set; }
        public float ItemUseRadius { get; set; } = 0f;

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

        private void DisplayCursor()
        {
            // Get position for cursor
            Vector3 cursorWorldPosition = GetWorldPositionForCursor();

            // Set cursor Sprite
            SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCenterPosition());

            // Get rectTransform position for cursor
            cursorRectTransform.position = GetRectTransformPositionForCursor();
        }
        private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
        {
            SetCursorToValid();

            //Check use radius corners
            if (
                cursorPosition.x > (playerPosition.x + ItemUseRadius * 0.5f) &&
                cursorPosition.y > (playerPosition.y + ItemUseRadius * 0.5f)
                ||
                cursorPosition.x < (playerPosition.x - ItemUseRadius * 0.5f) &&
                cursorPosition.y > (playerPosition.y + ItemUseRadius * 0.5f)
                ||
                cursorPosition.x < (playerPosition.x - ItemUseRadius * 0.5f) &&
                cursorPosition.y < (playerPosition.y - ItemUseRadius * 0.5f)
                ||
                cursorPosition.x > (playerPosition.x + ItemUseRadius * 0.5f) &&
                cursorPosition.y < (playerPosition.y - ItemUseRadius * 0.5f)
            )
            {
                SetCursorToInValid();
                return;
            }

            // Check if item use radius is valid
            if(Mathf.Abs(cursorPosition.x-playerPosition.x)>ItemUseRadius
                        || (cursorPosition.y-playerPosition.y)>ItemUseRadius)
            {
                SetCursorToInValid();
                return;
            }

            // Get selected item details
            ItemDetails itemDetails =
                InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);

            if (itemDetails == null)
            {
                SetCursorToInValid();
                return;
            }

            // Determine cursor validity based on inventory item selected and what object the cursor id over
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                case ItemType.Commodity:
                case ItemType.Watering_Tool:
                case ItemType.HoeingTool:
                case ItemType.Chopping_Tool:
                case ItemType.BreakingTool:
                case ItemType.Reaping_Tool:
                case ItemType.Collecting_Tool:
                    if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                    {
                        SetCursorToInValid();
                        break;
                    }
                    break;

                case ItemType.None:
                    break;
                case ItemType.Count:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Set the cursor to be valid
        /// </summary>
        private void SetCursorToValid()
        {
            cursorImage.sprite = greenCursorSprite;
            CursorPositionIsValid = true;

            gridCursor.DisableCursor();
        }

        /// <summary>
        /// Set the cursor to be invalid
        /// </summary>
        private void SetCursorToInValid()
        {
            cursorImage.sprite = transparentCursorSprite;
            CursorPositionIsValid = false;

            gridCursor.EnableCursor();
        }

        /// <summary>
        /// Set the cursor as either valid or invalid for the tool for the target. Returns true if valid or false if invalid
        /// </summary>
        /// <param name="cursorPosition"></param>
        /// <param name="playerPosition"></param>
        /// <param name="itemDetails"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
        {
            // Switch on tool
            switch (itemDetails.itemType)
            {

                case ItemType.Chopping_Tool:
                    break;
                case ItemType.BreakingTool:
                    break;
                case ItemType.Reaping_Tool:
                    return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
                case ItemType.Collecting_Tool:
                    break;
                default:
                    return false;
            }

            return false;
        }

        private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
        {
            List<Item> itemList=new List<Item>();

            if (HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList, cursorPosition))
            {
                if (itemList.Count != 0)
                {
                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType ==
                            ItemType.Reapable_scanary)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void DisableCursor()
        {
            cursorImage.color=new Color(1f,1f,1f,0f);
            CursorIsEnabled = false;
        }

        public void EnableCursor()
        {
            cursorImage.color=new Color(1f,1f,1f,1f);
            CursorIsEnabled = true;
        }

        private Vector3 GetRectTransformPositionForCursor()
        {
            Vector2 screenPosition=new Vector2(Input.mousePosition.x,Input.mousePosition.y);

            return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
        }

        public Vector3 GetWorldPositionForCursor()
        {
            return mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        }
    }
}
