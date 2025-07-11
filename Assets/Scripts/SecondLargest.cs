using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondLargest : MonoBehaviour
{
    int[] array = new int[10];
     Stack<int> stack = new Stack<int>();   
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < array.Length; i++) {
            array[i] = Random.Range(0, 100);
        }
        for (int i = 0; i < array.Length; i++) {
            for (int j = 0; j < array.Length; j++)
            {
                if (array[i] < array[j]) { 
                     
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
