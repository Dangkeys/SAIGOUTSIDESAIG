using System;
using SceneSystem;
using Unity.Services.Authentication;
using UnityEngine;
using VContainer.Unity;
namespace MenuSystem
{
    public class MenuController : IStartable, IDisposable
    {

        private SceneController _sceneController;
        private IDisposable _messagePipeDisposable;
        public MenuController( SceneController sceneController)
        {
            _sceneController = sceneController;

        }

        public void Start()
        {

            _sceneController.OnShouldRefreshSceneList += OnRefreshList;

        }
        public void Dispose()
        {
            _messagePipeDisposable?.Dispose();
            _sceneController.OnShouldRefreshSceneList -= OnRefreshList;
        }

        private void OnRefreshList()
        {
            _sceneController.LoadSceneSOAndStatusList();
        }
    }
}