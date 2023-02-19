using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IReceiveAttackModifier 
{
    public int Priority { get; }
    public void ModifyAttack(Attack attack);
}
