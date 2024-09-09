using System.Numerics;
using FlappyBird.States;

namespace FlappyBird.Components;

public class Pause {
    private readonly Texture2D Resume = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Buttons", "resume.png"));
    private readonly Texture2D Menu = Raylib.LoadTexture(Path.Combine(FlappyBird.AssetPath, "Buttons", "menu.png"));

    private List<Button> _buttons = [];

    public void Initialise(StateContext ctx) {
        Button resume = new Button(
            Resume,
            new Vector2(10, 10),
            (ctx) => {
                ctx.GamePaused = false;
            }, 
            false
        );
        
        Button menu = new Button(
            Menu,
            new Vector2(FlappyBird.GameSize.X / 2 - Menu.Width / 2, FlappyBird.GameSize.Y / 2 - Menu.Height),
            (ctx) => {
                Menu menu = new Menu();
                menu.Initialise(ctx);

                ctx.GamePaused = false;

                ctx.StateMachine.SetState(menu);
            }
        );

        _buttons.Add(resume);
        _buttons.Add(menu);
    }

    public void Update(StateContext ctx, Vector2 mouse) {
        foreach (Button button in _buttons)
            button.Update(mouse, ctx);
    }

    public void Draw() {
        Raylib.DrawRectangleV(Vector2.Zero, FlappyBird.GameSize, Raylib.Fade(Color.Black, 0.7f));

        foreach (Button button in _buttons)
            button.Draw();
    }
}
