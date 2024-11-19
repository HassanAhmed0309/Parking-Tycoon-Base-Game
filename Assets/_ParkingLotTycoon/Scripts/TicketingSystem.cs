using System;
using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Processors.Transformers;
using UnityEngine;

namespace ArcadeBridge
{
    //Saves the amount of tickets in the machine
    public class TicketingSystem : MonoBehaviour
    {
        private int amountOfTickets = 0;

        public int tickets
        {
            get => amountOfTickets;
            set
            {
                amountOfTickets = value;
                //Debug.Log("Tickets Value: " + amountOfTickets);
                UIManager.Instance.UpdateTicketsText(amountOfTickets);
                if (!ticketsAvailable && amountOfTickets > 0)
                {
                    Debug.Log("Invoked the Tickets Available event!");
                    OnTicketAvailable?.Invoke();
                    ticketsAvailable = true;
                }
                else if(amountOfTickets <= 0)
                {
                    ticketsAvailable = false;
                }
            }
        }

        public Action OnTicketAvailable;
        
        public bool ticketsAvailable = false;
        private TicketToCashPickableTransformStockpiler machine;
        
        //If some person wants to add tickets to the system
        public Action TicketUsed;
        public Action<TicketToCashPickableTransformStockpiler> giveRef;


        [ContextMenu("Change Tickets Available")]
        private void ChangeTickets()
        {
            tickets = 10;
        }
        
        private void OnEnable()
        {
            giveRef += SetMachineReference;
            TicketUsed += TicketAssigned;
            if(UIManager.Instance != null)
                UIManager.Instance.UpdateTicketsText(amountOfTickets);
        }
        
        private void SetMachineReference(TicketToCashPickableTransformStockpiler reference)
        {
            machine = reference;
        }
        
        public void AddToTickets(int newTickets)
        {
            tickets += newTickets;
        }

        public void TicketAssigned()
        {
            if(tickets != 0)
                tickets -= 1;
            //Also, remove the item from the machine
            machine.RemoveTheItem();
        }

        private void OnDisable()
        {
            giveRef -= SetMachineReference;
            TicketUsed -= TicketAssigned;
        }
    }
}
