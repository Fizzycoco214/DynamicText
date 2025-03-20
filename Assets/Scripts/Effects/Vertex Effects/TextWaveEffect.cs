using UnityEngine;
using TMPro;
using System;

public class TextWaveEffect : TextVertexEffect
{
    static float waveFrequency = 0.01f;
    static float waveSpeed = 4f;

    public override void UpdateText(Vector3[][] textVertices, int start, int end, float input) {
        for (int i = start; i < end; i++) {

            for (int j = 0; j < 4; j++) {
                Vector3 orig = textVertices[i][j];

                textVertices[i][j] = orig + getSin(orig.x) * (Vector3.up * input);
            }

        }

    }

    static float getSin(float x) {
        return Mathf.Sin(x * waveFrequency + Time.time * waveSpeed);
    }
}