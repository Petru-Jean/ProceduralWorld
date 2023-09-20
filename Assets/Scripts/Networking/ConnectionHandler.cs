using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.IO;

public class ConnectionHandler : MonoBehaviour
{
  
    public struct ConnectionData
    {
        public string clientName;
        public string serverPassword;


        public ConnectionData(string clientName, string serverPassword)
        {
            this.clientName     = clientName;
            this.serverPassword = serverPassword;
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new ConnectionData("Name", "Password")));
        
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.OnClientConnectedCallback  += ClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnect;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            NetworkManager.Singleton.StartHost();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void ClientConnect(ulong clientId)
    {
        Debug.Log("Client " + clientId + " has connected.");
    }

    private void ClientDisconnect(ulong clientId)
    {
        SessionManager.Instance.RemoveClient(clientId);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        response.Approved = false;
        response.CreatePlayerObject = false;

        try
        {

            ConnectionData connData = JsonUtility.FromJson<ConnectionData>(System.Text.Encoding.UTF8.GetString(request.Payload));

            if (SessionManager.Instance.IsConnected(connData.clientName))
            {
                response.Reason = String.Format("Client {0} is already connected", connData.clientName);
                return;
            }

            SessionManager.Instance.AddClient(connData.clientName, clientId);
        }
        catch(Exception ex)
        {
            Debug.LogError("Failed to decode connection data: " + ex.Message);

            response.Reason = "Failed to decode connection data: " + ex.Message;

            return;
        }

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = new Vector3(UnityEngine.Random.Range(-10, 10), 0);

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

    }


}
