using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Figure : MonoBehaviour
{
    public Cell location;

    public UnityEvent<bool> afterMove;

    public void TryMove(Vector2Int delta) {
        var newPosition = location.Shift(delta);
        if (!newPosition.fieldCell.wall) {
            Move(newPosition);
        } else {
            var jumpPosition = location.Shift(delta * 2);
            if (!jumpPosition.fieldCell.wall) {
                Move(jumpPosition);
            }
        }
    }

    public void Move(Cell newPosition, bool isTeleport = false) {
        if (location != null) {
            location.figures.Remove(this);
        }
        location = newPosition;
        if (location != null) {
            location.figures.Add(this);
        }
        afterMove.Invoke(isTeleport);
        UpdateTransform();
    }

    private void UpdateTransform() {
        transform.position = location.transform.position.Change(z: location.transform.position.z - 1);
    }

    public void OnDestroy() {
        if (location != null) {
            location.figures.Remove(this);
        }
    }
}
