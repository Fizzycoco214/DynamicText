using UnityEngine;

public abstract class TextColorEffect : TextEffect
{
    public abstract void UpdateText(Color32[][] vertexColors, int startIndex, int endIndex, string input);
}
