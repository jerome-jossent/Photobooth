using UnityEngine;
using UnityEngine.Events;

public class SerialReadWritePanel : MonoBehaviour
{
    [SerializeField] Arduino arduino;
    [SerializeField] TMPro.TMP_InputField if_txtToSend;

    [SerializeField] GameObject messagesReceiver_Go_Parent;
    [SerializeField] GameObject messagesSended_Go_Parent;
    [SerializeField] int messages_max = 50;
    [SerializeField] GameObject texte_prefab;

    public UnityEvent<Message> onNewMessage;

    void Start()
    {
        if_txtToSend.onEndEdit.AddListener(delegate { _SendByUI(); });
        _SendedClear();
        _ReceivedClear();
    }

    public void _Send(string message)
    {
        Message m = new Message(message);
        _Send(m);
    }
    public void _Send(Message message)
    {
        arduino.SendMessage(message.texte);
        UpdateMessagesSended(message);
    }

    public void _SendByUI()
    {
        _Send(if_txtToSend.text);
        if_txtToSend.text = "";
    }

    private void Update()
    {
        if (arduino == null)
            return;
        if (arduino._messages_Count == 0)
            return;

        Message m = arduino._messagePop();
        onNewMessage?.Invoke(m);
        UpdateMessagesReceived(m, false);
    }

    public void _ReceivedClear()
    {
        ClearAllChildren._ClearAllChildren(messagesReceiver_Go_Parent);
    }

    public void _SendedClear()
    {
        ClearAllChildren._ClearAllChildren(messagesSended_Go_Parent);
    }

    void UpdateMessagesReceived(Message message, bool warning)
    {
        ClearAllChildren._ClearAllChildren(messagesReceiver_Go_Parent, messages_max);
        GameObject go_txt = Instantiate(texte_prefab, messagesReceiver_Go_Parent.transform, true);
        TMPro.TextMeshProUGUI txt = go_txt.GetComponent<TMPro.TextMeshProUGUI>();
        txt.text = message.ToString();
        if (warning)
            txt.color = Color.red;
        //Debug.Log("warning");
        Debug.Log(txt.text);
    }

    void UpdateMessagesSended(Message message)
    {
        ClearAllChildren._ClearAllChildren(messagesSended_Go_Parent, messages_max);
        GameObject go_txt = Instantiate(texte_prefab, messagesSended_Go_Parent.transform, true);
        go_txt.GetComponent<TMPro.TextMeshProUGUI>().text = message.ToString();
    }
}