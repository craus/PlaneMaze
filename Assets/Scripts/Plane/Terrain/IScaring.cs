using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface IScaring
{
    public virtual bool Scaring(Unit unit) => false;
}
