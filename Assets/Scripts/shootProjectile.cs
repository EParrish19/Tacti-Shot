using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEditor;
using UnityEngine.UI;

public class shootProjectile : MonoBehaviour
{
    //Is a target in sight of the entity
    bool targetSighted = false;

    private GameObject thisEntity;
    private GameObject target;

    public unitWeapon autoRifle = new unitWeapon("Auto Rifle", "fullAuto" ,10.0f, 0.2f, 0.3f);
    public unitWeapon rifle = new unitWeapon("Semi-Auto Rifle", "semiAuto" ,33.0f, 0.7f, 0.1f);
    public unitWeapon burstRifle = new unitWeapon("Burst Rifle", "burst" ,20.0f, 0.5f, 0.2f, 3);

    private LogMaker logger;

    [SerializeField]
    private AudioSource shotAudio;

    public unitWeapon[] unitWeapons = new unitWeapon[3];

    private unitWeapon currentWeapon;

    [SerializeField]
    private Text weaponIndicator;

    private int currentWeaponIndex = 0;

    private float timer;

    private LineRenderer shotTracer;

    //Class and constructor for different unit weapons
public class unitWeapon
    {
        public string weaponName;
        public float weaponDamage;
        public float fireTime;
        public float accuracyRange;
        public int burstLength;
        public string tag;

        //if burst length specified
        public unitWeapon(string newWeaponName, string newTag ,float newWeaponDamage, float newWeaponfireTime, float newWeaponAccuracyRange, int newWeaponBurstLength)
        {
            weaponName = newWeaponName;
            weaponDamage = newWeaponDamage;
            fireTime = newWeaponfireTime;
            accuracyRange = newWeaponAccuracyRange;
            burstLength = newWeaponBurstLength;
            tag = newTag;
        }

        //if weapon does not use burst fire
        public unitWeapon(string newWeaponName, string newTag ,float newWeaponDamage, float newWeaponFireTime, float newWeaponAccuracyRange)
        {
            weaponName = newWeaponName;
            weaponDamage = newWeaponDamage;
            fireTime = newWeaponFireTime;
            accuracyRange = newWeaponAccuracyRange;
            burstLength = 1;
            tag = newTag;
        }
    }

    //on start, stores the current gameobject as a variable, initialises other variables
    private void Start()
    {
        thisEntity = gameObject;

        unitWeapons[0] = autoRifle;
        unitWeapons[1] = rifle;
        unitWeapons[2] = burstRifle;

        currentWeapon = unitWeapons[0];

        timer = currentWeapon.fireTime;

        weaponIndicator = GameObject.Find("UIElements").GetComponentInChildren<Text>();

        weaponIndicator.text = "Current Weapon: " + currentWeapon.weaponName;

        shotTracer = gameObject.GetComponentInChildren<LineRenderer>();

        shotTracer.startColor = Color.white;

        shotAudio = Camera.main.GetComponent<AudioSource>();

        logger = GameObject.Find("LogMaker").GetComponent<LogMaker>();

    }

    //changes sighted enemy
    public void setTargetSighted(bool targetInSight)
    {
        targetSighted = targetInSight;
    }

    //sets new target
    public void setTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    //cycles weapon to the next weapon, returns to first weapon if last weapon is swapped
    public void nextWeapon()
    {
        if (currentWeaponIndex == unitWeapons.Length - 1)
        {
            currentWeaponIndex = 0;
            currentWeapon = unitWeapons[currentWeaponIndex];
            weaponIndicator.text = "Current Weapon: " + currentWeapon.weaponName;
            Debug.Log("Swapping weapon to " + currentWeapon.weaponName);
        }
        else
        {
            currentWeaponIndex += 1;
            currentWeapon = unitWeapons[currentWeaponIndex];
            weaponIndicator.text = "Current Weapon: " + currentWeapon.weaponName;
            Debug.Log("Swapping weapon to " + currentWeapon.weaponName);
        }
    }

