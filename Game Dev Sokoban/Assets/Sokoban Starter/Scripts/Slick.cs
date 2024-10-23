using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slick : MonoBehaviour
{
    private GridObject gridObject;

    private void Start()
    {
        gridObject = GetComponent<GridObject>();
    }

    public void Move(Vector2Int movement)
    {
        Vector2Int targetPosition = gridObject.gridPosition + movement;

        if (IsPositionOccupied(targetPosition) || !IsWithinGridBoundaries(targetPosition))
        {
            return;
        }

        if (FindSlickAtPosition(targetPosition) != null)
        {
            Slick nextSlick = FindSlickAtPosition(targetPosition);
            if (nextSlick != null)
            {
                nextSlick.Move(movement);
            }
            else
            {
                return;
            }
        }

        gridObject.gridPosition = targetPosition;
    }

    private bool IsPositionOccupied(Vector2Int position)
    {
        Slick[] slickBlocks = FindObjectsOfType<Slick>();
        Wall[] wallBlocks = FindObjectsOfType<Wall>();

        foreach (Slick slick in slickBlocks)
        {
            if (slick.GetComponent<GridObject>().gridPosition == position)
            {
                return true;
            }
        }

        foreach (Wall wall in wallBlocks)
        {
            if (wall.GetComponent<GridObject>().gridPosition == position)
            {
                return true;
            }
        }

        return false;
    }

    private Slick FindSlickAtPosition(Vector2Int position)
    {
        Slick[] slickBlocks = FindObjectsOfType<Slick>();
        foreach (Slick slick in slickBlocks)
        {
            if (slick.GetComponent<GridObject>().gridPosition == position)
            {
                return slick;
            }
        }
        return null;
    }

    public bool IsAtBoundary()
    {
        Vector2Int position = gridObject.gridPosition;
        return position.x == 1 || position.x == 10 || position.y == 1 || position.y == 5;
    }

    private bool IsWithinGridBoundaries(Vector2Int position)
    {
        return position.x >= 1 && position.x <= 10 && position.y >= 1 && position.y <= 5;
    }
}
