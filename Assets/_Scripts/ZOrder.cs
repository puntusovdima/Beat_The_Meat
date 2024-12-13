using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZOrder : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (anchor)
        {
            spriteRenderer.sortingOrder = (int)(anchor.position.y * -10);
        }
        else
        {
            spriteRenderer.sortingOrder = (int)(transform.position.y * -10);
        }
        
    }
}
