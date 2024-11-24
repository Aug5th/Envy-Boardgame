using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private SO_Dice _diceConfiguration;


    private void Start()
    {
        
        for (int i = 0; i < 5; i++)
        {
            int diceIdx = RollDice();
            Debug.Log(_diceConfiguration.GetName(diceIdx));
        }
        
    }

    private int RollDice()
    {
        // Get the total weight of all sides
        float totalWeight = 0f;
        foreach (var side in _diceConfiguration.Sides)
        {
            totalWeight += side.Weight;
        }

        // Generate a random number between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);

        // Find which side the random value falls on
        float cumulativeWeight = 0f;
        for (int i = 0; i < _diceConfiguration.Sides.Length; i++)
        {
            cumulativeWeight += _diceConfiguration.GetWeight(i);

            // If the random value is less than the cumulative weight, return the current side index
            if (randomValue <= cumulativeWeight)
            {
                return i;
            }
        }

        // If no side was selected (it should never reach here), return the last side as a fallback
        return _diceConfiguration.Sides.Length - 1;
    }
}