using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BagManager<T>  where T : class
{
    private static T instance;
    public static T Instance => instance;

    public static List<ItemBase<T>> bag = new List<ItemBase<T>>();

    private int index = 0;

}
