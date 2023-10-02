using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionController : MonoBehaviour
{
    [SerializeField] private Transform gamePos; // Reference to the "game-pos" object in the Unity Inspector
    [SerializeField] private Transform fullGame; // Reference to the "full-game" object in the Unity Inspector
    private bool wasPositionSet = false;

    private void Awake() {
        fullGame.gameObject.SetActive(false);
    }
    public void SetGamePosition() {
        // Set the position of the "full-game" object to match the position of "game-pos"
        if (!wasPositionSet) {
            fullGame.gameObject.SetActive(true);
            fullGame.position = gamePos.position;
            wasPositionSet = true;
        }
    }
}
