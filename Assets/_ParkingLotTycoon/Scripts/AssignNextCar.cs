using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class AssignNextCar : MonoBehaviour
    {
        [SerializeField]private CarMovement lastCar = null;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarMovement carMovement))
            {
                if (lastCar != null)
                {
                    if (carMovement.gameObject != lastCar.gameObject)
                        carMovement.nextCarTransform = lastCar.transform;
                    else
                        carMovement.nextCarTransform = null;
                }
                else
                {
                    carMovement.nextCarTransform = null;
                }
                lastCar = carMovement;
            }
        }
    }
}
