using Mirror;
using UnityEngine;

public class Attack : NetworkBehaviour
{
    [Header("Attacking")]
    [SerializeField] float attackRange = 2f;
    [SerializeField]Camera mainCamera;

    public void TargetRaycast()
    {



        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, attackRange))
        {
            //GameObject hitObject = hit.transform.parent.gameObject;
           // Debug.Log(hitObject);
            //Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * attackRange, Color.red, 2f);
            //Debug.Log("Hit: " + hit.transform.gameObject);

            if (hit.transform.root.gameObject.tag == "Player")
            {
               
                CmdDealDamage(hit.transform.root.gameObject);
            }
          

        }
    }

    [Command]

 


    void CmdDealDamage(GameObject target)
    {
        Health health = target.GetComponent<Health>();
        
        health.takeDamage(20);
        
    }
 
}
