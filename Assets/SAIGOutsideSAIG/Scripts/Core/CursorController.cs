using UnityEngine;

public static class CursorController
{
    public static void ShowCursor(bool shouldShow = true)
    {
        Cursor.lockState = shouldShow ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
