using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager
{
    private Texture2D texture;

    public CursorManager()
    {
        texture = Resources.Load("cursor") as Texture2D;
        Enter();
    }

    void Enter()
    {
        Cursor.SetCursor(texture,new Vector2(texture.width/2,texture.height/2),CursorMode.Auto);
    }

    void Exit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
