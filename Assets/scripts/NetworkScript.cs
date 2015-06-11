using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkScript : MonoBehaviour {
	public bool mode = true; // server/client

    public NetworkView rpcNet;
    public Text console;

    const int    NETWORK_PORT    = 4585;        // сетевой порт
    const int    MAX_CONNECTIONS = 20;          // максимальное количество входящих подключений
    const bool   USE_NAT         = false;       // использовать NAT?
    const string SERVER_URL      = "127.0.0.1"; // адрес сервера

    class User {
    	public NetworkViewID id;
    	public NetworkPlayer netPlayer;

    	public User(NetworkPlayer player) {
    		this.id = Network.AllocateViewID();
    		this.netPlayer = player;
    	}
    }

    private List<User> users = new List<User>();

    public  GameObject player = null;
	private List<GameObject> enemies = new List<GameObject>();

    void OnServerInitialized() {
    	consoleLog("Server | initialization complete");
        consoleLog("Server | rpcNet.viewID : " + rpcNet.viewID);
    }

    void OnPlayerConnected(NetworkPlayer newPlayer) {
    	consoleLog("Server | player connected | newPlayer : " + newPlayer);
        User newUser = new User(newPlayer);

        int count = users.Count;
    	for (int i = 0; i < count; i++) {
    		consoleLog("Server | player connected | count : " + count + "; new id : " + newUser.id + "; old id : " + users[i].id);
    		rpcNet.RPC("addPlayer", users[i].netPlayer, newUser.id);
    		rpcNet.RPC("addPlayer", newUser.netPlayer, users[i].id);
    	}

        users.Add(newUser);
        consoleLog("Server | player connected | newUser.id : " + newUser.id);
        rpcNet.RPC("setPlayer", newUser.netPlayer, newUser.id);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
    	consoleLog("Server | player disconnected");

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

    void OnFailedToConnect(NetworkConnectionError error) {
        consoleLog("Client | Failed to connect : " + error.ToString()); // при ошибке подключения к серверу выводим саму ошибку
    }

    void OnDisconnectedFromServer(NetworkDisconnection info) {
        if (Network.isClient) {
            consoleLog("Client | Disconnected from server : " + info.ToString()); // при успешном либо неуспешном отключении выводим результат
        } else {
            consoleLog("Client | Connections closed"); // сообщение выводится при выключении сервера Network.Disconnect()
        }
    }

    void OnConnectedToServer() {
        consoleLog("Client | Connected to server | rpcNet.viewID : " + rpcNet.viewID); // сообщение выводится при успешном подключении к серверу
    }

	NetworkView addNetworkComponent(GameObject target) {
		NetworkView netView = target.AddComponent(typeof(NetworkView)) as NetworkView; // добавляем компонент NetworkView нашему игровому объекту
        netView.stateSynchronization = NetworkStateSynchronization.Unreliable; // нам подходит способ быстрой передачи с потерями, поскольку наше передвижение интерполируется
        
        netView.observed = target.AddComponent<PositionSynchronizer>(); // указываем этот скрипт (компонент) для синхронизации
	
        return netView;
	}

	GameObject drawSphere(Vector3 pos, Color color) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = pos;

		MeshRenderer gameObjectRenderer = sphere.GetComponent("MeshRenderer") as MeshRenderer;
		gameObjectRenderer.material.color = color;

		return sphere;
	}

	[RPC]
	void addPlayer(NetworkViewID index) {
		consoleLog("Client | RPC > addPlayer | index : " + index);
		GameObject enemy = drawSphere(player.transform.position, Color.yellow);

		NetworkView enemyNetwork = addNetworkComponent(enemy);
		enemyNetwork.viewID = index;

		player.GetComponent<WorldDestroyer>().players.Add(enemy);

		enemies.Add(enemy);
	}

	[RPC]
	void setPlayer(NetworkViewID index) {
		consoleLog("Client | RPC > setPlayer | index : " + index);
		NetworkView playerNetwork = addNetworkComponent(player);
        playerNetwork.viewID = index;
	}

	[RPC]
	void removePlayer(NetworkPlayer player) {
		consoleLog("Client | RPC > removePlayer | player : " + player);
		Network.RemoveRPCs(player); // очищаем список процедур игрока
        Network.DestroyPlayerObjects(player); // уничтожаем все объекты игрока
	}

	//========<Utils>=======

    void consoleLog(string text) {
        console.text += text + "\n";
    }

    //=======</Utils>=======

	// Use this for initialization
	void Start () {
		if (mode) {
			Network.InitializeSecurity(); // инициализируем защиту
        	Network.InitializeServer(MAX_CONNECTIONS, NETWORK_PORT, USE_NAT);
		} else {
			Network.Connect(SERVER_URL, NETWORK_PORT);
		}
	}
}
