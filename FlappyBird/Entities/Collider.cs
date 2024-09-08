using System.Numerics;

namespace FlappyBird.Entities;

public class Collider {
    public Rectangle Area;

    public void UpdateArea(Vector2 pos, Texture2D sprite) {
        Area.X = pos.X;
        Area.Y = pos.Y;

        Area.Width = sprite.Width;
        Area.Height = sprite.Height;        
    }

    public void UpdateArea(float x, float y, Texture2D sprite) 
        => UpdateArea(new Vector2(x, y), sprite);
}
