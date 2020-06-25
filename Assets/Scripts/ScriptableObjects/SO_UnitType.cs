
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitType", order = 1)]
public class SO_UnitType : ScriptableObject
{
    public UnitType _type;
    public string _name;

    public int _actionPoints;
    public int _hp;
    public int _attackPower;
}