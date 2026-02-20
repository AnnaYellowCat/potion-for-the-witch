using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private Vector3 pos;

    private void Awake()
    {
        if (!player)
        {
            player = FindFirstObjectByType<Hero>().transform;
        }
    }

    private void Update()
    {
        Vector3 temp = transform.position;
        pos = player.position;
        pos.z = -10f;
        temp.x = player.position.x;
        temp.y = player.position.y;
        transform.position = temp;
        //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
    }
}