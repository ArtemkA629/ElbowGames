using UnityEngine;

namespace Framework.Helpers
{
    public interface IColliderProxyReceiver {}
        
    public interface IColliderProxyCollisionReceiver: IColliderProxyReceiver
    {
        void OnCollisionEnterChild(ColliderProxy child, Collision collision);
        void OnCollisionStayChild(ColliderProxy child, Collision collision);
        void OnCollisionExitChild(ColliderProxy child, Collision collision);
    }

    public interface IColliderProxyTriggerReceiver: IColliderProxyReceiver
    {
        void OnTriggerEnterChild(ColliderProxy child, Collider other);
        void OnTriggerStayChild(ColliderProxy child, Collider other);
        void OnTriggerExitChild(ColliderProxy child, Collider other);
    }
}