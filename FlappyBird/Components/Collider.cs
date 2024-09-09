using System.Numerics;

namespace FlappyBird.Components;

public class Collider {
    public Rectangle Area;

    public void UpdateArea(Vector2 pos, Texture2D sprite) {
        Area.X = pos.X;
        Area.Y = pos.Y;

        Area.Width = sprite.Width;
        Area.Height = sprite.Height;        
    }

    public void UpdateArea(float x, float y, float width, float height) {
        Area.X = x;
        Area.Y = y;

        Area.Width = width;
        Area.Height = height;
    }

    public void UpdateArea(float x, float y, Texture2D sprite) 
        => UpdateArea(new Vector2(x, y), sprite);

    public bool Overlaps(Collider other) 
        => Raylib.CheckCollisionRecs(Area, other.Area);

    public bool Overlaps(Vector2 mouse) 
        => Raylib.CheckCollisionPointRec(mouse, Area);
}
