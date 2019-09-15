using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAlpha : MonoBehaviour
{
    private CanvasGroup cg;
    private float alphaSin = 1;
    public bool flickering = false;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (flickering)
        {
            cg.alpha = Mathf.Abs(Mathf.Sin(alphaSin));
            alphaSin += 2 * Time.deltaTime;
        }
        else
        {
            cg.alpha = 1f;
        }
    }
}
