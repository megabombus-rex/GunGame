using Godot;
using System;
using System.Linq;

public partial class GameManager : Node2D
{
	private Camera2D _camera;
	private PlayerManager _playerManager;

	private readonly Vector2 _minZoom = new(0.5f, 0.5f);
	private readonly Vector2 _defaultZoom = new(1.0f, 1.0f);
	private readonly Vector2 _maxZoom = new(3.0f, 3.0f);
	private readonly float _minZoomDistance = 200.0f;
	private readonly float _maxZoomDistance = 1000.0f;
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

        // max zoom = min distance => 200.0f - 3.0f
        // min zoom = max distance => 1000.0f - 0.5f
        var maxDistance = playerPositions.Max(p => p.DistanceTo(_camera.GlobalPosition));

		var targetZoom = GetDistanceVector(maxDistance);


		_camera.Zoom = _camera.Zoom.Lerp(targetZoom.Clamp(_minZoom, _maxZoom), (float)delta * _zoomLerpSpeed);
	}

	// distance is to camera
	private Vector2 GetDistanceVector(float distance)
	{
		GD.Print(distance);
		distance = distance - _minZoomDistance;
        if (distance > _maxZoomDistance)
        {
            return _minZoom;
        }
		if (distance < _minZoomDistance)
		{
			return _maxZoom;
		}
		var ratio = distance / (_maxZoomDistance - _minZoomDistance);
		var tar = _minZoom + (_maxZoom - _minZoom) * ratio;

		GD.Print(tar);
		return tar;
    }
}
