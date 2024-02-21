import requests

# Adresse IP ou nom d'hôte du dispositif distant
remote_host = "192.168.1.100"  # Remplacez par l'adresse IP correcte

# Port utilisé par le serveur web sur le dispositif distant
port = 55055

# Fonction pour envoyer une commande au dispositif distant
def send_command(command):
    url = f"http://{remote_host}:{port}/"
    try:
        response = requests.post(url, data=command)
        if response.status_code == 200:
            print("Commande envoyée avec succès au dispositif distant.")
        else:
            print("Échec de l'envoi de la commande au dispositif distant.")
    except Exception as e:
        print(f"Erreur lors de l'envoi de la commande : {e}")

# Exemples de commandes à envoyer
# Vous pouvez remplacer ces exemples par vos propres commandes
send_command("stepmotor 1 100")  # Commande pour déplacer le moteur pas à pas 1 de 100 pas
send_command("dcmotor 50 75")     # Commande pour contrôler un moteur DC avec les paramètres x=50 et y=75
