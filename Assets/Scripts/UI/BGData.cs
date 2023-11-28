using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BGData : MonoBehaviour
{
    private Image renderer;
    public Image Renderer { get => renderer; }

    public float moveSpeed;
    public float rotateSpeed;

    private void Awake()
    {
        renderer = GetComponent<Image>();
    }
}

