using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mousSensitivity = 1f;
    public float moveSpeed = 3f,runSpeed = 10f,jumpForce = 12f,gr = 2.5f;
    public Transform groundPonit;
    public LayerMask groundLa;
    public GameObject bulletImpact;
    public float bllTime = 0.1f;
    
    private float verticalRotStore;
    private Vector2 mouseInput;
    private bool isGrounded;
    private float shootCD;

    public float MaxHt = 10f,HP=1f,CD=4f,OHC=5f;
    private float hC;
    private bool OH;
   
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
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"))*mousSensitivity;
     
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
             
             //鎖定滑鼠上下視移動最小值最大值
        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -30f, 30f);
     
        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y,viewPoint.rotation.eulerAngles.z);

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
        
        if (Input.GetButtonDown("Jump") /*&& isGrounded*/)
        {
            Debug.Log("jump");
            movement.y = jumpForce;
        }
        
        isGrounded = Physics.Raycast(groundPonit.position, Vector3.down, 1f,groundLa);
        
        //若碰到地板則停止運算
        if (Char.isGrounded)
        {
            movement.y = 0f;
        }
        //運算其值
        movement.y += Physics.gravity.y*Time.deltaTime*gr;
        
        Char.Move(movement * moveSpeed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                
            }
            
        }

        if (!OH)
        {
           if ((Input.GetMouseButtonDown(0)))
           {
                      Shoot();
           }
          
           if (Input.GetMouseButton(0))
           {
                      shootCD -= Time.deltaTime;
                      
                      if (shootCD<=0f)
                      {
                          Shoot();                
                      }
                      
           }
          
           hC -= CD * Time.deltaTime;
        }else
        {
            hC -= OHC * Time.deltaTime;
            if (hC<=0)
            {
                OH = false;
            }
        }

        if (hC<0)
        {
            hC = 0f;

        }
        
    }

    private void LateUpdate()
    {
        //設置Camera即使沒有群組也會跟隨腳色
        Cam.transform.position = viewPoint.position;
        Cam.transform.rotation = viewPoint.rotation;
    }

    private void Shoot()
    {
        Ray ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = Cam.transform.position;

        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            Debug.Log("We hit" + hit.collider.gameObject.name);

           GameObject blletImpeobject= Instantiate(bulletImpact, hit.point+(hit.normal*0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
        Destroy(blletImpeobject,3f);
           
        }

        shootCD = bllTime;

        hC += HP;
        if (HP>=MaxHt)
        {
            hC = MaxHt;
            OH = true;
        }
    }
}