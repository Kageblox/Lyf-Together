using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Secondary
{
    //This class should be attached to objects that are meant to be viewable.
    public class Viewable : MonoBehaviour
    {
        #region Variables
        //Different Viewable types. Add more here in case more are required
        public enum ViewableType
        {
            Normal,
            Special,
            Reward
        }
        [Header("Other Components")]
        //The location the viewable returns to when no longer being viewed
        GameObject home;

        [Header("Variables")]
        //The name and description of the Viewable
        public string viewable_Name;
        public string viewable_Description;
        public ViewableType viewable_Type;
        public UnityEvent onViewEvents;
        public UnityEvent onReturnEvents;
        public float viewnScale = 1;
        float homeScale;
        //A variable used to store the currently running IEnumerator
        IEnumerator runningEnu;

        [Header("Settings")]
        //The distance before the Viewable snaps to either Home or the View Target. This should be left alone for the most part, but can be changed if need be
        public float distToSnap = 0.1f;
        #endregion

        #region MonoBehaviour Functions
        void Start()
        {
            home = transform.parent.gameObject;
            homeScale = transform.localScale.x;
        }
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
            runningEnu = MoveEnu(lerp, target, viewnScale, onViewEvents);
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
            runningEnu = MoveEnu(lerp, home, homeScale, onReturnEvents);
            StartCoroutine(runningEnu);
        }

        //The IEnumerator which moves and rotates the Viewable from point to point
        IEnumerator MoveEnu(float lerp, GameObject target, float targetScale, UnityEvent eventsToRun)
        {
            //Start off by changing the parent of the Viewable, to improve stability
            transform.parent = target.transform;
            Vector3 targetScaleVec = new Vector3(targetScale, targetScale, targetScale);
            //Next, while the Viewable is still far away from the target position
            while (Vector3.Distance(transform.position, target.transform.position) > distToSnap)
            {
                //Lerp the position and rotation to the target every frame
                transform.position = Vector3.Lerp(transform.position, target.transform.position, lerp);
                transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, lerp);


                transform.localScale = Vector3.Lerp(transform.localScale, targetScaleVec, lerp);
                yield return new WaitForEndOfFrame();
            }

            //Once the Viewable is close enough to the target, snap its position and rotation to match the target
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
            transform.localScale = targetScaleVec;

            eventsToRun.Invoke();
            //Finally, end the IEnumerator
            yield break;
        }
        #endregion
    }
}