using UnityEngine;

namespace Collisions
{
    public struct AABB
    {
        public Vector2 Size;
        public Vector2 Position;
        
        public Vector2 HalfExtents => Size * 0.5f;

        public bool Contains(Vector2 point)
        {
            var halfExtents = HalfExtents;
            
            if (point.x > Position.x + halfExtents.x)
            {
                return false;
            }

            if (point.y > Position.x + halfExtents.x)
            {
                return false;
            }

            if (point.x < Position.x - halfExtents.x)
            {
                return false;
            }

            if (point.y < Position.y - halfExtents.y)
            {
                return false;
            }

            return true;
        }

        public bool Intersect(AABB aabb)
        {
            return Position.x + Size.x >= aabb.Position.x &&
                   aabb.Position.x + aabb.Size.x >= Position.x &&
                   Position.y + Size.y >= aabb.Position.y &&
                   aabb.Position.y + aabb.Size.y >= Position.y;
        }
    }
}