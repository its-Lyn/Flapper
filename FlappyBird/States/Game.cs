using FlappyBird.Entities;

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

    private readonly Texture2D StartTexture = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Sprites", "message.png"));

    public void Initialise() {
        _bird.Initialise();

        foreach (Ground ground in _ground) 
            ground.Initialise();
    }

    public void Update(StateContext ctx) {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            _playing = true;

        if (!_playing) {
            // Make the bird still do it's animation even when paused
            _bird.CycleAnimation();

            // Fade the start message in.
            if (_startAlpha < 1)
                _startAlpha += 0.1f;
                
            return;
        }

        // Fade the message out
        if (!_animDone) {
            _startAlpha = Math.Max(_startAlpha - 0.1f, 0);
            if (_startAlpha == 0) _animDone = true;
        }

        _bird.Update();
 
        foreach (Ground ground in _ground) 
            ground.Update();

        // Cannot use foreach as c# doesn't allow mutating
        // While iterating at the same time
        // ...rust
        for (int idx = 0; idx < _pipes.Count; idx++) {
            Pipes pipes = _pipes[idx];

            pipes.Update();

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
        _bird.Draw();

        foreach (Pipes pipes in _pipes)
            pipes.Draw();

        foreach (Ground ground in _ground) 
            ground.Draw();

        if (!_animDone) {
            Raylib.DrawTexture(StartTexture, 50, 40, Raylib.Fade(Color.White, _startAlpha));
        }
    }

    public void OnExit() {
        Raylib.UnloadTexture(StartTexture);
        Raylib.UnloadTexture(PipeSprite);

        _bird.OnExit();
        foreach (Ground ground in _ground)
            ground.OnExit();

        foreach (Pipes pipes in _pipes) 
            pipes.OnExit();
    }
}
