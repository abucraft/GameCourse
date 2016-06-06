using UnityEngine;
using System.Collections;

namespace MemoryTrap
{
    public class AsyncJob
    {

        private bool m_IsDone = false;
        public bool IsDone
        {
            get
            {
                return m_IsDone;
            }
        }

        public virtual IEnumerator Start()
        {
            yield return AsyncFunction();
            m_IsDone = true;
        }

        protected virtual IEnumerator AsyncFunction() { yield return null; }

        protected virtual void OnFinished() { }
        
    }
}