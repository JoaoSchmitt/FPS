using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FPSGunController : MonoBehaviour
{
    public FPSGun[] inventory;
    public FPSGun currentWeapon;
    private FPSAnimations animations;
    public GameObject cameraContainer;
    public string wallTag, floorTag, enemyTag, waterTag;
    public GameObject wallHole, floorHole;
    public GameObject wallImpact, floorImpact, waterImpact, enemyImpact;
    public float test;
    [HideInInspector]
    public bool reloading;

    private float cooldownCount;

    void Start()
    {
        animations = GetComponent<FPSAnimations>();

        foreach (FPSGun gun in inventory)
        {
            gun.model.SetActive(false);
        }

        currentWeapon.model.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.Length > 0)
            StartCoroutine("ChangeGun", 0);

        if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.Length > 1)
            StartCoroutine("ChangeGun", 1);


        if (!reloading)
        {
            if (Input.GetKeyDown(KeyCode.R))
                Reload();

            if (currentWeapon.gunType == Enums.GunType.Automatic)
                FireAutomatic();
            else
                FireSemiAutomatic();
        }
    }

    void FireAutomatic()
    {
        animations.ToFireAutomatic(Input.GetMouseButton(0) && currentWeapon.ammunition != 0 && !reloading);

        if (Input.GetMouseButton(0) && currentWeapon.ammunition != 0 && !reloading)
        {
            if (!currentWeapon.fireSound.isPlaying)
            {
                currentWeapon.fireSound.Play();
                currentWeapon.fireSound.volume = 1;
            }
        }

        if (!Input.GetMouseButton(0) || currentWeapon.ammunition == 0)
        { 
            currentWeapon.fireSound.volume = Mathf.Clamp(currentWeapon.fireSound.volume -= test * Time.deltaTime, 0, 1);

            if (currentWeapon.fireSound.volume == 0)
                currentWeapon.fireSound.Stop();
        }

        if (Input.GetMouseButton(0) && cooldownCount <= Time.time)
        {
            cooldownCount = Time.time + currentWeapon.cooldown;

            if (currentWeapon.ammunition == 0)
            {
                currentWeapon.fireSound.volume = 0;
                Reload();
                return;
            }
            else
            {
                currentWeapon.fireSound.volume = 1;
            }

            currentWeapon.ammunition = Mathf.Clamp(currentWeapon.ammunition -= 1, 0, currentWeapon.maximumAmmo);

            Shoot();
        }
    }

    void FireSemiAutomatic()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(currentWeapon.ammunition == 0)
            {
                Reload();
                return;
            }

            currentWeapon.ammunition = Mathf.Clamp(currentWeapon.ammunition -= 1, 0, currentWeapon.maximumAmmo);

            animations.ToFire();
            cameraContainer.transform.position = cameraContainer.transform.position - (cameraContainer.transform.forward * currentWeapon.recoilForce);

            currentWeapon.fireSound.Play();

            Shoot();
        }      
    }

    void Shoot()
    {
        RaycastHit hit;
        float rayDistance = 200;

        var ray = Camera.main.ScreenPointToRay(new Vector2(
                Screen.width / 2,
                Screen.height / 2
            ));

        var collided = Physics.Raycast(ray, out hit, rayDistance);
        Instantiate(currentWeapon.muzzeEffect, currentWeapon.firePoint);
        if (collided)
        {
            if (hit.transform.tag == enemyTag)
            {
                Instantiate(enemyImpact, hit.point, enemyImpact.transform.rotation);
                //implementar dano
            }
            else if (hit.transform.tag == wallTag)
            {
                Instantiate(wallImpact, hit.point, wallImpact.transform.rotation);
                Instantiate(wallHole, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.tag == floorTag)
            {
                Instantiate(floorImpact, hit.point, floorImpact.transform.rotation);
                Instantiate(floorHole, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.tag == waterTag)
            {
                Instantiate(waterImpact, hit.point, waterImpact.transform.rotation);
            }
        }
    }

    void Reload()
    {
        if (currentWeapon.ammunition == currentWeapon.maximumAmmo)
            return;

        if (currentWeapon.ammunitionToReload == 0)
        {
            currentWeapon.noBulletsSound.Play();
            return;
        }

        reloading = true;

        animations.ToReload();
        currentWeapon.fireSound.Stop();
        currentWeapon.reloadSound.Play();

        var difBullets = currentWeapon.maximumAmmo - currentWeapon.ammunition;
        
        if (difBullets > currentWeapon.ammunitionToReload)
            currentWeapon.ammunition += currentWeapon.ammunitionToReload;
        else
            currentWeapon.ammunition += difBullets;

        currentWeapon.ammunitionToReload = Mathf.Clamp(currentWeapon.ammunitionToReload -= difBullets, 0, currentWeapon.ammunitionToReload);
    }

    IEnumerator ChangeGun(int index)
    {
        currentWeapon.animator.SetTrigger("change");

        yield return new WaitForSeconds(0.3f);
        currentWeapon.model.SetActive(false);

        yield return new WaitForSeconds(0.1f);
        currentWeapon = this.inventory[index];

        currentWeapon.model.SetActive(true);
        currentWeapon.animator.Play("get");
    }

}

