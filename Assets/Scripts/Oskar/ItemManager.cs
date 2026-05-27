using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class ItemManager : NetworkBehaviour
{
	[SerializeField] GameObject[] WeaponSpawnList;

    private void Awake()
    {
        NetworkManager networkManager = FindFirstObjectByType<NetworkManager>();
        foreach (GameObject weapon in WeaponSpawnList)
        {
            networkManager.spawnPrefabs.Add(weapon);
        }
    }
    public GameObject getWeaponByID(int index)
	{
		return WeaponSpawnList[index];
    }
}
