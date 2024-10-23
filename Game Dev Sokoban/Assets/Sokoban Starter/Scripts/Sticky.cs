using System.Linq;
using UnityEngine;

public class Sticky : MonoBehaviour
{
    private GridObject gridObject;

    private void Start()
    {
        gridObject = GetComponent<GridObject>();
    }

    public void Move(Vector2Int movement)
    {
        Vector2Int newPosition = gridObject.gridPosition + movement;

        if (IsWithinGridBoundaries(newPosition) && !IsPositionOccupied(newPosition))
        {
            gridObject.gridPosition += movement;
        }
    }

    private bool IsWithinGridBoundaries(Vector2Int position)
    {
        return position.x >= 1 && position.x <= 10 && position.y >= 1 && position.y <= 5;
    }

    private bool IsPositionOccupied(Vector2Int position)
    {
        return FindObjectsOfType<Slick>().Any(slick => slick.GetComponent<GridObject>().gridPosition == position) ||
               FindObjectsOfType<Clingy>().Any(clingy => clingy.GetComponent<GridObject>().gridPosition == position) ||
               FindObjectsOfType<Wall>().Any(wall => wall.GetComponent<GridObject>().gridPosition == position) ||
               FindObjectsOfType<Sticky>().Any(sticky => sticky.GetComponent<GridObject>().gridPosition == position);
    }

    public bool IsAdjacent(Vector2Int position)
    {
        return gridObject.gridPosition == position + new Vector2Int(1, 0) ||
               gridObject.gridPosition == position + new Vector2Int(-1, 0) ||
               gridObject.gridPosition == position + new Vector2Int(0, 1) ||
               gridObject.gridPosition == position + new Vector2Int(0, -1);
    }
}
