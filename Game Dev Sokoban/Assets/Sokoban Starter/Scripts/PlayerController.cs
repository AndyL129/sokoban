using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridObject gridObject;

    private void Start()
    {
        gridObject = GetComponent<GridObject>();
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        Vector2Int movement = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) && gridObject.gridPosition.y != 1)
        {
            movement.y = -1;
        }
        else if (Input.GetKeyDown(KeyCode.S) && gridObject.gridPosition.y != 5)
        {
            movement.y = 1;
        }
        else if (Input.GetKeyDown(KeyCode.A) && gridObject.gridPosition.x != 1)
        {
            movement.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D) && gridObject.gridPosition.x != 10)
        {
            movement.x = 1;
        }

        if (movement != Vector2Int.zero)
        {
            gridObject.gridPosition += movement;
        }
    }
}
