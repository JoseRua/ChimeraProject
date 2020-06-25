using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region Public Variables
    public float _speed = 5;
    public SO_UnitType _unitType;
    
    #endregion

    #region Private Variables
    // unit variables
    protected UnitType _type;
    protected string _name;
    protected int _actionPoints;
    protected int _hp;
    protected int _attackPower;
    protected GridManager _gridManager;
    protected Vector2Int _gridPos;
    protected bool _nearEnemy = false;


    // movement variables
    protected List<GameObject> _path = new List<GameObject>();
    private Coroutine _moving, _actioning;

    // Properties
    public GridManager GridManager { get => _gridManager; set => _gridManager = value; }
    public Vector2Int GridPos { get => _gridPos; set => _gridPos = value; }
    public string Name { get => _name; set => _name = value; }
    public int Hp { get => _hp; set => _hp = value; }

    //action varibales
    protected List<Actions> _actionsToDo = new List<Actions>();
    protected int _actionIndex = 0;
    protected GameObject _unitToAttack = null; // reference to a possible unit to attack
    #endregion

    private void Start() {
        _type = _unitType._type;
        _actionPoints = _unitType._actionPoints;
        _hp = _unitType._hp;
        _attackPower = _unitType._attackPower;
    }

    public void SetInitPos(Vector3 pos) {
        transform.position = pos;
    }

    protected void SetPathAndGo(List<GameObject> list) {
        StartCoroutine(CR_MoveThroughPositions(list));
    }

    protected IEnumerator CR_MoveThroughPositions(List<GameObject> list) {
        _path.Clear();
        _path.AddRange(list);

        
        int temp = Mathf.Min(_actionPoints,list.Count);
        
        for (int i = 0; i < temp; i++) {
            //_actionPoints--;
            _moving = StartCoroutine(CR_Moving(i));
            yield return _moving;
        }

        _gridPos = list[temp-1].GetComponent<Node>()._gridPosition;

        // at the end of moving check for more actions (i.e. can kill unit after moving)
        EndAction(); 
    }

    protected IEnumerator CR_Moving(int currentPosition) {
        while (transform.position != _path[currentPosition].transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, _path[currentPosition].transform.position, _speed * Time.deltaTime);
            yield return null;
        }

    }

    virtual protected void DoAction(Actions action) {
    }

    virtual protected void EndAction() {
        if (++_actionIndex < _actionsToDo.Count) {
            DoAction(_actionsToDo[_actionIndex]);
        } else {
            // Ended all actions of this unit
            // Move to next unit or different team
            _actionPoints = _unitType._actionPoints;
            _actionIndex = 0;
            _gridManager.EndedTurn();
        }
    }

    protected void MoveToTile(Vector2Int endPos) {
        List<GameObject> list = _gridManager.FindPath(_gridPos, endPos);
        if (list.Count > 0)
            SetPathAndGo(list);
    }

    protected void MoveToEnemyTile(Vector2Int endPos) {
        _nearEnemy = false;
        List<GameObject> list = _gridManager.FindPath(_gridPos, endPos);

        // if last of the list is the enemy, stop at previous node
        if(list[Mathf.Max(0,Mathf.Min(list.Count - 1, _actionPoints))].GetComponent<Node>()._gridPosition == endPos) {
            list.RemoveAt(list.Count - 1);
            _nearEnemy = true;
        }
        if (list.Count > 0)
            SetPathAndGo(list);
    }

    protected void AttackUnit() {
        if(_unitToAttack != null && _nearEnemy) {
            GameManager._instance.Log(_name + "attacked " + _unitToAttack.GetComponent<Unit>().Name + " with " + _attackPower + " points of damage!");
            _unitToAttack.GetComponent<Unit>().Damage(_attackPower);
        }
        //Attacking always clears all actions
        //_actionsToDo.Clear();

        EndAction();
    }

    protected void Damage(int attackPower) {
        _hp -= attackPower;
        if(_hp <= 0) {
            Die();
        }
    }

    virtual protected void Die() {
        GameManager._instance.Log(_name + "died!");
        gameObject.SetActive(false);
    }

}
