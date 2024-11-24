using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Card", menuName = "Scriptable Objects/Character Card")]
public class SO_CharacterCard : ScriptableObject
{
    [SerializeField] private CharacterStats _stats;
    public CharacterStats Stats => _stats;
}


[Serializable]
public struct CharacterStats
{
    public String Name;
    public String Description;
    public int Health;
}
