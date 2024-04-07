using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Fire : Terrain, IAttacker
{
    public int damage = 1;

    public GameObject attackProjectileSample;

    public override void Awake() {
        base.Awake();

        //Player.instance.figure.afterMove.Add(AfterPlayerMove);

        GetComponent<Figure>().collide = async (from, figure) => {
            if (figure == null) {
                return;
            }
            var victim = figure.GetComponent<Unit>();
            if (victim != null && !victim.Flying) {
                await Attack(victim);
                if (this == null) {
                    return;
                }
                //Destroy(gameObject);
                gameObject.SetActive(false);
                GetComponent<Figure>().OnDestroy();
            }
        };
    }

    public void OnDestroy() {
        //Player.instance.figure.afterMove.Remove(AfterPlayerMove);
    }

    private async Task Attack(Unit victim) {
        Game.Debug($"Wolftrap {GetComponent<Figure>()} attacks {victim}");
        if (!Game.CanAttack(null, victim)) {
            Game.Debug($"Wolftrap {GetComponent<Figure>()} cannot attack {victim}");
            return;
        }
        SoundManager.instance.wolftrapAttack.Play();

        var ap = Instantiate(attackProjectileSample);
        ap.transform.position = victim.transform.position;
        await Helpers.Delay(0.1f); 
        Destroy(ap);
        if (this == null) {
            return;
        }

        await victim.Hit(new Attack(Vector2Int.zero, GetComponent<Figure>(), victim.figure, GetComponent<Figure>().Location, victim.figure.Location, damage));
        
    }

    private async Task AfterPlayerMove(Cell from, Cell to) {
        if (Mathf.Abs((to.position - GetComponent<Figure>().Location.position).magnitude - 1) < 1e-4) {
            await GetComponent<Health>().Hit(1);
        }
    }

    public async Task Die() {
        SoundManager.instance.wolftrapAttack.Play();
        Destroy(gameObject);
    }
}
