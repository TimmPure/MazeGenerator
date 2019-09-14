using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    public int x;
    public int y;
    public bool visited = false;
    public bool flooded = false;
    public bool current = false;
    public bool startCell = false;
    public bool targetCell = false;
    public Cell parent;

    public Material visitedMat;
    public Material currentMat;
    public Renderer meshRenderer;

    public Renderer topWall;
    public Renderer rightWall;
    public Renderer bottomWall;
    public Renderer leftWall;

    public void IsVisited(bool isVisited)
    {
        visited = isVisited;
        meshRenderer.material = visitedMat;
    }

    public void IsCurrent(bool isCurrent)
    {
        current = isCurrent;
        if (current)
        {
            meshRenderer.material = currentMat;
        } else if (!current)
        {
            meshRenderer.material = visitedMat;
            visited = true;
        }
    }
}
