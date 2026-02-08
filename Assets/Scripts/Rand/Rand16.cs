using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand16 : MonoBehaviour
{
    public static Rand16 Instance {get; set; }

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
