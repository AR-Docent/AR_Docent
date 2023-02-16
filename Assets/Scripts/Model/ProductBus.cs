using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductBus
{
    public int id { get; set; }

    public string productName { get; set; }

    public string artist { get; set; }

    public string content { get; set; }

    public Texture2D image { get; set; }

    public AudioClip audio { get; set; }
}
