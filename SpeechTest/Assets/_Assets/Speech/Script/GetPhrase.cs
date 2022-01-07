using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class GetPhrase : MonoBehaviour
{
    [SerializeField]
    private float _timeOut = 1f;
    private DictationRecognizer _dictationRecognizer;
    //private int _killString = 0;
    //public string outputText;
    //private float _timeCounter = 0f;
    void Start()
    {
        _dictationRecognizer = new DictationRecognizer();
        _dictationRecognizer.InitialSilenceTimeoutSeconds = _timeOut;
        _dictationRecognizer.DictationHypothesis += _dicatationHypothesis;
        _dictationRecognizer.DictationResult += _dicatationResult;
        _dictationRecognizer.DictationComplete += (_complete) => _dictationRecognizer.Start();
        _dictationRecognizer.Start();
    }

    void Update()
    {
        //_timeCounter += Time.deltaTime;
        //if (_timeCounter >= _timeOut)
        //{
        //    _dictationRecognizer.Stop();
        //    _dictationRecognizer.Dispose();
        //    _dictationRecognizer.Start();
        //}
    }

    private void _dicatationHypothesis(string text)
    {
        //outputText = text.Substring(_killString);
        //Debug.Log(text);
        //_dictationRecognizer.Stop();
        //_dictationRecognizer.Dispose();
        //_dictationRecognizer.Start();
        //_killString = text.Length;
    }

    private void _dicatationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log(text);
    }

    //private IEnumerator _dispose()
    //{
    //   while (true)
    //   {
    //        yield return new WaitForSeconds(1);
    //        _dictationRecognizer.Stop();
    //        _dictationRecognizer.Dispose();
    //    }
    //}
}
