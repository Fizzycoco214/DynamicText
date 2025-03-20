using UnityEngine;

public abstract class TextAnimationEffect : TextEffect
{
    //Returns true while still animating
    public abstract void UpdateText(Vector3[] characterVertices, Color32[] characterColors, float input, float time);
}
