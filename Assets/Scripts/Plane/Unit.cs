using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Unit : MonoBehaviour
{
    public Figure figure;

    public bool alive = true;

    public virtual void Awake() {
        if (figure == null) figure = GetComponent<Figure>();
    }

    public virtual async Task Hit(int damage) {
        if (this == null) {
            return;
        }
        await GetComponent<Health>().SetCurrent(GetComponent<Health>().Current - damage);
    }

    public virtual async Task Die() {
        if (!alive) {
            return;
        }
        alive = false;
        Destroy(gameObject);
        foreach (var listener in GameEvents.instance.onUnitDeath) {
            await listener(this);
        }
    }
}
