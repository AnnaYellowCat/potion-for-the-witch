using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private Vector3 pos;
    private float cameraMovingSpeed = 5f;

    private void Awake()
    {
        if (!player)
        {
            player = FindFirstObjectByType<Hero>().transform;
        }
    }

    private void Update()
    {
        Vector3 newCameraPosition = new Vector3(player.position.x, player.position.y, -10f);
        transform.position = Vector3.Lerp(transform.position, newCameraPosition, cameraMovingSpeed * Time.deltaTime);
    }
}