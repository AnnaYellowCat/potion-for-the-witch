using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    void OnMouseOver()
    {
       if(Input.GetMouseButtonDown(0)){
            Hero.Instance.isOver = true;
        }
    }
}
