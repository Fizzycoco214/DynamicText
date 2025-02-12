using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TextParser : MonoBehaviour
{
    //Remainder Regex from https://github.com/markv12/VertexTextAnimationDemo

    private const string REMAINDER_REGEX = "(.*?((?=>)|(/|$)))";

    //Because text mesh pro parses it's own tags, we need to find them
    private static  readonly Regex richTagRegex = new Regex("<" + REMAINDER_REGEX + ">");

    static readonly TextRegex[] textRegexes = {
        new("shake", new TextCharacterShakeEffect()),
        new("wave", new TextWaveEffect())
    };

    
    



    //Removes the tags from the text and while tracking where they begin and end
    public static List<EffectRange> ParseText(ref string text) {
        List<EffectRange> effectRanges = new List<EffectRange>();

        void UpdateRanges(int index, int length) {
            for(int i = 0; i < effectRanges.Count; i++) {
                if(effectRanges[i].startIndex > index) {
                    effectRanges[i] = new EffectRange(effectRanges[i].startIndex - length, effectRanges[i].endIndex, effectRanges[i].startTagLength, effectRanges[i].endTagLength, effectRanges[i].input, effectRanges[i].effect);
                }

                if(effectRanges[i].endIndex > index) {
                    effectRanges[i] = new EffectRange(effectRanges[i].startIndex, effectRanges[i].endIndex - length, effectRanges[i].startTagLength, effectRanges[i].endTagLength, effectRanges[i].input, effectRanges[i].effect);
                }
            }
        }
        
        foreach(TextRegex textRegex in textRegexes) 
        {
            MatchCollection startMatches = textRegex.startRegex.Matches(text);
            MatchCollection endMatches = textRegex.endRegex.Matches(text);
            for(int i = 0; i < startMatches.Count; i++) {
                Match startMatch = startMatches[i];
                Match endMatch = null;
                try {
                    endMatch = endMatches[i];
                } catch (ArgumentOutOfRangeException e) {
                    //If there is no end match, the tag applies to the rest of the text
                }
                if(startMatch.Success) {
                    float input = float.Parse(startMatch.Groups["input"].Value);
                    int startIndex = startMatch.Index;
                    int endIndex = text.Length; //Because there can be no end tag, the default end index is the end of the text

                    //If an end tag exists
                    if(endMatch != null) {
                        endIndex = endMatch.Index;
                    }

    
                    effectRanges.Add(new EffectRange(startIndex, endIndex, startMatch.Length, endMatch == null ? 0 : endMatch.Length, input, textRegex.effect));
                }
            }
        }

        //Fixing the removing tags and fixing indexes for the preceding tags
        for(int i = 0; i < effectRanges.Count; i++) {
            

            text = text.Remove(effectRanges[i].startIndex, effectRanges[i].startTagLength);
            UpdateRanges(effectRanges[i].startIndex, effectRanges[i].startTagLength);

            if(effectRanges[i].endIndex == text.Length) {
                continue;
            }

            text = text.Remove(effectRanges[i].endIndex, effectRanges[i].endTagLength);
            UpdateRanges(effectRanges[i].endIndex, effectRanges[i].endTagLength);
        }

        //Modifying the indexes to account for the removed non custom rich tags
        MatchCollection nonCustomTagMatches = richTagRegex.Matches(text);
        nonCustomTagMatches.OrderBy(match => match.Index);
        int offset = 0;
        for(int i = 0; i < nonCustomTagMatches.Count; i++) {
            Match match = nonCustomTagMatches[i];
            UpdateRanges(match.Index - offset, match.Length);
            offset += match.Length;
        }

        return effectRanges;
    }

    public struct EffectRange {
        public int startIndex;
        public int endIndex;

        public float input;

        //Only used for fixing the indexes after removing tags
        public int startTagLength;
        public int endTagLength;


        public TextVertexEffect effect;

        public EffectRange(int startIndex, int endIndex, int startTagLength, int endTagLength, float input, TextVertexEffect effect) {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.startTagLength = startTagLength;
            this.endTagLength = endTagLength;
            this.input = input;
            this.effect = effect;
        }
    }

    struct TextRegex {
        public Regex startRegex;
        public Regex endRegex;
        public TextVertexEffect effect;

        public TextRegex(string identifier, TextVertexEffect effect) {
            startRegex = new Regex("<" + identifier + ":(?<input>" + REMAINDER_REGEX + ")>");
            endRegex = new Regex("</" + identifier + ">");
            this.effect = effect;
        }
    }
}
