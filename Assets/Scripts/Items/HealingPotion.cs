using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class HealingPotion : MonoBehaviour
{
    public int heal = 1;

    public void OnEnable() {
        GetComponent<Item>().onPick.Add(OnPick);
    }

    public void OnDisable() {
        GetComponent<Item>().onPick.Remove(OnPick);
    }

    private async Task OnPick() {
        await GetComponent<Item>().Owner.GetComponent<Health>().Heal(heal);
        Destroy(gameObject);
    }
}
