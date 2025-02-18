using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class Hero : MonoBehaviour
{

    public GameManager manager;
    public DialogManager dialogManager;
    public PcManager pcManager;
    public Inventory InvManager;

    public Rigidbody2D rigid;
    Animator anime;
    SpriteRenderer spriter;
    public GameObject scanObj;

    public Vector3 dirVec;

    bool isHorizonMove;

    public float Speed = 3;
    
    float h;
    float v;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
    }

    private void Update()
    {
        
        if (!GameManager.stopKeyInput)
            HandleMovement();
            HandleAnimator();
            HandleInteraction();
            InvManager.InventoryOnOff();

        InvManager.HandleInventory();
    }

    private void FixedUpdate()
    {
        HandlePlayerMove();
        HandleScanObject();
    }

    private void LateUpdate()
    {
        HandlePlayerFlip();
    }
    void HandleMovement()
    {
        h = GameManager.isAction ? 0 : Input.GetAxisRaw("Horizontal");
        v = GameManager.isAction ? 0 : Input.GetAxisRaw("Vertical");

        bool hDown = GameManager.isAction ? false : Input.GetButtonDown("Horizontal");
        bool vDown = GameManager.isAction ? false : Input.GetButtonDown("Vertical");
        bool hUP = GameManager.isAction ? false : Input.GetButtonUp("Horizontal");
        bool vUP = GameManager.isAction ? false : Input.GetButtonUp("Vertical");

        if (hDown || vUP)
            isHorizonMove = true;

        else if (vDown || hUP)
            isHorizonMove = false;

        if (vDown && v == 1)
            dirVec = Vector3.up;
        else if (vDown && v == -1)
            dirVec = Vector3.down;
        else if (hDown && h == 1)
            dirVec = Vector3.right;
        else if (hDown && h == -1)
            dirVec = Vector3.left;

        if (isHorizonMove == true)
            anime.SetBool("side", true);
        else
            anime.SetBool("side", false);
    }

    void HandleAnimator()
    {
        if (anime.GetInteger("hAxisRaw") != h)
        {
            anime.SetBool("isChange", true);
            anime.SetInteger("hAxisRaw", (int)h);
        }
        else if (anime.GetInteger("vAxisRaw") != v)
        {
            anime.SetBool("isChange", true);
            anime.SetInteger("vAxisRaw", (int)v);
        }
        else
            anime.SetBool("isChange", false);
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (scanObj != null && GameManager.Progress_Id == 0)
            {

                ObjData objData = scanObj.GetComponent<ObjData>();
                manager.Action(objData.Id, objData);
            }
            else
            {
                manager.Action(GameManager.Progress_Id, GameManager.Progress_ObjData);
            }

            
        }
    }

    void HandlePlayerMove()
    {
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * Speed;
    }
    void HandleScanObject()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

        if (rayHit.collider != null)
            scanObj = rayHit.collider.gameObject;
        else
            scanObj = null;
    }

    void HandlePlayerFlip()
    {
        if(h != 0)
            spriter.flipX = h < 0;
    }


}
