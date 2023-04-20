using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header ("Board Dimensions")]
    public int xSice;
    public int ySice;
    [Header ("Avilable props")]
    public Sprite[] propsSprites;

    [Header ("Avilable props")]
    public int[] scoreGoals;

    [Header ("Avilable ingredients")]
    public Sprite[] ingredienstSprites;

    [Header ("Avilable moves per level")]
    public int moves;

    [Header ("Cauldro Section")]
    public GameObject CauldronSection;
}
