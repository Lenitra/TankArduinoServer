import asyncio
import websockets

# Spécifiez l'adresse IP publique de votre serveur et le port sur lequel le serveur WebSocket écoute
server_ip = "217.160.99.153"
server_port = 55055

# Liste pour stocker les websockets connectées
connected_websockets = []

async def video_stream(websocket, path):
    print(f"Client connecté, nombre de clients connectés: {len(connected_websockets)}")
    connected_websockets.append(websocket)  # Ajouter la nouvelle websocket à la liste
    try:
        while True:
            # Recevoir les données binaires de l'image depuis le websocket client
            data = await websocket.recv()
            # Réacheminer les données reçues à tous les autres clients connectés
            for ws in connected_websockets:
                if ws != websocket and ws.open:
                    await ws.send(data)
                    print(f"Réacheminement des données à {connected_websockets.index(ws)} clients")
    except websockets.ConnectionClosedError:
        print("Client déconnecté")
        connected_websockets.remove(websocket)  # Retirer la websocket de la liste si elle se déconnecte

async def server():
    async with websockets.serve(video_stream, server_ip, server_port):
        print(f"Serveur WebSocket démarré sur {server_ip}:{server_port}")
        await asyncio.Future()  # Garde le serveur en cours d'exécution indéfiniment

asyncio.run(server())
