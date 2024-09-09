namespace FlappyBird.States;

public class StateContext {
    public StateMachine StateMachine = new StateMachine();

    public enum FadeStates {
        FadeIn,
        FadeOut
    }

    public FadeStates FadeState = FadeStates.FadeIn;
    public float FadeValue = 0;
    public bool Fade = false;
    public bool FadedIn = false; 
}
