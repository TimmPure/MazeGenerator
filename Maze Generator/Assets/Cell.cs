using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    public int x;
    public int y;
    public bool visited = false;
    public bool current = false;
    public Material visitedMat;
    public Material currentMat;
    public Renderer meshRenderer;
	
    public void IsVisited()
    {
        visited = true;
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
        }
    }
}
