using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                }
                return instance;
            }
        }

        void Awake()
        {
            instance = this as T;
        }
    }
}