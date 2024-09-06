using FlappyBird.Entities;

namespace FlappyBird.States;

public class Game : State {
    private readonly Bird _bird = new Bird();

    public void Initialise() {
        _bird.Initialise();
    }

    public void Update() {
        _bird.Update();
    }

    public void Draw() {
        _bird.Draw();
    }

}
