using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public GridGenerator _gridGenerator;
    public int _gridRows = 10, _gridColumns = 10;
    public int _enemyNumber = 0, _friendlyNumber = 0;

    // This should be stacks
    private Dictionary<string, GameObject> _enemyUnits = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _playerUnits = new Dictionary<string, GameObject>();

    private int _turn;
    private int _friendlyCounter = 0;
    private int _enemyCounter = 0;
    private int _totalPlayerUnits = 0;
    private int _totalEnemyUnits = 0;
    private bool _playerTurn = false;
    private bool _isPlayerActive = false;

    private UIManager _uiManager;

    public Dictionary<string, GameObject> PlayerUnits { get => _playerUnits; set => _playerUnits = value; }

    private void CreateUnits() {
        Object enemyPrefab = Resources.Load("prefabs/prefab_enemy");
        Object friendlyPrefab = Resources.Load("prefabs/prefab_friendly");
        if (_enemyNumber < 4 || _enemyNumber > 8) _enemyNumber = Random.Range(4, 8);
        if (_friendlyNumber < 1 || _friendlyNumber > 4) _friendlyNumber = Random.Range(1, 4);

        // Create enemies
        int enemyPositionOffsetZ = (int)Mathf.Floor(_gridColumns / _enemyNumber);
        int friendlyPositionOffsetZ = (int)Mathf.Floor(_gridColumns / _friendlyNumber);
        for (int i = 0; i < _enemyNumber; i++) {
            Vector3 pos = new Vector3(_gridGenerator.RightTopPos.x, _gridGenerator.RightTopPos.y, _gridGenerator.RightTopPos.z - i * _gridGenerator._scale * enemyPositionOffsetZ);
            GameObject enemy = (GameObject)Instantiate(enemyPrefab, pos, Quaternion.Euler(new Vector3(90, 0, 0)));
            enemy.GetComponent<EnemyScript>().GridManager = this;
            enemy.GetComponent<EnemyScript>().GridPos = new Vector2Int(_gridColumns - 1, (_gridRows - 1) - i * enemyPositionOffsetZ);
            string name = "enemy_" + i;
            enemy.GetComponent<Unit>().Name = name;
            _enemyUnits.Add(name, enemy);


        }
        GameManager._instance.Log("Created " + _enemyNumber + " enemies.");

        // Create friendlies
        for (int i = 0; i < _friendlyNumber; i++) {
            Vector3 pos = new Vector3(_gridGenerator.LeftTopPos.x, _gridGenerator.RightTopPos.y, _gridGenerator.LeftTopPos.z - i * _gridGenerator._scale * friendlyPositionOffsetZ);
            GameObject ally = (GameObject)Instantiate(friendlyPrefab, pos, Quaternion.Euler(new Vector3(90, 0, 0)));
            ally.GetComponent<FriendlyScript>().GridManager = this;
            ally.GetComponent<FriendlyScript>().GridPos = new Vector2Int(0, (_gridRows - 1) - i * friendlyPositionOffsetZ);
            string name = "ally_" + i;
            ally.GetComponent<Unit>().Name = name;
            _playerUnits.Add(name, ally);
        }
        GameManager._instance.Log("Created " + _friendlyNumber + " friendlies.");
    }


    public void InitGame(UIManager ui) {
        _uiManager = ui;
        _gridGenerator.GenerateGrid(_gridRows, _gridColumns);
        CreateUnits();
        _playerUnits["ally_0"].GetComponent<FriendlyScript>().IsSelected = true;
        _playerTurn = true;
        _totalPlayerUnits = _playerUnits.Count;
        _totalEnemyUnits = _enemyUnits.Count;
    }

    public void UnitDestroyed(bool isPlayerUnit) {
        if (isPlayerUnit) {
            _totalPlayerUnits--;
        } else {
            _totalEnemyUnits--;
        }
    }

    public List<GameObject> FindPath(Vector2Int startPos, Vector2Int endPos) {
        return _gridGenerator.FindPath(this, startPos, endPos);
    }

    private void NextFriendly() {
        if (++_friendlyCounter >= _playerUnits.Count) {
            _friendlyCounter = 0;
        }
        // search for next unit which is not dead
        while (_playerUnits[("ally_" + (Mathf.Min(_friendlyCounter, _playerUnits.Count-1)))].GetComponent<FriendlyScript>().Hp <= 0) {
            if (_friendlyCounter < _playerUnits.Count)
                _friendlyCounter++;
            else
                _friendlyCounter = 0;
        }

        _playerUnits[("ally_" + _friendlyCounter)].GetComponent<FriendlyScript>().IsSelected = true;
    }

    private void NextEnemy() {
        if (++_enemyCounter >= _enemyUnits.Count) {
            _enemyCounter = 0;
        }
        // search for next unit which is not dead
        while (_enemyUnits[("enemy_" + Mathf.Min(_enemyCounter, _enemyUnits.Count-1))].GetComponent<EnemyScript>().Hp <= 0) {
            if (_enemyCounter < _enemyUnits.Count)
                _enemyCounter++;
            else
                _enemyCounter = 0;
        }

        _enemyUnits[("enemy_" + _enemyCounter)].GetComponent<EnemyScript>().CycleActions();
    }

    private void NewTurn() {
        if (_isPlayerActive) {
            _isPlayerActive = false;
            _playerTurn = !_playerTurn;
            if (_playerTurn) {
                NextFriendly();
            } else {
                NextEnemy();
            }
        }
    }

    public void EndedTurn() {
        if (_totalPlayerUnits > 0 && _totalEnemyUnits > 0) {
            _isPlayerActive = true;
            _uiManager.NextTurn(!_playerTurn);
            NewTurn();
        } else {
            _uiManager.EndGame(_totalEnemyUnits <= 0);
        }
    }

    public bool IsUnitAt(Vector2Int v, Vector2Int e2) {
        foreach (GameObject go in _enemyUnits.Values) {
            if(go.GetComponent<Unit>().GridPos != e2 && go.GetComponent<Unit>().GridPos == v && go.GetComponent<Unit>().Hp > 0) {
                return true;
            }
        }

        foreach (GameObject go in _playerUnits.Values) {
            if (go.GetComponent<Unit>().GridPos != e2 && go.GetComponent<Unit>().GridPos == v && go.GetComponent<Unit>().Hp > 0) {
                return true;
            }
        }

        return false;
    }


}
