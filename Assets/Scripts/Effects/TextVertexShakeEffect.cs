using UnityEngine;
using TMPro;

public class TextVertexShakeEffect : TextEffect
{
    static float shakeStrength = 2f;

    public new static void UpdateText(TMP_TextInfo info) {

        for (int i = 0; i < info.characterCount; i++) { 
            TMP_CharacterInfo charInfo = info.characterInfo[i];

            if (!charInfo.isVisible) {
                continue;
            
            }

            Vector3[] vertices = info.meshInfo[charInfo.materialReferenceIndex].vertices;
            

            for (int j = 0; j < 4; ++j) {
                float shakeX = Random.Range(-shakeStrength, shakeStrength);
                float shakeY = Random.Range(-shakeStrength, shakeStrength); 
                Vector3 orig = vertices[charInfo.vertexIndex + j];
                vertices[charInfo.vertexIndex + j] = orig + new Vector3(shakeX, shakeY);
        
            }

        }
    }
}