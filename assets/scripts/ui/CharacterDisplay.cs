using Godot;
using System;

public partial class CharacterDisplay : PanelContainer
{
    // animation
	private float _hpChangeSpeedPerSecond = 10.0f;


	// uninitialized value
	private int _playerId = -1;
    private int _currentLives = 0;
    private float _currentHp = 0.0f;
    private float _hpToAchieve = 0.0f;

    private TextureRect _playerIconTexture;
    private Label _hitpointsLabel;
	private Label _livesLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (_playerIconTexture == null)
		{
			_playerIconTexture = GetNode<TextureRect>("PlayerIcon");
		}

        if (_hitpointsLabel == null)
        {
			_hitpointsLabel = GetNode<Label>("HPBox/Hitpoints");
        }

        if (_livesLabel == null)
        {
            _livesLabel = GetNode<Label>("HPBox/Lives");
        }
    }

	private void HandleHitpointsChange(int playerId, int oldValue, int newValue)
	{
		if (playerId != _playerId)
		{
			return;
		}
		_currentHp = oldValue;
		_hpToAchieve = newValue;
		GD.Print($"Player {_playerId} hit, Hp to achieve: {_hpToAchieve}, current display: {_currentHp.ToString("0.00")}");
    }

    private void AnimateHpChange(double delta)
	{
		if (_currentHp.ToString("0.00") != _hpToAchieve.ToString("0.00"))
		{
			_currentHp = Mathf.Min(_currentHp + (float)delta * _hpChangeSpeedPerSecond, _hpToAchieve);
			GD.Print($"Hp to achieve: {_hpToAchieve}, current display: {_currentHp.ToString("0.00")}");
			_hitpointsLabel.Text = $"{_currentHp.ToString("0.00")}%";
        }
    }

	public void Initialize(PlayerMovementRigidbody player, string characterSpriteTexture)
	{
		_playerId = player.PlayerId;
		_currentHp = 0.0f;
        _currentLives = player.LivesCount;

        if (_playerIconTexture == null)
        {
            _playerIconTexture = GetNode<TextureRect>("PlayerIcon");
        }

        if (_hitpointsLabel == null)
        {
            _hitpointsLabel = GetNode<Label>("HPBox/Hitpoints");
        }

        if (_livesLabel == null)
        {
            _livesLabel = GetNode<Label>("HPBox/Lives");
        }

        player.HealthChanged += HandleHitpointsChange;
        player.PlayerDied += HandlePlayerDeath;

        _hitpointsLabel.Text = $"{_currentHp.ToString("0.00")}%";
        _livesLabel.Text = $"x{_currentLives}";
        _playerIconTexture.Texture = GD.Load<Texture2D>(characterSpriteTexture);
    }

    private void HandlePlayerDeath(int playerId, int newValue)
    {
        if (playerId != _playerId)
        {
            return;
        }
        if (newValue < 0) 
        {
            _currentLives = 0;
        }
        else
        {
            _currentLives = newValue;
        }
        _livesLabel.Text = $"x{_currentLives}";
    }

    public override void _Process(double delta)
	{
		AnimateHpChange(delta);
	}
}
