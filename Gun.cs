using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public enum FireMode {Auto};
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetWeenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;
    public int projectilesPerMag;
    public float reloadTime = .3f;
    public AudioSource audioSource1;
    public AudioSource audioSource2;

    Text shellCount;


    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.5f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    [Header("Audio")]
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    
    MuzzleFlash muzzleFlash;
    float nextShootTime;

    int shootRemainingInBurst;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    
    void Start()
    {
        this.shellCount = GameObject.Find("ShellText").GetComponent<Text>();
        muzzleFlash = GetComponent<MuzzleFlash>();
        audioSource1 = GetComponent <AudioSource> ();
        audioSource2 = GetComponent <AudioSource> ();
        shootRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
        
        shellCount.text = projectilesRemainingInMag + "/" + "40";
    }

    void LateUpdate()
    {
        //animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        shellCount.text = projectilesRemainingInMag + "/" + "40";

        if (!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if (!isReloading && Time.time > nextShootTime && projectilesRemainingInMag > 0)
        {
            for (int i = 0; i < projectileSpawn.Length; i ++)
            {
                if (projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag --;
                nextShootTime = Time.time + msBetWeenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            muzzleFlash.Activate();
            audioSource1.PlayOneShot (shootAudio);
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
            audioSource2.PlayOneShot (reloadAudio);
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
    }

    public void OnTriggerRelease()
    {
        shootRemainingInBurst = burstCount;
    }
}
