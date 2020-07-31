using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MazeGen : MonoBehaviour
{
    //TODO: State machine: am I idle, generating maze, pathfinding?

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
    public bool BeRetracin = false;
    public bool BeFloodFillin = false;
    public CanvasAlpha canvasAlpha;

    private float coolDown = .01f;

    private int gridRows = 30;
    private int gridCols = 30;
    private float timeStamp;

    void Start()
    {
        stack = new List<Cell>();
        canvasAlpha = FindObjectOfType<CanvasAlpha>();
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
        //TODO: refactor coroutines to not be referenced by string
        //StartCoroutine returns a reference to the Coroutine; use that to cache it for StopCoroutine?

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeAMazin = true;
            StartCoroutine("GenerateMaze");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            BeFloodFillin = true;
            StartCoroutine("FloodFill");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            BeRetracin = true;
            StartCoroutine("RetracePathFromParents");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator GenerateMaze() {
        //TODO: move UI/canvas handling elsewhere
        canvasAlpha.flickering = true;
        instructionText.text = "Carving out the maze...";

        //TODO: initialize stack here, or clear if it already exists

        //TODO: check if maze has already been generated; if so, regenerate/randomize?


        while (BeAMazin) {
            CheckNeighbours();              //Populates a List<Cell>, neighbours, with unvisited cells orthogonally adjacent to currentCell
            currentCell.IsCurrent(false);   //Sets the cell material to Visited, and cell.visited = true
            
            //Exit condition
            if (neighbours.Count == 0 && stack.Count == 0) {
                canvasAlpha.flickering = false;
                instructionText.gameObject.SetActive(false);
                BeAMazin = false;
                start.SetActive(true);
                finish.SetActive(true);
                StopCoroutine("GenerateMaze");
                break;
            }

            if (neighbours.Count != 0) {
                stack.Add(currentCell);
                Cell newCell = neighbours[Random.Range(0, neighbours.Count)];
                RemoveWalls(currentCell, newCell);
                newCell.parent = currentCell;
                currentCell = newCell;
                currentCell.IsCurrent(true);
            } else {
                currentCell = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
                currentCell.IsCurrent(true);
            }

            yield return new WaitForSeconds(coolDown);
        }
    }

    IEnumerator RetracePathFromParents() {
        List<Cell> path = PopulatePath();

        while (BeRetracin) {
            if (path.Count == 0) {
                BeRetracin = false;
                StopCoroutine("RetracePathFromParent");
                break;
            }

            path[path.Count - 1].IsCurrent(true);
            path.RemoveAt(path.Count - 1);
            yield return new WaitForSeconds(coolDown);
        }
    }

    private List<Cell> PopulatePath() {
        List<Cell> path = new List<Cell>();

        while (targetCell.parent) {
            path.Add(targetCell);
            targetCell = targetCell.parent;
        }

        path.Add(targetCell);
        return path;
    }

    IEnumerator FloodFill()
    {
        List<Cell> cellsToFlood = new List<Cell>
        {
            startingCell
        };

        while(BeFloodFillin)
        {
            //TODO: exit condition: targetCell is flooded
            //TODO: set parent as the cell flooded from

            //Using a temporary list of cells, because we cannot modify the collection being iterated over
            //by the foreach loop inside of said loop.
            List<Cell> tempList = new List<Cell>();

            foreach (Cell c in cellsToFlood)
            {
                c.IsCurrent(true);
                c.flooded = true;
                if (c.topWall.enabled == false)
                {
                    Cell n = GetNeighbour(c.x, c.y + 1);
                    if (!n.flooded) { tempList.Add(n); }
                }
                if (c.rightWall.enabled == false)
                {
                    Cell n = GetNeighbour(c.x + 1, c.y);
                    if (!n.flooded) { tempList.Add(n); }
                }
                if (c.bottomWall.enabled == false)
                {
                    Cell n = GetNeighbour(c.x, c.y - 1);
                    if (!n.flooded) { tempList.Add(n); }
                }
                if (c.leftWall.enabled == false)
                {
                    Cell n = GetNeighbour(c.x - 1, c.y);
                    if (!n.flooded) { tempList.Add(n); }
                }
            }
            cellsToFlood = tempList;

            yield return new WaitForSeconds(0.05f);

            if (cellsToFlood.Count == 0)
            {
                Debug.Log("cellsToFlood is empty; setting FloodFill to false");
                BeFloodFillin = false;
                StopCoroutine("FloodFill");
            }
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
