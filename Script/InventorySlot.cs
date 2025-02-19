using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemManager itemManager;

    public Image icon;
    public Text item_Name_Text;
    public Text item_Count_Text;
    public GameObject selected_Item;

    public void Additem(ItemData _item)
    {
        item_Name_Text.text = _item.Item_Name;

        if (itemManager.Item_Icons[_item.Item_Id] != null)
            icon.sprite = itemManager.Item_Icons[_item.Item_Id];

        if (_item.Item_Type == "Item")
        {
            Debug.Log(_item.Item_Type);
            if (_item.Item_Count > 0)
                item_Count_Text.text = ": " + _item.Item_Count.ToString();
            else
                item_Count_Text.text = "";
        }
        else
            item_Count_Text.text = "";
    }


    public void RemoveItem()
    {
        item_Name_Text.text = "";
        item_Count_Text.text = "";
        icon.sprite = null;
    }
}
