using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Health : MonoBehaviour
{
    [SerializeField] private bool showHeartsOnFullHealth = false;

    public List<GameObject> hearts;

    public int current = 3;
    public int max = 3;

    public List<int> ascentionHealth = new List<int>() { 1, 2, 3 };

    public int Current => current;

    public async Task Hit(int amount) {
        Debug.LogFormat($"[{Game.instance.time}] {GetComponent<Figure>()} loses {amount} hp");
        if (amount <= 0) {
            return;
        }
        if (GetComponent<Curse>().Current > 0) {
            GetComponent<Unit>().lastAttacker = GetComponent<Curse>().attacker;
            await GetComponent<IMortal>().Die();
            return;
        }
        current = Mathf.Clamp(current - amount, 0, max); 
        if (current <= 0) {
            await GetComponent<IMortal>().Die();
        } else {
            if (GetComponent<Player>() != null) {
                SoundManager.instance.heroDamaged.Play();
            }
            if (GetComponent<Witch>() != null) {
                SoundManager.instance.witchDamaged.Play();
            }
            if (GetComponent<Sister>() != null) {
                SoundManager.instance.sisterDamaged.Play();
            }
            UpdateHearts();
        }
    }

    public async Task Heal(int amount) {
        if (GetComponent<Player>() != null) {
            SoundManager.instance.heal.Play();
        } 
        current = Mathf.Clamp(current + amount, 0, max);
        UpdateHearts();
    }

    public void Awake() {
        Init();

        new ValueTracker<int>(() => current, v => {
            current = v;
            UpdateHearts();
        });
    }

    public void Init() {
        if (GetComponent<Monster>() != null) {
            //current = max = ascentionHealth[Mathf.Clamp(Game.instance.Ascentions<MonstersHaveMoreHealth>(), 0, ascentionHealth.Count - 1)];
            if (!Metagame.instance.HasAscention<CommonEnemiesHaveMultipleHP>() && !GetComponent<Unit>().Boss) {
                current = max = (max + 1) / 2;
            }
        }
        if (GetComponent<Player>() != null) {
            if (Metagame.instance.HasAscention<PlayerHasLessHealth>()) {
                current = max = 5;
            } else {
                current = max = 8;
            }
            //current = max = ascentionHealth[Mathf.Clamp(Game.instance.Ascentions<PlayerHasLessHealth>(), 0, ascentionHealth.Count - 1)];
        }
        UpdateHearts();
    }


    public void UpdateHearts() {
        if (current < max || showHeartsOnFullHealth) {
            for (int i = 0; i < hearts.Count; i++) {
                hearts[i].SetActive(current >= i + 1);
            }
        } else {
            hearts.ForEach(h => h.SetActive(false));
        }
    }
}
