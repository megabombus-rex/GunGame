using Godot;
using System;

namespace GunGame.assets.scripts.weapon.ammo
{
    public enum BulletType
    {
        Small = 0, 
        Medium = 1,
        Large = 2
    }

    public static class BulletTypeResolver
    {
        public static Bullet InstantiateBullet(PackedScene scene, BulletType type)
        {
            return type switch
            {
                BulletType.Small => scene.Instantiate<Bullet>(),
                //BulletType.Medium => scene.Instantiate<MediumBullet>(), // not implemented yet
                //BulletType.Large => scene.Instantiate<LargeBullet>(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
