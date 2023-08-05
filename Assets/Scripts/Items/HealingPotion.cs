using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class HealingPotion : MonoBehaviour
{
    public GameObject healSample;

    public int heal = 1;

    [Multiline(8)]
    public string fullHealDescription;

    public void Start() {
        if (!GameManager.instance.metagame.Ascention<HealOnlyOneHP>()) {
            heal = 100500;
            GetComponent<Item>().description = fullHealDescription;
        }
    }

    public void OnEnable() {
        GetComponent<Item>().afterPick.Add(OnPick);
    }

    public void OnDisable() {
        GetComponent<Item>().afterPick.Remove(OnPick);
    }

    private async Task OnPick() {
        var healEffect = Instantiate(healSample, Game.instance.transform);
        healEffect.transform.position = transform.position;
        await Helpers.Delay(0.1f);
        Destroy(healEffect);

        await GetComponent<Item>().Owner.GetComponent<Health>().Heal(heal);
        this.SoftDestroy(gameObject);
    }
}
