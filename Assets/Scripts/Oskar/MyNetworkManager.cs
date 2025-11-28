using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
      NetworkPlayer player = conn.identity.GetComponent<NetworkPlayer>();
        player.SetDisplayName($"Player {numPlayers}");
    }
}
