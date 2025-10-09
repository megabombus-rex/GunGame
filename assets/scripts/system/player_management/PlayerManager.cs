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
        // also the spawn points can be fixed based on the map
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
                    Stats = stats,
                    PlayerNumber = _currentIndex + 1,
                };

                player.Initialize(preset);
                player.GlobalPosition = new Vector2(500.0f, 200.0f);

                GetTree().Root.AddChild(player);
                _presentPlayersList.Add(player);
                _currentIndex++;
            }
        }
        if (Input.IsActionJustPressed("Remove_player"))
        {
            if (_presentPlayersList.Count > 0)
            {
                _currentIndex--;
                _presentPlayersList[_currentIndex].QueueFree();
                _presentPlayersList.RemoveAt(_currentIndex);
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
                PickUpItemCommand = "p1_pickup",
                UseItemCommand = "p1_shoot_weapon"
            }
        },
        { 1, new PlayerMovementMapping()
            {
                DeviceId = 0,
                JumpCommand = "p2_jump",
                MoveLeftCommand = "p2_left",
                MoveRightCommand = "p2_right",
                PickUpItemCommand = "p2_pickup",
                UseItemCommand = "p2_shoot_weapon"
            }
        },
        { 2, new PlayerMovementMapping() // joypad mapping not added yet
            {
                DeviceId = 1,
                JumpCommand = "joypad_jump",
                MoveLeftCommand = "joypad_left",
                MoveRightCommand = "joypad_right",
                PickUpItemCommand = "joypad_pickup",
                UseItemCommand = "joypad_shoot_weapon"
            }
        },
        { 3, new PlayerMovementMapping()
            {
                DeviceId = 2,
                JumpCommand = "joypad_jump",
                MoveLeftCommand = "joypad_left",
                MoveRightCommand = "joypad_right",
                PickUpItemCommand = "joypad_pickup",
                UseItemCommand = "joypad_shoot_weapon"
            }
        }
    };

    private readonly Dictionary<int, PlayerDisplayAndPhysics> _playerDisplayAndPhysics = new()
    {
        { 0, new PlayerDisplayAndPhysics()
            {
                ShapeType = ShapeType.Circle,
                ShapeDetails = new Vector2()
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
                ShapeDetails = new Vector2()
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
                Acceleration = 1.2f,
                JumpForce = -600.0f,
                GroundRayLength = 80.0f
            }
        }
    };
}
