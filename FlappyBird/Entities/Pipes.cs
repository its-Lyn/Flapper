using System.Numerics;

namespace FlappyBird.Entities;

public class Pipes : Entity {
    public readonly Texture2D Sprite; 

    private readonly float SpawnOffset = 20.0f;
    private readonly float Speed = 2.0f;

    private readonly float Gap = 80.0f;
    private readonly float MinOffset = 180.0f;
    private readonly float MaxOffset = 120.0f;

    private readonly float ScoreSize = 20.0f;

    public Vector2 FirstPosition = Vector2.Zero;
    public Vector2 SecondPosition = Vector2.Zero;

    private Rectangle _source;

    public Collider TopPipe = new Collider();
    public Collider BottomPipe = new Collider();

    public Collider Score = new Collider();

    public Pipes(Texture2D sprite)
        => Sprite = sprite;

    public void Initialise() {
        FirstPosition.X = FlappyBird.GameSize.X + SpawnOffset;
        SecondPosition.X = FlappyBird.GameSize.X + SpawnOffset;

        int y = Random.Shared.Next((int)MaxOffset, (int)(FlappyBird.GameSize.Y - MinOffset));
        FirstPosition.Y = y;
        SecondPosition.Y = y - Sprite.Height - Gap;

        _source = new Rectangle(0, 0, Sprite.Width, -Sprite.Height);

        TopPipe.UpdateArea(SecondPosition, Sprite);
        BottomPipe.UpdateArea(FirstPosition, Sprite);
        
        Score.UpdateArea(
            FirstPosition.X + (Sprite.Width / 2) - (ScoreSize / 2),
            FirstPosition.Y - Gap,
            ScoreSize,
            Gap
        );
    }

    public void Update() {
        FirstPosition.X -= Speed;
        SecondPosition.X -= Speed;

        TopPipe.UpdateArea(SecondPosition, Sprite);
        BottomPipe.UpdateArea(FirstPosition, Sprite);

        Score.UpdateArea(
            FirstPosition.X + (Sprite.Width / 2) - (ScoreSize / 2),
            FirstPosition.Y - Gap,
            ScoreSize,
            Gap
        );
    }
 
    public void Draw() {
        Raylib.DrawTextureV(Sprite, FirstPosition, Color.White);

        // Draw the second pipe flipped vertically.
        Raylib.DrawTexturePro(
            Sprite,
            _source,
            new Rectangle(SecondPosition.X, SecondPosition.Y, Sprite.Width, Sprite.Height),
            Vector2.Zero,
            0, Color.White
        );

        if (FlappyBird.DEV_MODE) {
            Raylib.DrawRectangleLinesEx(BottomPipe.Area, 2, Color.Red);
            Raylib.DrawRectangleLinesEx(TopPipe.Area, 2, Color.Red);

            Raylib.DrawRectangleLinesEx(Score.Area, 2, Color.Blue);
        }
    }

    public void OnExit() 
        => Raylib.UnloadTexture(Sprite);
}
