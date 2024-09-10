using System.Numerics;
using FlappyBird.Components;
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

    private readonly Texture2D PanelTexture = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "panel.png"));
    private Vector2 _panelPos;
    private bool _panelAnimating = false;
    private float _panelTimer = 0;

    private readonly Texture2D GameOver = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "gameover.png"));
    private float _gameOverAlpha = 0;
    private bool _gameOverEase = false;
    private bool _gameOverShow = false;
    private float _gameOverTimer = 0;
    public float _gameOverY = 120;

    private readonly Texture2D Ok = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Buttons", "ok.png"));
    private readonly Texture2D Share = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Buttons", "share.png"));
    private List<Button> _buttons = [];
    private bool _showButtons = false;

    private List<Texture2D> _smallNumbers = [];
    private bool _animateNumbers = false;
    private bool _animating = false;
    private int _tick = 0;
    private float _animationTimer = 0;
    private bool _showNumbers = false;

    private readonly Texture2D Pause = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Buttons", "pause.png"));
    private Button _pauseButton = null!;
    private bool _gamePaused = false;        

    private readonly Pause PauseComponent = new Pause();

    private readonly Texture2D Bronze = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "bronze-medal.png"));
    private readonly Texture2D Silver = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "silver-medal.png"));
    private readonly Texture2D Gold = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "gold-medal.png"));
    private readonly Texture2D Platinum = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "platinum-medal.png"));

    private Texture2D? _medal;

    private Sparkle _sparkle = new Sparkle();

    private void UpdateScore() {
        _scoreTextured.Clear();        
        foreach (byte digit in FlapMath.SplitScore(_score))
            _scoreTextured.Add(_scoreSprites[digit]);
    }

    private void UpdateScoreSmall() {
        _scoreTextured.Clear();
        foreach (byte digit in FlapMath.SplitScore(_score))
            _scoreTextured.Add(_smallNumbers[digit]);
    }

    public void Initialise(StateContext ctx) {
        _bird.Initialise();

        foreach (Ground ground in _ground) 
            ground.Initialise();

        for (int i = 0; i <= 9; i++) {
            _smallNumbers.Add(Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", $"{i}_small.png")));
            _scoreSprites.Add(Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", $"{i}.png")));
        }

        _panelPos = new Vector2(
            33,
            FlappyBird.GameSize.Y + PanelTexture.Height + 50
        );

        float buttonY = FlappyBird.GameSize.Y - 160;

        Button ok = new Button(
            Ok,
            new Vector2(50, buttonY),
            (ctx) => {
                Menu menu = new Menu();
                menu.Initialise(ctx);

                ctx.StateMachine.SetState(menu);
            }
        );

        Button share = new Button(
            Share,
            new Vector2(FlappyBird.GameSize.X - Share.Width - 50, buttonY),
            (ctx) => {
                Console.WriteLine("This isn't a web game anymore man :3");
            }, false
        );

        _buttons.Add(ok);
        _buttons.Add(share);

        _pauseButton = new Button(
            Pause,
            new Vector2(10, 10),
            (ctx) => {
                ctx.GamePaused = true;
            }, false
        );

        PauseComponent.Initialise(ctx);
    }

    public void Update(StateContext ctx) {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsKeyPressed(KeyboardKey.Space))
            _playing = true;

        if (!_playing) {
            // Make the bird still do it's animation even when paused
            _bird.CycleAnimation();

            // Keep updating the ground as in the original game the ground moves
            foreach (Ground ground in _ground) 
                ground.Update();
        
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
                        _gameOverShow = true;
                        _gameOverEase = true;
                    }
                break;
            }
        }

        Vector2 realMouse = Raylib.GetMousePosition();
        Vector2 virtMouse = new Vector2(
            (realMouse.X - (Raylib.GetScreenWidth() - (FlappyBird.GameSize.X * FlappyBird.Scale)) * 0.5f) / FlappyBird.Scale,
            (realMouse.Y - (Raylib.GetScreenHeight() - (FlappyBird.GameSize.Y * FlappyBird.Scale)) * 0.5f) / FlappyBird.Scale
        );
        virtMouse = Vector2.Clamp(virtMouse, Vector2.Zero, FlappyBird.GameSize);

        if (ctx.GamePaused) {
            PauseComponent.Update(ctx, virtMouse);
            return;
        }

        _bird.Update();

        if (!_bird.Dead)
            _pauseButton.Update(virtMouse, ctx);

        if (_bird.Dead) {
            if (_gameOverShow) {
                if (_gameOverAlpha <= 1)
                    _gameOverAlpha = Math.Min(_gameOverAlpha + 0.04f, 1);

                if (_gameOverEase) {
                    _gameOverTimer += Raylib.GetFrameTime();
                    if (_gameOverTimer >= 0.9f) {
                        _gameOverTimer = 0.9f;
                        _gameOverEase = false;

                        _panelAnimating = true;
                    }

                    float overNormalised = _gameOverTimer / 0.9f;
                    _gameOverY = 100 + 20 * FlapMath.EaseOutElastic(overNormalised);
                }
            }

            if (_panelAnimating) {
                _panelTimer += Raylib.GetFrameTime();
                if (_panelTimer >= 0.6f) {
                    _panelTimer = 0.6f;
                    _panelAnimating = false;

                    // Calculate which medal to get
                    // 10 for bronze, 20 for silver
                    // 30 for gold, and 40 for platinum
                    // SOURCE: https://en.wikipedia.org/wiki/Flappy_Bird#Gameplay
                    _medal = _score switch {
                        >= 40 => Platinum,
                        >= 30 => Gold,
                        >= 20 => Silver,
                        >= 10 => Bronze,
                        _ => null,
                    };

                    _animateNumbers = true;
                    _animating = true;
                }

                float timeNormalised = _panelTimer / 0.6f;
                _panelPos.Y = FlappyBird.GameSize.Y + PanelTexture.Height - 430 * FlapMath.EaseOutCubic(timeNormalised);
            }

            if (_showButtons) {
                foreach (Button button in _buttons) 
                    button.Update(virtMouse, ctx);
            }

            return;
        }

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

            if ((_bird.Collider.Overlaps(pipes.BottomPipe) || _bird.Collider.Overlaps(pipes.TopPipe)) && !_bird.Dead && !FlappyBird.NoClip) {
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

            if (pipes.FirstPosition.X < -pipes.Sprite.Width) {
                _pipes.Remove(pipes);
                idx--; // Adjust the index so it doesn't skip.
            }
        }

        _pipeSpawner += Raylib.GetFrameTime();
        if (_pipeSpawner > PipeSpawnDelay) {
            _pipeSpawner = 0;

            Pipes pipes = new Pipes(PipeSprite);
            pipes.Initialise();

            _pipes.Add(pipes);
        }

        _gamePaused = ctx.GamePaused;
    }

    public void Draw() {
        foreach (Pipes pipes in _pipes)
            pipes.Draw();

        _bird.Draw();

        foreach (Ground ground in _ground) 
            ground.Draw();

        if (!_animDone)
            Raylib.DrawTexture(StartTexture, 50, 40, Raylib.Fade(Color.White, _startAlpha));
       
        // Use additive blending;
        // That way the screen doesnt turn grey.
        if (_flash) {
            Raylib.BeginBlendMode(BlendMode.Additive);
                Raylib.DrawRectangleV(Vector2.Zero, FlappyBird.GameSize, Raylib.Fade(Color.White, _flashAlpha));
            Raylib.EndBlendMode();
        }

        if (_bird.Dead) {
            Raylib.DrawTexture(GameOver, 50, (int)_gameOverY, Raylib.Fade(Color.White, _gameOverAlpha));

            float smallBase = _panelPos.X + PanelTexture.Width - _smallNumbers[0].Width - 22;
            
            Raylib.DrawTextureV(PanelTexture, _panelPos, Color.White);
            if (!_animateNumbers && !_showNumbers) {
                Raylib.DrawTexture(_smallNumbers[0], (int)smallBase, (int)_panelPos.Y + 33, Color.White);
            }

            if (_animateNumbers) {
                if (_score == 0) _animating = false;
                if (_score != 0) {
                    _animationTimer += Raylib.GetFrameTime();
                    if (_animationTimer >= 0.03f) {
                        _animationTimer = 0;

                        _tick += 1;
                        if (_tick == _score) 
                            _animating = false;
                    }
                    
                    if (_tick < 10) {
                        Raylib.DrawTexture(_smallNumbers[_tick], (int)smallBase, (int)_panelPos.Y + 33, Color.White);
                    } else {
                        var newBase = smallBase;
                        foreach (byte digit in FlapMath.SplitScore(_tick)) {
                            Raylib.DrawTexture(_smallNumbers[digit], (int)newBase, (int)_panelPos.Y + 33, Color.White);
                            newBase -= _smallNumbers[digit].Width;
                        }
                    }
                }

                if (!_animating) {
                    _animateNumbers = false;
                    _showNumbers = true;
                    _showButtons = true;
                }
            } 

            if (_showNumbers) {
                UpdateScoreSmall();

                foreach (Texture2D num in _scoreTextured) {
                    Raylib.DrawTexture(num, (int)smallBase, (int)_panelPos.Y + 33, Color.White);
                    smallBase -= num.Width;
                } 
            }

            if (_showButtons) {
                foreach (Button button in _buttons)
                    button.Draw();
                
                if (_medal is not null) {
                    Raylib.DrawTexture(_medal.Value, (int)_panelPos.X + 26, 241, Color.White);
                    _sparkle.Update((int)_panelPos.X + 32, 247, 30, 30);
                    _sparkle.Draw();
                }
            }
        }

        if (!_bird.Dead)
            _pauseButton.Draw();

        if (_gamePaused) 
            PauseComponent.Draw();

        if (_score == 0 || !_playing || _bird.Dead)
            return;

        float basePosition = (FlappyBird.GameSize.X / 2) + (_scoreSprites[1].Width / 2 * _scoreTextured.Count);
        foreach (Texture2D num in _scoreTextured) {
            basePosition -= num.Width;
            Raylib.DrawTexture(num, (int)basePosition, 10, Raylib.Fade(Color.White, _scoreAlpha));
        }
    }

    public void OnExit() {
        Raylib.UnloadSound(ScoreSound);
        Raylib.UnloadSound(DeathSound);
        Raylib.UnloadSound(FallSound);

        Raylib.UnloadTexture(StartTexture);
        Raylib.UnloadTexture(PipeSprite);
        Raylib.UnloadTexture(PanelTexture);
        Raylib.UnloadTexture(GameOver);

        Raylib.UnloadTexture(Bronze);
        Raylib.UnloadTexture(Silver);
        Raylib.UnloadTexture(Gold);
        Raylib.UnloadTexture(Platinum);

        _bird.OnExit();
        foreach (Ground ground in _ground)
            ground.OnExit();

        foreach (Pipes pipes in _pipes) 
            pipes.OnExit();

        foreach (Texture2D tex in _scoreSprites) 
            Raylib.UnloadTexture(tex);
        
        foreach (Texture2D tex in _smallNumbers)
            Raylib.UnloadTexture(tex);
    }
}
