using System.Numerics;
using FlappyBird.Entities;
using FlappyBird.Components;

namespace FlappyBird.States;

public class Menu : State {
    private List<Ground> _ground = [
        new Ground(false),
        new Ground(true)
    ];

    private List<Button> _buttons = []; 

    private readonly Texture2D StartTexture = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Buttons", "start.png"));
    private readonly Texture2D ScoreTexture = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Buttons", "score.png"));

    private readonly Texture2D Logo = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "logo.png"));
    private Vector2 _logoPos = new Vector2(20, 100);

    private float _animationTimer = 0;
    private float _animationSpeed = 0.15f;
    private bool _reverse = false;
    private int _activeSpriteIdx = 0;
    private List<Texture2D> _sprites = new List<Texture2D> {
        Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "yellowbird-upflap.png")),
        Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "yellowbird-midflap.png")),
        Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "yellowbird-downflap.png"))
    };

    private float _oscillateTimer = 0;

    public void Initialise(StateContext ctx) {
        foreach (Ground ground in _ground)
            ground.Initialise();

        Button start = new Button(
            StartTexture,
            new Vector2(40, FlappyBird.GameSize.Y - 150),
            (ctx) => {
                Game game = new Game();
                game.Initialise(ctx);

                ctx.StateMachine.SetState(game);
            }
        );

        Button score = new Button(
            ScoreTexture,
            new Vector2(FlappyBird.GameSize.X - ScoreTexture.Width - 40, FlappyBird.GameSize.Y - 150),
            (ctx) => {
                Console.WriteLine("I honestly don't know what this button is supposed to do :(");
            },
            false
        );

        _buttons.Add(start);
        _buttons.Add(score);
    }
 
    public void Update(StateContext ctx) {
        foreach (Ground ground in _ground)
            ground.Update();

        Vector2 realMouse = Raylib.GetMousePosition();
        Vector2 virtMouse = new Vector2(
            (realMouse.X - (Raylib.GetScreenWidth() - (FlappyBird.GameSize.X * FlappyBird.Scale)) * 0.5f) / FlappyBird.Scale,
            (realMouse.Y - (Raylib.GetScreenHeight() - (FlappyBird.GameSize.Y * FlappyBird.Scale)) * 0.5f) / FlappyBird.Scale
        );
        virtMouse = Vector2.Clamp(virtMouse, Vector2.Zero, FlappyBird.GameSize);

        foreach (Button btn in _buttons) 
            btn.Update(virtMouse, ctx);

        _oscillateTimer += Raylib.GetFrameTime();
        _logoPos.Y = 100 + 5 * MathF.Sin(_oscillateTimer * 5.5f);

        _animationTimer += Raylib.GetFrameTime();
        if (_animationTimer >= _animationSpeed) {
            _animationTimer = 0;

            if (_reverse) _activeSpriteIdx -= 1; else _activeSpriteIdx += 1;
            if (_activeSpriteIdx >= _sprites.Count - 1 || _activeSpriteIdx <= 0) _reverse = !_reverse;
        }
    }   
 
    public void Draw() {
        foreach (Ground ground in _ground) 
            ground.Draw();

        foreach (Button btn in _buttons) 
            btn.Draw();

        Raylib.DrawTextureV(Logo, _logoPos, Color.White);
        Raylib.DrawTexture(_sprites[_activeSpriteIdx], (int)_logoPos.X + Logo.Width + 15, (int)_logoPos.Y + 7, Color.White);
    }

    public void OnExit() {
        foreach (Ground ground in _ground)
            ground.OnExit();

        Raylib.UnloadTexture(StartTexture);
        Raylib.UnloadTexture(ScoreTexture);
        Raylib.UnloadTexture(Logo);
        foreach (Texture2D tex in _sprites)
            Raylib.UnloadTexture(tex);
    } 
}
