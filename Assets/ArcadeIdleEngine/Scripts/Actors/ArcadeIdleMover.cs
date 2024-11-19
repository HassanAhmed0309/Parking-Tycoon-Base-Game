using System;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Actors
{
    public class ArcadeIdleMover : MonoBehaviour, IInteractor
    {
        [SerializeField] Rigidbody _rbd;
        [SerializeField] ActorMovementData actorMovementData;
        [SerializeField] HumanoidAnimationManager _humanoidAnimationManager;
        [SerializeField] InputChannel inputChannel;

        Vector3 _currentInputVector;

        bool IInteractor.IsInteractable => _currentInputVector.sqrMagnitude < 0.2f;

        [Header("Player Speed Upgrade Data")] 
        public int upgradeCounter = 0, maxUpgradableLevels = 0;
        [SerializeField] private float animationSpeedIncrementFactor = 0;
        [SerializeField] private float playerSpeedIncrementFactor = 0;
        [SerializeField] private List<int> pricePerLevel;
        [SerializeField] private int cashRequiredForNextUpgrade;
        
        [Header("Changeable Values")]
        [SerializeField] int speedMultiplier = 1;

        private void Start()
        {
            //cashRequiredForNextUpgrade = pricePerLevel[0];
        }

        void OnEnable()
        {
            inputChannel.JoystickUpdate += OnJoystickUpdate;
        }

        void OnDisable()
        {
            inputChannel.JoystickUpdate -= OnJoystickUpdate;
        }

        void FixedUpdate()
        {
            _rbd.velocity = new Vector3(_currentInputVector.x, _rbd.velocity.y, _currentInputVector.z);
        }

        void OnJoystickUpdate(Vector2 newMoveDirection)
        {
            if (newMoveDirection.magnitude >= 1f)
            {
                newMoveDirection.Normalize();
            }

            _currentInputVector = new Vector3(newMoveDirection.x * actorMovementData.SideMoveSpeed * speedMultiplier, 0f, newMoveDirection.y * actorMovementData.ForwardMoveSpeed * speedMultiplier);
            _humanoidAnimationManager.PlayMove(newMoveDirection);
            Vector3 lookDir = new Vector3(_currentInputVector.x, 0f, _currentInputVector.z);
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            }
        }

        public void SetCharacterSpeedData(CharacterSpeed characterSpeed)
        {
            actorMovementData.SideMoveSpeed = characterSpeed.currentSpeed;
            actorMovementData.ForwardMoveSpeed = characterSpeed.currentSpeed;

            speedMultiplier = characterSpeed.tempSpeedMultiplier;
            upgradeCounter = characterSpeed.noOfUpgrades;

            if(upgradeCounter != 0)
                _humanoidAnimationManager._animator.speed = animationSpeedIncrementFactor * upgradeCounter;
        }

        public CharacterSpeed GetCharacterSpeed()
        {
            CharacterSpeed speed = new CharacterSpeed();

            speed.currentSpeed = actorMovementData.SideMoveSpeed;

            speed.noOfUpgrades = upgradeCounter;

            speed.tempSpeedMultiplier = speedMultiplier;
            
            return speed;
        }
        
        
        /*
         * Reference Upgrade Function from Tire Tycoon
         * public void IncreaseMovementSpeed()
        {
            SoundManager.Instance.PlayLevelingUpAbilitySound();
            if (PlayerProgressBar.Value <= 1 && upgradeCounter < maxUpgradableLevels)
            {
                if(upgradeCounter< maxUpgradableLevels)
                    currentCash = PricePerLevel[upgradeCounter];
                IsSpendable = _locker.SpendConstantAmount(MoneyPickableDefinition, currentCash);
                if (IsSpendable)
                {
                    actorMovementData.SideMoveSpeed += playerSpeedIncrementFactor;
                    actorMovementData.ForwardMoveSpeed += playerSpeedIncrementFactor;
                    if (_humanoidAnimationManager!= null)
                    {
                        _humanoidAnimationManager._animator.speed += 0.053f;
                    }
                    upgradeCounter++;
                    UIManager.PlayerUpgradesUIHandling?.Invoke();
                    PlayerProgressBar.Value = upgradeCounter*ProgressBarConstIncrement;
                    if (PlayerProgressBar.Value >= 1)
                    {
                        UIManager.DisableSpeedPlayerUpgrades?.Invoke();
                    }
                }
                else
                {
                    
                    UIManager.InsufficientFundsON?.Invoke();
                    IsSpendable = false;
                }
            }
        }
         */
        
        //This function will be called as callback To a button click or some unlocker area that upgrades player speed 
        public void UpgradeSpeed()
        {
            if (upgradeCounter < maxUpgradableLevels)
            {
                cashRequiredForNextUpgrade = pricePerLevel[upgradeCounter];
                //Pass scriptable objects of Money and Gems to GameManager and access them here to check if the player
                //has the required resources to upgrade his stats
            }
        }
        
    }
}