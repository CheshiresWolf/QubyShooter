using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DedicatedServer : MonoBehaviour {
	const int  NETWORK_PORT    = 4585;  // сетевой порт
    const int  MAX_CONNECTIONS = 20;    // максимальное количество входящих подключений
    const bool USE_NAT         = false; // использовать NAT?

    public NetworkView rpcNet;
    public Text console;

    class User {
    	public NetworkViewID id;
    	public NetworkPlayer netPlayer;

    	public User(NetworkPlayer player) {
    		this.id = Network.AllocateViewID();
    		this.netPlayer = player;
    	}
    }

    private List<User> users = new List<User>();

    void OnServerInitialized() {
    	console.text += "Server | initialization complete\n";
    }

    void OnPlayerConnected(NetworkPlayer newPlayer) {
    	console.text += "Server | player connected | newPlayer : " + newPlayer;
        User newUser = new User(newPlayer);

    	for (int i = 0; i < users.Count; i++) {
    		rpcNet.RPC("addPlayer", users[i].netPlayer, newUser.id);
    	}

        users.Add(newUser);
        rpcNet.RPC("setPlayer", newUser.netPlayer, newUser.id);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
    	console.text += "Server | player disconnected\n";

    	List<User> otherUsers = new List<User>();
    	for (int i = 0; i < users.Count; i++) {
    		if (users[i].netPlayer != player) {
    			otherUsers.Add(users[i]);
    			rpcNet.RPC("removePlayer", users[i].netPlayer, player);
    		}
    	}
    	users = otherUsers;

        Network.RemoveRPCs(player); // очищаем список процедур игрока
        Network.DestroyPlayerObjects(player); // уничтожаем все объекты игрока
    }

	// Use this for initialization
	void Start () {
		Network.InitializeSecurity(); // инициализируем защиту
        Network.InitializeServer(MAX_CONNECTIONS, NETWORK_PORT, USE_NAT);
	}
}
