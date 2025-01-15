using UnityEngine;
using TMPro;

public class TextCharacterShakeEffect : TextVertexEffect
{

    public override void UpdateText(Vector3[][] textVertices, int start, int end, float input) {
        for (int i = start; i < end; i++) {
            float shakeX = Random.Range(-input, input);
            float shakeY = Random.Range(-input, input); 

            for (int j = 0; j < 4; j++) {
                Vector3 orig = textVertices[i][j];
                textVertices[i][j] = orig + new Vector3(shakeX, shakeY);
            }
        }
    }
}