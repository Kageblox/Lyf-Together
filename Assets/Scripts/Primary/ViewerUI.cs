using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Patterns;
using Secondary;
using TMPro;
using UnityEngine.UI;

namespace Primary
{
    [RequireComponent(typeof(Animator))]
    //This script utilizes the Singleton Pattern, so that it can be accessed from anywhere
    public class ViewerUI : Singleton<ViewerUI>
    {
        [Serializable]
        class UIColorScheme
        {
            public Color title_Background_Color;
            public Color title_Text_Color;
            public Color content_Background_Color;
            public Color content_Text_Color;
        }
        #region Variables
        [Header("Other Components")]
        public Image title_Background;
        public TextMeshProUGUI title_Text;
        public Image content_Background;
        public TextMeshProUGUI content_Text;
        Animator animator;

        [Header("Variables")]
        //To accomodate for spamming, the animations work using a queue system. Will need to make more changes in the future, such as removing some items should the queue get too long
        [HideInInspector] public List<UnityEvent> queue;
        IEnumerator runningEnu;

        [Header("Settings")]
        //The amount of time in-between each queue item execution
        public float eventDuration;

        [SerializeField] UIColorScheme normalColorScheme;
        [SerializeField] UIColorScheme specialColorScheme;
        [SerializeField] UIColorScheme rewardColorScheme;
        
        #endregion

        #region MonoBehavior Functions
        void Start()
        {
            animator = GetComponent<Animator>();
        }
        #endregion

        #region Functions and Coroutines
        void ChangeColorScheme(UIColorScheme scheme)
        {
            title_Text.color = scheme.title_Text_Color;
            title_Background.color = scheme.title_Background_Color;
            content_Text.color = scheme.content_Text_Color;
            content_Background.color = scheme.content_Background_Color;
        }

        //The function queues a new Show event in the queue, and starts the clearing of the queue if it hasn't already started
        public void QueueShow(Viewable viewable)
        {
            UnityEvent evt = new UnityEvent();
            evt.AddListener(delegate { Show(viewable); });
            queue.Add(evt);

            StartQueueClearing();
        }

        //The function queues a new Hide event in the queue, and starts the clearing of the queue if it hasn't already started
        public void QueueHide()
        {
            UnityEvent evt = new UnityEvent();
            evt.AddListener(Hide);
            queue.Add(evt);

            StartQueueClearing();
        }

        //The function triggers the "Show" state for the UI, and changes the UI to match the viewable object
        void Show(Viewable viewable)
        {
            animator.SetTrigger("Show");
            switch (viewable.viewable_Type)
            {
                case Viewable.ViewableType.Normal:
                    ChangeColorScheme(normalColorScheme);
                    break;
                case Viewable.ViewableType.Special:
                    ChangeColorScheme(specialColorScheme);
                    break;
                case Viewable.ViewableType.Reward:
                    ChangeColorScheme(rewardColorScheme);
                    break;
            }
            title_Text.text = viewable.viewable_Name;
            content_Text.text = viewable.viewable_Description;

        }

        //The function hides the UI
        void Hide()
        {
            animator.SetTrigger("Hide");
        }

        //This function iniates the clearing of the animation queue, should it not be already running
        void StartQueueClearing()
        {
            if (runningEnu == null)
            {
                runningEnu = ClearEnu();
                StartCoroutine(runningEnu);
            }
        }

        //The actual coroutine in charge of clearing the queue.
        //It activates and removes the first event with a delay of event duration until there are no more events, at which point it will kill itself
        IEnumerator ClearEnu()
        {
            while (queue.Count > 0)
            {
                queue[0].Invoke();
                queue.RemoveAt(0);
                yield return new WaitForSeconds(eventDuration);
            }
            runningEnu = null;
            yield break;
        }
    }
    #endregion
}