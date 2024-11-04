using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("玩家設定")]
    public Transform viewPoint;           // 攝影機視角位置
    public float mousSensitivity = 1f;    // 滑鼠靈敏度
    public float moveSpeed = 3f;          // 移動速度
    public float runSpeed = 10f;          // 跑步速度
    public float jumpForce = 12f;         // 跳躍力量
    public float gravityMultiplier = 2.5f;// 重力加成
    public Transform groundPoint;         // 地面檢測點
    public LayerMask groundLayer;         // 地面圖層
    public GameObject bulletImpact;       // 子彈擊中效果
    public float bulletFireRate = 0.1f;   // 子彈射擊頻率
    public CreateGun[] AllGuns;
    private int seleetedGun;

    [Header("過熱設定")]
    public float maxHeat = 10f;           // 最大過熱值
    public float heatPerShot = 1f;        // 每次射擊增加的過熱值
    public float coolDownRate = 4f;       // 冷卻速率
    public float overHeatCooldown = 5f;   // 過熱後的冷卻時間

    // 內部變量
    private float verticalRotationStore;  // 垂直旋轉的暫存值
    private Vector2 mouseInput;           // 滑鼠輸入
    private bool isGrounded;              // 是否在地面
    private float shootCooldown;          // 射擊冷卻時間
    private float currentHeat;            // 當前過熱值
    private bool isOverheated;            // 是否過熱

    public float MuzzleDisTime;
    private float muzzleCounter;

    // 移動相關
    private Vector3 moveDirection, movement;

    [Header("組件引用")]
    // 組件引用
    public CharacterController charController;  // 角色控制器
    private Camera cam;                         // 攝影機
    private float runMultiplier;                // 加速倍數

    // Start is called before the first frame update
    void Start()
    {
        // 鎖定滑鼠並隱藏
        Cursor.lockState = CursorLockMode.Locked;

        // 獲取攝影機
        UIController.intnce.weaponTempSlider.maxValue = maxHeat;
        cam = Camera.main;
        switchGun();
    }

    private void switchGun()
    {
        foreach (CreateGun gun in AllGuns)
        {
            gun.gameObject.SetActive(false);
            
        }
        AllGuns[seleetedGun].gameObject.SetActive(true);
        AllGuns[seleetedGun].muzzleFlash.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 處理滑鼠輸入
        HandleMouseInput();

        // 處理移動
        HandleMovement();

        // 處理射擊
        HandleShooting();
    }

    private void HandleMouseInput()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mousSensitivity;

        // 水平旋轉
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        // 垂直旋轉，限制範圍
        verticalRotationStore += mouseInput.y;
        verticalRotationStore = Mathf.Clamp(verticalRotationStore, -30f, 30f);
        viewPoint.rotation = Quaternion.Euler(-verticalRotationStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
    }

    private void HandleMovement()
    {
        // 讀取移動輸入
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        // 設置跑步加速
        runMultiplier = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        // 記錄Y軸速度
        float yVelocity = movement.y;
        // 根據角色朝向移動
        movement = (transform.forward * moveDirection.z + transform.right * moveDirection.x).normalized * runMultiplier;
        movement.y = yVelocity;
     
        // 如果角色在地面，將Y軸速度設為0
        if (charController.isGrounded)
        {
            movement.y = 0f;
        }
       
        // 檢測跳躍
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("JUMP");
            movement.y = jumpForce;
        }
        // 應用重力
        movement.y += Physics.gravity.y * Time.deltaTime * gravityMultiplier;
    
        // 檢測是否接觸地面
            isGrounded = Physics.Raycast(groundPoint.position, Vector3.down, 1f, groundLayer);    


        
        // 移動角色
        charController.Move(movement * moveSpeed * Time.deltaTime);

        // 切換滑鼠鎖定狀態
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (AllGuns[seleetedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;
            if (muzzleCounter<=0)
            {
                AllGuns[seleetedGun].muzzleFlash.SetActive(false);
            }

        }
    }

    private void HandleShooting()
    {
        if (isOverheated) // 如果過熱
        {
            currentHeat -= overHeatCooldown * Time.deltaTime;
            if (currentHeat <= 0)
            {
                isOverheated = false;
                UIController.intnce.OverHrMess.gameObject.SetActive(false);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0)) // 開始射擊
            {
                Shoot();
            }

            if (Input.GetMouseButton(0)) // 持續射擊
            {
                shootCooldown -= Time.deltaTime;
                if (shootCooldown <= 0f)
                {
                    Shoot();
                }
            }

            // 冷卻過程
            currentHeat -= coolDownRate * Time.deltaTime;
        }

        // 過熱值不能低於0
        if (currentHeat < 0)
        {
            currentHeat = 0f;
        }
        UIController.intnce.weaponTempSlider.value = currentHeat;

        if (Input.GetAxisRaw("Mouse ScrollWheel")>0f)
        {
            seleetedGun++;
            if (seleetedGun>=AllGuns.Length)
            {
                seleetedGun = 0;
            }
            switchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel")<0f)
        {
            seleetedGun--;
            if (seleetedGun<0)
            {
                seleetedGun = AllGuns.Length - 1;
            }
            switchGun();
        }
        
    }

    private void LateUpdate()
    {
        // 攝影機跟隨角色
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    private void Shoot()
    {
        // 槍口射線
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            // 生成擊中效果
            GameObject impact = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(impact, 3f);
        }

        shootCooldown = AllGuns[seleetedGun].timeBrtweenShots; // 重置射擊冷卻時間

        // 增加過熱值
        currentHeat += AllGuns[seleetedGun].heatPerShot;
        if (currentHeat >= maxHeat) // 如果達到過熱狀態
        {
            currentHeat = maxHeat;
            isOverheated = true;
            
            UIController.intnce.OverHrMess.gameObject.SetActive(true);
            
        }

        AllGuns[seleetedGun].muzzleFlash.SetActive(true);
        muzzleCounter = MuzzleDisTime;
    }
}

