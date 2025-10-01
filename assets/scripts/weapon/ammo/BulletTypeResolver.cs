using Godot;
using System;

namespace GunGame.assets.scripts.weapon.ammo
{
    // rockets etc will probably also be bullet types
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
                BulletType.Medium => scene.Instantiate<Bullet>(),
                BulletType.Large => scene.Instantiate<Bullet>(),
                _ => throw new NotImplementedException()
            };
        }

        public static string GetBulletTexturePath(BulletType type) 
        {
            return type switch
            {
                BulletType.Small => "res://assets/sprites/weapon/ammo/Bullet-S_64x64.png",
                BulletType.Medium => "res://assets/sprites/weapon/ammo/Bullet-M_64x64.png",
                BulletType.Large => "res://assets/sprites/weapon/ammo/Bullet-L_64x64.png",
                _ => throw new NotImplementedException()
            };
        }
    }
}
