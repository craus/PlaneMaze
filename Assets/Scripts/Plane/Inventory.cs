using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Singletone<Inventory>
{
    public List<Item> items;
    public RectTransform itemsFolder;
}
