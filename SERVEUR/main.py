import asyncio
import websockets
import cv2
import numpy as np

# Fonction pour décoder les images JPEG binaires
def decode_image(data):
    nparr = np.frombuffer(data, np.uint8)
    img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
    return img

async def video_stream(websocket, path):
    print("Client connecté")
    while True:
        try:
            # Recevoir les données binaires de l'image depuis le websocket
            data = await websocket.recv()
            # Décoder l'image
            image = decode_image(data)
            # Afficher l'image (vous pouvez choisir de la sauvegarder ou la traiter autrement)
            cv2.imshow("ESP32 Video Stream", image)
            cv2.waitKey(1)  # Attendre un peu pour la mise à jour de l'affichage
        except websockets.ConnectionClosedError:
            print("Client déconnecté")
            break

# Spécifiez l'adresse IP publique de votre serveur et le port sur lequel le serveur WebSocket écoute
server_ip = "217.160.99.153"
server_port = 55055

start_server = websockets.serve(video_stream, server_ip, server_port)

asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()
