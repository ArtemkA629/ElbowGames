#if PLAYMAKER
using UnityEngine;
using System;
using System.Collections;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EasyMobile.PlayerMaker.Actions
{
    [ActionCategory("Easy Mobile - Privacy")]
    [Tooltip("Gets the ATT tracking authorization status.")]
    public class Privacy_AppTrackingManager_GetTrackingAuthorizationStatus : FsmStateAction
    {
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [ActionSection("Result")]

        [Tooltip("The tracking authorization status.")]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(AppTrackingAuthorizationStatus))]
        public FsmEnum trackingAuthStatus;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

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
            trackingAuthStatus = null;
            eventTarget = null;
            isTrackingAuthorizedEvent = null;
            isTrackingDeniedEvent = null;
            isTrackingNotDeterminedEvent = null;
            isTrackingRestrictedEvent = null;
        }

        public override void OnEnter()
        {
            DoMyAction();

            if (!everyFrame)
                Finish();
        }

        public override void OnUpdate()
        {
            DoMyAction();   
        }

        void DoMyAction()
        {
            trackingAuthStatus.Value = Privacy.AppTrackingManager.TrackingAuthorizationStatus;

            switch (Privacy.AppTrackingManager.TrackingAuthorizationStatus)
            {
                case AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusAuthorized:
                    Fsm.Event(eventTarget, isTrackingAuthorizedEvent);
                    break;
                case AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusDenied:
                    Fsm.Event(eventTarget, isTrackingDeniedEvent);
                    break;
                case AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusRestricted:
                    Fsm.Event(eventTarget, isTrackingRestrictedEvent);
                    break;
                case AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusNotDetermined:
                default:
                    Fsm.Event(eventTarget, isTrackingNotDeterminedEvent);
                    break;
            }
        }
    }
}
#endif

