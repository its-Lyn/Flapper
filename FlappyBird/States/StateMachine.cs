namespace FlappyBird.States;

public class StateMachine {
    public State? State { get; private set; }

    public void SetState<T>(T newState) where T : State {
        State?.OnExit();

        State = newState;
    }
}
