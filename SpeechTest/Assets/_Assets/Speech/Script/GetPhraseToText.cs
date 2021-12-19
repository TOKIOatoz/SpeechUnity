using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class GetPhraseToText : MonoBehaviour
{
    //確定する為に必要な空白時間
    [SerializeField]
    private float _timeOut = 1f;
    //１行あたりの最大文字数
    [SerializeField]
    private int _maxTextLength = 20;
    private DictationRecognizer _dictationRecognizer;
    private bool _dicatationHypothesisFlag;
    private bool _dicatationResultFlag;
    public string[] TentativeText {get; private set;} = new string[3];

    //テキストを仮で保持する量(表示テキストの行数)
    private int _maxIndex { get; } = 2;
    private int _currentTesxIndex;
    public int CurrentTesxIndex
    {
        get{ return this._currentTesxIndex; }
        set
        {
            this._currentTesxIndex = value;
            if(this._currentTesxIndex > _maxIndex)
            {
                this._currentTesxIndex = 0;
            }
        }
    }

    private int _pastTextIndex;

    private StringBuilder _stringBuilder = new StringBuilder();

    //表示するテキスト
    public Text TextToUI {get; private set;}
    private int _wordLength = 0;
    private int[] _pastTextLength = {0, 0, 0};
    void Start()
    {
        //テキスト初期化
        TextToUI = this.gameObject.GetComponents<Text>()[0];
        TextToUI.text = "";

        //仮テキスト番号初期化
        CurrentTesxIndex = 0;

        //WindowsAPI
        _dictationRecognizer = new DictationRecognizer();
        _dictationRecognizer.InitialSilenceTimeoutSeconds = _timeOut;
        _dictationRecognizer.DictationHypothesis += _dicatationHypothesis;
        _dictationRecognizer.DictationResult += _dicatationResult;
        _dictationRecognizer.DictationComplete += (_complete) => _dictationRecognizer.Start();
        _dictationRecognizer.Start();
    }

    void Update()
    {
        /*
        Debug.Log("\n" + TentativeText[0]);
        Debug.Log("\n" + TentativeText[1]);
        Debug.Log("\n" + TentativeText[2]);
        Debug.Log("\n" + CurrentTesxIndex);
        */
        if (_dicatationHypothesisFlag)
        {
            _addText();
            _dicatationHypothesisFlag = false;
        }

        if (_dicatationResultFlag)
        {
            _setText();
            _dicatationResultFlag = false;
        }
    }

    //テキストを追加
    private void _addText()
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(TextToUI.text);
        _stringBuilder.Remove(_stringBuilder.Length - _wordLength, _wordLength);
        _stringBuilder.Append(" ");
        _stringBuilder.Append(TentativeText[CurrentTesxIndex]);
        TextToUI.text = _stringBuilder.ToString();
        _wordLength = TentativeText[CurrentTesxIndex].Length + 1;
    }

    //テキストをセット
    private void _setText()
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(TextToUI.text);
        _stringBuilder.Remove(_stringBuilder.Length - _wordLength, _wordLength);
        _stringBuilder.Append(" ");
        _stringBuilder.Append(TentativeText[_pastTextIndex]);
        TextToUI.text = _stringBuilder.ToString();
        _wordLength = 0;
    }

    //WindowsSpeechが聞き取り中に仮テキストを更新
    private void _dicatationHypothesis(string speechText)
    {
        TentativeText[CurrentTesxIndex] = speechText;
        _dicatationHypothesisFlag = true;
    }

    //WindowsSpeechがフレーズを確定
    private void _dicatationResult(string speechText, ConfidenceLevel confidence)
    {
        TentativeText[CurrentTesxIndex] = speechText;
        _pastTextIndex = CurrentTesxIndex;
        _pastTextLength[CurrentTesxIndex] = speechText.Length;
        CurrentTesxIndex++;
        _dicatationResultFlag = true;
    }
}
