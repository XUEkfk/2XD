using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController intnce;

    private void Awake()
    {
        if (intnce ==null)
        {
            intnce = this;
        }
    }

    public TMP_Text OverHrMess;
    public Slider weaponTempSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
