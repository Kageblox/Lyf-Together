using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Secondary
{
    public class Viewable : MonoBehaviour
    {
        #region Variables
        [Header("Other Components")]
        //The location the viewable returns to when no longer being viewed
        public GameObject home;

        [Header("Variables")]
        //The name and description of the Viewable
        public string viewable_Name;
        public string viewable_Description;

        //A variable used to store the currently running IEnumerator
        IEnumerator runningEnu;

        [Header("Settings")]
        //The distance before the Viewable snaps to either Home or the View Target
        public float distToSnap = 0.1f;
        #endregion

        #region Functions and Coroutines
        //The function that initiates the viewing of the Viewable
        public void View(float lerp, GameObject target)
        {
            //If an IEnumerator is currently running, stop it
            if (runningEnu != null)
            {
                StopCoroutine(runningEnu);
            }
            //And initiate MoveEnu
            runningEnu = MoveEnu(lerp, target);
            StartCoroutine(runningEnu);
        }

        //The function that initiates the returning of the Viewable
        public void Return(float lerp)
        {
            //If an IEnumerator is currently running, stop it
            if (runningEnu != null)
            {
                StopCoroutine(runningEnu);
            }
            //And initiate MoveEnu
            runningEnu = MoveEnu(lerp, home);
            StartCoroutine(runningEnu);
        }

        //The IEnumerator which moves and rotates the Viewable from point to point
        IEnumerator MoveEnu(float lerp, GameObject target)
        {
            //Start off by changing the parent of the Viewable, to improve stability
            transform.parent = target.transform;

            //Next, while the Viewable is still far away from the target position
            while (Vector3.Distance(transform.position, target.transform.position) > distToSnap)
            {
                //Lerp the position and rotation to the target every frame
                transform.position = Vector3.Lerp(transform.position, target.transform.position, lerp);
                transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, lerp);
                yield return new WaitForEndOfFrame();
            }

            //Once the Viewable is close enough to the target, snap its position and rotation to match the target
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;

            //Finally, end the IEnumerator
            yield break;
        }
        #endregion
    }
}