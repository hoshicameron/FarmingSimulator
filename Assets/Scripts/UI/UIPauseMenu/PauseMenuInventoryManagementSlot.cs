using Enums;
using Inventory;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenuInventoryManagementSlot : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Image inventoryManagementSlotImage;
        public TextMeshProUGUI textMeshProUGUI;
        public GameObject greyedOutImageGO;

        [SerializeField] private PauseMenuInventoryManagement inventoryManagement = null;
        [SerializeField] private GameObject inventoryTextBoxPrefab = null;

        [HideInInspector] public ItemDetails itemDetails;
        [HideInInspector] public int itemQuantity;
        [SerializeField] private int slotNumber = 0;    //from 0-47

        // private Vector3 startingPosition;
        [HideInInspector] public GameObject draggedItem;
        private Canvas parentCanvas;

        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemQuantity != 0)
            {
                // Instantiate gameObject as dragged item
                draggedItem = Instantiate(inventoryManagement.inventoryManagementDraggedItemPrefab);
                draggedItem.transform.SetParent( inventoryManagement.transform,false);

                // Get Image for dragged item
                Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
                draggedItemImage.sprite = inventoryManagementSlotImage.sprite;

            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Move gameobject as dragged item
            if (draggedItem != null)
            {
                draggedItem.transform.position = Input.mousePosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Destroy gameobject as dragged item
            if (draggedItem != null)
            {
                Destroy(draggedItem);

                // Get object drug is over
                if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject
                        .GetComponent<PauseMenuInventoryManagementSlot>() != null)
                {
                    // Get the slot number where the drag ended
                    int toSlotNumber = eventData.pointerCurrentRaycast.gameObject
                                            .GetComponent<PauseMenuInventoryManagementSlot>().slotNumber;

                    // Swap inventory items in inventory list
                    InventoryManager.Instance.SwapInventoryItems(InventoryLocation.Player,slotNumber,toSlotNumber);

                    // Destroy inventory text box
                    inventoryManagement.DestroyInventoryTextBoxGameObject();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Populate text box with item details
            if (itemQuantity != 0)
            {
                //Instantiate inventory text box
                inventoryManagement.inventoryTextBoxGameObject =
                    Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
                inventoryManagement.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

                UIInventoryTextBox inventoryTextBox =
                    inventoryManagement.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

                // Set item type description
                string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

                // Populate text box
                inventoryTextBox.SetTextBoxText(itemDetails.itemDescription,itemTypeDescription,"",
                    itemDetails.itemLongDescription,"","");

                // Set the text box position according to inventory bar position
                if (slotNumber>23)
                {
                    inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot=new Vector2(0.5f,0f);
                    inventoryManagement.inventoryTextBoxGameObject.transform.position=new Vector3(transform.position.x,transform.position.y+50f,
                        transform.position.z);
                } else
                {
                    inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot=new Vector2(0.5f,1f);
                    inventoryManagement.inventoryTextBoxGameObject.transform.position=new Vector3(transform.position.x,transform.position.y-50f,
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
            if (inventoryManagement.inventoryTextBoxGameObject != null)
            {
                Destroy(inventoryManagement.inventoryTextBoxGameObject);
            }
        }
    }
}
