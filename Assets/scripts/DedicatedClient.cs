using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DedicatedClient : MonoBehaviour {
    const string SERVER_URL   = "127.0.0.1"; // адрес сервера
	const int    NETWORK_PORT = 4585;        // сетевой порт

	public  GameObject player;
	private List<GameObject> enemies = new List<GameObject>();

    public NetworkView rpcNet;

    void OnFailedToConnect(NetworkConnectionError error) {
        Debug.Log("Client | Failed to connect : " + error.ToString()); // при ошибке подключения к серверу выводим саму ошибку
    }

    void OnDisconnectedFromServer(NetworkDisconnection info) {
        if (Network.isClient) {
            Debug.Log("Client | Disconnected from server : " + info.ToString()); // при успешном либо неуспешном отключении выводим результат
        } else {
            Debug.Log("Client | Connections closed"); // сообщение выводится при выключении сервера Network.Disconnect()
        }
    }

    void OnConnectedToServer() {
        Debug.Log("Client | Connected to server | rpcNet : " + rpcNet); // сообщение выводится при успешном подключении к серверу

        NetworkView playerNetwork = addNetworkComponent(player);
        playerNetwork.viewID = Network.AllocateViewID(); // присваиваем уникальный идентификатор в сети

        rpcNet.RPC("addPlayer", RPCMode.Server, playerNetwork.viewID);
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
		Debug.Log("Client | RPC > addPlayer | index : " + index);
		GameObject enemy = drawSphere(player.transform.position, Color.yellow);

		NetworkView enemyNetwork = addNetworkComponent(enemy);
		enemyNetwork.viewID = index;

		player.GetComponent<WorldDestroyer>().players.Add(enemy);

		enemies.Add(enemy);
	}

	[RPC]
	void setPlayer(NetworkViewID index) {
		NetworkView playerNetwork = addNetworkComponent(player);
        playerNetwork.viewID = index;
	}

	[RPC]
	void removePlayer(NetworkPlayer player) {
		Network.RemoveRPCs(player); // очищаем список процедур игрока
        Network.DestroyPlayerObjects(player); // уничтожаем все объекты игрока
	}

	// Use this for initialization
	void Start () {
		//enemies = new List<GameObject>();

		Network.Connect(SERVER_URL, NETWORK_PORT);
	}
}
