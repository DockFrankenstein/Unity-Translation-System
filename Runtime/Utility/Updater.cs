using System;
using UnityEngine;

namespace Translations
{
    public class Updater : MonoBehaviour
    {
        public static Updater Create()
        {
            var obj = new GameObject("Translation Updater");
            var script = obj.AddComponent<Updater>();
            DontDestroyOnLoad(obj);
            return script;
        }

        public event Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}