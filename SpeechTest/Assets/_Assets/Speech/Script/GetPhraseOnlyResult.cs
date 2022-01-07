using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class GetPhraseOnlyResult : MonoBehaviour
{
    [SerializeField]
    private float _timeOut = 1f;
    private DictationRecognizer _dictationRecognizer;
    void Start()
    {
        _dictationRecognizer = new DictationRecognizer();
        _dictationRecognizer.InitialSilenceTimeoutSeconds = _timeOut;
        _dictationRecognizer.DictationResult += _dicatationResult;
        _dictationRecognizer.DictationComplete += (_complete) => _dictationRecognizer.Start();
        _dictationRecognizer.Start();
        
    }

    private void _dicatationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log(text);
    }
}
