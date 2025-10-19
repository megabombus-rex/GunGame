using Godot;
using GunGame.assets.scripts.misc;
using GunGame.assets.scripts.system.player_management;
using System.Collections.Generic;
using System.Linq;

public struct PlayerGlobalPosition
{
    public int PlayerId {  get; init; }
    public Vector2 GlobalPosition { get; init; }
}

public partial class PlayerManager : Node2D
{
    private const int MAX_PLAYER_COUNT = 2;
    private PackedScene _playerScene = GD.Load<PackedScene>("res://scenes/player/player.tscn");
    private PackedScene _playerDisplaysScene = GD.Load<PackedScene>("res://scenes/ui/character_display.tscn");

    // this should be a spawn point list, maybe based on map, or a strategy, idk yet
    private Vector2 _spawnPoint = new Vector2(500.0f, 200.0f);

    [Export] public int StartingPlayerLivesCount { get; set; } = 3;

    public List<PlayerMovementRigidbody> PresentPlayersList { get { return _presentPlayersList; } }
    private List<PlayerMovementRigidbody> _presentPlayersList = new List<PlayerMovementRigidbody>(MAX_PLAYER_COUNT);
    private int _currentIndex = 0;

    private List<CharacterDisplay> _characterDisplayList = new List<CharacterDisplay>(MAX_PLAYER_COUNT);
    private HBoxContainer _characterDisplaysGrid;

    public override void _Ready()
    {
        if (_playerScene == null)
        {
            _playerScene = GD.Load<PackedScene>("res://scenes/player/player.tscn");
        }
        if (_playerDisplaysScene == null)
        {
            _playerDisplaysScene = GD.Load<PackedScene>("res://scenes/ui/character_display.tscn");
        }

        if (_characterDisplaysGrid == null)
        {
            _characterDisplaysGrid = GetNode<HBoxContainer>("UI/CharacterDisplayGrid");
            var gridChildren = _characterDisplaysGrid.GetChildren();

            foreach (var child in gridChildren)
            {
                if (child is CharacterDisplay)
                {
                    child.QueueFree();
                }
            }
            _characterDisplaysGrid.Position = new Vector2(0, 0);
            _characterDisplaysGrid.GlobalPosition = GlobalPosition;
            _characterDisplaysGrid.GrowHorizontal = Control.GrowDirection.End;
            var size = GetViewport().GetVisibleRect().Size;
            _characterDisplaysGrid.Position = new Vector2((size.X / 2) - 128, size.Y - 128);
            _characterDisplaysGrid.Scale = new Vector2(2.0f, 2.0f);
        }
    }

    public List<PlayerGlobalPosition> GetPlayerPositions()
    {
        return _presentPlayersList
            .Select(p => new PlayerGlobalPosition() { 
                GlobalPosition = p.GlobalPosition, 
                PlayerId = p.PlayerId })
            .ToList();
    }

    public void KillPlayer(int playerId)
    {
        var player = _presentPlayersList.Where(p => p.PlayerId == playerId).FirstOrDefault();

        if (player == null)
        {
            return;
        }
        player.Died();

        if (player.LivesCount > 0)
        {
            player.GlobalPosition = _spawnPoint;
            return;
        }
        _presentPlayersList.Remove(player);
        _currentIndex--;
        player.QueueFree();
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
                    LivesCount = StartingPlayerLivesCount
                };

                player.Initialize(preset);
                player.GlobalPosition = _spawnPoint;

                GetTree().Root.AddChild(player);
                _presentPlayersList.Add(player);
                var characterDisplay = _playerDisplaysScene.Instantiate<CharacterDisplay>();

                characterDisplay.Initialize(player, preset.DisplayAndPhysics.CharacterPortraitTexturePath);
                _characterDisplayList.Add(characterDisplay);
                _characterDisplaysGrid.AddChild(characterDisplay);
                GD.Print($"Display list: {_characterDisplayList.Count}. Player list: {_presentPlayersList.Count}.");
                _currentIndex++;
            }
        }
        //if (Input.IsActionJustPressed("Remove_player"))
        //{
        //    if (_presentPlayersList.Count > 0)
        //    {
        //        _currentIndex--;
        //        _presentPlayersList[_currentIndex].QueueFree();
        //        _presentPlayersList.RemoveAt(_currentIndex);
        //        _characterDisplayList[_currentIndex].QueueFree();
        //        _characterDisplayList.RemoveAt(_currentIndex);
        //    }
        //}
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
                AnimationResoucrePath = "res://assets/animations/character/kernel-animations.tres",
                CharacterPortraitTexturePath = "res://assets/sprites/kernel/Kernel-portrait-64x64.png"
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
                AnimationResoucrePath = "res://assets/animations/character/donut-man-animations.tres",
                CharacterPortraitTexturePath = "res://assets/sprites/character/donut-man/Donut-man-portrait-64x64.png"
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
