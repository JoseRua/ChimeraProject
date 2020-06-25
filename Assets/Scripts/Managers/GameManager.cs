using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { FRIENDLY, ENEMY}
public enum Actions { ATTACK, MOVE, MOVE_TO_ENEMY, RETREAT }

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public GridManager _grid;
    public UIManager _uiManager;

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    private void Start() {
        InitGame();
    }

    private void InitGame() {
        _grid.InitGame(_uiManager); 
        _uiManager.NextTurn(true);
    }
    
    public void Log(string s) {
        _uiManager.Log(s);
    }  
}
