using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProjectileTrigger : MonoBehaviour
{
    public GameObject projectile;
    public GameObject altProjectile;
    public Transform projectilePos;
    public float fireRate;
    public bool canFire;
    public int altAmmo;
    // Update is called once per frame

    private void Start()
    {
        //do not restrict firing
        canFire = true;
    }
    private void Update()
    {
        //single fire on left click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Invoke("Fire", 0f);
        }
        //rapid fire on hold left click
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Invoke("Fire", 0.5f);
        }
        else
        {
            CancelInvoke("Fire");
        }

        // alt fire on right click
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (altAmmo >= 1)
            {
                Invoke("AltFire", 0f);
            }
        }
    }
    //spawn projectile, motion is handled in projectile script
    void Fire()
    {
        if (canFire == true)
        {
            Instantiate(projectile, projectilePos.position, projectilePos.rotation);

            StartCoroutine(CanFire());            
        }
    }
    void AltFire()
    {
        Instantiate(altProjectile, projectilePos.position, projectilePos.rotation);
        altAmmo -= 1;
    }
    //weapon cooldown
    private IEnumerator CanFire()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
