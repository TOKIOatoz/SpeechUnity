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
    //行数
    [SerializeField]
    private int _maxRow = 2;
    //１行あたりの最大文字数
    [SerializeField]
    private int _maxTextLength = 20;
    //末尾の最大空白数
    [SerializeField]
    private int _maxBlank = 3;
    private DictationRecognizer _dictationRecognizer;
    private bool _dicatationHypothesisFlag;
    private bool _dicatationResultFlag;
    
    //音声入力でHypothesis又はResultで出てきたテキストの保管
    //public string[] TentativeText {get; private set;} = new string[3];
    public string TentativeText {get; private set;} = default;

    //テキストを仮で保持する量(表示テキストの行数)
    private int _maxIndex { get; } = 2;
    
    //_setTextでセットされた１つ前の確定テキスト
    private string _pastText = "";

    private StringBuilder _stringBuilder = new StringBuilder();

    private string _tentativeSubstitutionText = "";

    //for文用
    private int _num0, _num1;

    //表示するテキスト
    public Text TextToUI {get; private set;}
    private int _wordLength = 0, _pastTextLength = 0, _tentativeRow, _correctRow, _removeRow;
    private int[] _removeLength = new int[10];
    void Start()
    {
        //テキスト初期化
        TextToUI = this.gameObject.GetComponents<Text>()[0];
        TextToUI.text = "";

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
        //stringBuilder経由で仮代入
        _stringBuilder.Clear();
        _stringBuilder.Append(_pastText);
        _stringBuilder.Append(" ");
        _stringBuilder.Append(TentativeText);
        _tentativeSubstitutionText = _stringBuilder.ToString();

        //行数に応じて削除
        _tentativeRow = (int)(_tentativeSubstitutionText.Length / (_maxTextLength - _maxBlank));
        if (_tentativeRow > 0)
        {
            _correctRow = 0;
            _pastTextLength = 0;
            for (_num0 = 0; _num0 <= _tentativeRow; _num0++)
            {
                _correctRow++;
                if ((_pastTextLength + _maxTextLength - _maxBlank) <= _tentativeSubstitutionText.Length)
                {
                    for (_num1 = _maxBlank; _num1 > 0; _num1--)
                    {
                        //TODO Fix
                        //IndexOutOfRangeException: Index was outside the bounds of the array.
                        if (_tentativeSubstitutionText[_pastTextLength + _maxTextLength - _num1] == ' ' && _tentativeSubstitutionText[_pastTextLength + _maxTextLength - _num1] == '\n')
                        {
                            _stringBuilder.Insert(_pastTextLength + _maxTextLength - _num1, "\n");
                            _stringBuilder.Remove(_pastTextLength + _maxTextLength - _num1, 1);
                            _pastTextLength = _pastTextLength + _maxTextLength - _num1;
                            _removeLength[_num0] = _pastTextLength;
                            break;
                        }
                        else if (_num1 == 1)
                        {
                            _stringBuilder.Insert(_pastTextLength + _maxTextLength - _num1, "\n");
                            _pastTextLength = _pastTextLength + _maxTextLength;
                            _removeLength[_num0] = _pastTextLength;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            _removeRow = _correctRow - _maxRow;
            if (_removeRow > 0)
            {
                _stringBuilder.Remove(0, _removeLength[_removeRow - 1]);
            }
        }

        TextToUI.text = _stringBuilder.ToString();
        _wordLength = TentativeText.Length + 1;
    }

    //テキストをセット
    private void _setText()
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(_pastText);
        _stringBuilder.Append(" ");
        _stringBuilder.Append(TentativeText);
        TextToUI.text = _stringBuilder.ToString();
        _wordLength = 0;


        _pastText = TextToUI.text;
    }

    //改行入力
    private void _newLine()
    {

    }

    //行削除
    private void _deleteRow()
    {
        
    }

    //WindowsSpeechが聞き取り中に仮テキストを更新
    private void _dicatationHypothesis(string speechText)
    {
        TentativeText = speechText;
        _dicatationHypothesisFlag = true;
    }

    //WindowsSpeechがフレーズを確定
    private void _dicatationResult(string speechText, ConfidenceLevel confidence)
    {
        TentativeText = speechText;
        _dicatationResultFlag = true;
    }
}
