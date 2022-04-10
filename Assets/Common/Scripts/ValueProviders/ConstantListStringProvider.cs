using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConstantListStringProvider : ListStringValueProvider
{
    [SerializeField] private List<string> m_value;
    public override List<string> Value => m_value;
}
