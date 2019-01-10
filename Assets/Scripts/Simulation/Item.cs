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
    Herb,

    //Tools are in 0x10..
    Axe = 0x100,
    Pickaxe,
    Hoe,
    Hammer,
    Sword,

    //Items are in 0x20..
    Bread = 0x200,
    Flour,
    Medicine,
    Shingles,
    Bricks,
}
