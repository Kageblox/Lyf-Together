using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Secondary;
using DigitalSalmon;
using UnityEngine.EventSystems;

namespace Primary
{
    public class Viewer : MonoBehaviour
    {
        #region Variables
        [Header("Other Components")]
        //The gameObject that will store the currently viewed object
        public GameObject normalViewTarget;
        public GameObject rewardViewTarget;

        [Header("Variables")]
        //The amount of time held. If it is more than holdTime, it will be counted as a Hold. Else, it will be counted as a tap
        float timeHeld;

        //Whether the user is currently holding down
        bool held;

        //The currently running IEnumerator for the IEnumerator responsible for determining whether the user is tapping or holding
        IEnumerator timeHeldEnu;

        //The currently viewed Viewable Object
        Viewable currentlyViewed;

        //The position where the user last clicked, tapped or held
        Vector3 lastPos;

        //The Vector3 that rotates the currently viewed object
        Vector3 rotationVector;

        bool rotationEnabled;

        [Header("Settings")]
        //The lerp value used to bring or return objects
        public float lerp = 0.1f;

        //Settings and Variables used for the Harmonic Motion Rotation;
        [Header("Harmonic Motion Rotation Settings & Variables")]
        public float dampingRatio = 100;
        public float angularFrequency = 100;
        Vector3 harmonicMotionVelo = Vector3.zero;
        HarmonicMotion.DampenedSpringMotionParams harmonicParams;

        //The layers which the Viewer will collide with
        public LayerMask collideMask;
        public LayerMask ignoreMask;
        //The amount of time required for an input to be registered as a hold
        public float holdTime = 0.25f;
        #endregion

        #region MonoBehavior Functions
        void Start()
        {
            //At the start of the game, calculate the DampedSpringMotionParams required for harmonic motion
            harmonicParams = HarmonicMotion.CalcDampedSpringMotionParams(dampingRatio, angularFrequency);
        }
        // Update is called once per frame
        void Update()
        {
            //Starting off, the touch position will be set to Vector3.zero, which is the bottom left of the screen.
            //This is because it's highly unlikely that the player will ever touch that area,
            //and it's thus easy to check if any input was detected by simply checking the magnitude of the vector.
            Vector3 touchPos = Vector3.zero;

            //Here, there's 2 seperate chunks of code that run basically the same, but are tailored to their respective platforms.
#if UNITY_ANDROID
            //If a touch has been detected
            if (Input.touchCount > 0)
            {
                //And the first touch is not over any UI elements
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    //Then change touchPos to the position of the touch
                    touchPos = Input.GetTouch(0).position;

                    //If the User has just touched the screen, start a timer that will determine whether it's a tap or hold
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        timeHeldEnu = HoldEnu();
                        StartCoroutine(timeHeldEnu);

                        //Also, record the last touched position as touchPos
                        lastPos = touchPos;
                    }
                    //If the User has ended his touch
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        //Set held to false, and set lastPos to an invalid coordinate.
                        held = false;
                        lastPos = Vector3.zero;

                        //And if the User released his finger before the specified HoldTime, it is registed as a Tap
                        if (timeHeld < holdTime)
                        {
                            StopCoroutine(timeHeldEnu);
                            Tap(touchPos);
                        }
                    }
                }
                //If the user's drag goes over a UI element
                else
                {
                    //Then stop the hold
                    held = false;
                }
            }
#endif

#if UNITY_EDITOR
            //Same logic as above, but for PC instead.
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                touchPos = Input.mousePosition;
                if (Input.GetMouseButtonDown(0))
                {
                    timeHeldEnu = HoldEnu();
                    StartCoroutine(timeHeldEnu);
                    lastPos = touchPos;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    held = false;
                    lastPos = Vector3.zero;
                    if (timeHeld < holdTime)
                    {
                        StopCoroutine(timeHeldEnu);
                        Tap(touchPos);
                    }
                }
            }
            else
            {
                held = false;
            }
#endif
            //If there's currently a viewed object
            if (currentlyViewed != null && rotationEnabled)
            {
                //Then determine the swipe vector via the position of the held position last frame compared to the held position this frame
                Vector3 swipeVector = Vector3.zero;
                if (held && touchPos.magnitude > 0)
                {
                    swipeVector = lastPos - touchPos;
                    swipeVector = new Vector3(-swipeVector.y, swipeVector.z, swipeVector.x);
                    lastPos = touchPos;
                }

                //And using that, and a HarmonicMotion script I shamelessly stole online, I calculate a new rotation vector
                //Taken From https://digitalsalmon.co.uk/simple-harmonic-motion-an-alternative-to-continuous-lerp/
                HarmonicMotion.Calculate(ref rotationVector, ref harmonicMotionVelo, swipeVector, harmonicParams);

                //And finally, I rotate the currently viewed object using this vector, relative to World Space
                currentlyViewed.transform.Rotate(rotationVector, Space.World);
            }
        }
        #endregion

        #region Functions and Coroutines
        //This IEnumerator is in charge of controlling the held boolean based off how long the player has held down for.
        IEnumerator HoldEnu()
        {
            timeHeld = 0f;
            while (true)
            {
                timeHeld += Time.deltaTime;
                if (timeHeld > holdTime)
                {
                    held = true;

                    //Also, the rotation vector resets whenever a new held is detected, for easier maneuverability
                    rotationVector = Vector3.zero;
                    yield break;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        //This function is in charge of Retrieving and Returning viewable objects
        void Tap(Vector2 screenPos)
        {
            //First, return the currently viewed object
            if (currentlyViewed)
            {
                if (currentlyViewed.viewable_Type != Viewable.ViewableType.Reward)
                {
                    ViewerUI.Instance.QueueHide();
                }
                currentlyViewed.Return(lerp);
                currentlyViewed = null;
                
            }

            //Next, check the screen position for any viewable objects
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            //If one has been detected, then retrieve it and update it as the currently viewed object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, collideMask))
            {
                Viewable objViewable = hit.transform.gameObject.GetComponent<Viewable>();
                currentlyViewed = objViewable;
                switch (objViewable.viewable_Type)
                {
                    case Viewable.ViewableType.Normal:
                        rotationEnabled = true;
                        currentlyViewed.View(lerp, normalViewTarget);
                        ViewerUI.Instance.QueueShow(objViewable);
                        break;
                    case Viewable.ViewableType.Special:
                        rotationEnabled = true;
                        currentlyViewed.View(lerp, normalViewTarget);
                        ViewerUI.Instance.QueueShow(objViewable);
                        break;
                    case Viewable.ViewableType.Reward:
                        rotationEnabled = false;
                        currentlyViewed.View(lerp, rewardViewTarget);
                        break;
                }
                rotationVector = Vector3.zero;

                
                
                
            }
        }
        #endregion
    }
}