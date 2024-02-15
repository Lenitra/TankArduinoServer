using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NativeWebSocket;
using System.Linq;


// Classe permettant de configuer les boutons du menu
public class Manager : MonoBehaviour
{
    

    private WebSocket websocket;

    public GameObject childPrefab;
    public GameObject FORMPanel;
    public GameObject FORMLockPanel;
    public Text FORMtextCmd;
    public Text FORMtextTempo;
    public Button FORMbtn;

    public float lastScreenWidth = 0f;
    public Dictionary<string, int>[] activeCmds = new Dictionary<string, int>[10];
    public float[] cooldowns = new float[10];

    
    private void InstantiateBtns()
    {
        // instaciate 10 children
        for (int i = 0; i < 10; i++)
        {
            GameObject child = Instantiate(childPrefab, gameObject.transform);
            child.name = "Panel " + (i+1);
        }
    }
    
    private void setBtns()
    {
        // get the x and y size of the canvas
        float x = gameObject.GetComponent<RectTransform>().rect.width;
        float y = gameObject.GetComponent<RectTransform>().rect.height;

        // for each child of the canvas
        int count = 0;
        foreach (Transform child in gameObject.transform)
        {
            // if child.name don't start with "Panel" then continue
            if (!child.name.StartsWith("Panel"))
            {
                continue;
            }
            count++;

            // SIZE
            child.GetComponent<RectTransform>().sizeDelta = new Vector2(x / 5, y / 2.1f);
            
            // POSITION
            child.GetComponent<RectTransform>().localPosition = new Vector3((count - 3) * x / 5, y / 4, 0);
            if (count > 5)
            {
                child.GetComponent<RectTransform>().localPosition = new Vector3((count - 8) * x / 5, -y / 4, 0);
            }

            // LISTENERS & TEXT
            GameObject btn = child.transform.GetChild(0).gameObject;
            GameObject del = child.transform.GetChild(1).gameObject;
            GameObject txt = child.transform.GetChild(0).transform.GetChild(0).gameObject;
            int tmp = count;
            
            // delete the listeners on btn
            btn.GetComponent<Button>().onClick.RemoveAllListeners();

            if (PlayerPrefs.GetString("cmd" + count) != "" && PlayerPrefs.GetString("cmd" + count) != null && PlayerPrefs.GetInt("tempo" + count) != null)
            {
                // get the first child of the child
                txt.GetComponent<Text>().text = PlayerPrefs.GetString("cmd" + count) + "\n" + PlayerPrefs.GetInt("tempo" + count);
                // add a listener to the del
                del.GetComponent<Button>().onClick.AddListener(delegate { DeleteCmd(tmp); });
                // add a listener to the button while is clicked
                btn.GetComponent<Button>().onClick.AddListener(delegate { toggleCmd(tmp); });
            } else
            {
                txt.GetComponent<Text>().text = "Ajouter une commande";
                // add a listener to the del
                del.GetComponent<Button>().onClick.AddListener(delegate { DeleteCmd(tmp); });
                // add a listener if the button is clicked
                btn.GetComponent<Button>().onClick.AddListener(delegate { AddCmd(tmp); });
            }
        }
        // put the FORMPanel at the end of the list
        FORMLockPanel.transform.SetAsLastSibling();
        FORMPanel.transform.SetAsLastSibling();
    }


    public void debugDict()
    {
        for (int i = 0; i < activeCmds.Length; i++)
        {
            if (activeCmds[i] != null)
            {
                foreach (var cmdEntry in activeCmds[i])
                {
                    string cmd = cmdEntry.Key;
                    int tempo = cmdEntry.Value;
                    Debug.Log($"cmd: {cmd}, tempo: {tempo}");
                }
            }
        }
    }


    public void toggleCmd(int count)
    {
        // if the tempo is 0 then the command is not active
        if (PlayerPrefs.GetInt("tempo" + count) == 0)
        {
            SendWebSocketMessage(PlayerPrefs.GetString("cmd" + count));
            return;
        }

        GameObject child = gameObject.transform.Find("Panel " + count).gameObject.transform.GetChild(0).gameObject;
        if (child.GetComponent<Image>().color == new Color(1,1,1,1))
        {
            child.GetComponent<Image>().color = new Color(0, 1, 0, 1);
            activeCmds[count] = new Dictionary<string, int> { { PlayerPrefs.GetString("cmd" + count), PlayerPrefs.GetInt("tempo" + count) } };
            cooldowns[count] = 0;

        } else
        {
            child.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            activeCmds[count] = new Dictionary<string, int> { { "", 0 } };
        }
        debugDict();
    }

