using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class RingOfTerraforming : MonoBehaviour, IBeforeWalk
{
    public async Task<bool> BeforeWalk(Vector2Int delta, int priority) {
        if (priority != 0) {
            return false;
        }
        var target = GetComponent<Item>().Owner.figure.location.Shift(delta).GetFigure<Terrain>();
        if (target == null) {
            return false;
        }

        Destroy(Inventory.instance.GetItem<RingOfTerraforming>().gameObject);
        Destroy(target.gameObject);
        SoundManager.instance.terraform.Play();
        return true;
    }
}
