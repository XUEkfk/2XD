using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mousSensitivity = 1f;
    public float moveSpeed = 5f;
    private float verticalRotStore;
    private Vector2 mouseInput;
    private Vector3 moveDir;
    

    // Start is called before the first frame update
    void Start()
    {
        //滑鼠隱藏
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        //腳色移動
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"))*mousSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,
            transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -30f, 30f);

        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y,
            viewPoint.rotation.eulerAngles.z);

    }
    
    
    
    
}