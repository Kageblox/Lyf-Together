using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Primary
{
    public class ImageTargetFollow : MonoBehaviour
    {
        #region Variables
        [Header("Other Components")]
        //The holder of all the scene elements. Thus, by lerping this to the image target, we get smooth movement
        public GameObject sceneHolder;

        //The image target. Its position and rotation will change as the user moves and rotates their phone, and thus we lerp the sceneHolder to it.
        public GameObject imageTarget;

        [Header("Variables")]
        //Whether the scene should be enabled and following the imageTarget.
        public bool followOn = false;

        //The currently running follow enu.
        IEnumerator runningFollowEnu;

        [Header("Settings")]
        //How much the app should lerp from the sceneHolder to the imageTarget every frame.
        public float lerp = 0.5f;

        #endregion

        #region MonoBehavior Functions
        // Start is called before the first frame update
        void Start()
        {
            //When an image target has yet to be found, disable the sceneHolder, so that it does not appear on the user's device
            sceneHolder.SetActive(false);
        }
        #endregion

        #region Functions and Coroutines
        //This function enables or disables the Image Target Following feature.
        //As it is no required when the there's no image target, turning it off in cases like that improves performace, hence this function.
        public void SetFollow(bool val)
        {
            //If there's a change in the Follow value
            if (followOn != val)
            {
                //Change the follow value and update the sceneHolder
                followOn = val;
                sceneHolder.SetActive(val);

                //And if it's true
                if (val)
                {
                    //Then first, instantly snap the position and rotation to the image target
                    sceneHolder.transform.position = imageTarget.transform.position;
                    sceneHolder.transform.rotation = imageTarget.transform.rotation;

                    //Before starting the Follow coroutine
                    runningFollowEnu = FollowEnu();
                    StartCoroutine(runningFollowEnu);
                }
                //If it was false
                else
                {
                    //Then stop the follow coroutine, if it is currently running
                    if (runningFollowEnu != null)
                    {
                        StopCoroutine(runningFollowEnu);
                    }
                }
            }
        }
        //The coroutine in charge of the smooth follow of the Scene Holder to the Image Target
        IEnumerator FollowEnu()
        {
            //As this coroutine is meant to run until stopped, there's no exit condition.
            while (true)
            {
                //Every frame, lerp the scene holder to the image target
                sceneHolder.transform.position = Vector3.Lerp(sceneHolder.transform.position, imageTarget.transform.position, lerp);
                sceneHolder.transform.rotation = Quaternion.Lerp(sceneHolder.transform.rotation, imageTarget.transform.rotation, lerp);
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}