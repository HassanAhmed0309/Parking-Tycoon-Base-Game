using System;
using TMPro;
using UnityEngine;

namespace ArcadeBridge
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text CurrentTicketCountText;
        [SerializeField] private GameObject TicketsCountUI;
        public static UIManager Instance;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        
        public void UpdateTicketsText(int amount)
        {
            if(!TicketsCountUI.gameObject.activeInHierarchy)
                TicketsCountUI.gameObject.SetActive(true);
            
            CurrentTicketCountText.text = amount.ToString();
        }
    }
}
