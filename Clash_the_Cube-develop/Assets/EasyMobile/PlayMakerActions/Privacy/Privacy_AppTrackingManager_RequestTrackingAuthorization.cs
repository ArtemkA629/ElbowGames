#if PLAYMAKER
using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EasyMobile.PlayerMaker.Actions
{
    [ActionCategory("Easy Mobile - Privacy")]
    [Tooltip("Request app tracking authorization (show the ATT popup on iOS 14).")]
    public class Privacy_AppTrackingManager_RequestTrackingAuthorization : FsmStateAction
    {
        [ActionSection("Result")]

        [Tooltip("Event sent if the user authorized the tracking request.")]
        public FsmEvent isTrackingAuthorizedEvent;

        [Tooltip("Event sent if the user denied the tracking request.")]
        public FsmEvent isTrackingDeniedEvent;

        [Tooltip("Event sent if the tracking request result is not determined.")]
        public FsmEvent isTrackingNotDeterminedEvent;

        [Tooltip("Event sent if the tracking is restricted.")]
        public FsmEvent isTrackingRestrictedEvent;

        public override void Reset()
        {
            isTrackingAuthorizedEvent = null;
            isTrackingDeniedEvent = null;
            isTrackingNotDeterminedEvent = null;
            isTrackingRestrictedEvent = null;
        }

        public override void OnEnter()
        {
            DoMyAction();
        }

        void DoMyAction()
        {
            Privacy.AppTrackingManager.RequestTrackingAuthorization(status =>
            {
                 if (status == AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusAuthorized)
                    Fsm.Event(isTrackingAuthorizedEvent);
                 else if (status == AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusDenied)
                    Fsm.Event(isTrackingDeniedEvent);
                 else if (status == AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusNotDetermined)
                    Fsm.Event(isTrackingNotDeterminedEvent);
                 else
                    Fsm.Event(isTrackingRestrictedEvent);

                Finish();
            });
        }
    }
}
#endif

