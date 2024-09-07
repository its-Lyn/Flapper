using System.Numerics;
using FlappyBird.Utilities;

namespace FlappyBird.Entities;

public class Bird : Entity {
    // Bird constants
    private readonly float Gravity = 0.5f;
    private readonly float TerminalVelocity = 10.5f;
    private readonly float JumpVelocity = -6.0f;

    private readonly float MaxDownwardAngle = 40.0f;
    private readonly float MaxUpwardAngle = -20.0f;

    private readonly float FlappingSpeed = 0.05f;
    private readonly float FallingSpeed = 0.15f;

    // Bird non-constant fields
    private float _animationTimer = 0;
    private float _animationSpeed = 0.15f;

    private float _rotation = 0;
    private float _rotationSpeed = 8;
    private float _rotationAngle;

    private bool _reverse = false;

    private int _activeSpriteIdx = 0;
    private List<Texture2D> _sprites = new List<Texture2D> {
        Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "yellowbird-upflap.png")),
        Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "yellowbird-midflap.png")),
        Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "yellowbird-downflap.png"))
    };

    private readonly Sound FlapSound = Raylib.LoadSound(Path.Combine(FlappyBird.AssetPath, "Audio", "wing.wav"));

    private Vector2 _pos = Vector2.Zero;
    private Vector2 _vel = Vector2.Zero;

    // Sprite rotation fuckery
    private Vector2 _origin;
    private Rectangle _source;

    public void CycleAnimation() {
        _animationTimer += Raylib.GetFrameTime();
        if (_animationTimer >= _animationSpeed) {
            _animationTimer = 0;

            if (_reverse) _activeSpriteIdx -= 1; else _activeSpriteIdx += 1;
            if (_activeSpriteIdx >= _sprites.Count - 1 || _activeSpriteIdx <= 0) _reverse = !_reverse;
        }
    }

    public void Initialise() {
        _rotationAngle = MaxUpwardAngle;

        _pos.Y = (FlappyBird.GameSize.Y / 2.0f) - _sprites[_activeSpriteIdx].Width;
        _pos.X = 70;

        // All sprites are the same size so it doesn't matter at all
        _origin = new Vector2(
            _sprites[_activeSpriteIdx].Width / 2,
            _sprites[_activeSpriteIdx].Height / 2
        );

        _source = new Rectangle(
            0, 0, _sprites[_activeSpriteIdx].Width, _sprites[_activeSpriteIdx].Height
        );
    }

    public void Update() {
        CycleAnimation();

        // Apply gravity
        _vel.Y = FlapMath.MoveTowards(_vel.Y, TerminalVelocity, Gravity);

        // Rotate bird every frame
        _rotation = FlapMath.MoveTowards(_rotation, _rotationAngle, _rotationSpeed);

        // Flap
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            Raylib.PlaySound(FlapSound);

            _rotationAngle = MaxUpwardAngle;

            _vel.Y = JumpVelocity;
            _animationSpeed = FlappingSpeed;
        }

        // Bonk
        if (_pos.Y <= -_sprites[_activeSpriteIdx].Height * 2) {
            _pos.Y = -_sprites[_activeSpriteIdx].Height * 1.9f;
            _vel.Y = 0;
        }

        // Speed up the animation when the bird reaches a certain speed
        if (_vel.Y > 1.5f) {
            _rotationAngle = MaxDownwardAngle;
            _animationSpeed = FallingSpeed;
        }

        _pos += _vel;
    } 

    public void Draw() {
        Raylib.DrawTexturePro(
            _sprites[_activeSpriteIdx],
            _source,
            new Rectangle(_pos.X, _pos.Y, _sprites[_activeSpriteIdx].Width, _sprites[_activeSpriteIdx].Height), 
            _origin, 
            _rotation,
            Color.White 
        );
    }

    public void OnExit() {
        Raylib.UnloadSound(FlapSound);
        foreach(Texture2D tex in _sprites)
            Raylib.UnloadTexture(tex);
    }
}
