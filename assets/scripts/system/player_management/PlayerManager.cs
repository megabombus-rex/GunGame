using Godot;
using GunGame.assets.scripts.misc;
using GunGame.assets.scripts.system.player_management;
using System.Collections.Generic;

public partial class PlayerManager : Node2D
{
    private const int MAX_PLAYER_COUNT = 2;
    private PackedScene _playerScene = GD.Load<PackedScene>("res://scenes/player/player.tscn");

    private List<PlayerMovementRigidbody> _presentPlayersList = new List<PlayerMovementRigidbody>(MAX_PLAYER_COUNT);
    private int _currentIndex = 0;

    public override void _Ready()
    {
        if (_playerScene != null)
        {
            _playerScene = GD.Load<PackedScene>("res://scenes/player/player.tscn");
        }
    }

    public override void _Process(double delta)
    {
        // temporarily like this
        // make this work so given player joins on different button (pickup item or shoot action)
        if (Input.IsActionJustPressed("Add_player"))
        {
            if (_presentPlayersList.Count < MAX_PLAYER_COUNT)
            {
                var player = _playerScene.Instantiate<PlayerMovementRigidbody>();
                var movement = _playerMovementMappings[_currentIndex];
                var display = _playerDisplayAndPhysics[_currentIndex % 2];
                var stats = _playerStats[_currentIndex % 2];

                var preset = new PlayerPreset()
                {
                    MovementMapping = movement,
                    DisplayAndPhysics = display,
                    Stats = stats
                };

                player.Initialize(preset);
                _currentIndex++;
            }
        }
        if (Input.IsActionJustPressed("Remove_player"))
        {
            if (_presentPlayersList.Count > 0)
            {
                _presentPlayersList.RemoveAt(_currentIndex);
                _currentIndex--;
            }
        }
    }

    // preset id may be used as player number
    private readonly Dictionary<int, PlayerMovementMapping> _playerMovementMappings = new()
    {
        { 0, new PlayerMovementMapping()
            {
                DeviceId = 0,
                JumpCommand = "p1_jump",
                MoveLeftCommand = "p1_left",
                MoveRightCommand = "p1_right",
                PickUpItemCommand = "p1_pickup"
            }
        },
        { 1, new PlayerMovementMapping()
            {
                DeviceId = 0,
                JumpCommand = "p2_jump",
                MoveLeftCommand = "p2_left",
                MoveRightCommand = "p2_right",
                PickUpItemCommand = "p2_pickup"
            }
        },
        { 2, new PlayerMovementMapping()
            {
                DeviceId = 1,
                JumpCommand = "joypad_jump",
                MoveLeftCommand = "joypad_left",
                MoveRightCommand = "joypad_right",
                PickUpItemCommand = "joypad_pickup"
            }
        },
        { 3, new PlayerMovementMapping()
            {
                DeviceId = 1,
                JumpCommand = "joypad_jump",
                MoveLeftCommand = "joypad_left",
                MoveRightCommand = "joypad_right",
                PickUpItemCommand = "joypad_pickup"
            }
        }
    };

    private readonly Dictionary<int, PlayerDisplayAndPhysics> _playerDisplayAndPhysics = new()
    {
        { 0, new PlayerDisplayAndPhysics()
            {
                ShapeType = ShapeType.Circle,
                ShapeDetails = new System.Numerics.Vector2()
                {
                    X = 25.0f,
                    Y = 25.0f
                },
                AnimationResoucrePath = "res://scenes/kernel-animations.tres"
            }
        },
        { 1, new PlayerDisplayAndPhysics()
            {
                ShapeType = ShapeType.Capsule,
                ShapeDetails = new System.Numerics.Vector2()
                {
                    X = 20.0f,
                    Y = 35.0f
                },
                AnimationResoucrePath = "res://scenes/kernel-animations.tres"
            }
        }
    };

    private readonly Dictionary<int, PlayerStats> _playerStats = new()
    {
        { 0, new PlayerStats()
            {
                Mass = 1.0f,
                MaxSpeed = 40000,
                Acceleration = 1.0f,
                JumpForce = -400.0f,
                GroundRayLength = 75.0f
            }
        },
        { 1, new PlayerStats()
            {
                Mass = 2.0f,
                MaxSpeed = 30000,
                Acceleration = 1.5f,
                JumpForce = -1000.0f,
                GroundRayLength = 70.0f
            }
        }
    };
}
