using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int _visited = -1;
    public int _x = 0;
    public int _y = 0;
    public Vector2Int _gridPosition;
    public bool _wall = false;

    public SpriteRenderer _spriteRenderer;
    
    void OnMouseEnter() {
        _spriteRenderer.color = Color.blue;
    }

    void OnMouseExit() {
        _spriteRenderer.color = Color.white;
    }

    public void SetWall(bool isWall) {
        _wall = isWall;
        _spriteRenderer.color = Color.black;
    }
}
