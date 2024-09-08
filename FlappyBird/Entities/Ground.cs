using System.Numerics;

namespace FlappyBird.Entities;

public class Ground : Entity {
    private readonly Texture2D Sprite = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "base.png"));
    private readonly float Speed = 2.0f;

    private Vector2 _pos = Vector2.Zero;

    public Collider Collider = new Collider();

    public Ground(bool second) {
        if (second) _pos.X = Sprite.Width;
        Collider.UpdateArea(0, FlappyBird.GameSize.Y - Sprite.Height, Sprite);
    }

    public void Initialise()
        => _pos.Y = FlappyBird.GameSize.Y - Sprite.Height;

    public void Update() {
        _pos.X -= Speed;
        if (_pos.X < 0 - Sprite.Width) 
            _pos.X = FlappyBird.GameSize.X; 
    }
 
    public void Draw() {
        Raylib.DrawTextureV(Sprite, _pos, Color.White);

        if (FlappyBird.DEV_MODE)
            Raylib.DrawRectangleLinesEx(Collider.Area, 2, Color.Red);
    } 

    public void OnExit()
        => Raylib.UnloadTexture(Sprite);
}
