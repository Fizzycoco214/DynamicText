using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TextColorChangeEffect : TextColorEffect
{
    public override void UpdateText(Color32[][] vertexColors, int start, int end, string input)
    {
        Color32 color;
        switch(input) {
            case "red":
                color = Color.red;
                break;
            case "blue":
                color = Color.blue;
                break;
            case "yellow":
                color = Color.yellow;
                break;
            case "green":
                color = Color.green;
                break;
            case "magenta":
                color = Color.magenta;
                break;
            case "white":
                color = Color.white;
                break;
            case "grey":
                color = Color.grey;
                break;
            case "black":
                color = Color.black;
                break;
            default:
                color = Color.white;
                break;
        }

        for (int i = start; i < end; i++) {
            for(int j = 0; j < 4; j++) {
                vertexColors[i][j] = color;
            }
        }
    }
}
