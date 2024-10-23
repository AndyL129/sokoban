using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridObject gridObject;
    private Vector2Int lastPosition; 
    private bool slickWasPushed;//if a slick has been pushed

    private void Start()
    {
        gridObject = GetComponent<GridObject>();
        lastPosition = gridObject.gridPosition; //Initialize last position
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        Vector2Int movement = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.S) && gridObject.gridPosition.y < 5)
        {
            movement.y = 1;
        }
        else if (Input.GetKeyDown(KeyCode.W) && gridObject.gridPosition.y > 1)
        {
            movement.y = -1;
        }
        else if (Input.GetKeyDown(KeyCode.A) && gridObject.gridPosition.x > 1)
        {
            movement.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D) && gridObject.gridPosition.x < 10)
        {
            movement.x = 1;
        }

        // Only process movement if there's a valid direction
        if (movement != Vector2Int.zero)
        {
            Vector2Int targetPosition = gridObject.gridPosition + movement;

            if (FindClingyAtPosition(targetPosition) != null)
            {
                return;
            }

            // Check if there's a slick block at the target position
            Slick firstSlickBlock = FindSlickAtPosition(targetPosition);
            if (firstSlickBlock != null)
            {
                // Prevent player from moving into slick
                if (firstSlickBlock.IsAtBoundary() && IsPushingTowardsBoundary(firstSlickBlock, movement))
                {
                    return;
                }

                // Attempt to push the slick block and any subsequent slicks
                PushSlicks(firstSlickBlock, movement);
            }
            else
            {
                PullClingyIfAdjacent(movement);

                PullStickyIfAdjacent(movement);

                if (!IsPositionOccupied(targetPosition))
                {
                    lastPosition = gridObject.gridPosition;
                    gridObject.gridPosition += movement;
                }
            }
        }
    }

    private void PullClingyIfAdjacent(Vector2Int movement)
    {
        Vector2Int playerPosition = gridObject.gridPosition;
        Vector2Int newPlayerPosition = playerPosition + movement;

        // Check the four adjacent positions
        Vector2Int[] adjacentPositions = {
            playerPosition + new Vector2Int(0, 1),
            playerPosition + new Vector2Int(0, -1),
            playerPosition + new Vector2Int(-1, 0),
            playerPosition + new Vector2Int(1, 0)
        };

        foreach (var position in adjacentPositions)
        {
            Clingy clingyBlock = FindClingyAtPosition(position);
            if (clingyBlock != null)
            {
                Vector2Int clingyPosition = clingyBlock.GetComponent<GridObject>().gridPosition;

                // Prevent pulling if moving towards corners
                if (IsMovingTowardsCorner(newPlayerPosition, clingyPosition))
                {
                    return;
                }

                // Move clingy block to the player's last position
                Vector2Int clingyTargetPosition = playerPosition;
                if (!IsPositionOccupied(clingyTargetPosition) && IsWithinGridBoundaries(clingyTargetPosition))
                {
                    clingyBlock.Move(clingyTargetPosition - clingyPosition); // Move clingy to follow the player
                }
                break;
            }
        }
    }

    private void PullStickyIfAdjacent(Vector2Int movement)
    {
        Vector2Int playerPosition = gridObject.gridPosition;

        // Check the four adjacent positions
        Vector2Int[] adjacentPositions = {
            playerPosition + new Vector2Int(0, 1),
            playerPosition + new Vector2Int(0, -1),
            playerPosition + new Vector2Int(-1, 0),
            playerPosition + new Vector2Int(1, 0)
        };

        foreach (var position in adjacentPositions)
        {
            Sticky stickyBlock = FindStickyAtPosition(position);
            if (stickyBlock != null)
            {
                // Move the sticky block in the same direction as the player if adjacent
                Vector2Int stickyTargetPosition = stickyBlock.GetComponent<GridObject>().gridPosition;
                if (movement.x != 0)
                {
                    stickyTargetPosition.x += movement.x;
                }
                else if (movement.y != 0)
                {
                    stickyTargetPosition.y += movement.y;
                }

                // Check if the target position for sticky is free before moving
                if (!IsPositionOccupied(stickyTargetPosition) && IsWithinGridBoundaries(stickyTargetPosition))
                {
                    stickyBlock.Move(stickyTargetPosition - gridObject.gridPosition);
                }
                break;
            }
        }
    }

    private void PushSlicks(Slick firstSlick, Vector2Int movement)
    {
        Vector2Int currentPosition = firstSlick.GetComponent<GridObject>().gridPosition;

        //if the current slick can be pushed
        if (IsPositionOccupied(currentPosition + movement) || !IsWithinGridBoundaries(currentPosition + movement))
        {
            slickWasPushed = false;
            return;
        }

        // Move the current slick
        firstSlick.Move(movement);
        Vector2Int newSlickPosition = currentPosition + movement;
        gridObject.gridPosition += movement;

        slickWasPushed = true;

        // Find next slick and attempt to push if there's a chain
        Slick nextSlick = firstSlick;
        while (true)
        {
            currentPosition = nextSlick.GetComponent<GridObject>().gridPosition;
            nextSlick = FindSlickAtPosition(currentPosition + movement);

            // if the next slick can be pushed
            if (nextSlick == null || IsPositionOccupied(currentPosition + movement) || !IsWithinGridBoundaries(currentPosition + movement))
                break;

            nextSlick.Move(movement);
        }

        // Move sticky blocks only after all slicks have been pushed
        if (slickWasPushed) 
        {
            MoveStickyIfAdjacent(newSlickPosition, movement);
        }

        slickWasPushed = false;
    }

    private void MoveStickyIfAdjacent(Vector2Int slickPosition, Vector2Int movement)
    {
        // Get all adjacent positions for the slick
        foreach (Vector2Int offset in GetAdjacentPositions(slickPosition))
        {
            Sticky stickyBlock = FindStickyAtPosition(offset);
            if (stickyBlock != null)
            {
                Vector2Int stickyTargetPosition = stickyBlock.GetComponent<GridObject>().gridPosition;

                stickyTargetPosition += movement;

                if (!IsPositionOccupied(stickyTargetPosition) && IsWithinGridBoundaries(stickyTargetPosition))
                {
                    stickyBlock.Move(stickyTargetPosition - stickyBlock.GetComponent<GridObject>().gridPosition);
                }
            }
        }
    }

    private Vector2Int[] GetAdjacentPositions(Vector2Int position)
    {
        return new Vector2Int[]
        {
        position + new Vector2Int(0, 1), 
        position + new Vector2Int(0, -1), 
        position + new Vector2Int(-1, 0), 
        position + new Vector2Int(1, 0) 
        };
    }

    private bool IsMovingTowardsCorner(Vector2Int newPlayerPosition, Vector2Int clingyPosition)
    {
        return newPlayerPosition == clingyPosition + new Vector2Int(-1, -1) ||
               newPlayerPosition == clingyPosition + new Vector2Int(-1, 1) ||
               newPlayerPosition == clingyPosition + new Vector2Int(1, -1) ||
               newPlayerPosition == clingyPosition + new Vector2Int(1, 1);
    }

    private Slick FindSlickAtPosition(Vector2Int position)
    {
        Slick[] slickBlocks = FindObjectsOfType<Slick>();
        return slickBlocks.FirstOrDefault(slick => slick.GetComponent<GridObject>().gridPosition == position);
    }

    private Clingy FindClingyAtPosition(Vector2Int position)
    {
        Clingy[] clingyBlocks = FindObjectsOfType<Clingy>();
        return clingyBlocks.FirstOrDefault(clingy => clingy.GetComponent<GridObject>().gridPosition == position);
    }

    private Sticky FindStickyAtPosition(Vector2Int position)
    {
        Sticky[] stickyBlocks = FindObjectsOfType<Sticky>();
        return stickyBlocks.FirstOrDefault(sticky => sticky.GetComponent<GridObject>().gridPosition == position);
    }

    private bool IsPositionOccupied(Vector2Int position)
    {
        // Check if the target position is occupied by any block
        return FindSlickAtPosition(position) != null ||
               FindClingyAtPosition(position) != null ||
               FindStickyAtPosition(position) != null ||
               FindObjectsOfType<Wall>().Any(wall => wall.GetComponent<GridObject>().gridPosition == position);
    }

    private bool IsWithinGridBoundaries(Vector2Int position)
    {
        return position.x >= 1 && position.x <= 10 && position.y >= 1 && position.y <= 5;
    }

    private bool IsPushingTowardsBoundary(Slick slick, Vector2Int movement)
    {
        return !IsWithinGridBoundaries(slick.GetComponent<GridObject>().gridPosition + movement);
    }
}