    //cycles weapon to the previous weapon, goes to last weapon if first weapon is swapped
    public void previousWeapon()
    {
        if(currentWeaponIndex == 0)
        {
            currentWeaponIndex = unitWeapons.Length - 1;
            currentWeapon = unitWeapons[currentWeaponIndex];
        }
        else
        {
            currentWeaponIndex -= 1;
            currentWeapon = unitWeapons[currentWeaponIndex];
        }
    }


    //shoots a shot at the current target
    public void Shoot()
    {
        //seperate ray for the actual projectile
        Vector3 thisEntityPosition = thisEntity.transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 direction = (targetPosition - thisEntityPosition).normalized;

        //if the player entity has a height advantage on the target
        if(thisEntityPosition.y > targetPosition.y)
        {
            float advantagedAccuracyRange = currentWeapon.accuracyRange / 2;

            direction += thisEntity.transform.TransformDirection(new Vector3(
                Random.Range(-advantagedAccuracyRange, advantagedAccuracyRange), 
                Random.Range(0.0f, advantagedAccuracyRange), 
                Random.Range(-advantagedAccuracyRange, advantagedAccuracyRange)));

        //if the target has a height advantage on the player
        }else if(thisEntityPosition.y < targetPosition.y)
        {
            float disadvantagedAccuracyRange = currentWeapon.accuracyRange * 2;

            direction += thisEntity.transform.TransformDirection(new Vector3(
                Random.Range(-disadvantagedAccuracyRange, disadvantagedAccuracyRange), 
                Random.Range(0.0f, disadvantagedAccuracyRange), 
                Random.Range(-disadvantagedAccuracyRange, disadvantagedAccuracyRange)));
        }

        //if the player and the enemy are at the same height
        else
        {
            direction += thisEntity.transform.TransformDirection(new Vector3(
                Random.Range(-currentWeapon.accuracyRange, currentWeapon.accuracyRange), 
                Random.Range(0.0f, currentWeapon.accuracyRange), 
                Random.Range(-currentWeapon.accuracyRange, currentWeapon.accuracyRange)));
        }

        //direction = direction.normalized;*/

        /*float xSpread;
        float ySpread;
        float zSpread;
        float coneSize;

        if(thisEntityPosition.y > targetPosition.y)
        {
            float advantagedAccuracyRange = currentWeapon.accuracyRange / 2;

            coneSize = advantagedAccuracyRange;

            xSpread = Random.Range(-advantagedAccuracyRange, advantagedAccuracyRange);

            ySpread = Random.Range(-advantagedAccuracyRange, advantagedAccuracyRange);

            zSpread = Random.Range(-advantagedAccuracyRange, advantagedAccuracyRange);

        }else if(thisEntityPosition.y < targetPosition.y)
        {
            float disadvantagedAccuracyRange = currentWeapon.accuracyRange * 2;

            coneSize = disadvantagedAccuracyRange;

            xSpread = Random.Range(-disadvantagedAccuracyRange, disadvantagedAccuracyRange);

            ySpread = Random.Range(-disadvantagedAccuracyRange, disadvantagedAccuracyRange);

            zSpread = Random.Range(-disadvantagedAccuracyRange, disadvantagedAccuracyRange);
        }
        else
        {
            coneSize = currentWeapon.accuracyRange;

            xSpread = Random.Range(-currentWeapon.accuracyRange, currentWeapon.accuracyRange);

            ySpread = Random.Range(-currentWeapon.accuracyRange, currentWeapon.accuracyRange);

            zSpread = Random.Range(-currentWeapon.accuracyRange, currentWeapon.accuracyRange);
        }

        Vector3 spread = new Vector3(xSpread, ySpread, zSpread).normalized * coneSize;

        Vector3 shotDirection = ((Quaternion.Euler(spread) * thisEntity.transform.rotation).eulerAngles).normalized;

        Ray projectile = new Ray(thisEntityPosition, shotDirection);*/
        //Ray projectile = new Ray(thisEntityPosition, direction);

        /*Vector3 fwd = thisEntity.transform.right;
        
        if(thisEntityPosition.y > targetPosition.y)
        {
            float advantagedAccuracyRange = currentWeapon.accuracyRange / 2;

            fwd += thisEntity.transform.TransformDirection(new Vector3(
                Random.Range(-advantagedAccuracyRange, advantagedAccuracyRange), Random.Range(0.0f, advantagedAccuracyRange), Random.Range(-advantagedAccuracyRange, advantagedAccuracyRange)));

        }else if(thisEntityPosition.y < targetPosition.y)
        {
            float disadvantagedAccuracyRange = currentWeapon.accuracyRange * 2;

            fwd += thisEntity.transform.TransformDirection(new Vector3(
                Random.Range(-disadvantagedAccuracyRange, disadvantagedAccuracyRange), Random.Range(0.0f, disadvantagedAccuracyRange), Random.Range(-disadvantagedAccuracyRange, disadvantagedAccuracyRange)));

        }
        else
        {
            fwd += thisEntity.transform.TransformDirection(new Vector3(
                Random.Range(-currentWeapon.accuracyRange, currentWeapon.accuracyRange), Random.Range(0.0f, currentWeapon.accuracyRange), Random.Range(-currentWeapon.accuracyRange, currentWeapon.accuracyRange)));
        }

        Ray projectile = new Ray(thisEntityPosition, fwd);*/

        //create a ray using the player unit's position and the modified direction
        Ray projectile = new Ray(thisEntityPosition, direction);

        //only needs to run if the target is in sight
        if (targetSighted == true)
        {
            RaycastHit targetHit;
            

            //if projectile hits something, check if it's the target, if it is, damage the target and update logmaker, else miss the target and update logmaker
            if (Physics.Raycast(projectile,out targetHit))
            {
                if (targetHit.transform.gameObject.name != null && targetHit.transform.gameObject.name == target.name)
                {
                    Debug.Log("Target hit");
                    Debug.DrawRay(thisEntityPosition, projectile.direction * 10, Color.yellow, 10f);
                    shotTracer.endColor = Color.yellow;

                    shotTracer.SetPosition(0, gameObject.transform.position);
                    shotTracer.SetPosition(1, targetHit.point);
                    Invoke("resetTracer", currentWeapon.fireTime);

                    logger.addHit(currentWeapon.tag);
                    

                    targetHit.transform.gameObject.SendMessage("takeDamage", currentWeapon.weaponDamage);
                }
                else
                {
                    Debug.Log("Target Missed");
                    Debug.DrawRay(thisEntityPosition, projectile.direction * 10, Color.blue, 10f);
                    shotTracer.endColor = Color.blue;

                    shotTracer.SetPosition(0, gameObject.transform.position);
                    shotTracer.SetPosition(1, targetHit.point);
                    Invoke("resetTracer", currentWeapon.fireTime);

                    logger.addMiss(currentWeapon.tag);
                }


            }
            else
            {
                Debug.Log("Target Missed");
                Debug.DrawRay(thisEntityPosition, projectile.direction * 10, Color.blue, 10f);
                shotTracer.endColor = Color.blue;

                shotTracer.SetPosition(0, gameObject.transform.position);
                shotTracer.SetPosition(1, thisEntityPosition + (projectile.direction * 100));
                Invoke("resetTracer", currentWeapon.fireTime);

                logger.addMiss(currentWeapon.tag);
            }


        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //while the target is sighted, fire current weapon at target
        if (targetSighted)
        {
            
            //all weapons have a time between shots/bursts
            if(timer <= 0.0f)
            {
                //used for burst weapons, nmon-burst only goes once
                for (int i = 0; i <= currentWeapon.burstLength; i++)
                {
                    Invoke("Shoot", 0.2f);
                    shotAudio.Play();
                }

                //reset timer
                timer = currentWeapon.fireTime;
            }
            else
            {
                //if the fire timer isn't at 0, decrease it
                timer -= Time.deltaTime;
            }
        }

           
    }

    //reset linerenderer position
    void resetTracer()
    {
        shotTracer.SetPosition(1, gameObject.transform.position);
    }

    //resets target sighted variable to stop shooting
    void resetTarget()
    {
        targetSighted = false;
    }
}



