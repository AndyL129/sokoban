using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private GridObject gridObject;

    private void Start()
    {
        gridObject = GetComponent<GridObject>();
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

}
