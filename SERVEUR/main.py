import asyncio
import websockets
import time

# Stocker toutes les connexions clientes
clients = set()
tanks = set()

async def server(websocket, path):
    # Ajouter le client à la liste des clients connectés
    clients.add(websocket)
    print(f"-- Nouvelle connexion : {websocket.remote_address}")
    try:
        async for message in websocket:
            
            print(f"<< {websocket.remote_address}: {message}")
            
            if message == "Bonjour du ESP32!":
                tanks.add(websocket)
                print(f"-- Nouveau tank : {websocket.remote_address}")
            
            # Envoie un message à tout les tanks connectés
            # cmd : tanks <str:cmd>
            elif message.startswith("tanks"):
                message = message[6:]
                for tank in tanks:
                    await tank.send(message)
                    print(f">> {tank.remote_address}: {message}")

            # Envoie un message à un tank spécifique
            # cmd : tank<int:id> <str:cmd>
            elif message.startswith("tank"):
                message = message[5:] 
                id = int(message.split(" ")[0])
                await tanks.send(message)
                print(f">> {tanks[id].remote_address}: {message}")

            # Envoie un message à tout les clients connectés
            # cmd : all <str:cmd>
            elif message.startswith("all"):
                message = message[4:]
                for client in clients:
                    await client.send(message)
                    print(f">> {client.remote_address}: {message}")

            # Afficher les infos du réseau et les renvoie au client
            # cmd : infos
            elif message == "infos":
                msg = f"-- clients: {len(clients)}\n-- tanks: {len(tanks)}"
                print(msg)
                msg.replace("\n", ", ")
                msg.replace("-- ", "")
                await websocket.send(msg)

            # Effacer la console
            # cmd : clear
            elif message == "clear":
                for _ in range(1000):
                    print()
                

    except websockets.exceptions.ConnectionClosedError:
        pass
    finally:
        # Supprimer le client de la liste des clients connectés
        clients.remove(websocket)
        try:                    
            tanks.remove(websocket)
        except:
            pass
        print(f"-- Déconnexion : {websocket.remote_address}")


# Lancer le serveur WebSocket
start_server = websockets.serve(server, "217.160.99.153", 44045)
print("Serveur WebSocket lancé")
asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()
