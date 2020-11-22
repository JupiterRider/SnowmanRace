using Godot;
using System;

public class Can : Spatial
{
    public override void _Process(float delta)
    {
        RotateY(delta);
    }

    private void OnCanBodyEntered(Node body)
    {
        if (body is Player)
        {
            ((Player)body).AddCan();
            QueueFree();
        }
    }
}