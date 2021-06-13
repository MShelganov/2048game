using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private static SpriteManager spriteManager;
    public Sprite Empty;
    public List<Sprite> Sprites;
    public Sprite Default;
    public static SpriteManager Instance { get => spriteManager; }

    void Awake()
    {
        if (spriteManager == null)
            spriteManager = this;
    }
}
