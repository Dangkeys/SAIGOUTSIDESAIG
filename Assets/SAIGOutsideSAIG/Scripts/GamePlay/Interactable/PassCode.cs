using System;
using System.Collections;
using Core.Player;
using SceneSystem;
using UI;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.Core;
using UnityEngine;
using VContainer;
namespace Interactable
{
    public class PassCode : MonoBehaviour, IInteractable
    {
        [SerializeField] private Door _door;
        [SerializeField] private PassCodeSO _passCodeSO;
        private PassCodeServiceBindings _passCodeServiceBindings;
        private PassCodeUI _passCodeUI;
        private PlayerMovement _playerMovement;
        private SceneSO _currentSceneSO;
        [SerializeField] private bool _isFinalPassCode;

        [Inject]
        private void Construct(PassCodeUI passCodeUI, PlayerMovement playerMovement, SceneSO sceneSO)
        {
            _playerMovement = playerMovement;
            _passCodeUI = passCodeUI;
            _currentSceneSO = sceneSO;
        }
        private void Awake()
        {
            if (_isFinalPassCode)
            {
                _passCodeUI.OnPasswordSubmitted += HandlePasswordSubmittedAndSolveScene;
            }
            else
            {
                _passCodeUI.OnPasswordSubmitted += HandlePasswordSubmitted;

            }
            _passCodeUI.OnUIClosed += HandleUIClosed;
        }

        private async void HandlePasswordSubmittedAndSolveScene(string input)
        {
            try
            {
                bool isCorrect = await _passCodeServiceBindings.VerifyPasswordAndSolveScene(_passCodeSO.ID, inputPassword: input, _currentSceneSO.ID);
                if (isCorrect)
                {
                    _door.TriggerOpenAnimation();

                    _passCodeUI.ShowResult(isCorrect);
                    StartCoroutine(Test());
                }
                _passCodeUI.ShowResult(isCorrect);
            }
            catch (CloudCodeException ex)
            {
                Debug.LogException(ex);
            }
        }
        private IEnumerator Test()
        {
            Debug.Log("You win this map");
            yield return new WaitForSecondsRealtime(5);
            Debug.Log("Leave the game please");
        }

        private void Start()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                _passCodeServiceBindings = new PassCodeServiceBindings(CloudCodeService.Instance);
            }
            else
            {
                // Subscribe to event if not yet initialized
                UnityServices.Initialized += OnUnityInitialized;
            }
        }

        private void OnUnityInitialized()
        {
            _passCodeServiceBindings = new PassCodeServiceBindings(CloudCodeService.Instance);
            UnityServices.Initialized -= OnUnityInitialized;
        }
        private void HandleUIClosed()
        {
            _playerMovement.EnablePlayerMovement();
        }

        public void Interact(Interactor interactor)
        {
            _playerMovement.EnablePlayerMovement(false);
            _passCodeUI.Show();
        }

        private async void HandlePasswordSubmitted(string input)
        {
            try
            {
                bool isCorrect = await _passCodeServiceBindings.VerifyPassword(_passCodeSO.ID, inputPassword: input);
                if (isCorrect)
                {
                    _door.TriggerOpenAnimation();
                }
                _passCodeUI.ShowResult(isCorrect);
            }
            catch (CloudCodeException ex)
            {
                Debug.LogException(ex);
            }
        }

        private void OnDestroy()
        {
            if (_isFinalPassCode)
            {
                _passCodeUI.OnPasswordSubmitted -= HandlePasswordSubmittedAndSolveScene;
            }
            else
            {
                _passCodeUI.OnPasswordSubmitted -= HandlePasswordSubmitted;

            }
        }
    }
}