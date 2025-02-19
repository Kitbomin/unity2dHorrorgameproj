using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    public IdController idController;
    public DialogManager dialogManager;
    public PcManager pcManager;

    public static bool isAction;
    public static bool stopKeyInput;

    public static int Progress_Id = 0;
    public static int Next_Id;

    public static ObjData Progress_ObjData = null;

    
    public void Action(int Id, ObjData objData = null)
    {
        if (Id != 0)
            idController.IdControll(Id, objData);

    }
}