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
                cv2.waitKey(1)  # Attendre un peu pour la mise à jour de l'affichage
            except websockets.ConnectionClosedError:
                print("Déconnecté du serveur WebSocket")
                break

asyncio.get_event_loop().run_until_complete(receive_video())
