using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private InventorySlot[] slots_1;
    private InventorySlot[] slots_2;

    public static List<ItemData> inventoryItemList = new List<ItemData>();
    private List<ItemData> inventoryTabList;

    public Text Description_Text;
    public string[] tabDescription;

    public Transform tf1; //slot의 부모객체.
    public Transform tf2; //post_it의 부모객체.

    public GameObject InventoryPanel;
    public GameObject Normal_Inv_Panel;
    public GameObject Postit_Inv_Panel;
    public GameObject[] selectedTabImages;

    private int selectedItem;
    private int selectedTab;

    private int page;
    private int Slot_Count;
    private const int Normal_Inv_Max_Slot = 12;
    private const int Postit_Inv_Max_Slot = 32;

    private bool isInventoryOn;
    private bool isTabActivated;
    private bool isItemActivated;
    private bool preventExecute; //중복실행 제한

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    void Awake()
    {
        inventoryTabList = new List<ItemData>();
        slots_1 = tf1.GetComponentsInChildren<InventorySlot>();
        slots_2 = tf2.GetComponentsInChildren<InventorySlot>();
    }

    public void RemoveSlot()
    {
        for (int i = 0; i < slots_1.Length; i++)
        {
            slots_1[i].RemoveItem();
            slots_1[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < slots_2.Length; i++)
        {
            slots_2[i].RemoveItem();
            slots_2[i].gameObject.SetActive(false);
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

        UnityEngine.Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
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
            UnityEngine.Color color = selectedTabImages[0].GetComponent<Image>().color;
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
        page = 0;


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

        ShowPage();

        

        SelectedItem();
    } // 아이템 활성화 ( 선택된 탭에 해당되는 아이템 출력 )

    public void ShowPage()
    {
        Slot_Count = 0;

        for (int i = page * Postit_Inv_Max_Slot; i < inventoryTabList.Count; i++)
        {
            if (inventoryTabList[i].Item_Type == "Post_it")
            {
                Slot_Count = i - (page * Postit_Inv_Max_Slot);
                slots_2[Slot_Count].gameObject.SetActive(true);
                slots_2[Slot_Count].Additem(inventoryTabList[i]);

                if (Slot_Count == Postit_Inv_Max_Slot - 1)
                    break;
            }
        }

        for (int i = page * Normal_Inv_Max_Slot; i < inventoryTabList.Count; i++)
        {
            if (inventoryTabList[i].Item_Type != "Post_it")
            {
                Slot_Count = i - (page * Normal_Inv_Max_Slot);
                slots_1[Slot_Count].gameObject.SetActive(true);
                slots_1[Slot_Count].Additem(inventoryTabList[i]);

                if (Slot_Count == Normal_Inv_Max_Slot - 1)
                    break;
            }
        }
    }

    public void SelectedItem()
    {
        StopAllCoroutines();

        if (Slot_Count > -1)
        {
            if (Postit_Inv_Panel.activeSelf)
            {
                UnityEngine.Color color_2 = slots_2[0].selected_Item.GetComponent<Image>().color;
                color_2.a = 0f;

                for (int i = 0; i <= Slot_Count; i++)
                {
                    slots_2[i].selected_Item.GetComponent<Image>().color = color_2;
                }
            }

            else
            {
                UnityEngine.Color color_1 = slots_1[0].selected_Item.GetComponent<Image>().color;
                color_1.a = 0f;

                for (int i = 0; i <= Slot_Count; i++)
                {
                    slots_1[i].selected_Item.GetComponent<Image>().color = color_1;
                }
            }

            
            Description_Text.text = inventoryTabList[selectedItem].Item_Description;

            StartCoroutine(SelectedItemEffectCoroutine());
        }
        else
            Description_Text.text = "";
    } //선택된 아이템을 제외한 모든 탭 컬러값 조정

    IEnumerator SelectedItemEffectCoroutine()
    {
        if (selectedTab == 2)
        {
            while (isItemActivated)
            {
                UnityEngine.Color color = slots_2[0].GetComponent<Image>().color;
                while (color.a < 0.5f)
                {
                    color.a = color.a + 0.03f;
                    slots_2[selectedItem].selected_Item.GetComponent<Image>().color = color;
                    yield return waitTime;
                }
                while (color.a > 0f)
                {
                    color.a = color.a - 0.03f;
                    slots_2[selectedItem].selected_Item.GetComponent<Image>().color = color;
                    yield return waitTime;
                }

                yield return new WaitForSeconds(0.3f);
            }
        }
        else
        {
            while (isItemActivated)
            {
                UnityEngine.Color color = slots_1[0].GetComponent<Image>().color;
                while (color.a < 0.5f)
                {
                    color.a = color.a + 0.03f;
                    slots_1[selectedItem].selected_Item.GetComponent<Image>().color = color;
                    yield return waitTime;
                }
                while (color.a > 0f)
                {
                    color.a = color.a - 0.03f;
                    slots_1[selectedItem].selected_Item.GetComponent<Image>().color = color;
                    yield return waitTime;
                }

                yield return new WaitForSeconds(0.3f);
            }
        }
    } //선택된 아이템 반짝이게

    public void InventoryOnOff()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {

            isInventoryOn = !isInventoryOn;

            if (isInventoryOn)
            {
                InventoryOn();
            }
            else
            {
                InventoryOff();
            }
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInventoryOn)
            {
                InventoryOff();
            }
        }

        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (isInventoryOn && isTabActivated)
            {
                InventoryOff();
            }
        }
    } //인벤토리 열고 닫기
    void InventoryOn()
    {
        GameManager.isAction = true;
        InventoryPanel.SetActive(true);
        Normal_Inv_Panel.SetActive(true);
        Postit_Inv_Panel.SetActive(false);
        selectedTab = 0;
        isTabActivated = true;
        isItemActivated = false;

        page = 0;

        ShowTab();
    }
    void InventoryOff()
    {
        StopAllCoroutines();

        isInventoryOn = false;

        GameManager.isAction = false;
        GameManager.stopKeyInput = false;
        InventoryPanel.SetActive(false);
        Normal_Inv_Panel.SetActive(false);
        Postit_Inv_Panel.SetActive(false);
        isTabActivated = false;
        isItemActivated = false;
    }
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
                    UnityEngine.Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                    color.a = 0.25f;
                    selectedTabImages[selectedTab].GetComponent<Image>().color = color;

                    isItemActivated = true;
                    isTabActivated = false;
                    preventExecute = true;

                    if (selectedTab == 2)
                    {
                        Postit_Inv_Panel.SetActive(true);
                        Normal_Inv_Panel.SetActive(false);
                    }
                    else
                    {
                        Postit_Inv_Panel.SetActive(false);
                        Normal_Inv_Panel.SetActive(true);
                    }

                    ShowItem();
                }
            } // 탭 활성화시 키입력 처리.

            else if (isItemActivated)
            {
                HandleInvSelectItem();

            } //아이템 활성화시 키입력 처리.

            if (Input.GetKeyUp(KeyCode.Z))
            {
                preventExecute = false;
            } //중복 실행 방지.
        }
    } //인벤토리 선택

    void HandleInvSelectItem()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Postit_Inv_Panel.activeSelf)
                HandlePostitInvDown();
            else
                HandleNormalInvDown();
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Postit_Inv_Panel.activeSelf)
                HandlePostitInvUp();
            else
                HandleNormalInvUp();
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Postit_Inv_Panel.activeSelf)
                HandlePostitInvRight();
            else
                HandleNormalInvRight();
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (Postit_Inv_Panel.activeSelf)
                HandlePostitInvLeft();
            else
                HandleNormalInvLeft();
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
    }//Normal_Inv 와 Postit_Inv 구별

    void HandleNormalInvDown()
    {
        if (selectedItem + 2 > Slot_Count)
        {
            if (page < (inventoryTabList.Count - 1) / Normal_Inv_Max_Slot)
            {
                page++;
            }
            else
            {
                page = 0;
            }

            RemoveSlot();
            ShowPage();

            selectedItem = selectedItem % 2;

            SelectedItem();
        }
        else
        {
            if (selectedItem < Slot_Count - 1)
                selectedItem = selectedItem + 2;
            else
                selectedItem = selectedItem % 2;

            SelectedItem();
        }
    }
    void HandleNormalInvUp()
    {
        if (selectedItem - 2 < 0)
        {
            if (page == 0)
            {
                page = (int)(Mathf.Ceil(inventoryTabList.Count - 1) / Normal_Inv_Max_Slot);
                selectedItem = (inventoryTabList.Count - 1) % Normal_Inv_Max_Slot - (selectedItem % 2);
            }
            else
            {
                page--;
                selectedItem = (Normal_Inv_Max_Slot - 1) - ((selectedItem + 1) % 2);
            }

            RemoveSlot();
            ShowPage();
            SelectedItem();
        }
        else
        {
            if (selectedItem > 1)
                selectedItem = selectedItem - 2;
            else if (inventoryTabList.Count != 0)
                if (inventoryTabList.Count % 2 == 0)
                    selectedItem = selectedItem + inventoryTabList.Count - 2;
                else
                    selectedItem = Slot_Count - selectedItem;

            else
                selectedItem = 0;

            SelectedItem();
        }
    }
    void HandleNormalInvRight()
    {
        if (selectedItem + 1 > Slot_Count)
        {
            if (page < (inventoryTabList.Count - 1) / Normal_Inv_Max_Slot)
            {
                page++;
            }
            else
            {
                page = 0;
            }

            RemoveSlot();
            ShowPage();

            selectedItem = 0;

            SelectedItem();
        }
        else
        {
            if (selectedItem - 1 < Slot_Count)
                selectedItem++;
            else
                selectedItem = 0;

            SelectedItem();
        }
    }
    void HandleNormalInvLeft()
    {
        if (selectedItem - 1 < 0)
        {
            if (page == 0)
            {
                page = (int)(Mathf.Ceil(inventoryTabList.Count - 1) / Normal_Inv_Max_Slot);
                selectedItem = (inventoryTabList.Count - 1) % Normal_Inv_Max_Slot;
            }
            else
            {
                page--;
                selectedItem = Normal_Inv_Max_Slot - 1;
            }

            RemoveSlot();
            ShowPage();
            SelectedItem();
        }
        else
        {
            if (selectedItem > 0)
                selectedItem--;
            else if (inventoryTabList.Count != 0)
                selectedItem = Slot_Count;
            else
                selectedItem = 0;

            SelectedItem();
        }
    }

    void HandlePostitInvDown()
    {
        if (selectedItem + 8 > Slot_Count)
        {
            if (page < (inventoryTabList.Count - 1) / Postit_Inv_Max_Slot)
            {
                page++;
            }
            else
            {
                page = 0;
                
            }

            RemoveSlot();
            ShowPage();

            selectedItem = selectedItem % 8;

            SelectedItem();
        }
        else
        {
            if (selectedItem < Slot_Count - 1)
                selectedItem = selectedItem + 8;
            else
                selectedItem = selectedItem % 8;

            SelectedItem();
        }
    }
    void HandlePostitInvUp()
    {
        if (selectedItem < 8)
        {
            if (page == 0)
            {
                page = (int)(Mathf.Ceil(inventoryTabList.Count - 1) / Postit_Inv_Max_Slot);
            }
            else
            {
                page--;
            }

            RemoveSlot();
            ShowPage();

            selectedItem = selectedItem + (int)(Mathf.Floor(Slot_Count / 8)) * 8;
            if (selectedItem > Slot_Count)
                selectedItem = selectedItem - 8;

            SelectedItem();
        }
        else
        {
            if (selectedItem > 7)
                selectedItem = selectedItem - 8;
            else if (inventoryTabList.Count != 0)
            {
                selectedItem = selectedItem + (int)(Mathf.Floor(Slot_Count * 8));
                Debug.Log(selectedItem);
                if (selectedItem > Slot_Count)
                {
                    selectedItem = selectedItem - 8;
                }
            }


            else
                selectedItem = 0;

            SelectedItem();
        }
    }
    void HandlePostitInvRight()
    {
        if (selectedItem + 1 > Slot_Count)
        {
            if (page < (inventoryTabList.Count - 1) / Postit_Inv_Max_Slot)
            {
                page++;
                
            }
            else
            {
                page = 0;
            }

            RemoveSlot();
            ShowPage();

            selectedItem = 0;

            SelectedItem();
        }
        else
        {
            if (selectedItem - 1 < Slot_Count)
                selectedItem++;
            else
                selectedItem = 0;

            SelectedItem();
        }
    }
    void HandlePostitInvLeft()
    {
        if (selectedItem - 1 < 0)
        {
            if (page == 0)
            {
                page = (int)(Mathf.Ceil(inventoryTabList.Count - 1) / Postit_Inv_Max_Slot);
                selectedItem = (inventoryTabList.Count - 1) % Postit_Inv_Max_Slot;
            }
            else
            {
                page--;
                selectedItem = Postit_Inv_Max_Slot - 1;
            }

            RemoveSlot();
            ShowPage();
            SelectedItem();
        }
        else
        {
            if (selectedItem > 0)
                selectedItem--;
            else if (inventoryTabList.Count != 0)
                selectedItem = Slot_Count;
            else
                selectedItem = 0;

            SelectedItem();
        }
    }
}


