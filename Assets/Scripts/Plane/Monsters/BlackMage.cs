using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BlackMage : Monster
{
    public override async Task Hit(int damage) {
        await base.Hit(damage);
        await Helpers.TeleportAway(figure, 8);
    }
}
