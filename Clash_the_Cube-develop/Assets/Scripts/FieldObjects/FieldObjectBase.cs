using Framework.SystemInfo;
using Framework.Variables;
using UnityEngine;

namespace ClashTheCube
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(LineRenderer))]
    public abstract class FieldObjectBase : MonoBehaviour
    {
        [SerializeField] private TrajectoryPredictor trajectoryPredictor;
        [SerializeField] private FloatReference cubeMergeTorque;

        [SerializeField] protected Vector2Variable swipeDelta;
        [SerializeField] protected FloatReference swipeDeltaMultiplier;
        [SerializeField] protected FloatReference swipeDeltaMultiplierDesktop;

        protected Vector3 destPosition;
        protected bool sleeping;
        protected IFieldObjectHolder objectHolder;

        public FieldObjectState State { get; protected set; }
        public Rigidbody Body { get; private set; }

        public void SetObjectHolder(IFieldObjectHolder holder)
        {
            objectHolder = holder;
            objectHolder.AddObject(this);
        }

        protected void Awake()
        {
            Body = GetComponent<Rigidbody>();
            sleeping = true;
        }

        protected void Update()
        {
            if (State != FieldObjectState.Initial)
                return;

            if (trajectoryPredictor.Aimed)
                trajectoryPredictor.Predict(GetDeltaMultiplier());
        }

        protected void FixedUpdate()
        {
            sleeping = Body.velocity.magnitude < 0.1f;
        }

        public void Aim()
        {
            if (State != FieldObjectState.Initial)
                return;

            trajectoryPredictor.Predict(GetDeltaMultiplier());
        }

        public void Throw()
        {
            if (State != FieldObjectState.Initial)
                return;

            Body.isKinematic = false;
            Body.constraints = RigidbodyConstraints.None;
            Body.AddForce(trajectoryPredictor.Speed, ForceMode.VelocityChange);

            float torqueRatio = 0.5f;
            var torque = GenerateNormalizedTorque() * torqueRatio;
            Body.AddTorque(torque, ForceMode.Impulse);

            trajectoryPredictor.OnObjectThrow();
            State = FieldObjectState.Transition;
        }

        private float GetDeltaMultiplier()
        {
            return Platform.IsMobilePlatform() ? swipeDeltaMultiplier : swipeDeltaMultiplierDesktop;
        }

        private Vector3 GenerateNormalizedTorque()
        {
            float cubeMergeAngle = 10f;
            var randomX = Random.Range(-cubeMergeAngle, cubeMergeAngle);
            var randomZ = Random.Range(-cubeMergeAngle, cubeMergeAngle);

            return -(new Vector3(randomX, 0f, randomZ)).normalized;
        }
    }
}
