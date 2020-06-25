using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text _turnText;
    public Text _logText;

    public void Log(string s) {
        s += "\n";
        _logText.text += "- " + s;
    }

    public void NextTurn(bool playerTurn) {
        if (playerTurn) {
            _turnText.text = "Your turn!";
        } else {
            _turnText.text = "Rival playing...";
        }
    }

    public void EndGame(bool enemies) {
        if (enemies) {
            _turnText.text = "All enemies destroyed!\nYou won!";
        } else {
            _turnText.text = "All your units were destroyed!\nYou lost!";
        }
    }
}
