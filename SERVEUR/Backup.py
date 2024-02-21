from flask import Flask, request, render_template
from flask_socketio import SocketIO, emit
import json
import random
import datetime

app = Flask(__name__)
app.config["SECRET_KEY"] = "secret!"
socketio = SocketIO(app)


@app.route("/ctrl")
def index():
    return render_template("controles.html")


@app.route("/")
def test():
    return "Le serveur fonctionne correctement"


# on connect
@socketio.on("connect")
def test_connect():
    print("-------------------")
    print("Client connecté : ")
    # print les informations du client connecté
    print(request.sid)
    print(request.remote_addr)
    print(request.headers)
    print("-------------------")


# endpoint flask
@app.route("/endpoint", methods=["POST", "GET"])
def endpoint():
    # envoie un message à tout les clients connectés
    socketio.emit("message", "data from server")
    return "Okayyyyyy !"


# on disconnect
@socketio.on("disconnect")
def test_disconnect():
    print("Client déconnecté")


@socketio.on("message")
def handle_message(message):
    print("Received message: " + message)
    # Ici, vous pouvez ajouter la logique pour traiter le message reçu
    # Par exemple, vous pourriez vérifier si le message est "avancer" et effectuer une action en conséquence
    if message == "avancer":
        # Envoie un message 'avancer' vers tout les clients connectés
        socketio.emit("avancer", "avancer")


if __name__ == "__main__":

    ip = "217.160.99.153"
    port = 55055

    socketio.run(app, debug=True, host=ip, port=port)
