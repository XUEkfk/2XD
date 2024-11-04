using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class X01MG : MonoBehaviour
{
    public GameObject UIesc;
    private bool UiescisF = false;
    
    // Start is called before the first frame update
    void Start()
    {
        UIesc.SetActive(UiescisF);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUi();
        }
        
    }

    private void ToggleUi()
    {
        UiescisF = !UiescisF;
        UIesc.SetActive(UiescisF);

        if (UiescisF)
        {
            Time.timeScale = 0;     //stop the game
        }
        else
        {
            Time.timeScale = 1;     //start the game
        }
    }
}
