using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBossGenerator
{
    public void GenerateBoss(List<Cell> cellOrderList);
}
