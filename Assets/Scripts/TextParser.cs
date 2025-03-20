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
    private static readonly Regex richTagRegex = new Regex("<" + REMAINDER_REGEX + ">");

    //Our list of recognized custom tags
    static readonly TextRegex[] textRegexes = {
        new("shake", new TextCharacterShakeEffect()),
        new("wave", new TextWaveEffect()),
        new("pause", new TextPauseEffect()),
        new("speed", new TextSpeedEffect()),
        new("color", new TextColorChangeEffect()),
        new("grow", new TextGrowAnimationEffect()),
        new("slide", new TextSlideInAnimationEffect())
    };

    
    



    //Removes the tags from the text and while tracking where they begin and end
    public static List<EffectRange<object>> ParseText(ref string text) {
        List<EffectRange<object>> effectRanges = new();

        //Because removing the tags from the parsed text changes all of the proceding ranges values, we need to update their indexes
        void UpdateRanges(int index, int length) {
            for(int i = 0; i < effectRanges.Count; i++) {
                if(effectRanges[i].startIndex > index) {
                    effectRanges[i] = new EffectRange<object>(effectRanges[i].startIndex - length, effectRanges[i].endIndex, effectRanges[i].startTagLength, effectRanges[i].endTagLength, effectRanges[i].input, effectRanges[i].effect);
                }

                if(effectRanges[i].endIndex > index) {
                    effectRanges[i] = new EffectRange<object>(effectRanges[i].startIndex, effectRanges[i].endIndex - length, effectRanges[i].startTagLength, effectRanges[i].endTagLength, effectRanges[i].input, effectRanges[i].effect);
                }
            }
        }
        
        //Go through each recognized custom effect and construct effect ranges
        foreach(TextRegex textRegex in textRegexes) 
        {
            MatchCollection startMatches = textRegex.startRegex.Matches(text);
            MatchCollection endMatches = textRegex.endRegex.Matches(text);
            for(int i = 0; i < startMatches.Count; i++) {
                Match startMatch = startMatches[i];
                Match endMatch = null;
                try {
                    endMatch = endMatches[i];
                } catch (ArgumentOutOfRangeException) {
                    //If there is no end match, the tag applies to the rest of the text
                }


                if(startMatch.Success) {
                    int startIndex = startMatch.Index;
                    int endIndex = text.Length; //Because there can be no end tag, the default end index is the end of the text

                    //If an end tag exists
                    if(endMatch != null) {
                        endIndex = endMatch.Index;
                    }

                    switch(textRegex.effect) {
                        case TextColorEffect:
                            effectRanges.Add(new EffectRange<object>(startIndex, endIndex, startMatch.Length, endMatch == null ? 0 : endMatch.Length, startMatch.Groups["input"].Value, textRegex.effect));
                            break;
                        default:
                            effectRanges.Add(new EffectRange<object>(startIndex, endIndex, startMatch.Length, endMatch == null ? 0 : endMatch.Length, float.Parse(startMatch.Groups["input"].Value), textRegex.effect));
                            break;
                    }

                    
                }
            }
        }

        //Fixing the removing tags and fixing indexes for the preceding tags
        for(int i = 0; i < effectRanges.Count; i++) {
            
            //Updating for start tag
            text = text.Remove(effectRanges[i].startIndex, effectRanges[i].startTagLength);
            UpdateRanges(effectRanges[i].startIndex, effectRanges[i].startTagLength);

            //If there's no end tag, don't have to remove anything
            if(effectRanges[i].endIndex == text.Length) {
                continue;
            }

            //Updating for end tag
            text = text.Remove(effectRanges[i].endIndex, effectRanges[i].endTagLength);
            UpdateRanges(effectRanges[i].endIndex, effectRanges[i].endTagLength);
        }

        //Modifying the indexes to account for the removed non custom rich tags
        //We do this after applying everything from custom tags to avoid catching any custom tags in this regex
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

    //Everything needed to feed text into an effect once parsed
    public struct EffectRange<T> {
        public int startIndex;
        public int endIndex;

        public T input;

        //Only used for fixing the indexes after removing tags
        public int startTagLength;
        public int endTagLength;


        public TextEffect effect;

        public EffectRange(int startIndex, int endIndex, int startTagLength, int endTagLength, T input, TextEffect effect) {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.startTagLength = startTagLength;
            this.endTagLength = endTagLength;
            this.input = input;
            this.effect = effect;
        }
    }

    //Regexes for finding custom effects
    struct TextRegex {
        public Regex startRegex;
        public Regex endRegex;
        public TextEffect effect;

        public TextRegex(string identifier, TextEffect effect) {
            startRegex = new Regex("<" + identifier + ":(?<input>" + REMAINDER_REGEX + ")>");
            endRegex = new Regex("</" + identifier + ">");
            this.effect = effect;
        }
    }
}
