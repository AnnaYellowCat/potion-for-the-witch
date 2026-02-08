using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand17 : MonoBehaviour
{
    public static Rand17 Instance {get; set; }

    public Sprite[] Sprite_Pic;
    public int number = -1;

    void Start()
    {
        Instance = this;
    }
    
    private void Update()
    {
        if(number != -1){
            GetComponent<SpriteRenderer>().sprite = Sprite_Pic[number];
        }
    }
}
