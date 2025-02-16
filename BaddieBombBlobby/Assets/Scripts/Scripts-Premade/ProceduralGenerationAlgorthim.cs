using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public enum DungeonDirection
{
    Any,
    North,
    South,
    East,
    West
}

public static class ProceduralGenerationAlgorthim 
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>(); //make sure that we do not have duplicates

        path.Add(startPosition); //when walking path, track start
        var previousPosition = startPosition; //set previous

        for (int i = 0; i < walkLength; i++) //iterate through a random vaild point
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection(); //get random direction
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }

    /*
     public static List<Vector2Int> BrennanRandomWalkCorridor(Vector2Int startPosition, int corridorLength, DungeonDirection direction)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();

        // Set initial direction
        Vector2Int mainDirection = direction == DungeonDirection.Any
            ? Direction2D.GetRandomCardinalDirection()
            : Direction2D.GetDirectionVector(direction);

        Vector2Int currentPosition = startPosition;
        corridor.Add(currentPosition);
        visitedPositions.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            // 60% chance to follow main direction
            // 40% chance to create a branch
            if (UnityEngine.Random.value > 0.6f)
            {
                // Create a branch
                Vector2Int branchDirection;
                if (direction == DungeonDirection.North || direction == DungeonDirection.South)
                {
                    // Branch east or west when going north/south
                    branchDirection = UnityEngine.Random.value > 0.5f
                        ? new Vector2Int(1, 0)
                        : new Vector2Int(-1, 0);
                }
                else if (direction == DungeonDirection.East || direction == DungeonDirection.West)
                {
                    // Branch north or south when going east/west
                    branchDirection = UnityEngine.Random.value > 0.5f
                        ? new Vector2Int(0, 1)
                        : new Vector2Int(0, -1);
                }
                else
                {
                    // Random cardinal direction for Any direction
                    branchDirection = Direction2D.GetRandomCardinalDirection();
                }

                // Create a short branch (2-4 tiles)
                int branchLength = UnityEngine.Random.Range(2, 5);
                Vector2Int branchPosition = currentPosition;

                for (int j = 0; j < branchLength; j++)
                {
                    branchPosition += branchDirection;
                    if (!visitedPositions.Contains(branchPosition))
                    {
                        corridor.Add(branchPosition);
                        visitedPositions.Add(branchPosition);
                    }
                }

                // Continue from end of branch if going in the main direction
                if (UnityEngine.Random.value > 0.5f)
                {
                    currentPosition = branchPosition;
                }
            }

            // Continue in main direction
            currentPosition += mainDirection;
            if (!visitedPositions.Contains(currentPosition))
            {
                corridor.Add(currentPosition);
                visitedPositions.Add(currentPosition);
            }
        }

        return corridor;
    }
     */

    public static List<Vector2Int> BrennanRandomWalkCorridor(Vector2Int startPosition, int corridorLength, DungeonDirection direction, Vector2Int generationStartPosition, int brushSize)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();
        Vector2Int mainDirection = direction == DungeonDirection.Any
            ? Direction2D.GetRandomCardinalDirection()
            : Direction2D.GetDirectionVector(direction);

        Vector2Int currentPosition = startPosition;
        corridor.Add(currentPosition);
        visitedPositions.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            //Branching Logic (unchanged)
            if (UnityEngine.Random.value > 0.6f)
            {
                // Create a branch
                Vector2Int branchDirection;
                if (direction == DungeonDirection.North || direction == DungeonDirection.South)
                {
                    // Branch east or west when going north/south
                    branchDirection = UnityEngine.Random.value > 0.5f
                        ? new Vector2Int(1, 0)
                        : new Vector2Int(-1, 0);
                }
                else if (direction == DungeonDirection.East || direction == DungeonDirection.West)
                {
                    // Branch north or south when going east/west
                    branchDirection = UnityEngine.Random.value > 0.5f
                        ? new Vector2Int(0, 1)
                        : new Vector2Int(0, -1);
                }
                else
                {
                    // Random cardinal direction for Any direction
                    branchDirection = Direction2D.GetRandomCardinalDirection();
                }

                // Create a short branch (2-4 tiles)
                int branchLength = UnityEngine.Random.Range(2, 5);
                Vector2Int branchPosition = currentPosition;

                for (int j = 0; j < branchLength; j++)
                {
                    branchPosition += branchDirection;
                    if (!visitedPositions.Contains(branchPosition))
                    {
                        corridor.Add(branchPosition);
                        visitedPositions.Add(branchPosition);
                    }
                }

                // Continue from end of branch if going in the main direction
                if (UnityEngine.Random.value > 0.5f)
                {
                    currentPosition = branchPosition;
                }
            }

            // --- NEW BOUNDS CHECK ---
            Vector2Int nextPosition = currentPosition + mainDirection;

            // Check if the *entire brush area* around the next position is within bounds
            if (IsBrushAreaWithinBounds(nextPosition, direction, generationStartPosition, brushSize))
            {
                currentPosition = nextPosition;
                if (!visitedPositions.Contains(currentPosition))
                {
                    corridor.Add(currentPosition);
                    visitedPositions.Add(currentPosition);
                }
            }
            else
            {
                //Option 2 (More complex): Find a new direction
                mainDirection = FindNewDirection(currentPosition, direction, generationStartPosition);
                if (mainDirection == Vector2Int.zero)
                {
                    break; //No valid direction found - stop generating
                }
                // If a new direction is found, the loop continues and the next tile will be generated
                // in the new direction.  We do *not* increment 'i' here, so we don't lose steps.
                continue; //Try again with new direction
            }

        }
        return corridor;
    }

    // --- NEW FUNCTION ---
    private static bool IsBrushAreaWithinBounds(Vector2Int position, DungeonDirection direction, Vector2Int generationStartPosition, int brushSize)
    {
        int halfBrush = brushSize / 2;

        for (int x = -halfBrush; x <= halfBrush; x++)
        {
            for (int y = -halfBrush; y <= halfBrush; y++)
            {
                Vector2Int tilePosition = position + new Vector2Int(x, y);
                if (!IsWithinBounds(tilePosition, direction, generationStartPosition))
                {
                    return false; // At least one tile is out of bounds
                }
            }
        }
        return true; // All tiles within the brush area are within bounds
    }

    private static bool IsWithinBounds(Vector2Int position, DungeonDirection direction, Vector2Int generationStartPosition)
    {
        int threshold = 5; // Adjust this threshold as needed.  How far past the start is "significant"?

        switch (direction)
        {
            case DungeonDirection.North:
                return position.y >= generationStartPosition.y - threshold;
            case DungeonDirection.South:
                return position.y <= generationStartPosition.y + threshold;
            case DungeonDirection.East:
                return position.x >= generationStartPosition.x - threshold;
            case DungeonDirection.West:
                return position.x <= generationStartPosition.x + threshold;
            case DungeonDirection.Any: //No bounds check for Any
            default:
                return true;
        }
    }

    //Option 2 helper function (Find a new direction if out of bounds)
    private static Vector2Int FindNewDirection(Vector2Int position, DungeonDirection direction, Vector2Int generationStartPosition)
    {
        List<Vector2Int> possibleDirections = new List<Vector2Int>(Direction2D.cardinalDirectionsList);

        //Remove the *opposite* direction, to avoid immediately reversing.
        if (direction != DungeonDirection.Any) //Can't reverse "Any"
        {
            Vector2Int oppositeDirection = -Direction2D.GetDirectionVector(direction);
            possibleDirections.Remove(oppositeDirection);
        }


        //Try each possible direction
        foreach (Vector2Int dir in possibleDirections)
        {
            Vector2Int nextPos = position + dir;
            if (IsWithinBounds(nextPos, direction, generationStartPosition))
            {
                return dir; //Found a valid direction!
            }
        }
        return Vector2Int.zero; //No valid direction found
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (UnityEngine.Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, 0) //LEFT
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1,1), //UP-RIGHT
        new Vector2Int(1,-1), //RIGHT-DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 1) //LEFT-UP
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,1), //UP-RIGHT
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(1,-1), //RIGHT-DOWN
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0), //LEFT
        new Vector2Int(-1, 1) //LEFT-UP

    };

    public static Vector2Int GetRandomCardinalDirection() //returns a random direction for the path, can call it in the 2D direction class
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }

    public static Vector2Int GetDirectionVector(DungeonDirection direction)
    {
        switch (direction)
        {
            case DungeonDirection.North:
                return new Vector2Int(0, 1);
            case DungeonDirection.East:
                return new Vector2Int(1, 0);
            case DungeonDirection.South:
                return new Vector2Int(0, -1);
            case DungeonDirection.West:
                return new Vector2Int(-1, 0);
            default:
                throw new System.ArgumentException("Invalid DungeonDirection value");
        }
    }
}

    /*
    public static List<Vector2Int> BrennanRandomWalkCorridor(Vector2Int startPosition, int corridorLength, DungeonDirection direction)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();

        // Set initial direction
        Vector2Int mainDirection = direction == DungeonDirection.Any
            ? Direction2D.GetRandomCardinalDirection()
            : Direction2D.GetDirectionVector(direction);

        Vector2Int currentPosition = startPosition;
        corridor.Add(currentPosition);
        visitedPositions.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            // 60% chance to follow main direction
            // 40% chance to create a branch
            if (UnityEngine.Random.value > 0.6f)
            {
                // Create a branch
                Vector2Int branchDirection;
                if (direction == DungeonDirection.North || direction == DungeonDirection.South)
                {
                    // Branch east or west when going north/south
                    branchDirection = UnityEngine.Random.value > 0.5f
                        ? new Vector2Int(1, 0)
                        : new Vector2Int(-1, 0);
                }
                else if (direction == DungeonDirection.East || direction == DungeonDirection.West)
                {
                    // Branch north or south when going east/west
                    branchDirection = UnityEngine.Random.value > 0.5f
                        ? new Vector2Int(0, 1)
                        : new Vector2Int(0, -1);
                }
                else
                {
                    // Random cardinal direction for Any direction
                    branchDirection = Direction2D.GetRandomCardinalDirection();
                }

                // Create a short branch (2-4 tiles)
                int branchLength = UnityEngine.Random.Range(2, 5);
                Vector2Int branchPosition = currentPosition;

                for (int j = 0; j < branchLength; j++)
                {
                    branchPosition += branchDirection;
                    if (!visitedPositions.Contains(branchPosition))
                    {
                        corridor.Add(branchPosition);
                        visitedPositions.Add(branchPosition);
                    }
                }

                // Continue from end of branch if going in the main direction
                if (UnityEngine.Random.value > 0.5f)
                {
                    currentPosition = branchPosition;
                }
            }

            // Continue in main direction
            currentPosition += mainDirection;
            if (!visitedPositions.Contains(currentPosition))
            {
                corridor.Add(currentPosition);
                visitedPositions.Add(currentPosition);
            }
        }

        return corridor;
    }

    public static List<Vector2Int> BrennanRandomWalkCorridor(Vector2Int startPosition, int corridorLength, DungeonDirection direction)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        Vector2Int moveDirection;

        // Set initial direction
        if (direction == DungeonDirection.Any)
        {
            moveDirection = Direction2D.GetRandomCardinalDirection();
        }
        else
        {
            moveDirection = Direction2D.GetDirectionVector(direction);
        }

        Vector2Int currentPosition = startPosition;
        corridor.Add(currentPosition);

        // Generate the corridor
        for (int i = 0; i < corridorLength; i++)
        {
            // 80% chance to continue in same direction, 20% chance to slightly deviate
            if (UnityEngine.Random.value > 0.8f && direction == DungeonDirection.Any)
            {
                // Get a new random direction
                moveDirection = Direction2D.GetRandomCardinalDirection();
            }
            else if (UnityEngine.Random.value > 0.9f && direction != DungeonDirection.Any)
            {
                // Small deviation while maintaining general direction
                Vector2Int baseDirection = Direction2D.GetDirectionVector(direction);
                // Only allow slight sideways movement
                if (direction == DungeonDirection.North || direction == DungeonDirection.South)
                {
                    moveDirection = baseDirection + new Vector2Int(UnityEngine.Random.Range(-1, 2), 0);
                }
                else
                {
                    moveDirection = baseDirection + new Vector2Int(0, UnityEngine.Random.Range(-1, 2));
                }
            }

            currentPosition += moveDirection;
            corridor.Add(currentPosition);
        }

        return corridor;
    }
    */