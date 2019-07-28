using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    //Resources are in 0x0..
    Wood = 0x00,
    Log,
    Iron,
    Wheat,
    Water,
    Stone,

    //Tools are in 0x10..
    IronAxe = 0x100,
    IronPickaxe,
    IronHoe,
    FishingRod,
    StoneAxe,
    StonePickaxe,


    //Items are in 0x20..
    Food = 0x200,
    Flour,
}
