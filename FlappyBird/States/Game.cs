using FlappyBird.Entities;

namespace FlappyBird.States;

public class Game : State {
    private readonly Bird _bird = new Bird();
    private readonly Ground[] _ground = [
        new Ground(false),
        new Ground(true)
    ];   

    public void Initialise() {
        _bird.Initialise();

        foreach (Ground ground in _ground) 
            ground.Initialise();
    }

    public void Update() {
        _bird.Update();
 
        foreach (Ground ground in _ground) 
            ground.Update();
    }

    public void Draw() {
        _bird.Draw();

        foreach (Ground ground in _ground) 
            ground.Draw();
    }

}
