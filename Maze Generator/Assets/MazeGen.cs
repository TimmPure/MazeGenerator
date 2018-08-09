using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour {

    public GameObject cellPrefab;
    public List<GameObject> stack;
    public List<GameObject> grid;
    public Transform cellParent;

    private int height = 20;
    private int width = 20;
    
	void Start () {
        stack = new List<GameObject>();
        SpawnGrid();
        grid[0].GetComponent<Cell>().IsVisited();
	}

	void Update () {
		
	}

    void SpawnGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject obj = Instantiate(cellPrefab, new Vector3(j, i, 1f), Quaternion.identity, cellParent);
                grid.Add(obj);
            }
        }
    }
}
