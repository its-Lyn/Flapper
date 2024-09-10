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

    public static float EaseOutElastic(float x) {
        const float c4 = 2 * (float)Math.PI / 3;

        if (x == 0 || x == 1)
            return x;
            
        return (float)(Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1);
    }

    public static IEnumerable<byte> SplitScore(int score) {
        if (score == 0) yield return 0;

        while (score > 0) {
            byte digit = (byte)(score % 10);
            yield return digit;

            score /= 10;
        }
    }
}
