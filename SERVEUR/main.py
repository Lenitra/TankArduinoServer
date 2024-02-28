import asyncio
import websockets

# Spécifiez l'adresse IP publique de votre serveur et le port sur lequel le serveur WebSocket écoute
server_ip = "217.160.99.153"
server_port = 55055

async def video_stream(websocket, path):
    print("Client connecté")
    while True:
        try:
            # Recevoir les données binaires de l'image depuis le websocket client
            data = await websocket.recv()
            # Réacheminer les données reçues à tous les autres clients connectés
            for ws in server.websockets:
                if ws != websocket and ws.open:
                    await ws.send(data)
        except websockets.ConnectionClosedError:
            print("Client déconnecté")
            break

async def server():
    async with websockets.serve(video_stream, server_ip, server_port):
        print(f"Serveur WebSocket démarré sur {server_ip}:{server_port}")
        await asyncio.Future()  # Garde le serveur en cours d'exécution indéfiniment

asyncio.run(server())
