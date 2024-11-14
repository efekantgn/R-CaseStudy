using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event Channels/Scene Event Channel")]
public class SceneEventChannel : ScriptableObject
{
    public UnityAction OnAppQiutRequested;
    public void RaiseEvent() => OnAppQiutRequested?.Invoke();


    public UnityAction<string> OnSceneLoadRequested;
    public void RaiseEvent(string sceneName) => OnSceneLoadRequested?.Invoke(sceneName);
}
