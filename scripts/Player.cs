using Godot;
using System;

public class Player : KinematicBody
{
    private AnimationPlayer animationPlayer;
    private CollisionShape collisionShape;
    private Spatial camBase;
    private Spatial camBase2;
    private Spatial snowman;
    private Label label;

    private Vector3 velocity = Vector3.Zero;
    private int speed = 0;
    private float viewAngle = 0f;
    private float gravitySpeed = 0f;
    private int cans = 0;
    private float time = 0f;

    private const float MOUSE_SENSITIVITY = 0.05f;
    private const float GRAVITY = 78.4f;
    private const int JUMP_HEIGHT = 30;

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape>("CollisionShape");
        animationPlayer = GetNode<AnimationPlayer>("Snowman2/AnimationPlayer");
        camBase = GetNode<Spatial>("CamBase");
        camBase2 = GetNode<Spatial>("CamBase/CamBase2");
        snowman = GetNode<Spatial>("Snowman2");
        label = GetNode<Label>("Label");
    }

    public override void _Process(float delta)
    {
        Movement(delta);
        Animation();

        if (Translation.y < -10)
        {
            Translation = new Vector3(0f, 0.75f, 0f);
        }

        var strTime = ((long)time).ToString();

        if (cans < 6)
        {
            time += delta;
            var strCans = cans.ToString();
            label.Text = " Time: " + strTime + "sec" + "\r\n" + " Cans: " + strCans;
        }
        else
        {
            label.Text = " You got all 6 cans in " + strTime + " seconds!" + "\r\n" + " You can leave now ...";
        }
    }

    private void Movement(float delta)
    {
        var direction = new Vector3();
        var rot = new Vector3();

        var aim = camBase.GlobalTransform.basis;

        if (Input.IsActionPressed("DOWN"))
        {
            direction -= aim.z;
            snowman.RotationDegrees = camBase.RotationDegrees - new Vector3(0, 180, 0);
            rot.y = 45;
        }
        else if (Input.IsActionPressed("UP"))
        {
            direction += aim.z;
            snowman.Rotation = camBase.Rotation;
            rot.y = -45;
        }

        if (Input.IsActionPressed("RIGHT"))
        {
            direction -= aim.x;
            snowman.RotationDegrees = (camBase.RotationDegrees - new Vector3(0, 90, 0)) - rot;
        }
        else if (Input.IsActionPressed("LEFT"))
        {
            direction += aim.x;
            snowman.RotationDegrees = (camBase.RotationDegrees - new Vector3(0, -90, 0)) + rot;
        }

        collisionShape.RotationDegrees = snowman.RotationDegrees;

        if (Input.IsActionPressed("SHIFT"))
        {
            speed = 14;
            animationPlayer.PlaybackSpeed = 1.5f;
        }
        else if (Input.IsActionPressed("CTRL"))
        {
            speed = 3;
            animationPlayer.PlaybackSpeed = 0.5f;
        }
        else
        {
            speed = 8;
            animationPlayer.PlaybackSpeed = 1;
        }

        gravitySpeed -= GRAVITY * delta;
        velocity = direction.Normalized() * speed;
        velocity.y = gravitySpeed;

        if (Input.IsActionJustPressed("SPACE") && IsOnFloor())
        {
            velocity.y = JUMP_HEIGHT;
        }

        gravitySpeed = MoveAndSlide(velocity, Vector3.Up).y;
    }

    public override void _Input(InputEvent @event)
    {
        Input.SetMouseMode(Input.MouseMode.Captured);

        if (@event is InputEventMouseMotion EventMouseMotion)
        {
            camBase.RotateY(Mathf.Deg2Rad(-EventMouseMotion.Relative.x * MOUSE_SENSITIVITY));

            var viewChange = EventMouseMotion.Relative.y * MOUSE_SENSITIVITY;
            if (viewChange + viewAngle < 45 && viewChange + viewAngle > -90)
            {
                camBase2.RotateX(Mathf.Deg2Rad(viewChange));
                viewAngle += viewChange;
            }
        }

        if (Input.IsActionJustPressed("Escape"))
        {
            GetTree().Quit();
        }

        if (Input.IsActionJustPressed("R"))
        {
            GetTree().ReloadCurrentScene();
        }
    }

    private void Animation()
    {
        if (IsOnFloor() && (velocity.x != 0 || velocity.z != 0))
        {
            animationPlayer.Play("Move");
        }
        else
        {
            animationPlayer.Stop();
        }
    }

    private void OnTrampolineBodyEntered(Node body)
    {
        if (body is Player)
        {
            gravitySpeed = 100;
        }
    }

    public void AddCan()
    {
        cans += 1;
    }
}
