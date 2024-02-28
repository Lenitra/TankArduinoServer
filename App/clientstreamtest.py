import asyncio
import websockets
import cv2
import numpy as np

async def receive_video():
    uri = "ws://217.160.99.153:55055"  # Adresse IP publique de votre serveur et le port du serveur WebSocket
    async with websockets.connect(uri) as websocket:
        print("Connecté au serveur WebSocket")
        while True:
            try:
                # Recevoir les données binaires de l'image depuis le websocket
                data = await websocket.recv()
                # Décoder l'image
                nparr = np.frombuffer(data, np.uint8)
                img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
                # Afficher l'image
                cv2.imshow("Image reçue", img)
                cv2.waitKey(33)  # Définir la pause pour obtenir environ 30 FPS
            except websockets.ConnectionClosedError:
                print("Déconnecté du serveur WebSocket")
                break

asyncio.run(receive_video())
