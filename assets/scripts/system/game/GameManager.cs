using Godot;
using System;
using System.Linq;

public partial class GameManager : Node2D
{
	private Camera2D _camera;
	private PlayerManager _playerManager;

	private readonly Vector2 _minZoom = new(0.5f, 0.5f);
	private readonly Vector2 _maxZoom = new(3.0f, 3.0f);
	private readonly float _zoomLerpSpeed = 5.0f;

    public override void _Ready()
	{
		if (_camera == null)
		{
			_camera = GetNode<Camera2D>("Camera2D");
        }
		if (_playerManager == null)
		{
			_playerManager = GetNode<PlayerManager>("PlayerManager");
        }
		_camera.GlobalPosition = GlobalPosition;
		_camera.Position = Vector2.Zero;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_playerManager.PresentPlayersList.Count == 0)
		{
			_camera.GlobalPosition = GlobalPosition;
			_camera.Zoom = _camera.Zoom.Lerp(new Vector2(1.0f, 1.0f), (float)delta * _zoomLerpSpeed);
            return;
        }


        var playerPositions = _playerManager.PresentPlayersList.Select(p => p.GlobalPosition);

		_camera.GlobalPosition = playerPositions.Aggregate((a, b) => a + b) / _playerManager.PresentPlayersList.Count;
		//_camera.Zoom = new Vector2()
		var zoom = Mathf.Sqrt(playerPositions.Max(p => p.DistanceTo(_camera.GlobalPosition))) / 200.0f;
		_camera.Zoom = _camera.Zoom.Lerp(new Vector2(zoom, zoom).Clamp(_minZoom, _maxZoom), (float)delta * _zoomLerpSpeed);
	}
}
