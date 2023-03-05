using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IAttackModifier 
{
    public int Priority { get; }
    public Task ModifyAttack(Attack attack);
}
