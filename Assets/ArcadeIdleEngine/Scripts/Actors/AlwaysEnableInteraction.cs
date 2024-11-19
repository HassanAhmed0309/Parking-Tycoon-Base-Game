using ArcadeBridge.ArcadeIdleEngine.Interactables;

namespace ArcadeBridge.ArcadeIdleEngine.Actors
{
    public class AlwaysEnableInteraction : IInteractor
    {
        public bool IsInteractable => true;
    }
}