using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PcData
{
    public int Pc_Id;
    public string Pc_Password;
    public int Pc_Success_Id;
    public int Pc_Fail_Id;
    public int Pc_Clear_Id;
    public bool Pc_Clear;
    public string Pc_Image_Path;
}

[System.Serializable]
public class PcDatabase
{
    public List<PcData> Pcs;
}

public class PcManager : MonoBehaviour
{
    public static PcManager instance;
    public List<PcData> PcList = new List<PcData>();
    public PcData pcData;

    public Dictionary<int, Sprite> PcImg_Dict = new Dictionary<int, Sprite>();
    public Dictionary<int, PcData> Pc_Dict = new Dictionary<int, PcData>();

    public DialogManager dialogManager;
    public IdController idController;
    public Hero player;

    public GameObject PcPanel;
    public InputField Pc_InputField;
    public Image PcImg;


    private bool isPcOn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadPcData();
        }
    }

    public void LoadPcData()
    {
        string filePath = "Assets/Resources/Json/PcData.json";
        if (File.Exists(filePath))
        {
            string jsonFile = File.ReadAllText(filePath);
            PcDatabase data = JsonUtility.FromJson<PcDatabase>(jsonFile);
            PcList = data.Pcs;

            foreach (var _Pc in PcList)
            {
                Sprite Port = Resources.Load<Sprite>(_Pc.Pc_Image_Path);
                PcImg_Dict.Add(_Pc.Pc_Id, Port);
                Pc_Dict.Add(_Pc.Pc_Id, _Pc);
            }
        }
        else
        {
            Debug.LogError("Pc ������ ������ �������� �ʽ��ϴ�.");
        }
    }

    public void Pc(int Id)
    {
        if (Pc_Dict.ContainsKey(Id))
        {
            if (Pc_Dict[Id].Pc_Clear)
            {
                GameManager.Progress_Id = Pc_Dict[Id].Pc_Clear_Id;
                idController.IdControll(GameManager.Progress_Id);
                return;
            }

            else if (isPcOn)
            {

                if (Pc_InputField.text.ToString() == Pc_Dict[Id].Pc_Password) // ����
                {
                    isPcOn = false;
                    GameManager.isAction = false;


                    Pc_Dict[Id].Pc_Clear = true;

                    if (player.scanObj != null)
                        player.scanObj.GetComponent<ObjData>().Id = Id;

                    GameManager.Next_Id = Pc_Dict[Id].Pc_Success_Id;
                    idController.IdControll(GameManager.Next_Id);

                }
                else if (Pc_InputField.text == "") // ��ĭ�� �ƴҶ�
                {
                    return;
                }

                else // ����
                {
                    isPcOn = false;
                    GameManager.isAction = false;

                    GameManager.Next_Id = Pc_Dict[Id].Pc_Fail_Id;
                    idController.IdControll(GameManager.Next_Id);
                }
            }

            else
            {
                dialogManager.isDialogOutput = false;
                dialogManager.DialogPanel.SetActive(dialogManager.isDialogOutput);
                Pc_InputField.text = "";
                PcImg.sprite = PcImg_Dict[Id];
                GameManager.Next_Id = Id;
                GameManager.isAction = true;
                isPcOn = true;
            }

        }

        PcPanel.SetActive(isPcOn);
    } // Pc ��й�ȣ ��� �� �з�
}

