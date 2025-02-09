﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class script_basicEventTriggerer : MonoBehaviour
{

    [SerializeField] UnityEvent triggerEnter;
    [SerializeField] UnityEvent triggerExit;
    [SerializeField] string tag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == tag)
            triggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == tag)
            triggerExit.Invoke();
    }

}
