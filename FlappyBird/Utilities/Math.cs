namespace FlappyBird.Utilities;

public static class FlapMath {
    public static float MoveTowards(float from, float to, float delta) {
        if (Math.Abs(to - from) <= delta) 
            return to;

        return from + (Math.Sign(to - from) * delta);
    }

    public static IEnumerable<byte> SplitScore(int score) {
        while (score > 0) {
            byte digit = (byte)(score % 10);
            yield return digit;

            score /= 10;
        }
    }
}
