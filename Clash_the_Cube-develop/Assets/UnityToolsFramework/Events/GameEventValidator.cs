using UnityEngine;

namespace Framework.Events
{
    public abstract class GameEventValidator : ScriptableObject
    {
        public virtual bool Validate()
        {
            return true;
        }
    }
}