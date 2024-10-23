using UnityEngine;

public class Clingy : MonoBehaviour
{
    private GridObject gridObject;

    private void Start()
    {
        gridObject = GetComponent<GridObject>();
    }

    public bool CanBePulled(Vector2Int movement)
    {
        return movement.x != 0 || movement.y != 0;
    }

    public void Move(Vector2Int movement)
    {
        gridObject.gridPosition += movement;
    }
}
