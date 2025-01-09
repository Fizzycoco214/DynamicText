using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DynamicText : MonoBehaviour
{
    TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        // for (int i = 0; i < textInfo.characterCount; i++) { 
        //     TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

        //     if (!charInfo.isVisible) {
        //         continue;
        //     }
        //     Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
        //     float val = 5f;
        //     float offset = Random.Range(-val, val);
        //     float offset2 = Random.Range(-val, val);
        //     for (int j = 0; j < 4; ++j) {
        //         Vector3 orig = verts[charInfo.vertexIndex + j];
        //         verts[charInfo.vertexIndex + j] = orig + new Vector3(offset, offset2, 0);
        //     }
        // }

        TextCharacterShakeEffect.UpdateText(textInfo);
        TextWaveEffect.UpdateText(textInfo);


        for (int i = 0; i < textInfo.meshInfo.Length; ++i){
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            text.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
