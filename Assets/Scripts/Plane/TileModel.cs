using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileModel : MonoBehaviour
{
    public Vector2 cellsPerTexture = Vector2.one;

    public SpriteRenderer sprite;

    public void SetOffset(Vector2Int position) {
        sprite.material.mainTextureOffset += position / cellsPerTexture;
        sprite.material.mainTextureScale = Vector2.one / cellsPerTexture;
    }
}
