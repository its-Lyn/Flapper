namespace FlappyBird.States;

public interface State {
    void Initialise();
    void Update(StateContext ctx);
    void Draw();

    void OnExit();
}
