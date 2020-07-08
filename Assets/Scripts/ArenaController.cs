using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    public int numberOfArena;
    public List<GameObject> cubes;

    void Start()
    {
        foreach (MeshFilter child in GetComponentsInChildren<MeshFilter>())
        {
            if (child.transform != transform && !child.transform.CompareTag("Danger"))
            {
                cubes.Add(child.gameObject);
            }
        }
    }
    
    
}