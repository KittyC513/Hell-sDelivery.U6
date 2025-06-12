using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceCollectables : MonoBehaviour
{
    [SerializeField] public List<Collectable> collectables;

    private int numToCollect;
    [SerializeField] private int numCollected;

    [SerializeField] public UnityEvent onComplete;

    private void Start()
    {
        numToCollect = collectables.Count;
    }

    private void OnEnable()
    {
        foreach (Collectable collectable in collectables)
        {
            collectable.onCollect += CollectOne;
        }
    }

    private void OnDisable()
    {
        foreach (Collectable collectable in collectables)
        {
            collectable.onCollect -= CollectOne;
        }
    }

    private void CollectOne(GameObject p, Collectable collectable)
    {
        //this object was collected remove it from the list
        collectables.Remove(collectable);

        //add up our number
        numCollected++;

        if (numCollected >= numToCollect)
        {
            onComplete?.Invoke();
        }
    }

}
