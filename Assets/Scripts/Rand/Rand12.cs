using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand12 : MonoBehaviour
{
    private Rand13 randObject;
    public Sprite[] Sprite_Pic;
    public GameObject a;


    private void Awake()
    {
        randObject = a.GetComponent<Rand13>();
        Invoke("Randoming", 0.000001f);
    }

    public void Randoming()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite_Pic[randObject.rand];
    }
}
