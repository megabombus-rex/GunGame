using Godot;
using System;

namespace GunGame.assets.scripts.misc
{

    public static class ShapeCreator
    {
        public static Shape2D GetShapeBasedOnShapeType(ShapeType type, System.Numerics.Vector2 shapeDetails)
        {
            return type switch
            {
                ShapeType.Circle => new CircleShape2D() { Radius = shapeDetails.X },
                ShapeType.Rectangle => new RectangleShape2D() { Size = new Godot.Vector2(shapeDetails.X, shapeDetails.Y) },
                ShapeType.Capsule => new CapsuleShape2D() { Radius = shapeDetails.X, Height = shapeDetails.Y },
                _ => throw new NotImplementedException("Shape not implemented.")
            };
        }
    }
}
