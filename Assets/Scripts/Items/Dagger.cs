using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Dagger : MonoBehaviour
{
    public Unit Owner => Player.instance;

    public GameObject attackProjectile;

    public void Awake() {
        GetComponent<Item>().afterFailedWalk = TryAttack;
        attackProjectile.SetActive(false);
    }

    public async void Attack(Unit target) {
        var startPosition = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.25f);
        var endPosition = Vector3.Lerp(Owner.transform.position, target.transform.position, 0.75f);

        attackProjectile.SetActive(true);
        attackProjectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - Owner.transform.position);
        attackProjectile.transform.position = endPosition;

        await Task.Delay(100);
        attackProjectile.SetActive(false);
    }

    public bool TryAttack(Vector2Int delta) {
        var newPosition = Owner.figure.location.Shift(delta);
        if (newPosition.figures.Any(f => f.GetComponent<Unit>() != null)) {
            Attack(newPosition.GetFigure<Unit>());
            return true;
        }
        return false;
    }
}
