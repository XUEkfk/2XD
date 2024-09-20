using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OVO_NO : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }
}
