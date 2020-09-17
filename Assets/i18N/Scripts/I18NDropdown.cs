using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Honeti
{
    public class I18NDropdown : MonoBehaviour
    {
        private string []_key;
        private Dropdown _dropdown;
        private bool _initialized = false;
        private bool []_isValidKey;
        private Font _defaultFont;
        private float _defaultLineSpacing;
        private int _defaultFontSize;
        private TextAnchor _defaultAlignment;

        [SerializeField]
        private bool _dontOverwrite = false;

        [SerializeField]
        private string[] _params;
        
        void OnEnable()
        {
            if (!_initialized)
                _init();

            updateTranslation();
        }

        public string[] getKeys()
        {
            return _key;
        }
        void OnDestroy()
        {
            if (_initialized)
            {
                I18N.OnLanguageChanged -= _onLanguageChanged;
                I18N.OnFontChanged -= _onFontChanged;
            }
        }

        public void updateParam(string value, int index)
        {
            _params[index] = value;
            updateTranslation();
        }
        public string getParam( int index)
        {
            return _params[index];
        }
        /// <summary>
        /// Change text in Text component.
        /// </summary>
        private void _updateTranslation()
        {
            if (_dropdown)
            {
                int i = 0;
                foreach (var option in _dropdown.options)
                {
                    if (!_isValidKey[i])
                    {
                        _key[i] = option.text;

                        if (_key[i].StartsWith("^"))
                        {
                            _isValidKey[i] = true;
                        }
                    }
                    option.text = I18N.instance.getValue(_key[i], _params);
                    i++;
                }
            }
            _dropdown.RefreshShownValue();
        }

        /// <summary>
        /// Update translation text.
        /// </summary>
        /// <param name="invalidateKey">Force to invalidate current translation key</param>
        public void updateTranslation(bool invalidateKey = false)
        {
            if (invalidateKey)
            {
                int i = 0;
                foreach (var option in _dropdown.options)
                {
                    _isValidKey[i++] = false;
                }
            }

            _updateTranslation();
        }

        /// <summary>
        /// Init component.
        /// </summary>
        private void _init()
        {
            _dropdown = GetComponent<Dropdown>();
            _defaultFont = _dropdown.itemText.font;
            _defaultLineSpacing = _dropdown.itemText.lineSpacing;
            _defaultFontSize = _dropdown.itemText.fontSize;
            _defaultAlignment = _dropdown.itemText.alignment;
            
            _initialized = true;

            if (I18N.instance.useCustomFonts)
            {
                _changeFont(I18N.instance.customFont);
            }

            I18N.OnLanguageChanged += _onLanguageChanged;
            I18N.OnFontChanged += _onFontChanged;

           if (!_dropdown)
            {
                Debug.LogWarning(string.Format("{0}: Dropdown component was not found!", this));
            }
           else
           {
               _key = new string[_dropdown.options.Count];
               _isValidKey = new bool[_dropdown.options.Count];
               for (int i=0;i<_isValidKey.Length;i++)
               {
                   _key[i] = _dropdown.options[i].text;
                   _isValidKey[i]=false;
               }
           }
        }

        private void _onLanguageChanged(LanguageCode newLang)
        {
            _updateTranslation();
        }

        private void _onFontChanged(I18NFonts newFont)
        {
            _changeFont(newFont);
        }

        private void _changeFont(I18NFonts f)
        {
            if (_dontOverwrite)
            {
                return;
            }

            if (f != null)
            {
                if (f.font)
                {
                    _dropdown.itemText.font = f.font;
                }
                else
                {
                    _dropdown.itemText.font = _defaultFont;
                }
                if (f.customLineSpacing)
                {
                    _dropdown.itemText.lineSpacing = f.lineSpacing;
                }
                if (f.customFontSizeOffset)
                {
                    _dropdown.itemText.fontSize = (int)(_defaultFontSize + (_defaultFontSize * f.fontSizeOffsetPercent /100));
                }
                if (f.customAlignment)
                {
                    _dropdown.itemText.alignment = _getAnchorFromAlignment(f.alignment);
                }
            }
            else
            {
                _dropdown.itemText.font = _defaultFont;
                _dropdown.itemText.lineSpacing = _defaultLineSpacing;
                _dropdown.itemText.fontSize = _defaultFontSize;
                _dropdown.itemText.alignment = _defaultAlignment;
            }
        }

        private TextAnchor _getAnchorFromAlignment(TextAlignment alignment)
        {
            switch (_defaultAlignment)
            {
                case TextAnchor.UpperLeft:
                //case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    if (alignment == TextAlignment.Left)
                        return TextAnchor.UpperLeft;
                    else if (alignment == TextAlignment.Right)
                        return TextAnchor.UpperRight;
                    break;
                case TextAnchor.MiddleLeft:
                //case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    if (alignment == TextAlignment.Left)
                        return TextAnchor.MiddleLeft;
                    else if (alignment == TextAlignment.Right)
                        return TextAnchor.MiddleRight;
                    break;
                case TextAnchor.LowerLeft:
                //case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    if (alignment == TextAlignment.Left)
                        return TextAnchor.LowerLeft;
                    else if (alignment == TextAlignment.Right)
                        return TextAnchor.LowerRight;
                    break;
            }

            return _defaultAlignment;
        }
    }
}