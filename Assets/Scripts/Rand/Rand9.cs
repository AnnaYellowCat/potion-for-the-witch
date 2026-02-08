using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand9 : MonoBehaviour
{
    public static Rand9 Instance {get; set; }

    public int rand;
    public Sprite[] Sprite_Pic;

    int[] array = { 9, 26, 30 };

    void Start()
    {
        Instance = this;
        Randoming();
    }

    public void Randoming()
    {
        rand = array[Random.Range(0, array.Length)];
        GetComponent<SpriteRenderer>().sprite = Sprite_Pic[rand];
    }
}
