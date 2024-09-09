namespace FlappyBird.States;

public interface State {
    void Initialise(StateContext ctx);
    void Update(StateContext ctx);
    void Draw();

    void OnExit();
}
