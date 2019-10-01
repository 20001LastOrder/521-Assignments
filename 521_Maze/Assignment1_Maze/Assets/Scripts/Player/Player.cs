using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The class is used to achieve player related behaviour, excluding the movements
 */
public class Player : MonoBehaviour
{
    public GameObject PlayerProjectile;
    public GameObject ProjectileSpawnPoint;
    public bool CanShootProjectile = true;


    private void ResetProjectile(Projectile projectile)
    {
        CanShootProjectile = true;
    }
    // Update is called once per frame
    void Update()
    {

        //Only allow Projectile Outside the maze
        if(!(GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.OutsideMaze))
        {
            return;
        }

        // if press the left mouse, and there is still a projectile, then shoot it.
        if(Input.GetKeyDown(KeyCode.Mouse0) && CanShootProjectile)
        {
            var proj = Instantiate(PlayerProjectile, ProjectileSpawnPoint.transform.position, ProjectileSpawnPoint.transform.rotation);
            proj.GetComponent<Projectile>().OnDestroy += ResetProjectile;
            CanShootProjectile = false;

        }
    }
}
