using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayDictionary<TForwardKey,TReverseKey>
{
    public Dictionary<TForwardKey, TReverseKey> Forward { get; private set; } = new Dictionary<TForwardKey, TReverseKey>();
    public Dictionary<TReverseKey, TForwardKey> Reverse { get; private set; } = new Dictionary<TReverseKey, TForwardKey>();

    public TwoWayDictionary()
    {

    }

    public void Add(TForwardKey t1, TReverseKey t2)
    {
        if (Forward.ContainsKey(t1))
            throw new System.ArgumentException("Forward Key already exists");
        if (Reverse.ContainsKey(t2))
            throw new System.ArgumentException("Reverse Key already exists");

        Forward.Add(t1, t2);
        Reverse.Add(t2, t1);

    }

    public bool Remove(TReverseKey reverseKey)
    {
        if (Reverse.ContainsKey(reverseKey) == false)
            return false;

        var forwardKey = Reverse[reverseKey];

        if (!Reverse.Remove(reverseKey)) return false;

        if (Forward.Remove(forwardKey)) return true;

        // Reverse-Key record could not be removed, restore the Fwd-Key record
        Forward.Add(forwardKey, reverseKey);// = reverseKey;

        return false;
    }

    public bool Remove(TForwardKey forwardKey)
    {
        if (Forward.ContainsKey(forwardKey) == false) 
            return false;

        var reverseKey = Forward[forwardKey];

        if (!Forward.Remove(forwardKey)) return false;

        if (Reverse.Remove(reverseKey))  return true;

        // Reverse-Key record could not be removed, restore the Fwd-Key record
        Forward.Add(forwardKey, reverseKey);// = reverseKey;

        return false;
    }

    public int Count()
    {
        return Forward.Count;
    }


}
