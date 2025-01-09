using UnityEngine;
using TMPro;
using System;

public class TextWaveEffect : TextEffect
{
    static float waveStrength = 10f;
    static float waveFrequency = 2f;
    static float waveSpeed = 2f;

    public new static void UpdateText(TMP_TextInfo info) {
        for (int i = 0; i < info.characterCount; i++) { 
            TMP_CharacterInfo charInfo = info.characterInfo[i];

            if (!charInfo.isVisible) {
                continue;
            
            }

            Vector3[] vertices = info.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; ++j) {
                Vector3 orig = vertices[charInfo.vertexIndex + j];
                vertices[charInfo.vertexIndex + j] = orig + Mathf.Sin((Time.time * waveSpeed) + (charInfo.vertexIndex * waveFrequency)) * Vector3.up * waveStrength;
        
            }

        }
    }
}