using System.Numerics;
using System.Security.Cryptography;
using FlappyBird.States;

namespace FlappyBird.Entities;

public class Button(Texture2D sprite, Vector2 pos, Action<StateContext> action, bool performFade = true) {
    public Action<StateContext> Action = action;

    private Vector2 _pos = pos;
    private bool _isHeld = false;
    private bool _pressed = false;

    private readonly Collider Collider = new Collider();
    private readonly Vector2 OriginalPos = pos;
    private readonly Texture2D Sprite = sprite;

    public void Initialise() 
        => Collider.UpdateArea(_pos, Sprite);

    public void Update(Vector2 mouse, StateContext ctx) {
        if (_pos.Y != OriginalPos.Y && !_isHeld) 
            _pos.Y = OriginalPos.Y;

        if (Collider.Overlaps(mouse)) {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                _isHeld = true;

                _pos.Y = OriginalPos.Y + 2;
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.Left) && _isHeld) {
                _isHeld = false;
                _pressed = true;
                _pos.Y = OriginalPos.Y;

                ctx.Fade = performFade;
                if (!performFade) {
                    Action.Invoke(ctx);
                    _pressed = false;
                }
            }
        } else {
            _isHeld = false;
        }

        if (ctx.Fade && ctx.FadedIn && _pressed) {
            Action.Invoke(ctx);
            _pressed = false;
        }

        Collider.UpdateArea(_pos, Sprite);
    }

    public void Draw() {
        Raylib.DrawTextureV(Sprite, _pos, Color.White);

        if (FlappyBird.DevMode) 
            Raylib.DrawRectangleLinesEx(Collider.Area, 2, Color.Purple);
    }
}
