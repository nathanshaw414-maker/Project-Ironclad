using Mirror;
using TMPro;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(DisplaynameUpdated))]
    [SerializeField] string displayName = "Nameless";
    [SerializeField] TMP_Text txtDisplayName;

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }


    private void DisplaynameUpdated(string oldName, string newName)
    {
        txtDisplayName.text = newName;
    }
}
