using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour {
    const string SERVER_URL   = "127.0.0.1"; // адрес сервера
	const int    NETWORK_PORT = 4585;        // сетевой порт

	public GameObject player;
    public GameObject enemy;

    public NetworkView netView;

    void OnFailedToConnect(NetworkConnectionError error) {
        Debug.Log("Failed to connect : " + error.ToString()); // при ошибке подключения к серверу выводим саму ошибку
    }

    void OnDisconnectedFromServer(NetworkDisconnection info) {
        if (Network.isClient) {
            Debug.Log("Disconnected from server : " + info.ToString()); // при успешном либо неуспешном отключении выводим результат
        } else {
            Debug.Log("Connections closed"); // сообщение выводится при выключении сервера Network.Disconnect()
        }
    }

    void OnConnectedToServer() {
        Debug.Log("Connected to server"); // сообщение выводится при успешном подключении к серверу
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
	    } else {
	        // Receiving
	        stream.Serialize(ref syncPosition);
	        enemy.transform.position = syncPosition;
	    }
	    
	}

	// Use this for initialization
	void Start () {
        netView.observed = this;

		Network.Connect(SERVER_URL, NETWORK_PORT);
		
		//addNetworkComponent(player);
		//addNetworkComponent(enemy);
	}
}
