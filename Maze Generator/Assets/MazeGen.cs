using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MazeGen : MonoBehaviour
{
    //TODO: State machine: am I in the initial position, generating maze, pathfinding?

    public GameObject cellPrefab;
    public List<Cell> stack;
    public List<Cell> grid;
    public Transform cellParent;
    public List<Cell> neighbours = new List<Cell>();
    public Cell currentCell;
    public Cell startingCell;
    public Cell targetCell;
    public GameObject start;
    public GameObject finish;
    public TextMeshProUGUI instructionText;
    public bool BeAMazin = false;

    private float coolDown = .01f;

    private int gridRows = 30;
    private int gridCols = 30;
    private float timeStamp;

    void Start()
    {
        stack = new List<Cell>();
        SpawnGrid();

        startingCell = grid[0];
        targetCell = grid[gridCols * gridRows - 1];
        currentCell = startingCell;
        currentCell.IsVisited(true);
        currentCell.IsCurrent(true);

        timeStamp = Time.time + coolDown;
    }

    void SpawnGrid()
    {
        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                Cell obj = Instantiate(cellPrefab, new Vector3(j, i, 1f), Quaternion.identity, cellParent).GetComponent<Cell>();
                obj.x = j;
                obj.y = i;
                grid.Add(obj);
            }
        }
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
        if (i < 0 || j < 0 || i >= gridCols || j >= gridRows)
        {
            //Cell to check is out of bounds
            return currentCell;
        } else
        {
            /* Translating the 1D array to 2D array (of the Cartesian coordinates) 
             * f.e., in a grid of 4 by 3, numbering the cells:
             * 0 , 1 , 2 , 3
             * 4 , 5 , 6 , 7
             * 8 , 9 , 10, 12
             * the first cell of the second row, is the first cell of the first row + 4;
             * the first cell of the third row, is the first cell of the first row + 8;
             * 
             * abstracting, we can deduce that (when the array is ordered across rows): 
             * index = x + ( y * cols )
             */

             return grid[i + j * gridCols];
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeAMazin = true;
            instructionText.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            while (targetCell.parent)
            {
                targetCell.IsCurrent(true);
                targetCell = targetCell.parent;            
            }
        }

        if (BeAMazin)
        {
            GenerateMaze();
        }
    }

    private void GenerateMaze()
    {
        if (timeStamp < Time.time)
        {
            CheckNeighbours();
            currentCell.IsCurrent(false);

            if (neighbours.Count != 0)
            {
                stack.Add(currentCell);
                Cell newCell = neighbours[Random.Range(0, neighbours.Count)];
                RemoveWalls(currentCell, newCell);
                newCell.parent = currentCell;
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

        if (neighbours.Count == 0 && stack.Count == 0)
        {
            //TODO: refactor into SpawnStartAndFinish()
            BeAMazin = false;
            start.SetActive(true);
            finish.SetActive(true);
            //grid[0]?
            GetNeighbour(0, 0).leftWall.enabled = false;
            //grid[cols * rows - 1]?
            GetNeighbour(gridCols - 1, gridRows - 1).rightWall.enabled = false;
        }
    }

    private void RemoveWalls(Cell current, Cell next)
    {
        if (next.x > current.x)
        {
            //The next cell we're travelling to is to the right of the current
            current.rightWall.enabled = false;
            next.leftWall.enabled = false;
        } else if (next.x < current.x)
        {
            //Left
            current.leftWall.enabled = false;
            next.rightWall.enabled = false;
        } else if (next.y > current.y)
        {
            //Top
            current.topWall.enabled = false;
            next.bottomWall.enabled = false;
        } else if (next.y < current.y)
        {
            //Bottom
            current.bottomWall.enabled = false;
            next.topWall.enabled = false;
        }
    }
}
