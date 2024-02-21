import socket

# Adresse IP de l'ESP32
esp32_ip = "192.168.1.45"  # Remplacez par l'adresse IP réelle de votre ESP32

# Port utilisé par le serveur web sur l'ESP32
esp32_port = 44045

def send_command_to_esp32(command):
    try:
        # Création d'une socket TCP/IP
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            # Connexion à l'ESP32
            s.connect((esp32_ip, esp32_port))
            # Envoi de la commande
            s.sendall(command.encode())
            # Attente de la réponse de l'ESP32
            data = s.recv(1024)
            print("Réponse de l'ESP32 :", data.decode())
    except Exception as e:
        print("Erreur lors de la connexion à l'ESP32 :", e)

# Commande à envoyer à l'ESP32
command_to_send = "getip"  # Exemple de commande, ajustez selon vos besoins

# Envoi de la commande à l'ESP32
send_command_to_esp32(command_to_send)
