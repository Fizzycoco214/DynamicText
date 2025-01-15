using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System.Collections.Generic;
using static TextParser;

public class DynamicText : MonoBehaviour
{
    TMP_Text text;

    List<EffectRange> effectRanges = new List<EffectRange>();

    void Start()
    {
        text = GetComponent<TMP_Text>();
        //Text mesh pro text doesn't like being passed as a reference 
        string parsedText = text.text;
        effectRanges = ParseText(ref parsedText);
        text.text = parsedText;
    }

    void Update()
    {

        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        Vector3[][] textVertices = new Vector3[textInfo.characterCount][];
        
        //Get the vertices of the text
        for(int i = 0; i < textInfo.characterCount; i++){
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            textVertices[i] = new Vector3[4];
            for(int j = 0; j < 4; j++) {
                textVertices[i][j] = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + j];
            }
        }

        void UpdateMesh() {
            for(int i = 0; i < textInfo.characterCount; i++){
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                /*
                For some reason text mesh pro decides to use quantum entaglement when dealing with whitespace characters so we just ignore changes made to them
                (for some reason the vertices for a space character are both not real and actually just references to the first character in the text????)
                */
                if(charInfo.character == ' ' || charInfo.character == '\n' || charInfo.character == '\t') {
                    continue;
                }

                for(int j = 0; j < 4; j++) {
                    textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + j] = textVertices[i][j];
                }
            }
        }



        foreach(EffectRange effectRange in effectRanges) {
            effectRange.effect.UpdateText(textVertices, effectRange.startIndex, effectRange.endIndex, effectRange.input);
            UpdateMesh();
        }

        

        for (int i = 0; i < textInfo.meshInfo.Length; i++){
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            text.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
