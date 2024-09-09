using System.Numerics;
using FlappyBird.Entities;
using FlappyBird.Utilities;

namespace FlappyBird.States;

public class Game : State {
    private readonly Bird _bird = new Bird();
    private readonly Ground[] _ground = [
        new Ground(false),
        new Ground(true)
    ];

    private readonly float PipeSpawnDelay = 1.5f;
    private readonly Texture2D PipeSprite = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "pipe-green.png"));

    private float _pipeSpawner = 0;
    private List<Pipes> _pipes = [];

    private bool _playing = false;

    private bool _animDone = false;
    private float _startAlpha = 0;

    private bool _flash = false;
    private float _flashSpeed = 0.35f;
    private float _flashAlpha = 0;
    private enum FlashStates {
        FadeIn,
        FadeOut
    }
    private FlashStates _flashState = FlashStates.FadeIn;

    private int _score = 0;
    private float _scoreAlpha = 0;
    private List<Texture2D> _scoreTextured = new List<Texture2D>();
    private List<Texture2D> _scoreSprites = new List<Texture2D>();

    private readonly Texture2D StartTexture = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "message.png"));

    private readonly Sound ScoreSound = Raylib.LoadSound(Path.Combine(FlappyBird.AssetPath, "Audio", "point.wav"));

    private readonly Sound DeathSound = Raylib.LoadSound(Path.Combine(FlappyBird.AssetPath, "Audio", "hit.wav"));
    private readonly Sound FallSound = Raylib.LoadSound(Path.Combine(FlappyBird.AssetPath, "Audio", "die.wav"));

    private void UpdateScore() {
        _scoreTextured.Clear();        
        foreach (byte digit in FlapMath.SplitScore(_score))
            _scoreTextured.Add(_scoreSprites[digit]);
    }

    public void Initialise() {
        _bird.Initialise();

        foreach (Ground ground in _ground) 
            ground.Initialise();

        for (int i = 0; i <= 9; i++)
            _scoreSprites.Add(Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", $"{i}.png")));
    }

    public void Update(StateContext ctx) {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsKeyPressed(KeyboardKey.Space))
            _playing = true;

        if (!_playing) {
            // Make the bird still do it's animation even when paused
            _bird.CycleAnimation();

            // Fade the start message in.
            if (_startAlpha < 1)
                _startAlpha += 0.1f;
                
            return;
        }

        // Die from the ground.
        // It doesn't which index as the Area is the same.
        if (_bird.Collider.Overlaps(_ground[0].Collider)) {
            if (!_bird.Dead) {
                Raylib.PlaySound(DeathSound);
                _flash = true;
            }

            _bird.Dead = true;
            _bird.Paused = true;
        }

        if (_flash) {
            switch (_flashState) {
                case FlashStates.FadeIn:
                    _flashAlpha += _flashSpeed;
                    if (_flashAlpha >= 1) {
                        _flashAlpha = 1;
                        _flashState = FlashStates.FadeOut;
                    }
                break;

                case FlashStates.FadeOut:
                    _flashAlpha -= _flashSpeed;
                    if (_flashAlpha <= 0) {
                        _flashAlpha = 0;
                        _flashState = FlashStates.FadeIn;

                        _flash = false;
                    }
                break;
            }
        }

        _bird.Update();

        if (_bird.Dead) return;

        if (_score > 0 && _scoreAlpha <= 1)
            _scoreAlpha += 0.2f;

        // Fade the message out
        if (!_animDone) {
            _startAlpha = Math.Max(_startAlpha - 0.1f, 0);
            if (_startAlpha == 0) _animDone = true;
        }
 
        foreach (Ground ground in _ground) 
            ground.Update();

        // Cannot use foreach as c# doesn't allow mutating
        // While iterating at the same time
        // ...rust
        for (int idx = 0; idx < _pipes.Count; idx++) {
            Pipes pipes = _pipes[idx];

            pipes.Update();

            if ((_bird.Collider.Overlaps(pipes.BottomPipe) || _bird.Collider.Overlaps(pipes.TopPipe)) && !_bird.Dead) {
                Raylib.PlaySound(DeathSound);
                Raylib.PlaySound(FallSound);

                _bird.Dead = true;

                _flash = true;
            }

            if (_bird.Collider.Overlaps(pipes.Score) && !pipes.Scored) {
                Raylib.PlaySound(ScoreSound);
                pipes.Scored = true;

                _score += 1;
                UpdateScore();
            }

            if (pipes.FirstPosition.X < -pipes.Sprite.Width)
                _pipes.Remove(pipes);
        }

        _pipeSpawner += Raylib.GetFrameTime();
        if (_pipeSpawner > PipeSpawnDelay) {
            _pipeSpawner = 0;

            Pipes pipes = new Pipes(PipeSprite);
            pipes.Initialise();

            _pipes.Add(pipes);
        }
    }

    public void Draw() {
        foreach (Pipes pipes in _pipes)
            pipes.Draw();

        _bird.Draw();

        foreach (Ground ground in _ground) 
            ground.Draw();

        if (!_animDone)
            Raylib.DrawTexture(StartTexture, 50, 40, Raylib.Fade(Color.White, _startAlpha));
       
        if (_flash) {
            Raylib.BeginBlendMode(BlendMode.Additive);
                Raylib.DrawRectangleV(Vector2.Zero, FlappyBird.GameSize, Raylib.Fade(Color.White, _flashAlpha));
            Raylib.EndBlendMode();
        }

        if (_score == 0 || !_playing || _bird.Dead)
            return;

        var basePosition = (FlappyBird.GameSize.X / 2) + (_scoreSprites[1].Width / 2 * _scoreTextured.Count);
        foreach (Texture2D num in _scoreTextured) {
            basePosition -= num.Width;
            Raylib.DrawTexture(num, (int)basePosition, 10, Raylib.Fade(Color.White, _scoreAlpha));
        }
    }

    public void OnExit() {
        Raylib.UnloadSound(ScoreSound);
        Raylib.UnloadSound(DeathSound);

        Raylib.UnloadTexture(StartTexture);
        Raylib.UnloadTexture(PipeSprite);

        _bird.OnExit();
        foreach (Ground ground in _ground)
            ground.OnExit();

        foreach (Pipes pipes in _pipes) 
            pipes.OnExit();

        foreach (Texture2D tex in _scoreSprites) 
            Raylib.UnloadTexture(tex);
    }
}
