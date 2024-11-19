using System;
using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Processors.Transformers;
using UnityEngine;

namespace ArcadeBridge
{
    public class HandleCarOutputQueue : MonoBehaviour
    {
        public TicketToCashPickableTransformStockpiler machine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarMovement car))
            {
                //Time To Get cash
                machine.ModifyItem(car);
            }
        }
    }
}
