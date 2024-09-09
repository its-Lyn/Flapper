namespace FlappyBird.Utilities;

public static class FlapMath {
    public static float MoveTowards(float from, float to, float delta) {
        if (Math.Abs(to - from) <= delta) 
            return to;

        return from + (Math.Sign(to - from) * delta);
    }

    public static float EaseInCubic(float x) 
        => x * x * x; 

    public static float EaseOutCubic(float x)
        => 1 - MathF.Pow(1 - x, 3);

    public static IEnumerable<byte> SplitScore(int score) {
        while (score > 0) {
            byte digit = (byte)(score % 10);
            yield return digit;

            score /= 10;
        }
    }
}
