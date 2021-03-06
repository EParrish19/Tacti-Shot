using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sight : MonoBehaviour
{
    bool targetSighted = false;

    private GameObject thisEntity;

    private shootProjectile projectileShoot;

    [SerializeField]
    private GameObject target;

    //on start, stores the current gameobject as a variable
    void Start()
    {
        thisEntity = gameObject;
        projectileShoot = gameObject.GetComponent<shootProjectile>();
    }

    //sets new sight target
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {

            //RaycastHit used to get the target name when 
            RaycastHit targetInSight;



            //ray used as an indicator of if the target can be seen
            Vector3 thisEntityPosition = thisEntity.transform.position;
            Vector3 targetPosition = target.transform.position;
            Vector3 direction = (targetPosition - thisEntityPosition).normalized;


            Ray sight = new Ray(thisEntityPosition, direction);
            Debug.DrawRay(thisEntityPosition, direction * 10, Color.red);

            //does a raycast to stored target
            if (Physics.Raycast(sight, out targetInSight, 100000))
            {

                //if the raycast hits the right target, print to console the event
                if (targetInSight.transform.gameObject.name != null && targetInSight.transform.gameObject.name == target.name)
                {
                    Debug.Log(targetInSight.transform.gameObject.name + " sighted by: " + thisEntity.name);
                    thisEntity.transform.LookAt(targetPosition);
                    thisEntity.transform.eulerAngles = new Vector3(0.0f, thisEntity.transform.eulerAngles.y - 90, 0.0f);
                    targetSighted = true;
                }
                else if (targetInSight.transform.gameObject.name != null)
                {
                    Debug.Log("Target Obstructed");
                }
                else
                {
                    Debug.Log("Target not Found");
                }
            }

            //sends target to shooting script if target is visible
            if (targetSighted == true)
            {

                projectileShoot.setTarget(target);
                projectileShoot.setTargetSighted(targetSighted);
            }
            else
            {
                projectileShoot.setTargetSighted(targetSighted);
            }
              
        }
    }

    void resetTarget()
    {
        targetSighted = false;
        target = null;
    }
}
