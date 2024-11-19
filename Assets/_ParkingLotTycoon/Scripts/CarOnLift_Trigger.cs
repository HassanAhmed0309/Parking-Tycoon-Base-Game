using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class CarOnLift_Trigger : MonoBehaviour
    {
        [SerializeField] private LiftController lift;
        private void OnTriggerEnter(Collider other)
        {
            if (lift.IN_LiftPad == gameObject)
            {
                if (lift.IN_LiftPad_Collider.enabled && other.TryGetComponent(out CarMovement carMovement))
                {
                    carMovement.CarAssignedTo_InLift();
                    lift.CarOnInLift = true;
                }
            }
            else
            {
                if (lift.OUT_LiftPad_Collider.enabled && other.TryGetComponent(out CarMovement car))
                {
                    car.CarAssignedTo_OutLift();
                    lift.CarOnOutLift = false;
                }
            }
        }
    }
}
