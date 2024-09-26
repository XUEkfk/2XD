using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mousSensitivity = 1f;
    public float moveSpeed = 3f,runSpeed = 10f;
    private float verticalRotStore;
    private Vector2 mouseInput;
   
    //移動正規畫
    private Vector3 moveDir,movement;
    
    //腳色控制器
    public CharacterController Char;
    
    //設置CAMERA鏡頭
    private Camera Cam;
    
    //設置一個加速的開關
    private float Run;
    

    // Start is called before the first frame update
    void Start()
    {
        //滑鼠隱藏
        Cursor.lockState = CursorLockMode.Locked;
        //攝影機
        Cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        //腳色移動
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        //加速開關
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Run = runSpeed;
        }
        else
        {
            Run = moveSpeed;
        }

        //紀錄Y軸值
        float yVie = movement.y;
        
        //腳色移動
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized*Run;
        movement.y = yVie;

        if (Input.GetButtonDown("Jump"))
        {
            
        }
        
        //若碰到地板則停止運算
        if (Char.isGrounded)
        {
            movement.y = 0f;
        }
        //運算其值
        movement.y += Physics.gravity.y;
        
        Char.Move(movement * moveSpeed * Time.deltaTime);
        
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"))*mousSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,
            transform.rotation.eulerAngles.z);
        
        //鎖定滑鼠上下視移動最小值最大值
        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -30f, 30f);

        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y,
            viewPoint.rotation.eulerAngles.z);

    }

    private void LateUpdate()
    {
        //設置Camera即使沒有群組也會跟隨腳色
        Cam.transform.position = viewPoint.position;
        Cam.transform.rotation = viewPoint.rotation;
    }
}