using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {
	const int  NETWORK_PORT    = 4585;  // сетевой порт
    const int  MAX_CONNECTIONS = 20;    // максимальное количество входящих подключений
    const bool USE_NAT         = false; // использовать NAT?

    public GameObject player;
    public GameObject enemy;

    public NetworkView netView;

    void OnServerInitialized() {
    	Debug.Log("Server | initialization complete");
    }

    void OnPlayerConnected(NetworkPlayer player) {
    	Debug.Log("Server | player connected");
        //networkView.RPC( "SpawnPlayer", player, "Player " + playerCount.ToString() ); // вызываем у игрока процедуру создания экземпляра префаба
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
    	Debug.Log("Server | player disconnected");
        Network.RemoveRPCs(player); // очищаем список процедур игрока
        Network.DestroyPlayerObjects(player); // уничтожаем все объекты игрока
    }

	void addNetworkComponent(GameObject target) {
		NetworkView netView = target.AddComponent(typeof(NetworkView)) as NetworkView; // добавляем компонент NetworkView нашему игровому объекту
        netView.viewID = Network.AllocateViewID(); // присваиваем уникальный идентификатор в сети
        netView.observed = this; // указываем этот скрипт (компонент) для синхронизации
        netView.stateSynchronization = NetworkStateSynchronization.Unreliable; // нам подходит способ быстрой передачи с потерями, поскольку наше передвижение интерполируется
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 syncPosition = Vector3.zero; // для синхронизации позиции

	    if (stream.isWriting) {
	        // Sending
	        syncPosition = player.transform.position;
            stream.Serialize(ref syncPosition);

            Debug.Log("Server | OnSerializeNetworkView | write | syncPosition : " + syncPosition);
	    }
	    if (stream.isReading) {
	        // Receiving
	        stream.Serialize(ref syncPosition);
	        enemy.transform.position = syncPosition;

	        Debug.Log("Server | OnSerializeNetworkView | read | syncPosition : " + syncPosition);
	    }
	    
	}

	// Use this for initialization
	void Start () {
        netView.observed = this;
        
		Network.InitializeSecurity(); // инициализируем защиту
        Network.InitializeServer(MAX_CONNECTIONS, NETWORK_PORT, USE_NAT);

		//addNetworkComponent(player);
		//addNetworkComponent(enemy);
	}
}
