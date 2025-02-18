using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IdController : MonoBehaviour
{
    public DialogManager dialogManager;
    public PcManager pcManager;
    public ItemManager itemManager;

    public Hero player;
    public static bool isIdControllEnd;

    public Dictionary<int, Sprite> Portraits_Dict = new Dictionary<int, Sprite>();
        

    public void IdControll(int Id, ObjData objData = null)
    {
        if (Id.ToString()[0] == '1' || dialogManager.isTyping) // DIalog
        {
            dialogManager.Dialog(Id, objData);
        }

        else if (Id.ToString()[0] == '2') // Item
        {
            itemManager.Item(Id);
        }

        else if (Id.ToString()[0] == '3') // Pc
        {
            pcManager.Pc(Id);
        }

        else if (Id.ToString()[0] == '4') // Interaction
        {
            return;
        }

        GameManager.Progress_Id = GameManager.Next_Id;

        if (GameManager.Progress_Id == 0)
        {
            GameManager.Progress_ObjData = null;
            GameManager.isAction = false;
            isIdControllEnd = false;
         }

        else
            GameManager.Progress_ObjData = objData;

        Debug.Log(GameManager.Progress_Id);
    } // Id에 따라 Dialog, Item, Pc 등의 함수로 분류
}
