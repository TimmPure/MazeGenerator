using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour
{

    public GameObject cellPrefab;
    public List<GameObject> stack;
    public List<GameObject> grid;
    public Transform cellParent;

    public List<Cell> neighbours = new List<Cell>(1);

    public Cell currentCell;
    private int height = 20;
    private int width = 20;

    void Start()
    {
        stack = new List<GameObject>();
        SpawnGrid();
        currentCell = grid[25].GetComponent<Cell>();
        currentCell.IsCurrent(true);
    }

    void CheckNeighbours()
    {
        neighbours.Clear();
        Debug.Log("Populating neighbours[] at x=" + currentCell.x + ", y=" + currentCell.y);
        Cell Top = grid[Index(currentCell.x, currentCell.y + 1)].GetComponent<Cell>();
        Cell Right = grid[Index(currentCell.x + 1, currentCell.y)].GetComponent<Cell>();
        Cell Bottom = grid[Index(currentCell.x, currentCell.y - 1)].GetComponent<Cell>();
        Cell Left = grid[Index(currentCell.x + -1, currentCell.y)].GetComponent<Cell>();

        if (Top != currentCell && !Top.visited)
        {
            neighbours.Add(Top);
        }
        if (Right != currentCell && !Right.visited)
        {
            neighbours.Add(Right);
        }
        if (Bottom != currentCell && !Bottom.visited)
        {
            neighbours.Add(Bottom);
        }
        if (Left != currentCell && !Left.visited)
        {
            neighbours.Add(Left);
        }
    }

    int Index(int i, int j)
    {
        if (i < 0 || j < 0 || i >= width || j >= height)
        {
            return currentCell.x + currentCell.y;
        } else
        {
            return i + j * width;
        }
    }

    void SpawnGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject obj = Instantiate(cellPrefab, new Vector3(j, i, 1f), Quaternion.identity, cellParent);
                obj.GetComponent<Cell>().x = j;
                obj.GetComponent<Cell>().y = i;
                grid.Add(obj);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckNeighbours();
            currentCell.IsCurrent(false);
            currentCell = neighbours[Random.Range(0, neighbours.Count)];
            currentCell.IsCurrent(true);
        }
    }

}
