using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public interface ISideDefence 
{
    public bool GivesDefenceFrom(Vector2Int direction);
}
