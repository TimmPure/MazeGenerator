using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAlpha : MonoBehaviour
{
    private CanvasGroup cg;
    private float alphaSin = 1;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        cg.alpha = Mathf.Abs(Mathf.Sin(alphaSin));
        alphaSin += 2* Time.deltaTime;
    }
}
