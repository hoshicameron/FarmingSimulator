using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "so_ItemList", menuName = "Scriptable Objects/Item/ItemList", order = 1)]
    public class SO_ItemList : ScriptableObject
    {
        [SerializeField] public List<ItemDetails> ItemDetails;
    }
}

