using Godot;
using System.Collections.Generic;
using System.Linq;

public enum ZoomCalculationFunction
{
    Linear,
    Sigmoid,
    Exponential
}

public partial class GameManager : Node2D
{
	private Camera2D _camera;
	private PlayerManager _playerManager;

	private Rect2 _mapBorder = new Rect2() { Position = new Vector2(-200.0f, 0.0f), End = new Vector2(1600.0f, 1800.0f) };

	private readonly Vector2 _minZoom = new(0.5f, 0.5f);
	private readonly Vector2 _defaultZoom = new(1.0f, 1.0f);
	private readonly Vector2 _maxZoom = new(3.0f, 3.0f);
	private readonly float _minZoomDistance = 25.0f;
	private readonly float _maxZoomDistance = 400.0f;
	private readonly float _zoomLerpSpeed = 5.0f;

	[Export] public ZoomCalculationFunction ZoomFunc = ZoomCalculationFunction.Sigmoid;

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
		_playerManager.Position = new Vector2(0.0f, 0.0f);
		_playerManager.GlobalPosition = GlobalPosition;
		_camera.GlobalPosition = GlobalPosition;
		_camera.Position = Vector2.Zero;
	}

	public override void _Process(double delta)
	{
		if (_playerManager.PresentPlayersList.Count == 0)
		{
			_camera.GlobalPosition = GlobalPosition;
			_camera.Zoom = _camera.Zoom.Lerp(new Vector2(1.0f, 1.0f), (float)delta * _zoomLerpSpeed);
			return;
		}

        var playerPositions = _playerManager.GetPlayerPositions();
		ManageCamera(delta, playerPositions.Select(p => p.GlobalPosition));
		ManagePlayerDeaths(playerPositions);
	}

	private void ManagePlayerDeaths(IEnumerable<PlayerGlobalPosition> playerGlobalPositions) 
	{
		var playersOutsideBordersIds = playerGlobalPositions.Where(p => !_mapBorder.HasPoint(p.GlobalPosition)).Select(p => p.PlayerId);

		foreach (var playerId in playersOutsideBordersIds) {
			_playerManager.KillPlayer(playerId);
		}
	}

	private void ManageCamera(double delta, IEnumerable<Vector2> playerPositions)
	{
        // these lines are left like this for clarity
        _camera.GlobalPosition = playerPositions.Aggregate((a, b) => a + b) / _playerManager.PresentPlayersList.Count;

        var maxDistance = playerPositions.Max(p => p.DistanceTo(_camera.GlobalPosition));
        _camera.Zoom = _camera.Zoom.Lerp(GetDistanceVector(maxDistance).Clamp(_minZoom, _maxZoom), (float)delta * _zoomLerpSpeed);
    }

	// distance is to camera
	private Vector2 GetDistanceVector(float distance)
	{
		if (distance > _maxZoomDistance)
		{
			return _minZoom;
		}
		if (distance < _minZoomDistance)
		{
			return _maxZoom;
		}
		// linear interpolation between min and max zoom based on ratio
		var ratio = (distance - _minZoomDistance) / (_maxZoomDistance - _minZoomDistance);

		var multiplier = ZoomFunc switch
		{
			ZoomCalculationFunction.Linear => ratio,
			ZoomCalculationFunction.Sigmoid => Sigmoid(ratio),
			ZoomCalculationFunction.Exponential => Exponential(ratio),
			_ => ratio
        };

        return (_minZoom - _maxZoom) * multiplier + _maxZoom;
    }

	private float Sigmoid(float ratio)
	{
        float steepness = 10.0f;
		return 1.0f / (1.0f + Mathf.Exp(-steepness * (ratio - 0.5f)));
    }

    private float Exponential(float ratio)
    {
        float exponent = 2.0f;
        return Mathf.Pow(ratio, exponent);
    }

}
