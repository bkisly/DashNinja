using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType itemType;

    private const uint _timePointsToAdd = 5;
    private const float _invulnerabilityTime = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
            CollectItem();
    }

    private void CollectItem()
    {
        switch(itemType)
        {
            case ItemType.Gold:
                PlayerStats.Instance.AddTimePoints(_timePointsToAdd);
                break;
            case ItemType.Vial:
                PlayerStats.Instance.SetInvulnerable(_invulnerabilityTime);
                break;
            case ItemType.FulgentPearl:
                PlayerStats.Instance.IncrementLives();
                break;
        }

        Destroy(gameObject);
    }
}

public enum ItemType
{
    Gold,
    Vial,
    FulgentPearl
}
