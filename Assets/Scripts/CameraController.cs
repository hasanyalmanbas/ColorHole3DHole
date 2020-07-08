using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    private void Start()
    {
        GameController.Instance.nextArenaEvent.AddListener(NextArena);
    }

    private void NextArena(Vector3 nextPosition)
    {
        StartCoroutine(GoToNextArena(nextPosition));
    }

    IEnumerator GoToNextArena(Vector3 nextPosition)
    {
        Vector3 position = new Vector3();
        position.x = 0;
        position.y = transform.position.y;
        position.z = nextPosition.z - 30;
        
        while (transform.position != position)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, 0.5f);
            yield return null;
        }
    }
}