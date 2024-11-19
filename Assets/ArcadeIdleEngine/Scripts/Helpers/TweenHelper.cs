using System;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using DG.Tweening;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Helpers
{
    public static class TweenHelper
    {
        public static void Jump(Transform item, Vector3 target, float duration)
        {
            item.DOJump(target, 1f, 1, duration).SetRecyclable().SetAutoKill();
            item.DOLocalRotate(new Vector3(0f, 180f, 0f), duration).SetRecyclable().SetAutoKill();
        }

        
        
        public static void SpendResource(int requiredResource, int collectedResource, int myResource, out Tween resourceSpendingTween, float spendingSpeed,
                                         Ease resourceSpendEase, TweenCallback<int> onTweenUpdate)
        {
            int remainedMoney = requiredResource - collectedResource;
            int to = myResource >= remainedMoney ? requiredResource : collectedResource + myResource;
            resourceSpendingTween = DOVirtual.Int(collectedResource, to, (float)to / requiredResource * spendingSpeed, onTweenUpdate)
                .SetEase(resourceSpendEase).SetAutoKill();
        }

        public static void SetParentAndJump(this Transform transform, Transform to, Action onJumped)
        {
            transform.SetParent(to);
            transform.DOLocalJump(Vector3.zero, 1f, 1, 0.4f).SetRecyclable().SetAutoKill().OnComplete(() => onJumped?.Invoke());
            transform.DOScale(Vector3.kEpsilon, 0.4f).SetEase(Ease.InBack, 5f).SetRecyclable().SetAutoKill().OnComplete(() => transform.gameObject.SetActive(false));
        }

        public static Sequence Jump(Transform transform, Transform targetPoint)
        {
            return transform.DOJump(targetPoint.position, 1f, 1, 1f).SetRecyclable().SetAutoKill();
        }

        public static Sequence Jump(Transform transform, Vector3 targetPoint, float jumpPower, int numJumps, float duration)
        {
            return transform.DOJump(targetPoint, jumpPower, numJumps, duration).SetEase(Ease.Linear).SetRecyclable().SetAutoKill();
        }

        public static void Jump(Transform transform, Vector3 targetPoint, float jumpPower, int numJumps, float duration, TweenCallback onComplete)
        {
            transform.DOJump(targetPoint, jumpPower, numJumps, duration).SetRecyclable().SetAutoKill().OnComplete(onComplete);
        }

        public static void ShowSlowly(Transform transform, Vector3 targetScale, float duration, TweenCallback onComplete)
        {
            transform.gameObject.SetActive(true);
            transform.DOScale(targetScale, duration).SetEase(Ease.OutBack, 2f).OnComplete(onComplete);
        }

        public static void DisappearSlowly(Transform transform)
        {
            transform.DOScale(Vector3.one * Mathf.Epsilon, 0.2f).SetAutoKill().SetRecyclable().OnComplete(() => { transform.gameObject.SetActive(false); });
        }
        
        public static void DisappearSlowly(Transform transform, float duration, TweenCallback onCompleted)
        {
            transform.DOScale(Vector3.one * Mathf.Epsilon, duration).SetEase(Ease.InBack, 2f).SetAutoKill().SetRecyclable().OnComplete(onCompleted);
        }

        public static void CompleteAll(Transform transform)
        {
            transform.DOComplete();
        }
        
        public static Tween DisappearSlowlyToPool(this Pickable pickable)
        {
            return pickable.transform.DOScale(Vector3.one * Mathf.Epsilon, 0.2f).SetRecyclable().SetAutoKill().OnComplete(pickable.ReleaseToPool);
        }

        public static void JumpToDisappearIntoPool(Pickable pickable, Vector3 targetPoint, float jumpPower, int numJumps, float duration)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(Jump(pickable.transform, targetPoint, jumpPower, numJumps, duration));
            sequence.Append(DisappearSlowlyToPool(pickable));
            sequence.SetAutoKill().SetRecyclable();
            sequence.Play();
        }

        public static void KillAllTweens(Transform transform)
        {
            transform.DOKill();
        }

        public static void DelayedCall(float delay, TweenCallback callback)
        {
            DOVirtual.DelayedCall(delay, callback);
        }

        public static void ShakeScale(Transform trans, Vector3 targetScale, float duration)
        {
            trans.DOShakeScale(duration, targetScale).SetRecyclable();
        }
        
        public static void PunchScale(Transform trans, Vector3 targetScale, float duration)
        {
            trans.DOPunchScale(targetScale, duration, 2).SetRecyclable();
        }

        public static void MoveUI(Transform trans, Vector3 target, float duration, TweenCallback onComplete)
        {
            Sequence sequence = DOTween.Sequence().SetRecyclable();
            sequence.Append(trans.DOMove(target, duration * 0.3f));
            sequence.Join(trans.DOScale(Vector3.one, duration * 0.3f));
            sequence.Append(trans.DOScale(Vector3.one / 2f, duration).SetEase(Ease.InBack, 5f));
            sequence.Join(trans.DOLocalMove(Vector3.zero, duration * 0.7f).SetEase(Ease.InQuart));
            sequence.AppendCallback(onComplete);
        }
    }
}