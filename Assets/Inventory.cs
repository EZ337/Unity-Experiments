using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<Item, int> BackPack = new Dictionary<Item, int>();
    public static Inventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public int AddItem(Item item)
    {
        if (!BackPack.ContainsKey(item))
            BackPack.Add(item, 1);
        else
            ++BackPack[item];

        return BackPack[item];
    }

    public bool HasItem(Item item)
    {  return BackPack.ContainsKey(item); }

    public int GetItemCount(Item item)
    {
        BackPack.TryGetValue(item, out var count);
        return count;
    }
}
