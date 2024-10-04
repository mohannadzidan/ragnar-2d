using UnityEditor.Build.Content;
using UnityEngine;

public partial class OverCursor : MonoBehaviour
{
    public CursorType type = CursorType.Default;

    public void OnMouseEnter()
    {
        GameManager.current.SetCursor(type);
    }

    public void OnMouseExit()
    {
        GameManager.current.ResetCursor();
    }

    void OnDestroy() {
        GameManager.current.ResetCursor();
    }

}
