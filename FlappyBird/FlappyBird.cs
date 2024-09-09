using System.Numerics;
using FlappyBird.States;
using static FlappyBird.States.StateContext;

namespace FlappyBird;

public static class FlappyBird {
    public static float Scale = 0;

    public static readonly bool DevMode = false;
    public static readonly bool NoClip = false;
    
    public static readonly string AssetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
    public static readonly Vector2 GameSize = new Vector2(288, 512);

    public static void Main() {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow((int)GameSize.X, (int)GameSize.Y, "Flappy Bird");
        Raylib.SetWindowMinSize(144, 256);

        Raylib.InitAudioDevice();
        Raylib.SetMasterVolume(0.3f);

        // Create game world
        RenderTexture2D renderer = Raylib.LoadRenderTexture((int)GameSize.X, (int)GameSize.Y);
        Raylib.SetTextureFilter(renderer.Texture, TextureFilter.Point);

        // Lock the game to 60 fps
        // ...I can't be fucked to make DT physics
        Raylib.SetTargetFPS(60);

        StateContext context = new StateContext();
        context.StateMachine.SetState(new Menu());
        context.StateMachine.State?.Initialise(context);

        Texture2D background = Raylib.LoadTexture(Path.Combine(AssetPath, "Sprites", "background-day.png"));

        while (!Raylib.WindowShouldClose()) {
            Scale = Math.Min(
                Raylib.GetScreenWidth() / GameSize.X,
                Raylib.GetScreenHeight() / GameSize.Y
            );

            context.StateMachine.State?.Update(context);

            if (context.Fade) {
                switch (context.FadeState) {
                    case FadeStates.FadeIn:
                        context.FadeValue += 0.05f;
                        if (context.FadeValue >= 1) {
                            context.FadeValue = 1;
                            context.FadeState = FadeStates.FadeOut;

                            context.FadedIn = true;
                        }
                    break;
                    case FadeStates.FadeOut:
                        context.FadeValue -= 0.05f;
                        if (context.FadeValue <= 0) {
                            context.FadeValue = 0;
                            context.FadeState = FadeStates.FadeIn;

                            context.Fade = false;
                            context.FadedIn = false;
                        }
                    break;
                }
            }

            // Drawing inside the game world
            Raylib.BeginTextureMode(renderer);
                Raylib.ClearBackground(Color.Black);
                Raylib.DrawTexture(background, 0, 0, Color.White);

                context.StateMachine.State?.Draw();
                
                if (context.Fade)
                    Raylib.DrawRectangleV(Vector2.Zero, GameSize, Raylib.Fade(Color.Black, context.FadeValue));
            Raylib.EndTextureMode();

            // Drawing to the whole screen
            Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);
                Raylib.DrawTexturePro(
                    renderer.Texture,
                    new Rectangle(
                        0, 0, renderer.Texture.Width, -renderer.Texture.Height
                    ),
                    new Rectangle(
                        (Raylib.GetScreenWidth() - (GameSize.X * Scale)) * 0.5f,
                        (Raylib.GetScreenHeight() - (GameSize.Y * Scale)) * 0.5f,
                        GameSize.X * Scale, GameSize.Y * Scale
                    ),
                    Vector2.Zero,
                    0,
                    Color.White
                );

                if (DevMode) Raylib.DrawFPS(5, 5);
            Raylib.EndDrawing();
        }

        Raylib.UnloadRenderTexture(renderer);

        context.StateMachine.State?.OnExit();

        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}
