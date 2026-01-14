using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using VContainer.Unity;
using IInitializable = VContainer.Unity.IInitializable;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.CloudCode.Subscriptions;
using Newtonsoft.Json;
using Unity.Services.CloudCode;

public class ApplicationManager : IInitializable, ITickable, IDisposable
{
    private bool _initialized;
    public event Action InitializeEvent;

    public async void Initialize()
    {
#if UNITY_EDITOR
        // Skip Unity Services when running in EditMode or Tests
        if (UnityEngine.Application.isEditor && !UnityEngine.Application.isPlaying)
        {
            Debug.Log("[GameBootFlow] Skipped UnityServices initialization in editor/test mode.");
            return;
        }
#endif

        // Prevent re-initialization
        if (_initialized)
            return;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            await SignUpAnonymouslyIfNeeded();
            await SubscribeToProjectMessages();
            _initialized = true;
            InitializeEvent.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[GameBootFlow] Initialization failed: {ex.GetType().Name} - {ex.Message}");
        }
    }
    Task SubscribeToProjectMessages()
    {
        var callbacks = new SubscriptionEventCallbacks();
        callbacks.MessageReceived += @event =>
        {
            Debug.Log(DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK"));
            Debug.Log($"Got project subscription Message: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
        };
        callbacks.ConnectionStateChanged += @event =>
        {
            Debug.Log($"Got project subscription ConnectionStateChanged: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
        };
        callbacks.Kicked += () =>
        {
            Debug.Log($"Got project subscription Kicked");
        };
        callbacks.Error += @event =>
        {
            Debug.Log($"Got project subscription Error: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
        };
        return CloudCodeService.Instance.SubscribeToProjectMessagesAsync(callbacks);
    }
    public void Tick() { /* runs every frame */ }
    public void Dispose()
    {
        try
        {
            // Skip if Unity Services not initialized
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.Log("[GameBootFlow] Dispose skipped (UnityServices not initialized).");
                return;
            }

            // Sign out only if actually signed in
            if (AuthenticationService.Instance?.IsSignedIn ?? false)
            {
                AuthenticationService.Instance.SignOut();
                Debug.Log("[GameBootFlow] Signed out on dispose.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[GameBootFlow] Dispose skipped due to: {ex.GetType().Name} - {ex.Message}");
        }
    }

    private async Task SignUpAnonymouslyIfNeeded()
    {
        if (AuthenticationService.Instance?.IsSignedIn ?? false)
        {
            Debug.Log("[GameBootFlow] Already signed in.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"[GameBootFlow] Signed in anonymously! PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogWarning($"[GameBootFlow] Auth failed: {ex.Message}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogWarning($"[GameBootFlow] Request failed: {ex.Message}");
        }
    }
}
