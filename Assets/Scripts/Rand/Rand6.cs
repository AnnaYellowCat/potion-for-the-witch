using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand6 : MonoBehaviour
{
    private Rand15 randObject;
    public Sprite[] Sprite_Pic;
    public GameObject a;


    private void Awake()
    {
        randObject = a.GetComponent<Rand15>();
        Invoke("Randoming", 0.000001f);
    }

    public void Randoming()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite_Pic[randObject.rand];
    }
}
