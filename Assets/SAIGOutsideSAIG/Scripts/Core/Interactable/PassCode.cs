using System;
using Core.Player;
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

        [Inject]
        private void Construct(PassCodeUI passCodeUI, PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
            _passCodeUI = passCodeUI;
        }
        private void Awake()
        {
            _passCodeUI.OnPasswordSubmitted += HandlePasswordSubmitted;
            _passCodeUI.OnUIClosed += HandleUIClosed;
        }
        private void Start()
        {
            InitServices();
            _passCodeServiceBindings = new PassCodeServiceBindings(CloudCodeService.Instance);
        }
        private async void InitServices()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                Debug.Log("Services Initializing");
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
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
                bool isCorrect = await _passCodeServiceBindings.VerifyPassword(_passCodeSO.ID, _passCodeSO.Name, inputPassword: input);
                if (isCorrect)
                {
                    _door.TriggerOpenAnimation();
                    TestSayHello();
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
            _passCodeUI.OnPasswordSubmitted -= HandlePasswordSubmitted;
        }

        private async void TestSayHello()
        {

        }
    }
}