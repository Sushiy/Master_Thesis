using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    //Resources are in 0x0..
    Wood = 0x00,
    Log = 0x01,
    Iron = 0x02,
    Wheat = 0x03,
    Flour = 0x04,
    Water = 0x05,
    Stone = 0x06,

    //Tools are in 0x10..
    Axe = 0x101,
    Pickaxe = 0x102,

    //Items are in 0x100..
    Bread = 0x1001,
}
