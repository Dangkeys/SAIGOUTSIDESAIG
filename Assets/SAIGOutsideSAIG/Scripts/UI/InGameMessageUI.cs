using System;
using SceneSystem;
using TMPro;
using UnityEngine;
using VContainer;
namespace UI
{

    public class InGameMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _inGameMessageText;
        private SceneController _sceneController;
        private ApplicationController _applicationController;
        [Inject]
        private void Construct(ApplicationController applicationController, SceneController sceneController)
        {
            _sceneController = sceneController;
            _applicationController = applicationController;
        }
        private void Start()
        {
            _sceneController.OnCurrentSceneSolved += ShowSceneSolveMessage;
            if (!_applicationController.IsInitialized)
            {
                _applicationController.InitializeEvent += ShouldDisplayMessage;
            }
            else
            {
                ShouldDisplayMessage();

            }
        }
        private void ShouldDisplayMessage()
        {
            Show(!(_sceneController.GetCurrentSceneStatus() == SceneStatus.Available));
        }
        private void ShowSceneSolveMessage()
        {
            Show();
        }
        public void Show(bool shouldShow = true)
        {
            gameObject.SetActive(shouldShow);
        }
        void OnDestroy()
        {
            _applicationController.InitializeEvent -= ShouldDisplayMessage;
            _sceneController.OnCurrentSceneSolved -= ShowSceneSolveMessage;
        }
    }

}