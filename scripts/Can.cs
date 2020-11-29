using Godot;
using System;

public class Can : Spatial
{
    AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(float delta)
    {
        RotateY(delta);
    }

    private void OnCanBodyEntered(Node body)
    {
        if (body is Player)
        {
            ((Player)body).AddCan();
            animationPlayer.Play("Taken");
        }
    }

    private void OnAnimationPlayerAnimationFinished(string name)
    {
        QueueFree();
    }
}