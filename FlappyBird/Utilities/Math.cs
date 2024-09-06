namespace FlappyBird.Utilities;

public static class FlapMath {
    public static float MoveTowards(float from, float to, float delta) {
        if (Math.Abs(to - from) <= delta) 
            return to;

        return from + (Math.Sign(to - from) * delta);
    }
}