    // AddCmd
    public void AddCmd(int count)
    {
        FORMPanel.SetActive(true);
        FORMLockPanel.SetActive(true);
        FORMbtn.onClick.AddListener(delegate { AddCmdForm(count); });
    }


    public void AddCmdForm(int count)
    {
        PlayerPrefs.SetString("cmd" + count, FORMtextCmd.text);
        PlayerPrefs.SetInt("tempo" + count, int.Parse(FORMtextTempo.text));
        PlayerPrefs.Save();
        setBtns();
        FORMPanel.SetActive(false);
        FORMLockPanel.SetActive(false);
        // delete the listeners
        FORMbtn.onClick.RemoveAllListeners();
    }


    // DeleteCmd
    public void DeleteCmd(int count)
    {
        cooldowns[count] = -1;
        activeCmds[count] = new Dictionary<string, int> { { "", 0 } };
        GameObject child = gameObject.transform.Find("Panel " + count).gameObject.transform.GetChild(0).gameObject;
        child.GetComponent<Image>().color = new Color(1, 1, 1, 1);

        PlayerPrefs.SetString("cmd" + count, "");
        PlayerPrefs.SetInt("tempo" + count, -1);
        PlayerPrefs.Save();
        setBtns();
    }


    public void closeForm()
    {
        FORMPanel.SetActive(false);
        FORMLockPanel.SetActive(false);
        FORMbtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        // remplir les activeCmds et cooldowns
        for (int i = 0; i < 10; i++)
        {
            activeCmds[i] = new Dictionary<string, int> { { "", 0 } };
            cooldowns[i] = -1;
        }
        InstantiateBtns();
        setBtns();
        websocket = new WebSocket("ws://217.160.99.153:55055");

        // Abonnement aux événements
        websocket.OnOpen += OnWebSocketOpen;
        websocket.OnError += OnWebSocketError;
        websocket.OnClose += OnWebSocketClose;
        websocket.OnMessage += OnWebSocketMessage;

        // Connexion au serveur WebSocket
        websocket.Connect();
    }
 


void Update()
{
    #if !UNITY_WEBGL || UNITY_EDITOR
    websocket.DispatchMessageQueue();
    #endif
    if (lastScreenWidth != Screen.width)
    {
        lastScreenWidth = Screen.width;
        setBtns();
    }

    for (int i = 0; i < cooldowns.Length; i++)
    {
        if (cooldowns[i] > 0)
        {
            cooldowns[i] -= Time.deltaTime*1000;
        }
        else if (cooldowns[i] <= 0 && activeCmds[i] != null && activeCmds[i].Count > 0)
        {
            foreach (var cmdEntry in activeCmds[i])
            {
                string cmd = cmdEntry.Key;
                int tempo = cmdEntry.Value;

                if (!string.IsNullOrEmpty(cmd) && tempo > 0)
                {
                    Debug.Log($"Sending command: {cmd} to the server.");
                    SendWebSocketMessage(cmd);
                    cooldowns[i] = tempo;
                    break;
                }
            }
        }
    }
}

public async void SendWebSocketMessage(string message)
{
    if (websocket.State == WebSocketState.Open)
    {
        Debug.Log($"WebSocket is open. Sending message: {message}");
        await websocket.SendText(message);
    }
    else
    {
        Debug.LogError($"WebSocket is not open. Current state: {websocket.State}");
    }
}



    private void OnWebSocketOpen()
    {
        Debug.Log("Connexion WebSocket établie.");
    }

    private void OnWebSocketError(string errorMsg)
    {
        Debug.LogError("Erreur WebSocket : " + errorMsg);
    }

    private void OnWebSocketClose(WebSocketCloseCode closeCode)
    {
        Debug.Log("Connexion WebSocket fermée avec le code : " + closeCode);
    }

    private void OnWebSocketMessage(byte[] message)
    {
        // Convertir le message en string et le logger
        // var messageString = System.Text.Encoding.UTF8.GetString(message);
        // Debug.Log("Message reçu du serveur : " + messageString);
    }

    private async void OnApplicationQuit()
    {
        // Fermeture de la connexion WebSocket lors de la fermeture de l'application
        await websocket.Close();
    }


}

    


