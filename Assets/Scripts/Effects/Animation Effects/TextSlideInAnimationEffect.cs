using System;
using UnityEngine;

public class TextSlideInAnimationEffect : TextAnimationEffect
{
    //Expands the letter from the center over a time specified from the input
    public override void UpdateText(Vector3[] characterVertices, Color32[] characterColors, float input, float time) {
        float percentTime = time / input;
        percentTime = Mathf.Min(percentTime, 1);

        float leftMostPoint = float.MaxValue;
        for(int i = 0; i < 4; i++) {
            leftMostPoint = Mathf.Min(leftMostPoint, characterVertices[i].x);
        }   

        for(int i = 0; i < 4; i++) {
            float distance = characterVertices[i].x - leftMostPoint;
            characterVertices[i].x -= distance * (1f - percentTime);
        }
    }
}
