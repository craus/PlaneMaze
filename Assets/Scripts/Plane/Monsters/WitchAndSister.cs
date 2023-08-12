using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WitchAndSister : Monster
{
    public override bool HasSoul => false;
    public override bool Boss => true;

    public Witch witch;
    public Sister sister;

    [SerializeField] private int teleportRadius = 8;
    [SerializeField] private int playerInvulnerabilityDuration = 2;

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        if (alive) {
            attack.afterAttack.Add(async () => {
                await attack.from.GetComponent<Invulnerability>().Gain(playerInvulnerabilityDuration);
                await Helpers.TeleportAway(attack.from, teleportRadius);
            });
        }
    }

    protected override async Task MakeMove() {
    }

    public override async Task Die() {
        await base.Die();
        if (!witch.alive && !sister.alive) {
            await Game.instance.Win();
        }
    }
}
