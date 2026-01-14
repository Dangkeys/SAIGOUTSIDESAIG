using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using VContainer;

namespace SceneController
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
    public class SceneController : MonoBehaviour
    {

        private SceneDatabaeSO _sceneDatabaseSO;
        private ApplicationManager _applicationManager;
        public List<SceneSOAndStatus> SceneSOAndStatusList { get; private set; }

        [Inject]
        private void Construct(SceneDatabaeSO sceneDatabaeSO, ApplicationManager applicationManager)
        {
            _sceneDatabaseSO = sceneDatabaeSO;
            _applicationManager = applicationManager;
        }
        void Start()
        {
            _applicationManager.InitializeEvent += OnInitialized;
        }

        private void OnInitialized()
        {
            LoadSceneSOAndStatusList();
            _applicationManager.InitializeEvent -= OnInitialized;
        }

        private class SceneStatusEntry
        {
            public string id;
            public string name;
            public string status;
        }

        public async void LoadSceneSOAndStatusList()
        {
            if (!IsReadyToLoad())
            {
                return;
            }

            var entries = await FetchSceneEntriesFromCloud();
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

        private async Task<List<SceneStatusEntry>> FetchSceneEntriesFromCloud()
        {
            SceneSOAndStatusList = new List<SceneSOAndStatus>();
            var keys = new HashSet<string> { "data" };
            var results = await CloudSaveService.Instance.Data.Custom.LoadAsync("CONFIG_SCENES", keys);

            if (!results.TryGetValue("data", out var item))
            {
                return null;
            }

            return item.Value.GetAs<List<SceneStatusEntry>>();
        }

        private void MapEntriesToSceneSOAndStatusList(List<SceneStatusEntry> entries)
        {
            foreach (var entry in entries)
            {
                var sceneSO = _sceneDatabaseSO.FindSceneSOById(entry.id) ?? _sceneDatabaseSO.FindSceneSOByName(entry.name);
                if (sceneSO == null)
                {
                    continue;
                }

                var sceneStatus = ParseSceneStatus(entry.status);
                SceneSOAndStatusList.Add(new SceneSOAndStatus(sceneSO, sceneStatus));
            }

            LogSceneSOAndStatusList();
        }

        private void LogSceneSOAndStatusList()
        {
            Debug.Log($"[SceneProvider] Loaded {SceneSOAndStatusList.Count} scenes:");
            foreach (var item in SceneSOAndStatusList)
            {
                Debug.Log($"  - {item.SceneSO.Name}: {item.SceneStatus}");
            }
        }



        private SceneStatus ParseSceneStatus(string statusString)
        {
            if (Enum.TryParse<SceneStatus>(statusString, ignoreCase: true, out var sceneStatus))
            {
                return sceneStatus;
            }
            return SceneStatus.Archived; // Default fallback
        }

    }

}