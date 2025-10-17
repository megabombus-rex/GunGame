using Godot;
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


        // these lines are left like this for clarity
        var playerPositions = _playerManager.PresentPlayersList.Select(p => p.GlobalPosition);
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
