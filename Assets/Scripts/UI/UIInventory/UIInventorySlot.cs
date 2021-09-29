using Enums;
using Inventory;
using Items;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using _Player;

namespace UI
{
    public class UIInventorySlot : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        private Camera mainCamera;
        private Transform parentItem;
        private GameObject draggedItem;


        public Image inventorySlotHighlight;
        public Image inventorySlotImage;
        public TextMeshProUGUI text;

        [SerializeField] private UIInventoryBar inventoryBar=null;
        [HideInInspector] public ItemDetails ItemDetails;
        [SerializeField] private GameObject itemPrefab = null;
        [HideInInspector] public int itemQuantity;
        [SerializeField] private int slotNumber = 0;

        private void Start()
        {
            mainCamera=Camera.main;
            parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (ItemDetails != null)
            {
                // Disable keyboard input
                Player.Instance.DisablePlayerInputAndResetMovement();

                // Instantiate gameobject as dragged item
                draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem);
                draggedItem.transform.SetParent(inventoryBar.transform,false);

                // Get image for dragged item
                Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
                draggedItemImage.sprite = inventorySlotImage.sprite;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Move game object as dragged item
            if (draggedItem!=null)
            {
                draggedItem.transform.position = Input.mousePosition;

            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Destroy game object as dragged item
            if (draggedItem != null)
            {
                Destroy(draggedItem);

                // If drag ends over inventory bar, get item drag is over and swap them
                if (eventData.pointerCurrentRaycast.gameObject != null &&
                    eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
                {
                    // Get the slot number where the drag ended
                    int toSlotnumber = eventData.pointerCurrentRaycast.gameObject.
                        GetComponent<UIInventorySlot>().slotNumber;

                    // Swap inventory items in inventory list
                    InventoryManager.Instance.SwapInventoryItems(InventoryLocation.Player, slotNumber, toSlotnumber);
                }
                //else attempt to drop the item if it can be dropped
                else
                {
                    if (ItemDetails.canBeDropped)
                    {
                        DropSelectedItemAtMousePosition();
                    }
                }
            }

            // Enable player input
            Player.Instance.EnablePlayerInput();
        }

        /// <summary>
        /// Drop the item (if selected) at the current mouse position, Called by DropItem event.
        /// </summary>
        private void DropSelectedItemAtMousePosition()
        {
            if (ItemDetails != null)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y,
                    -mainCamera.transform.position.z));

                //Create item from prefab at mouse position
                GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = ItemDetails.itemCode;

                // Remove item from player inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);
            }
        }
    }
}
