using System.Numerics;

namespace FlappyBird.Components;

public class Sparkle {
    private readonly Texture2D SprakleSheet = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "sparkle-sheet.png"));
    
    private Rectangle _frameRect;

    private Vector2 _pos;

    private float _frameTimer = 0;
    private int _frame = 0;
    private bool _done = false;
    private bool _reverse = false;

    public Sparkle() {
        _frameRect = new Rectangle(0, 0, SprakleSheet.Width / 3, SprakleSheet.Height);
    } 

    public void Update(float x, float y, float width, float height) {
        _frameTimer += Raylib.GetFrameTime();
        if (_frameTimer >= 0.15f) {
            _frameTimer = 0;

            if (_done) {
                _done = false;
                _pos = new Vector2(
                    Random.Shared.Next((int)x, (int)x + (int)width),
                    Random.Shared.Next((int)y, (int)y + (int)height)
                );

                return;
            }

            if (!_reverse) _frame++; else _frame--;

            if (_frame >= 2) 
                _reverse = true;
            
            if (_frame <= 0) {
                _reverse = false;
                _done = true;
            }

            _frameRect.X = _frame * SprakleSheet.Width / 3;
        }
    }

    public void Draw() {
        if (!_done) Raylib.DrawTextureRec(SprakleSheet, _frameRect, _pos, Color.White);
    }
}
