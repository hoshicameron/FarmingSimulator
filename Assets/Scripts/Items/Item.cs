using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private int itemCode;

        public int ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; }
        }

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            if (ItemCode != 0)
            {
                Init(ItemCode);
            }
        }

        private void Init(int itemCode)
        {

        }
    }
}
