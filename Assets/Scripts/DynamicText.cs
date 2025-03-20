using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System.Collections.Generic;
using static TextParser;
using System.Collections;
using System;
using JetBrains.Annotations;

public class DynamicText : MonoBehaviour
{
    //The amount of characters shown a second
    public float textDisplaySpeed = 8;
    bool isPaused;


    TMP_Text text;
    
    int lastDisplayedIndex = 0;
    List<EffectRange<object>> effectRanges = new List<EffectRange<object>>();
    float[] animStartTimes;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        StartDisplayText(text.text);
    }

    void Update()
    {

        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        Vector3[][] textVertices = new Vector3[textInfo.characterCount][];
        Color32[][] textColors = new Color32[textInfo.characterCount][];
        
        //Get the vertices of the text and format it for the effects
        for(int i = 0; i < textInfo.characterCount; i++){
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            textVertices[i] = new Vector3[4];
            textColors[i] = new Color32[4];
            for(int j = 0; j < 4; j++) {
                textVertices[i][j] = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + j];
                
                if(!charInfo.isVisible) {
                    continue;
                }
                
                textColors[i][j] = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + j];
            }
        }
        
        


        //Pass the text into our effects and update the mesh accordingly
        foreach(EffectRange<object> effectRange in effectRanges) {
            //Different actions need to be perfomed based on the type of effect
            switch(effectRange.effect) {
                case TextColorEffect colorEffect:
                    colorEffect.UpdateText(textColors, effectRange.startIndex, effectRange.endIndex, (string)effectRange.input);
                    break;
                case TextVertexEffect vertexEffect:
                    vertexEffect.UpdateText(textVertices, effectRange.startIndex, effectRange.endIndex, (float)effectRange.input);
                    break;
                case TextAnimationEffect animationEffect:
                    for(int i = effectRange.startIndex; i < effectRange.endIndex; i++) {
                        animationEffect.UpdateText(textVertices[i], textColors[i], (float)effectRange.input, Time.time - animStartTimes[i]);
                    }
                    break;
            }

            // We update the mesh everytime we apply an effect so they can stack on top of one another
            UpdateMesh(textInfo, textVertices, textColors);
        }

        
        //Final push to the mesh
        UpdateText(textInfo);
    }

    //Once we Update the text using an effect, we need to Update the actual mesh
    private void UpdateMesh(TMP_TextInfo textInfo, Vector3[][] textVertices, Color32[][] textColors) {
        for(int i = 0; i < textInfo.characterCount; i++){
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            /*
            Ignoring whitespace characters
            (for some reason the vertices for a space character are references to the first character in the text)
            */
            if(charInfo.character == ' ' || charInfo.character == '\n' || charInfo.character == '\t' || !charInfo.isVisible) {
                continue;
            }

            for(int j = 0; j < 4; j++) {
                textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + j] = textVertices[i][j];

                //If this character isn't shown yet, set it's alpha to zero
                textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + j] = textColors[i][j];
                textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + j].a = i < lastDisplayedIndex ? byte.MaxValue : byte.MinValue;
            }
        }
    }

    private void UpdateText(TMP_TextInfo textInfo) {
        //Final pass to push the updated mesh to the TMP component
        for (int i = 0; i < textInfo.meshInfo.Length; i++){
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            meshInfo.mesh.colors32 = meshInfo.colors32;
            text.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void StartDisplayText(string textToDisplay) {
        text.text = ParseText(textToDisplay);

        HideText();
        StartCoroutine(DisplayText());
    }

    public void HideText() {
        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;
        for(int i = 0; i < textInfo.characterCount; i++) {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            for(int e = 0; e < 4; e++) {
                text.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + e].a = 0;
            }
        }


        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public string ParseText(string text) {
        effectRanges = TextParser.ParseText(ref text);
        return text;
    }

    
    IEnumerator DisplayText() {
        TMP_TextInfo textInfo = text.textInfo;
        animStartTimes = new float[textInfo.characterCount];
        lastDisplayedIndex = 0;
        for(int i = 0; i < textInfo.characterCount; i++) {
            lastDisplayedIndex++;

            //Checking for any effects that specifically happen when first displaying the text like speed effects
            foreach(EffectRange<object> effectRange in effectRanges)
            {
                switch(effectRange.effect) {
                    case TextPauseEffect:
                        if(effectRange.startIndex == lastDisplayedIndex) {
                            StartCoroutine(Pause((float)effectRange.input));
                        }
                        break;
                    case TextSpeedEffect:
                        if(effectRange.startIndex == lastDisplayedIndex) {
                            textDisplaySpeed = (float)effectRange.input;
                        }
                        break;
                }
            }

            animStartTimes[i] = Time.time;

            if(isPaused) {
                yield return new WaitUntil(() => !isPaused);
            }
            else {
                yield return new WaitForSeconds(1f / textDisplaySpeed);
            }
        }
    }

    IEnumerator Pause(float time) {
        isPaused = true;
        yield return new WaitForSeconds(time);
        isPaused = false;
    }
}
