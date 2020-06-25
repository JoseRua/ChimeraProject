using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : Unit
{

    private int _hpOffset = 2;
    GameObject friendlyFound = null;

    public void CycleActions() {
        // search for closest player unit
        // if stronger (hp > hp +offset) don't move or flee
        // if weaker, attack

        int tempListCount = 0;
        friendlyFound = null;
        List<GameObject> tempList = null;
        _actionsToDo.Clear();
        foreach (GameObject go in _gridManager.PlayerUnits.Values) {
            if (go.GetComponent<Unit>().Hp > 0) {
                Vector2Int endPos = go.GetComponent<Unit>().GridPos;
                List<GameObject> list = _gridManager.FindPath(_gridPos, endPos);
                if (tempListCount == 0 || list.Count < tempListCount) {
                    tempListCount = list.Count;
                    tempList = list;
                    friendlyFound = go;
                }
            }
        }

        // Check closest unit to decide what to do
        if (friendlyFound != null) {
            GameManager._instance.Log(_name + " found nearest friendly: " + friendlyFound.GetComponent<FriendlyScript>().Name + " at " + friendlyFound.GetComponent<FriendlyScript>().GridPos);
            
            if(friendlyFound.GetComponent<Unit>().Hp <= _hp + _hpOffset) {
                // Attack!
                GameManager._instance.Log(_name + " decides to attack "+ friendlyFound.GetComponent<FriendlyScript>().Name+"!");
                _unitToAttack = friendlyFound;
                _actionsToDo.Add(Actions.MOVE_TO_ENEMY);
                _actionsToDo.Add(Actions.ATTACK);
            } else {
                // Run away! or don't move
                GameManager._instance.Log(_name + " decides to better stay put.");
            }
        }

        // Do actions
        DoAction(_actionsToDo[0]);
        

    }

    protected override void DoAction(Actions action) {
        switch (action) {
            case Actions.MOVE:
                MoveToTile(friendlyFound.GetComponent<FriendlyScript>().GridPos);
                break;
            case Actions.MOVE_TO_ENEMY:
                MoveToEnemyTile(friendlyFound.GetComponent<FriendlyScript>().GridPos);
                break;
            case Actions.ATTACK:
                AttackUnit();
                break;
        }
    }

    protected override void Die() {
        base.Die();
        _gridManager.UnitDestroyed(false);
    }
}
