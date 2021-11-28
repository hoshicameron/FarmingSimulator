using System.Collections.Generic;
using Enums;
using Events;
using Inventory;
using Items;
using UnityEngine;
using _Player;

namespace UI
{
    public class UIInventoryBar : MonoBehaviour
    {
        [SerializeField] private Sprite blank16x16Sprite = null;
        [SerializeField] private UIInventorySlot[] inventorySlotArray = null;
        public GameObject inventoryBarDraggedItem;

        private void OnEnable()
        {
            EventHandler.InventoryUpdatedEvent+=OnInventoryUpdatedEvent;
        }

        private void OnDisable()
        {
            EventHandler.InventoryUpdatedEvent-=OnInventoryUpdatedEvent;
        }
        private void ClearInventorySlots()
        {
            if (inventorySlotArray.Length > 0)
            {
                // Loop through all slots and update with blank sprite
                for (int i = 0; i < inventorySlotArray.Length; i++)
                {
                    inventorySlotArray[i].inventorySlotImage.sprite = blank16x16Sprite;
                    inventorySlotArray[i].text.text = "";
                    inventorySlotArray[i].itemDetails = null;
                    inventorySlotArray[i].itemQuantity = 0;
                    SetHighlightedInventorySlots(i);
                }
            }
        }

        private void OnInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
        {
            if (inventoryLocation == InventoryLocation.Player)
            {
                ClearInventorySlots();
                // If there is room for new item and inventory list item not empty
                if (inventorySlotArray.Length > 0 && inventoryList.Count > 0)
                {
                    // Loop through inventory slots and update with corresponding inventory list item
                    for (int i = 0; i < inventorySlotArray.Length; i++)
                    {
                        if (i < inventoryList.Count)
                        {
                            int itemCode = inventoryList[i].itemCode;

                            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                            if (itemDetails != null)
                            {
                                // Add image and details to inventory item slot
                                inventorySlotArray[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                                inventorySlotArray[i].text.text=inventoryList[i].itemQuantity.ToString();
                                inventorySlotArray[i].itemDetails = itemDetails;
                                inventorySlotArray[i].itemQuantity = inventoryList[i].itemQuantity;
                                SetHighlightedInventorySlots(i);
                            }
                        } else
                        {
                            break;
                        }
                    }
                }
            }
        }



        private RectTransform rectTransform;
        [HideInInspector] public GameObject inventoryTextBoxGameObject;

        public bool IsInventoryBarPositionBottom { get; set; } = true;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            SwitchInventoryBarPosition();
        }

        private void SwitchInventoryBarPosition()
        {
            Vector3 playerViewvportPosition = Player.Instance.GetPlayerViewPortPosition();

            if(playerViewvportPosition.y>0.3f && IsInventoryBarPositionBottom==false)
            {
                // transform.position= new Vector3(transform.position.x,7.5f,0f); // This was changed to control the rectTransform see below
                rectTransform.pivot=new Vector2(0.5f,0f);
                rectTransform.anchorMin=new Vector2(0.5f,0f);
                rectTransform.anchorMax=new Vector2(0.5f,0f);
                rectTransform.anchoredPosition= new Vector2(0f,2.5f);

                IsInventoryBarPositionBottom = true;
            }else if (playerViewvportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
            {
                // transform.position= new Vector3(transform.position.x,mainCamera.pixelHeight -120f,0f); // This was changed to control the rectTransform see below
                rectTransform.pivot=new Vector2(0.5f,1f);
                rectTransform.anchorMin=new Vector2(0.5f,1f);
                rectTransform.anchorMax=new Vector2(0.5f,1f);
                rectTransform.anchoredPosition= new Vector2(0f,-2.5f);

                IsInventoryBarPositionBottom = false;
            }
        }

        /// <summary>
        /// Clear all highlighted from inventory bar
        /// </summary>
        public void ClearHighlightOnInventorySlots()
        {
            if (inventorySlotArray.Length > 0)
            {
                //Loop through inventory slots and clear highlight sprites
                for (int i = 0; i < inventorySlotArray.Length; i++)
                {
                    if (inventorySlotArray[i].isSelected)
                    {
                        inventorySlotArray[i].isSelected = false;
                        inventorySlotArray[i].inventorySlotHighlight.color=new Color(0f,0f,0f,0f);
                        //Update inventory to show item as not selected
                        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.Player);
                    }
                }
            }
        }

        /// <summary>
        /// Set the selected highlight if set on all inventory item positions
        /// </summary>
        public void SetHighlightedInventorySlots()
        {
            if (inventorySlotArray.Length > 0)
            {
                // Loop through inventory slots and clear highlight sprites
                for (int i = 0; i < inventorySlotArray.Length; i++)
                {
                    SetHighlightedInventorySlots(i);
                }
            }
        }

        /// <summary>
        /// Set the selected highlight if set on an inventory item for a given slot item position
        /// </summary>
        /// <param name="itemPosition"></param>

        public void SetHighlightedInventorySlots(int itemPosition)
        {
            if (inventorySlotArray.Length > 0 && inventorySlotArray[itemPosition].itemDetails != null)
            {
                if (inventorySlotArray[itemPosition].isSelected)
                {
                    inventorySlotArray[itemPosition].inventorySlotHighlight.color=new Color(1f,1f,1f,1f);

                    // Update Inventory to show item as selected
                    InventoryManager.Instance.SetSelectedItemInventory(InventoryLocation.Player,
                        inventorySlotArray[itemPosition].itemDetails.itemCode);
                }
            }
        }

        public void DestroyCurrentlyDraggedItems()
        {
            for (int i = 0; i < inventorySlotArray.Length; i++)
            {
                if (inventorySlotArray[i].draggedItem != null)
                {
                    Destroy(inventorySlotArray[i].draggedItem);
                }
            }
        }

        public void ClearCurrentlySelectedItems()
        {
            for (int i = 0; i < inventorySlotArray.Length; i++)
            {
                inventorySlotArray[i].ClearSelectedItem();
            }
        }
    }
}
