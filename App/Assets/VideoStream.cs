using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;

public class VideoStream : MonoBehaviour
{
    WebSocket websocket;

    // Taille de l'image
    int imageWidth = 1280;
    int imageHeight = 720;

    // Nombre de canaux de l'image (par exemple, 3 pour RGB)
    int numChannels = 3;

    async void Start()
    {
        websocket = new WebSocket($"ws://217.160.99.153:55055");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection opened");
        };

        websocket.OnMessage += (byte[] data) =>
        {
            // Vérifier si la taille des données reçues correspond à la taille attendue de l'image
            int expectedDataLength = imageWidth * imageHeight * numChannels;
            if (data.Length != expectedDataLength)
            {
                Debug.LogError($"Received data length does not match expected image size: {data.Length} != {expectedDataLength}");
                return;
            }
            Debug.Log("Received data");

            // Créer une texture pour afficher l'image
            Texture2D texture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
            texture.LoadRawTextureData(data);
            texture.Apply();

            // Appliquer la texture à l'élément RawImage
            GetComponent<RawImage>().texture = texture;
        };

        websocket.OnError += (string errorMsg) =>
        {
            Debug.LogError($"WebSocket error: {errorMsg}");
        };

        websocket.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log($"Connection closed with code {code}");
        };

        await websocket.Connect();
    }

    void OnDestroy()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            websocket.Close();
        }
    }
}
