using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient")]
public class Ingredient : ScriptableObject
{
    public enum EmotionType {Negative, Positive, Common}
    public enum Emotion {Angry, Sad, Happy, Inspired, Common}

    public bool isKaiju;

    public EmotionType emotionType;
    public Emotion emotion;
}
