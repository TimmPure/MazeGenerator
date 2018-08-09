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

	// Use this for initialization
	public void Start () {
        //meshRenderer = this.GetComponent<Renderer>();
	}
	
    public void IsVisited()
    {
        visited = true;
        meshRenderer.material = visitedMat;
    }

    public void IsCurrent()
    {
        current = true;
        meshRenderer.material = currentMat;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
