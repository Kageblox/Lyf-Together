using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Secondary
{
    //Meant to hold and call events that trigger on enter or exit of a timing
    public class EventsGroup : MonoBehaviour
    {
        #region Variables
        [Header("Variables")]
        //Events that trigger on enter and exit respectively 
        public List<UnityEvent> enterEvents;
        public List<UnityEvent> exitEvents;

        //The running enter or exit events coroutine
        IEnumerator runningEnu;

        //A variable meant to hold the amount of time that will be waited
        float waitTime;
        #endregion

        #region Functions and Coroutines
        //On enter and exit, trigger their respective events
        public void OnEnter()
        {
            StartNewEnu(EventsEnu(enterEvents));
        }

        public void onExit()
        {
            StartNewEnu(EventsEnu(exitEvents));
        }

        //Meant to be used in events, where by calling this function, the events will wait for the specified amount of time before proceeding
        public void Wait(float time)
        {
            waitTime = time;
        }

        //Runs through a list of Unity Events
        IEnumerator EventsEnu(List<UnityEvent> events)
        {
            foreach (UnityEvent evt in events)
            {
                evt.Invoke();
                
                //If the event is not Wait, then set waitTime to zero, so that the next event can trigger instantly
                if (evt.GetPersistentMethodName(0) != "Wait")
                {
                    waitTime = 0;
                }
                else
                {
                    //Else, as the wait time has already been specified in Wait, there technically doesn't need to be anything inside this else statement. 
                    //Only used for debugging
                    Debug.Log(evt.GetPersistentMethodName(0) + " " + waitTime);
                }

                //Finally, if the event was not wait, trigger the next event instantly, else trigger the next event after the wait
                yield return new WaitForSeconds(waitTime);
            }
            yield break;
        }

        //Starts a new coroutine by first ending the currently running one
        void StartNewEnu(IEnumerator newEnu)
        {
            if (runningEnu != null)
            {
                StopCoroutine(runningEnu);
            }
            runningEnu = newEnu;
            StartCoroutine(newEnu);
        }
        #endregion
    }
}
