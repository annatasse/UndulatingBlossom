using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
public class Discrete8DirectionsProcessor : InputProcessor<Vector2> {

    #if UNITY_EDITOR
    static Discrete8DirectionsProcessor() {
        Initialize();
    }
    #endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize() {
        InputSystem.RegisterProcessor<Discrete8DirectionsProcessor>();
    }

    // the built-in deadzone uses magnitude so pointing your stick upwards (x=0.1,y=0.9) triggers movement to the right
    public float deadzone = 0.4f;
    public override Vector2 Process(Vector2 value, InputControl control) {
        float x = 0;
        if (value.x > deadzone) x = 1;
        else if (value.x < -deadzone) x = -1;

        float y = 0;
        if (value.y > deadzone) y = 1;
        else if (value.y < -deadzone) y = -1;

        return new Vector2(x,y);
    }
}
