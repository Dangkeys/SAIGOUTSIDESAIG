using System;
using UnityEngine;

namespace Messages
{
    public class ProjectEventMessage
    {
        public string messageType;
        public string message;
    }
    public enum ProjectMessageType
    {
        GlobalSceneAlert
    }
}
