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

    public void Initialise() {
        _bird.Initialise();

        foreach (Ground ground in _ground) 
            ground.Initialise();
    }

    public void Update() {
        _bird.Update();
 
        foreach (Ground ground in _ground) 
            ground.Update();

        foreach (Pipes pipes in _pipes) {
            pipes.Update();
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

        foreach (Pipes pipes in _pipes) {
            pipes.Draw();
        }

        foreach (Ground ground in _ground) 
            ground.Draw();
    }

}
