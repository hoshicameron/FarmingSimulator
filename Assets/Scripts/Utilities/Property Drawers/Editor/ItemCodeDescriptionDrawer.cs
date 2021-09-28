
using System;
using System.Collections.Generic;
using Items;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]

public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Change the returned property height to be double to cater for additional
        // item code description that we will draw
        return EditorGUI.GetPropertyHeight(property) * 2f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();    //Start of check for changed values

            // Draw item code
            var newValue = EditorGUI.IntField(
                new Rect(position.x, position.y, position.width, position.height / 2),
                label, property.intValue);

            // Draw item discription
            EditorGUI.LabelField(new Rect(position.x,position.y+position.height/2,
                position.width,position.height/2),"Item Description",GetItemDescription(property.intValue) );


            // if item code values changed, then set value to new value
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }
        EditorGUI.EndProperty();
    }

    private string GetItemDescription(int itemCode)
    {
        SO_ItemList soItemList = (SO_ItemList) AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/Items/so_ItemList.asset",
            typeof(SO_ItemList));

        List<ItemDetails> itemDetailsList = soItemList != null ? soItemList.ItemDetailsList : null;
        if (itemDetailsList == null) return string.Empty;

        ItemDetails itemDetails = itemDetailsList.Find(x => x.itemCode==itemCode);
        return itemDetails != null ? itemDetails.itemDescription : string.Empty;

    }
}
