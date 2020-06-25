using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyScript : Unit
{
    private bool _isSelected = false;
    private Vector2Int _positionClicked;

    public bool IsSelected { get => _isSelected; set => _isSelected = value; }



    void OnEnable() {
        InputManager.onInputMouseDown += OnInputMouseDown;
    }
    void OnDisable() {
        InputManager.onInputMouseDown -= OnInputMouseDown;
    }

    private void OnInputMouseDown(Actions action, Vector2Int position, GameObject go) {
        if (_isSelected) {
            _positionClicked = position;
            _actionsToDo.Clear();
            switch (action) {
                case Actions.MOVE:
                    _actionsToDo.Add(Actions.MOVE);
                    break;
                case Actions.ATTACK:
                    GameManager._instance.Log(_name+" is going to attack "+go.GetComponent<Unit>().Name);
                    _unitToAttack = go;
                    _actionsToDo.Add(Actions.MOVE_TO_ENEMY);
                    _actionsToDo.Add(Actions.ATTACK);
                    break;
            }
            DoAction(_actionsToDo[0]);
        }
    }

    protected override void DoAction(Actions action) {
        switch (action) {
            case Actions.MOVE:
                MoveToTile(_positionClicked);
                break;
            case Actions.MOVE_TO_ENEMY:
                MoveToEnemyTile(_positionClicked);
                break;
            case Actions.ATTACK:
                AttackUnit();
                break;
        }
    }
    protected override void Die() {
        base.Die();
        _gridManager.UnitDestroyed(true);
    }

    protected override void EndAction() {
        base.EndAction();
        _isSelected = false;
    }
}
