using UnityEngine;
using TMPro;

public class TextCharacterShakeEffect : TextEffect
{
    static float shakeStrength = 2f;

    public new static void UpdateText(TMP_TextInfo info) {

        for (int i = 0; i < info.characterCount; i++) { 
            TMP_CharacterInfo charInfo = info.characterInfo[i];

            if (!charInfo.isVisible) {
                continue;
            
            }

            Vector3[] vertices = info.meshInfo[charInfo.materialReferenceIndex].vertices;

            float shakeX = Random.Range(-shakeStrength, shakeStrength);
            float shakeY = Random.Range(-shakeStrength, shakeStrength); 

            for (int j = 0; j < 4; ++j) {
                Vector3 orig = vertices[charInfo.vertexIndex + j];
                vertices[charInfo.vertexIndex + j] = orig + new Vector3(shakeX, shakeY);
        
            }

        }
    }
}