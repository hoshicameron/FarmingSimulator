using Enums;
using Events;
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
    public class UIInventorySlot : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,
        IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
    {
        private Camera mainCamera;
        private Canvas parentCanvas;
        private Transform parentItem;
        private GameObject draggedItem;


        public Image inventorySlotHighlight;
        public Image inventorySlotImage;
        public TextMeshProUGUI text;

        [SerializeField] private UIInventoryBar inventoryBar=null;
        [SerializeField] private GameObject inventoryTextBoxPrefab=null;
        [HideInInspector] public ItemDetails itemDetails;
        [HideInInspector] public bool isSelected = false;
        [SerializeField] private GameObject itemPrefab = null;
        [HideInInspector] public int itemQuantity;
        [SerializeField] private int slotNumber = 0;

        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += SceneLoaded;
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        }

        private void SceneLoaded()
        {
            parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
        }

        private void Start()
        {
            mainCamera=Camera.main;

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemDetails != null)
            {
                // Disable keyboard input
                Player.Instance.DisablePlayerInputAndResetMovement();

                // Instantiate gameobject as dragged item
                draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem);
                draggedItem.transform.SetParent(inventoryBar.transform,false);

                // Get image for dragged item
                Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
                draggedItemImage.sprite = inventorySlotImage.sprite;

                SetSelectedItem();
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

                    // Destroy inventory text box
                    DestroyInventoryTextBox();

                    // Clear selected item
                    ClearSelectedItem();
                }
                //else attempt to drop the item if it can be dropped
                else
                {
                    if (itemDetails.canBeDropped)
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
            if (itemDetails != null && isSelected)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y,
                    -mainCamera.transform.position.z));

                //Create item from prefab at mouse position
                GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                // Remove item from player inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);

                // If no more of item then clear selected
                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.Player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Populate text box with item details
            if (itemQuantity != 0)
            {
                //Instantiate inventory text box
                inventoryBar.inventoryTextBoxGameObject =
                    Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
                inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

                UIInventoryTextBox inventoryTextBox =
                    inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

                // Set item type description
                string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

                // Populate text box
                inventoryTextBox.SetTextBoxText(itemDetails.itemDescription,itemTypeDescription,"",
                    itemDetails.itemLongDescription,"","");

                // Set the text box position according to inventory bar position
                if (inventoryBar.IsInventoryBarPositionBottom)
                {
                    inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot=new Vector2(0.5f,0f);
                    inventoryBar.inventoryTextBoxGameObject.transform.position=new Vector3(transform.position.x,transform.position.y+50f,
                        transform.position.z);
                } else
                {
                    inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot=new Vector2(0.5f,1f);
                    inventoryBar.inventoryTextBoxGameObject.transform.position=new Vector3(transform.position.x,transform.position.y-50f,
                        transform.position.z);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DestroyInventoryTextBox();
        }

        private void DestroyInventoryTextBox()
        {
            if (inventoryBar.inventoryTextBoxGameObject != null)
            {
                Destroy(inventoryBar.inventoryTextBoxGameObject);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // If left click
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // If inventory slot currently selected then deselect
                if (isSelected)
                {
                    ClearSelectedItem();
                } else
                {
                    if (itemQuantity > 0)
                    {
                        SetSelectedItem();
                    }
                }
            }
        }

        private void SetSelectedItem()
        {
            // Clear currently highlited items
            inventoryBar.ClearHighlightOnInventorySlots();

            // Highlight in inventory slots
            isSelected = true;

            // Set highlighted inventory slots
            inventoryBar.SetHighlightedInventorySlots();


            // Set item selected in invertory
            InventoryManager.Instance.SetSelectedItemInventory(InventoryLocation.Player,itemDetails.itemCode);

            if (itemDetails.canBeCarried == true)
            {
                // Show player carrying item
                Player.Instance.ShowCarriedItem(itemDetails.itemCode);
            } else  // show player carrying nothing
            {
                Player.Instance.clearCarriedItem();
            }
            {

            }
        }

        private void ClearSelectedItem()
        {
            // Clear currently highlighted items
            inventoryBar.ClearHighlightOnInventorySlots();

            isSelected = false;

            // Set no item selected in inventory
            InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.Player);

            // Clear player carrying item
            Player.Instance.clearCarriedItem();
        }
    }
}
