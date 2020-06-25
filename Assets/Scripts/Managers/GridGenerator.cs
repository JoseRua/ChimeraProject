using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{

    public float _scale = 1;
    public GameObject _gridPrefab;
    public bool _findDistance = false;
    public List<GameObject> _pathList = new List<GameObject>();

    private GridManager _gridManager;

    private Vector3 _leftBottomPos = new Vector3(0, 0, 0);
    private Vector3 _rightBottomPos = new Vector3(0, 0, 0);
    private Vector3 _leftTopPos = new Vector3(0, 0, 0);
    private Vector3 _rightTopPos = new Vector3(0, 0, 0);

    private int _rows = 10;
    private int _columns = 10;
    private GameObject[,] _gridArray;
    private int _startNodeX, _startNodeY;
    private int _endNodeX, _endNodeY;



    public Vector3 LeftBottomPos { get => _leftBottomPos; set => _leftBottomPos = value; }
    public Vector3 RightBottomPos { get => _rightBottomPos; set => _rightBottomPos = value; }
    public Vector3 LeftTopPos { get => _leftTopPos; set => _leftTopPos = value; }
    public Vector3 RightTopPos { get => _rightTopPos; set => _rightTopPos = value; }


    public List<GameObject> FindPath(GridManager gm, Vector2Int startPos, Vector2Int endPos) {
        _gridManager = gm;
        _startNodeX = startPos.x;
        _startNodeY = startPos.y;
        _endNodeX = endPos.x;
        _endNodeY = endPos.y;

        SetDistance();
        SetPath();
        return _pathList;
    }

    public void GenerateGrid(int rows, int columns) {

        _rows = rows;
        _columns = columns;
        _gridArray = new GameObject[_rows, _columns];
        for (int i = 0; i < _columns; i++) {
            for (int j = 0; j < _rows; j++) {

                GameObject obj = Instantiate(_gridPrefab, new Vector3(_leftBottomPos.x + _scale * i, _leftBottomPos.y, _leftBottomPos.z + _scale * j), Quaternion.identity * Quaternion.Euler(new Vector3(90, 0, 0)));
                obj.transform.SetParent(transform);
                obj.GetComponent<Node>()._x = i;
                obj.GetComponent<Node>()._y = j;
                obj.GetComponent<Node>()._gridPosition = new Vector2Int(i, j);
                obj.name = "tile(" + i + "," + j + ")";

                _gridArray[i, j] = obj;
            }
        }

        _rightBottomPos = new Vector3(_leftBottomPos.x + _scale * (_rows - 1), _leftBottomPos.y, _leftBottomPos.z);
        _leftTopPos = new Vector3(_leftBottomPos.x, _leftBottomPos.y, _leftBottomPos.z + _scale * (_columns - 1));
        _rightTopPos = new Vector3(_leftBottomPos.x + _scale * (_rows - 1), _leftBottomPos.y, _leftBottomPos.z + _scale * (_columns - 1));
    }

    private void Init() {
        foreach (GameObject go in _gridArray) {
            go.GetComponent<Node>()._visited = -1;
        }

        _gridArray[_startNodeX, _startNodeY].GetComponent<Node>()._visited = 0;
    }

    /// <summary>
    /// Tests the node in the direction indicated
    /// </summary>
    private bool TestDirection(int x, int y, int step, int direction) {
        // 1 - up, 2 - right, 3 - down, 4 - left
        try {
            switch (direction) {
                case 1:
                    if ((y + 1) < _rows && _gridArray?[x, y + 1].GetComponent<Node>()._visited == step && !_gridArray[x, y + 1].GetComponent<Node>()._wall && !IsUnitAt(x, y + 1)) {
                        return true;
                    }
                    break;
                case 2:
                    if ((x + 1) < _columns && _gridArray?[x + 1, y].GetComponent<Node>()._visited == step && !_gridArray[x + 1, y].GetComponent<Node>()._wall && !IsUnitAt(x +1 , y)) {
                        return true;
                    }
                    break;
                case 3:
                    if ((y - 1) > -1 && _gridArray?[x, y - 1].GetComponent<Node>()._visited == step && !_gridArray[x, y - 1].GetComponent<Node>()._wall && !IsUnitAt(x, y - 1)) {
                        return true;
                    }
                    break;
                case 4:
                    if ((x - 1) > -1 && _gridArray?[x - 1, y].GetComponent<Node>()._visited == step && !_gridArray[x - 1, y].GetComponent<Node>()._wall && !IsUnitAt(x -1, y)) {
                        return true;
                    }
                    break;

            }
        } catch (System.Exception e) {
            return false;
        }
        return false;
    }
    /// <summary>
    /// Checks, for each neighbour nodes of the (x,y), if it wasn't visited (step = -1)
    /// If not , set it as visited with the current step
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="step"></param>
    private void TestFourDirections(int x, int y, int step) {
        if (TestDirection(x, y, -1, 1)) { // -1 to check if it was not visited
            SetVisited(x, y + 1, step);
        }
        if (TestDirection(x, y, -1, 2)) {
            SetVisited(x + 1, y, step);
        }
        if (TestDirection(x, y, -1, 3)) {
            SetVisited(x, y - 1, step);
        }
        if (TestDirection(x, y, -1, 4)) {
            SetVisited(x - 1, y, step);
        }

    }

    private void SetVisited(int x, int y, int step) {
        _gridArray[x, y].GetComponent<Node>()._visited = step;
    }

    private void SetDistance() {
        Init();
        int x = _startNodeX;
        int y = _startNodeY;
        int[] testArray = new int[_rows * _columns];
        for (int step = 1; step < _rows * _columns; step++) {
            foreach (GameObject go in _gridArray) {
                if (go.GetComponent<Node>()._visited == step - 1) {
                    TestFourDirections(go.GetComponent<Node>()._x, go.GetComponent<Node>()._y, step);
                }
            }
        }
    }

    /// <summary>
    /// Set the list of nodes where the unit moves
    /// </summary>
    private void SetPath() {
        int step = -1;
        int x = _endNodeX;
        int y = _endNodeY;

        List<GameObject> tempList = new List<GameObject>();

        _pathList.Clear();

        if (_gridArray[_endNodeX, _endNodeY].GetComponent<Node>()._visited > 0) {
            _pathList.Add(_gridArray[x, y]); // Adds last node
            step = _gridArray[x, y].GetComponent<Node>()._visited - 1;
        } else {
            Debug.Log("impossible to reach location");
        }


        for (int i = step; step > -1; step--) {
            // adds to tempList all neighbours which step equal to step
            if (TestDirection(x, y, step, 1)) {
                tempList.Add(_gridArray[x, y + 1]);
            }
            if (TestDirection(x, y, step, 2)) {
                tempList.Add(_gridArray[x + 1, y]);
            }
            if (TestDirection(x, y, step, 3)) {
                tempList.Add(_gridArray[x, y - 1]);
            }
            if (TestDirection(x, y, step, 4)) {
                tempList.Add(_gridArray[x - 1, y]);
            }
            // checks for the closest of the list
            GameObject tempGo = FindClosest(_gridArray[_endNodeX, _endNodeY].transform.transform, tempList);
            _pathList.Add(tempGo);
            x = tempGo.GetComponent<Node>()._x;
            y = tempGo.GetComponent<Node>()._y;
            tempList.Clear();
        }

        _pathList.Reverse();



    }

    private GameObject FindClosest(Transform target, List<GameObject> list) {
        float currentDistance = _scale * _rows * _columns;
        int index = 0;
        for (int i = 0; i < list.Count; i++) {
            if (Vector3.Distance(target.position, list[i].transform.position) < currentDistance) {
                currentDistance = Vector3.Distance(target.position, list[i].transform.position);
                index = i;
            }
        }
        return list[index];
    }

    private bool IsUnitAt(int x, int y) {
        if (new Vector2Int(x, y) == new Vector2Int(_endNodeX, _endNodeY))
            return false;
        return _gridManager.IsUnitAt(new Vector2Int(x,y), new Vector2Int(_startNodeX, _startNodeY));
    }
}
