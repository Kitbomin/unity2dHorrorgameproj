using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ItemType { Item, Key_Item, Post_it }

[System.Serializable]
public class ItemData
{
    public int Item_Id;
    public string Item_Name;
    public string Item_Description;
    public string Item_Type;
    public int Item_Count;
    public int Next_Id;
    public bool Item_Useable;
    public string Item_Icon_Path;
}

[System.Serializable]
public class ItemDatabase
{
    public List<ItemData> Items;
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public List<ItemData> ItemList = new List<ItemData>();
    public Dictionary<int, Sprite> Item_Icons = new Dictionary<int, Sprite>();
    public Dictionary<int, ItemData> Item_Dict = new Dictionary<int, ItemData>();

    public IdController idController;
    public DialogManager dialogManager;
    public Hero player;
    public GameObject ItemImage_Panel;
    public Image ItemImage;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadItemData();
        }
    }

    public void LoadItemData()
    {
        string filePath = "Assets/Resources/Json/ItemData.json";
        if (File.Exists(filePath))
        {
            string jsonFile = File.ReadAllText(filePath);
            ItemDatabase data = JsonUtility.FromJson<ItemDatabase>(jsonFile);
            ItemList = data.Items;

            foreach (var item in ItemList)
            {
                Sprite Icon = Resources.Load<Sprite>(item.Item_Icon_Path);
                Item_Dict.Add(item.Item_Id, item);
                Item_Icons.Add(item.Item_Id, Icon);
            }
        }
        else
        {
            Debug.LogError("Dialog 데이터 파일이 존재하지 않습니다.");
        }
    }

    public void Item(int Id)
    {
        if (Item_Dict.ContainsKey(Id))
        {
            ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), Item_Dict[Id].Item_Type);

            if (itemType == ItemType.Item)
            {
                Get_Item(Id);
            }

            else if (itemType == ItemType.Key_Item)
            {
                if (ItemImage.sprite == Item_Icons[Id])
                {
                    ItemImage_Panel.SetActive(false);
                    ItemImage.sprite = null;
                    Get_Item(Id);
                    idController.IdControll(GameManager.Next_Id);
                }
                else
                {
                    dialogManager.DialogPanel.SetActive(false);
                    ItemImage_Panel.SetActive(true);
                    ItemImage.sprite = Item_Icons[Id];
                }
            }

            else if ( itemType == ItemType.Post_it)
            {
                if (ItemImage.sprite == Item_Icons[Id])
                {
                    ItemImage_Panel.SetActive(false);
                    ItemImage.sprite = null;
                    Get_Item(Id);
                    idController.IdControll(GameManager.Next_Id);
                }
                else
                {
                    dialogManager.DialogPanel.SetActive(false);
                    ItemImage_Panel.SetActive(true);
                    ItemImage.sprite = Item_Icons[Id];
                }
            }
        }
    } // Item 획득 및 분류

    void Get_Item(int Id)
    {
        if (Inventory.inventoryItemList.Contains(Item_Dict[Id]))
            Item_Dict[Id].Item_Count++;
        else
        {
            Inventory.inventoryItemList.Add(Item_Dict[Id]);
            Item_Dict[Id].Item_Count++;
        }

        if (player.scanObj != null)
            if (player.scanObj.GetComponent<ObjData>().isItem)
            {
                player.scanObj.SetActive(false);
            }

            else
            {
                player.scanObj.GetComponent<ObjData>().Id = Item_Dict[Id].Next_Id;
            }

        GameManager.Next_Id = Item_Dict[Id].Next_Id;

        dialogManager.isDialogOutput = false;
        dialogManager.DialogPanel.SetActive(dialogManager.isDialogOutput);

        GameManager.isAction = false;
    }
}