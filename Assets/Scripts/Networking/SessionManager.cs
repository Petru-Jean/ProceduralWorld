using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SessionManager 
{

    private static SessionManager instance;
    public  static SessionManager Instance => instance ??= new SessionManager();

    // For 2-way dictionary search
    private TwoWayDictionary<string, ulong> clients;

    private SessionManager()
    {
        clients = new TwoWayDictionary<string, ulong>();
    }

    public bool IsConnected(string clientName)
    {
        return clients.Forward.ContainsKey(clientName);
    }

    public bool TryGetClient(ulong clientId,   out string clientName)
    {
        return clients.Reverse.TryGetValue(clientId, out clientName);
    }

    public bool TryGetClient(string clientName, out ulong clientId)
    {
        return clients.Forward.TryGetValue(clientName, out clientId);
    }

    public bool AddClient(string clientName, ulong clientId)
    {
        if (clients.Forward.ContainsKey(clientName))
        {
            Debug.LogWarning("Client " + clientName +" already connected");

            return false;
        }

        clients.Add(clientName, clientId);

        return true;
    }

    public bool RemoveClient(ulong clientId)
    {
        return clients.Remove(clientId);
    }

    public bool RemoveClient(string clientName)
    {
        return clients.Remove(clientName);
    }
    
    public Dictionary<string, ulong> GetClients()
    {
        return clients.Forward;
    }


}
