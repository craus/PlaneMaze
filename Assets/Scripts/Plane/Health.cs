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
        current = Mathf.Clamp(current - amount, 0, max); 
        if (current <= 0) {
            if (GetComponent<Player>() != null) {
                SoundManager.instance.playerDeath.Play();
            } else if (GetComponent<Lich>() != null) {
                SoundManager.instance.lichDeath.Play();
            } else if (GetComponent<Monster>() != null) {
                SoundManager.instance.monsterDeath.Play();
            }
            await GetComponent<IMortal>().Die();
        } else {
            if (GetComponent<Player>() != null) {
                SoundManager.instance.heroDamaged.Play();
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
    }

    public void Init() {
        if (GetComponent<Monster>() != null) {
            //current = max = ascentionHealth[Mathf.Clamp(Game.instance.Ascentions<MonstersHaveMoreHealth>(), 0, ascentionHealth.Count - 1)];
            if (!Game.instance.Ascention<CommonEnemiesHaveMultipleHP>() && GetComponent<Lich>() == null) {
                current = max = 1;
            }
        }
        if (GetComponent<Player>() != null) {
            if (Metagame.instance.Ascention<PlayerHasLessHealth>()) {
                current = max = 3;
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
