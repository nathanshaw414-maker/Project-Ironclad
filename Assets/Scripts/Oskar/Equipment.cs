using Mirror;
using UnityEngine;

public class Equipment :NetworkBehaviour
{
    [SyncVar]
    [SerializeField]public GameObject Weapon1;
    [SyncVar]
    [SerializeField]public GameObject Weapon2;




}
