using System.Collections;
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

    private int height = 20;
    private int width = 20;
    private float timeStamp;

    void Start()
    {
        stack = new List<Cell>();
        SpawnGrid();

        currentCell = grid[0];
        currentCell.IsCurrent(true);
        currentCell.IsVisited(true);

        timeStamp = Time.time + coolDown;
    }

    void CheckNeighbours()
    {
        neighbours.Clear();
        Debug.Log("Populating neighbours[] at x=" + currentCell.x + ", y=" + currentCell.y);
        Cell Top = GetNeighbour(currentCell.x, currentCell.y + 1);
        Cell Right = GetNeighbour(currentCell.x + 1, currentCell.y);
        Cell Bottom = GetNeighbour(currentCell.x, currentCell.y - 1);
        Cell Left = GetNeighbour(currentCell.x - 1, currentCell.y);

        if (Top != null && Top != currentCell && !Top.visited)
        {
            neighbours.Add(Top);
            Debug.Log("Added Top");
        }
        if (Right != null && Right != currentCell && !Right.visited)
        {
            neighbours.Add(Right);
            Debug.Log("Added Right");
        }
        if (Bottom != null && Bottom != currentCell && !Bottom.visited)
        {
            neighbours.Add(Bottom);
            Debug.Log("Added Bottom");
        }
        if (Left != null && Left != currentCell && !Left.visited)
        {
            neighbours.Add(Left);
            Debug.Log("Added Left");
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
                currentCell = neighbours[Random.Range(0, neighbours.Count)];
                currentCell.IsCurrent(true);
            } else
            {
                currentCell = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                currentCell.IsCurrent(true);
            }
            timeStamp = Time.time + coolDown;
        }
    }
}
