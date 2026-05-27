using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class Item : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] int ItemID;

	public int GetItemID()
	{
		return ItemID;
    }
}
