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

    //for文, while分用
    private int _num0, _num1, _countNum;

    //削除する文字数
    private int _removeLength;

    //表示するテキスト
    public Text TextToUI {get; private set;}
    private int _pastTextLength = 0, _tentativeRow, _correctRow, _removeRow;

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
        if (_pastText.Length > 0)
        {
            _stringBuilder.Append(_pastText);
            _stringBuilder.Append(' ');
            _stringBuilder.Replace('\n', ' ');
        }
        _stringBuilder.Append(TentativeText);
        _tentativeSubstitutionText = _stringBuilder.ToString();

        //文字数に応じて改行及び行削除
        if (_tentativeSubstitutionText.Length > _maxTextLength)
        {
            //改行追加
            _newLine();

            //改行後代入
            _tentativeSubstitutionText = _stringBuilder.ToString();

            //削除行数確認
            _removeRow = _correctRow - _maxRow;
            if (_removeRow > 0)
            {
                //行削除
                _deleteRow();
            }
        }

        TextToUI.text = _stringBuilder.ToString();
    }

    //テキストをセット
    private void _setText()
    {
        //stringBuilder経由で仮代入
        _stringBuilder.Clear();
        if (_pastText.Length > 0)
        {
            _stringBuilder.Append(_pastText);
            _stringBuilder.Append(' ');
            _stringBuilder.Replace('\n', ' ');
        }
        _stringBuilder.Append(TentativeText);
        _tentativeSubstitutionText = _stringBuilder.ToString();

        //文字数に応じて改行及び行削除
        if (_tentativeSubstitutionText.Length > _maxTextLength)
        {
            //改行追加
            _newLine();

            //改行後代入
            _tentativeSubstitutionText = _stringBuilder.ToString();

            //TODO _correctRowが間違ってる場合が生じるので直す
            Debug.Log(_correctRow);

            //削除行数確認
            _removeRow = _correctRow - _maxRow;
            if (_removeRow > 0)
            {
                //行削除
                _deleteRow();
            }
        }
        TextToUI.text = _stringBuilder.ToString();
        _pastText = TextToUI.text;

        Debug.Log(_pastText);
    }

    //改行入力
    private void _newLine()
    {
        //想定される最大行数
        _tentativeRow = Mathf.CeilToInt((_tentativeSubstitutionText.Length - _maxTextLength + _maxBlank) / _maxTextLength) + 1;
        
        //実際の行数を加算していく変数のリセット
        _correctRow = 0;
        _pastTextLength = 0;
        
        for (_num0 = 0; _num0 < _tentativeRow; _num0++)
        {
            _correctRow++;
            if ((_pastTextLength + _maxTextLength) < _tentativeSubstitutionText.Length)
            {
                for (_num1 = _maxBlank; _num1 > 0; _num1--)
                {
                    if (_tentativeSubstitutionText[_pastTextLength + _maxTextLength - _num1] == ' ')
                    {
                        _stringBuilder.Remove(_pastTextLength + _maxTextLength - _num1, 1);
                        _stringBuilder.Insert(_pastTextLength + _maxTextLength - _num1, '\n');
                        _pastTextLength = _pastTextLength + _maxTextLength - _num1 + 1;
                        break;
                    }
                    else if (_num1 == 1)
                    {
                        _stringBuilder.Insert(_pastTextLength + _maxTextLength, "\n");
                        _pastTextLength = _pastTextLength + _maxTextLength + 1;
                    }
                }
            }
            else
            {
                break;
            }
        }
    }

    //行削除
    private void _deleteRow()
    {
        //削除する行数の判定をし，その行数分の\nに到達するまでの先頭からの文字数を数え，その分を削除
        _countNum = 0;
        _removeLength = 0;
        while (_countNum < _removeRow && _removeLength < _tentativeSubstitutionText.Length)
        {
            if (_tentativeSubstitutionText[_removeLength] == '\n')
            {
                _countNum++;
            }
            _removeLength++;
        }
        _stringBuilder.Remove(0, _removeLength);
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
