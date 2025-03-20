using UnityEngine;

public class TextGrowAnimationEffect : TextAnimationEffect
{
    //Expands the letter from the center over a time specified from the input
    public override void UpdateText(Vector3[] characterVertices, Color32[] characterColors, float input, float time) {
        float percentTime = time / input;
        percentTime = Mathf.Min(percentTime, 1);

        Vector3 avgPoint = Vector3.zero;
        for(int i = 0; i < 4; i++) {
            avgPoint += characterVertices[i];
        }      
        avgPoint /= 4;

        for(int i = 0; i < 4; i++) {
            Vector3 centerToVert = characterVertices[i] - avgPoint;
            characterVertices[i] = avgPoint + centerToVert * percentTime;
        }
    }
}
