using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIInventorySlot : MonoBehaviour
    {
        public Image inventorySlotHighlight;
        public Image inventorySlotImage;
        public TextMeshProUGUI text;

        [HideInInspector] public ItemDetails ItemDetails;
        [HideInInspector] public int itemQuantity;


    }
}
