using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MessagePipe;
using Messages;
using SaveSystem;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace SceneSystem
{
    public enum SceneStatus
    {
        Available,
        Solved,
        Archived
    }
    public class SceneSOAndStatus
    {
        public SceneSOAndStatus(SceneSO sceneSO, SceneStatus sceneStatus)
        {
            SceneSO = sceneSO;
            SceneStatus = sceneStatus;
        }

        public SceneSO SceneSO { get; private set; }
        public SceneStatus SceneStatus { get; private set; }
    }
    public class SceneController : IStartable, IDisposable
    {

        private SceneDatabaeSO _sceneDatabaseSO;
        private ApplicationController _applicationController;
        public List<SceneSOAndStatus> SceneSOAndStatusList { get; private set; }

        private ICustomSaveClient _customSaveClient;
        public event Action OnShouldRefreshSceneList;
        public event Action OnCurrentSceneSolved;

        public SceneController(SceneDatabaeSO sceneDatabaeSO, ApplicationController applicationController, ICustomSaveClient customSaveClient)
        {
            _sceneDatabaseSO = sceneDatabaeSO;

            _customSaveClient = customSaveClient;
            _applicationController = applicationController;

        }

        public void Start()
        {
            SceneSOAndStatusList = new List<SceneSOAndStatus>();
            if (!_applicationController.IsInitialized)
            {
                _applicationController.InitializeEvent += LoadSceneSOAndStatusList;
            }
            else
            {
                LoadSceneSOAndStatusList();
            }
            _applicationController.OnProjectEventMessage += OnProjectMessageInvoke;
        }

        public void Dispose()
        {
            _applicationController.InitializeEvent -= LoadSceneSOAndStatusList;
            _applicationController.OnProjectEventMessage -= OnProjectMessageInvoke;
        }

        private void OnProjectMessageInvoke(ProjectEventMessage message)
        {
            if (ProjectMessageType.GlobalSceneAlert !=
             EnumHelper.Parse<ProjectMessageType>(message.messageType)) return;

            string currentSceneName = SceneManager.GetActiveScene().name;
            string mainMenuSceneString = "MenuScene";
            Debug.Log(message.message);
            if (currentSceneName == mainMenuSceneString)
            {
                OnShouldRefreshSceneList?.Invoke();
            }
            else if (_sceneDatabaseSO.FindSceneSOById(message.message).Name == currentSceneName)
            {
                OnCurrentSceneSolved?.Invoke();
            }
        }



        private class SceneStatusEntry : EntityData
        {
            [Newtonsoft.Json.JsonProperty("id")]
            public new string Id { get; set; }

            [Newtonsoft.Json.JsonProperty("name")]
            public string name { get; set; }

            [Newtonsoft.Json.JsonProperty("status")]
            public SceneStatus status { get; set; }
        }

        public async void LoadSceneSOAndStatusList()
        {
            if (!IsReadyToLoad())
            {
                return;
            }

            var entries = await _customSaveClient.Query<SceneStatusEntry>(
                new List<FieldFilter>
                {
                    new FieldFilter("itemType", "Scene", FieldFilter.OpOptions.EQ, true)

                },
                new HashSet<string> { "name", "status" }
            );
            if (entries == null)
            {
                return;
            }

            MapEntriesToSceneSOAndStatusList(entries);
        }

        private bool IsReadyToLoad()
        {
            if (_sceneDatabaseSO?.SceneSOList == null)
            {
                SceneSOAndStatusList = new List<SceneSOAndStatus>();
                return false;
            }
            return true;
        }


        private void MapEntriesToSceneSOAndStatusList(List<SceneStatusEntry> entries)
        {
            foreach (var entry in entries)
            {
                var sceneSO = _sceneDatabaseSO.FindSceneSOById(entry.Id);
                if (sceneSO == null)
                {
                    continue;
                }
                SceneSOAndStatusList.Add(new SceneSOAndStatus(sceneSO, entry.status));
            }

            LogSceneSOAndStatusList();
        }

        private void LogSceneSOAndStatusList()
        {
            Debug.Log($"[SceneProvider] Loaded {SceneSOAndStatusList.Count} scenes:");
            foreach (var item in SceneSOAndStatusList)
            {
                Debug.Log($"  - {item.SceneSO.ID}: {item.SceneStatus}");
            }
        }
        public SceneStatus GetCurrentSceneStatus()
        {
            return SceneSOAndStatusList.FirstOrDefault
        (s => s.SceneSO.Name == SceneManager.GetActiveScene().name)?.SceneStatus ?? SceneStatus.Available;
        }

    }

}