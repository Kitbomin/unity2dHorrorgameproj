using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private InventorySlot[] slots;

    public static List<ItemData> inventoryItemList = new List<ItemData>();
    private List<ItemData> inventoryTabList;

    public Text Description_Text;
    public string[] tabDescription;

    public Transform tf; //slot의 부모객체.

    public GameObject InventoryPanel;
    public GameObject[] selectedTabImages;

    private int selectedItem;
    private int selectedTab;

    private bool isInventoryOn;
    private bool isTabActivated;
    private bool isItemActivated;
    private bool preventExecute; //중복실행 제한

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    void Awake()
    {
        inventoryTabList = new List<ItemData>();
        slots = tf.GetComponentsInChildren<InventorySlot>();
    }

    public void RemoveSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    } //인벤토리 슬롯 초기화

    public void ShowTab()
    {
        RemoveSlot();
        SelectedTab();
        
    } //탭 활성화

    public void SelectedTab()
    {
        StopAllCoroutines();

        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
        color.a = 0f;

        for (int i = 0; i < selectedTabImages.Length; i++)
            selectedTabImages[i].GetComponent<Image>().color = color;

        Description_Text.text = tabDescription[selectedTab];

        StartCoroutine(SelectedTabEffectCoroutine());
    } //선택된 탭을 제외한 모든 탭 컬러값 조정

    IEnumerator SelectedTabEffectCoroutine()
    {
        while (isTabActivated)
        {
            Color color = selectedTabImages[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a = color.a + 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a = color.a - 0.03f;
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    } //선택된 탭 반짝이게

    public void ShowItem()
    {
        inventoryTabList.Clear();
        RemoveSlot();
        selectedItem = 0;

        switch (selectedTab)
        {

            case 0:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (inventoryItemList[i].Item_Type == "Item")
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;

            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (inventoryItemList[i].Item_Type == "Key_Item")
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;

            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (inventoryItemList[i].Item_Type == "Post_it")
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
        }

        for (int i = 0; i < inventoryTabList.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].Additem(inventoryTabList[i]);
        }

        SelectedItem();
    } // 아이템 활성화 ( 선택된 탭에 해당되는 아이템 출력 )

    public void SelectedItem()
    {
        StopAllCoroutines();

        if (inventoryTabList.Count > 0)
        {
            Color color = slots[0].selected_Item.GetComponent<Image>().color;
            color.a = 0f;

            for (int i = 0; i < inventoryTabList.Count; i++)
                slots[i].selected_Item.GetComponent<Image>().color = color;

            Description_Text.text = inventoryTabList[selectedItem].Item_Description;

            StartCoroutine(SelectedItemEffectCoroutine());
        }
        else
            Description_Text.text = "";
    } //선택된 아이템을 제외한 모든 탭 컬러값 조정

    IEnumerator SelectedItemEffectCoroutine()
    {
        while (isItemActivated)
        {
            Color color = slots[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a = color.a + 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a = color.a - 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    } //선택된 아이템 반짝이게

    public void InventoryOnOff()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {

            isInventoryOn = !isInventoryOn;

            if (isInventoryOn)
            {
                GameManager.isAction = true;
                InventoryPanel.SetActive(true);
                selectedTab = 0;
                isTabActivated = true;
                isItemActivated = false;

                ShowTab();
            }
            else
            {
                StopAllCoroutines();

                GameManager.isAction = false;
                GameManager.stopKeyInput = false;

                InventoryPanel.SetActive(false);
                isTabActivated = false;
                isItemActivated = false;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInventoryOn)
            {
                StopAllCoroutines();

                isInventoryOn = false;

                GameManager.isAction = false;
                GameManager.stopKeyInput = false;
                InventoryPanel.SetActive(false);
                isTabActivated = false;
                isItemActivated = false;
            }
        }
    } //인벤토리 열고 닫기

    public void HandleInventory()
    {
        if (isInventoryOn)
        {
            if (isTabActivated)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedTab < selectedTabImages.Length - 1)
                        selectedTab++;
                    else
                        selectedTab = 0;

                    SelectedTab();
                }

                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedTab > 0)
                        selectedTab--;
                    else
                        selectedTab = selectedTabImages.Length - 1;

                    SelectedTab();
                }

                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                    color.a = 0.25f;
                    selectedTabImages[selectedTab].GetComponent<Image>().color = color;

                    isItemActivated = true;
                    isTabActivated = false;
                    preventExecute = true;

                    ShowItem();
                }
            } // 탭 활성화시 키입력 처리.

            else if (isItemActivated)
            {
                
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selectedItem < inventoryTabList.Count - 2)
                        selectedItem = selectedItem + 2;
                    else
                        selectedItem = selectedItem % 2;

                    SelectedItem();
                }

                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (selectedItem > 1)
                        selectedItem = selectedItem - 2;
                    else if (inventoryTabList.Count != 0)
                        if (inventoryTabList.Count % 2 == 0)
                            selectedItem = selectedItem + inventoryTabList.Count - 2;
                        else
                            selectedItem = inventoryTabList.Count - ( 1 + selectedItem );

                    else
                        selectedItem = 0;

                    SelectedItem();
                }

                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedItem < inventoryTabList.Count - 1)
                        selectedItem++;
                    else
                        selectedItem = 0;

                    SelectedItem();
                }

                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedItem > 0)
                        selectedItem--;
                    else if (inventoryTabList.Count != 0)
                        selectedItem = inventoryTabList.Count - 1;
                    else
                        selectedItem = 0;

                    SelectedItem();
                }

                else if (Input.GetKeyDown(KeyCode.Z) && !preventExecute)
                {
                    if (selectedTab == 0) //기타 아이템
                    {
                        GameManager.stopKeyInput = true;
                        isTabActivated = false;
                    }

                    else if (selectedTab == 1) //핵심 아이템
                    {
                        GameManager.stopKeyInput = true;
                        isTabActivated = false;
                    }

                    else if (selectedTab == 2) //포스트잇
                    {
                        GameManager.stopKeyInput = true;
                        isTabActivated = false;
                    }
                }

                else if (Input.GetKeyDown(KeyCode.X))
                {
                    StopAllCoroutines();

                    isItemActivated = false;
                    isTabActivated = true;

                    ShowTab();
                }

            } //아이템 활성화시 키입력 처리.

            if (Input.GetKeyUp(KeyCode.Z))
            {
                preventExecute = false;
            } //중복 실행 방지.
        }
    } //인벤토리 선택
}


