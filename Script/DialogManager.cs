using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogData
{
    public int Dialog_Id;
    public int Dialog_Next_Id;
    public string Dialog_String;
    public bool isEnd;
    public bool isNPC;
    public string Portrait;
    public string Portrait_Path;
}

[System.Serializable]
public class DialogDatabase
{
    public List<DialogData> Dialogs;
}

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;
    public List<DialogData> DialogList = new List<DialogData>();
    public Dictionary<int, Sprite> Portraits_Dict = new Dictionary<int, Sprite>();
    public Dictionary<int, DialogData> Dialog_Dict = new Dictionary<int, DialogData>();

    public PcManager pcManager;

    public GameObject DialogPanel;
    public Text DialogText;
    public Image PortraitImg;

    public bool isDialogOutput = false;
    public bool isTyping;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadDialogData();
        }
    }

    public void LoadDialogData()
    {
        string filePath = "Assets/Resources/Json/DialogData.json";
        if (File.Exists(filePath))
        {
            string jsonFile = File.ReadAllText(filePath);
            DialogDatabase data = JsonUtility.FromJson<DialogDatabase>(jsonFile);
            DialogList = data.Dialogs;

            foreach (var Dialog in DialogList)
            {
                Sprite Port = Resources.Load<Sprite>(Dialog.Portrait_Path + Dialog.Portrait);
                Portraits_Dict.Add(Dialog.Dialog_Id, Port);
                Dialog_Dict.Add(Dialog.Dialog_Id, Dialog);
            }
        }
        else
        {
            Debug.LogError("Dialog 데이터 파일이 존재하지 않습니다.");
        }
    }

    public void Dialog(int Id, ObjData objData)
    {
        if (isDialogOutput && isTyping)
        {
            isTyping = false;
            return;
        }


        else if (IdController.isIdControllEnd)
        {
            isDialogOutput = false;
            GameManager.Next_Id = 0;
            DialogPanel.SetActive(isDialogOutput);
            return;
        }

        if (Dialog_Dict.ContainsKey(Id))
        {
            bool isNPC = Dialog_Dict[Id].isNPC;
            GameManager.Next_Id = Id;

            GameManager.Progress_Id = Id;
            GameManager.Progress_ObjData = objData;

            GameManager.isAction = true;

            isDialogOutput = true;

            string DialogString = Dialog_Dict[Id].Dialog_String;

            StartCoroutine(TextOutput(DialogString));

            if (isNPC)
            {
                PortraitImg.sprite = Portraits_Dict[Id];
                PortraitImg.color = new Color(1, 1, 1, 1);
            }

            else
                PortraitImg.color = new Color(1, 1, 1, 0);

            if (Dialog_Dict[Id].isEnd)
                IdController.isIdControllEnd = true;

            GameManager.Next_Id = Dialog_Dict[Id].Dialog_Next_Id;
        }

        else
        {
            isTyping = false;
            isDialogOutput = false;
        }

        DialogPanel.SetActive(isDialogOutput);
    } // DIalog 출력 및 분류

    public IEnumerator TextOutput(string DialogInput)
    {
        DialogText.text = string.Empty;

        StringBuilder sb = new StringBuilder();

        isTyping = true;

        for (int i = 0; i < DialogInput.Length; i++)
        {
            if (!isTyping)
            {
                DialogText.text = DialogInput;
                break;
            }

            sb.Append(DialogInput[i]);
            DialogText.text = sb.ToString();
            yield return new WaitForSeconds(0.05f);

        }
        isTyping = false;

    }

}
