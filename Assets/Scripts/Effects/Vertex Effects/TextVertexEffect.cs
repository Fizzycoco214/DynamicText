using UnityEngine;

public abstract class TextVertexEffect : TextEffect
{
    public abstract void UpdateText(Vector3[][] textVertices, int start, int end, float input);
}
