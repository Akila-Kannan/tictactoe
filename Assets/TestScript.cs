using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Awake");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");

        int a = 10, b = 11;
        Debug.Log("before passing "+ a +" "+ b);
        fun(a,b);
        Debug.Log("after passing "+ a +" "+ b);

    }
    void fun(int a, int b) {
        a = a++;
        b = b++;    
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
