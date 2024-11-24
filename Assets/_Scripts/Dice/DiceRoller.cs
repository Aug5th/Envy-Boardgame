using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] Dice[] dices;

    private void Awake()
    {
        dices = GetComponentsInChildren<Dice>();
    }

    private void Start()
    {
        
    }

    public void RollDices()
    {
        foreach (var dice in dices)
        {
            var name = dice.RollDice();
            Debug.Log(dice.name + " " + name);
        }
    }

}
