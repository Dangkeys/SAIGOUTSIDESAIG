using System;
using InputSystem;
using Interactable;
using UnityEngine;
using Utils;
using VContainer;
namespace Interactable
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private float _interactDistance = 3f;
        private GameInputReader _gameInputReader;
        [Inject]
        private void Construct(GameInputReader gameInputReader)
        {
            _gameInputReader = gameInputReader;
            _gameInputReader.InteractEvent += OnInteract;
        }

        private void OnInteract()
        {
            if (!RaycastHelper.TryGetComponentFromCenterRaycast(_interactDistance, out IInteractable interactable, out var _)) return;
            interactable.Interact(this);
        }

        private void OnDestroy()
        {
            _gameInputReader.InteractEvent -= OnInteract;
        }

        private void OnDrawGizmosSelected()
        {
            GizmosHelper.DrawRay(transform.position, transform.forward, _interactDistance, Color.yellow);
        }
        
    }
}