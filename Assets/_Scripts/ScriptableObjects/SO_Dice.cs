using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dice Configuration", menuName = "Scriptable Objects/Dice Configuration")]
public class SO_Dice : ScriptableObject
{
    [SerializeField] private DiceSide[] _sides = new DiceSide[6];
    public DiceSide[] Sides => _sides;

    public float GetWeight(int sideIndex)
    {
        if (sideIndex >= 0 && sideIndex < _sides.Length)
        {
            return _sides[sideIndex].Weight;
        }
        Debug.LogWarning("Side index out of range!");
        return 0f;
    }

    // Optional: Method to get the name of a specific side by index
    public SideName GetName(int sideIndex)
    {
        if (sideIndex >= 0 && sideIndex < _sides.Length)
        {
            return _sides[sideIndex].Name;
        }
        Debug.LogWarning("Side index out of range!");
        return SideName.None;
    }
}

[Serializable]
public struct DiceSide
{
    public SideName Name;
    public int Weight;
}

[Serializable]
public enum SideName
{
    None,
    Attack1,
    Attack2,
    Healing,
    Sealing,
    Curse,
    AOE
}