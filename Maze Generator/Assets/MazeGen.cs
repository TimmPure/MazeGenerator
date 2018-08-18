using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour
{

    public GameObject cellPrefab;
    public List<Cell> stack;
    public List<Cell> grid;
    public Transform cellParent;
    public List<Cell> neighbours = new List<Cell>();
    public Cell currentCell;
    public bool BeAMazin = false;
    private float coolDown = .02f;

    private int height = 30;
    private int width = 30;
    private float timeStamp;

    void Start()
    {
        stack = new List<Cell>();
        SpawnGrid();

        currentCell = grid[0];
        currentCell.IsVisited(true);
        currentCell.IsCurrent(true);

        timeStamp = Time.time + coolDown;
    }

    void CheckNeighbours()
    {
        neighbours.Clear();
        Cell Top = GetNeighbour(currentCell.x, currentCell.y + 1);
        Cell Right = GetNeighbour(currentCell.x + 1, currentCell.y);
        Cell Bottom = GetNeighbour(currentCell.x, currentCell.y - 1);
        Cell Left = GetNeighbour(currentCell.x - 1, currentCell.y);

        if (Top != null && Top != currentCell && !Top.visited)
        {
            neighbours.Add(Top);
        }
        if (Right != null && Right != currentCell && !Right.visited)
        {
            neighbours.Add(Right);
        }
        if (Bottom != null && Bottom != currentCell && !Bottom.visited)
        {
            neighbours.Add(Bottom);
        }
        if (Left != null && Left != currentCell && !Left.visited)
        {
            neighbours.Add(Left);
        }
    }

    Cell GetNeighbour(int i, int j)
    {
        if (i < 0 || j < 0 || i >= width || j >= height)
        {
            return currentCell;
        } else
        {
            return grid[i + j * width];
        }
    }

    void SpawnGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cell obj = Instantiate(cellPrefab, new Vector3(j, i, 1f), Quaternion.identity, cellParent).GetComponent<Cell>();
                obj.x = j;
                obj.y = i;
                grid.Add(obj);
            }
        }
    }

    private void Update()
    {
        if (!BeAMazin)
        {
            return;
        }
        if (timeStamp < Time.time)
        {
            CheckNeighbours();
            currentCell.IsCurrent(false);

            if (neighbours.Count != 0)
            {
                stack.Add(currentCell);
                Cell newCell = neighbours[Random.Range(0, neighbours.Count)];
                RemoveWalls(currentCell, newCell);
                currentCell = newCell;
                currentCell.IsCurrent(true);
            } else
            {
                currentCell = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                currentCell.IsCurrent(true);
            }
            timeStamp = Time.time + coolDown;
        }

        if(neighbours.Count == 0 && stack.Count == 0)
        {
            BeAMazin = false;
        }
    }

    private void RemoveWalls(Cell c, Cell n)
    {
        if (n.x > c.x)
        {
            c.rightWall.enabled = false;
            n.leftWall.enabled = false;
        } else if (n. x < c.x)
        {
            c.leftWall.enabled = false;
            n.rightWall.enabled = false;
        } else if (n.y > c.y)
        {
            c.topWall.enabled = false;
            n.bottomWall.enabled = false;
        } else if(n.y < c.y)
        {
            c.bottomWall.enabled = false;
            n.topWall.enabled = false;
        }
    }
}
