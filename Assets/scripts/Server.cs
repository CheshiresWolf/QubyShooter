using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {
	const int  NETWORK_PORT    = 4585;  // сетевой порт
    const int  MAX_CONNECTIONS = 20;    // максимальное количество входящих подключений
    const bool USE_NAT         = false; // использовать NAT?

    public  GameObject player;
    private GameObject enemy;

    public NetworkView rpcNet;

    void OnServerInitialized() {
    	Debug.Log("Server | initialization complete");
    }

    void OnPlayerConnected(NetworkPlayer newPlayer) {
    	Debug.Log("Server | player connected | rpcNet : " + rpcNet);

    	NetworkView playerNetwork = addNetworkComponent(player);
        playerNetwork.viewID = Network.AllocateViewID(); // присваиваем уникальный идентификатор в сети

        rpcNet.RPC("addPlayer", newPlayer, playerNetwork.viewID);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
    	Debug.Log("Server | player disconnected");
        Network.RemoveRPCs(player); // очищаем список процедур игрока
        Network.DestroyPlayerObjects(player); // уничтожаем все объекты игрока
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
		Debug.Log("Server | RPC > addPlayer | index : " + index);
		enemy = drawSphere(player.transform.position, Color.yellow);

		NetworkView enemyNetwork = addNetworkComponent(enemy);
		enemyNetwork.viewID = index;
		
		player.GetComponent<WorldDestroyer>().players.Add(enemy);
	}

	// Use this for initialization
	void Start () {
        //netView.observed = this;
        //rpcNet = new NetworkView();
        
		Network.InitializeSecurity(); // инициализируем защиту
        Network.InitializeServer(MAX_CONNECTIONS, NETWORK_PORT, USE_NAT);

		//addNetworkComponent(player);
		//addNetworkComponent(enemy);
	}
}
