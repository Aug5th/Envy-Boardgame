using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Role Card", menuName = "Scriptable Objects/Role Card")]
public class SO_RoleCard : ScriptableObject
{
    public Role Role;
    public RoleCard Prefab;
}


[Serializable]
public enum Role
{
    None,
    Sheriff,
    Deputy,
    Outlaw,
    Renegade
}
