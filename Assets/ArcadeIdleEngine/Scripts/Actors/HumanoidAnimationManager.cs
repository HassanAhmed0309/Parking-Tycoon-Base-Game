using ArcadeIdleEngine.ExternalAssets.NaughtyAttributes_2._1._4.Core.DrawerAttributes;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Actors
{
    public class HumanoidAnimationManager : MonoBehaviour
    {
        [SerializeField] public Animator _animator;
        [SerializeField, AnimatorParam(nameof(_animator))] string _interactionName;
        [SerializeField, AnimatorParam(nameof(_animator))] string _interactionSpeedName;
        [SerializeField, AnimatorParam(nameof(_animator))] string _moveName;

        int _moveId;
        int _interactionId;
        int _interactionSpeedId;

        void Awake()
        {
            _moveId = Animator.StringToHash(_moveName);
            _interactionId = Animator.StringToHash(_interactionName);
            _interactionSpeedId = Animator.StringToHash(_interactionSpeedName);
        }

        public void PlayMove(Vector2 moveVector)
        {
            _animator.SetFloat(_moveId, moveVector.magnitude);
        }
        
        public void PlayMove(float moveMagnitude)
        {
            _animator.SetFloat(_moveId, moveMagnitude);
        }

        public void PlayInteraction(int type, float speed)
        {
            _animator.SetFloat(_interactionSpeedId, speed);
            _animator.SetInteger(_interactionId, type);
        }
        
        public void StopInteraction()
        {
            _animator.SetInteger(_interactionId, -1);
        }
    }
}