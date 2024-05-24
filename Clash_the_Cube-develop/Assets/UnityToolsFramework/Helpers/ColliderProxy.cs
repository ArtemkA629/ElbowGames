using UnityEngine;

namespace Framework.Helpers
{
    public class ColliderProxy: MonoBehaviour
    {
        [SerializeField] private GameObject receiverObject;
        private IColliderProxyReceiver receiver;

        private void Start()
        {
            receiver = receiverObject.GetComponent<IColliderProxyReceiver>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            (receiver as IColliderProxyCollisionReceiver)?.OnCollisionEnterChild(this, collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            (receiver as IColliderProxyCollisionReceiver)?.OnCollisionStayChild(this, collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            (receiver as IColliderProxyCollisionReceiver)?.OnCollisionExitChild(this, collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            (receiver as IColliderProxyTriggerReceiver)?.OnTriggerEnterChild(this, other);
        }

        private void OnTriggerStay(Collider other)
        {
            (receiver as IColliderProxyTriggerReceiver)?.OnTriggerStayChild(this, other);
        }

        private void OnTriggerExit(Collider other)
        {
            (receiver as IColliderProxyTriggerReceiver)?.OnTriggerExitChild(this, other);
        }
    }
}