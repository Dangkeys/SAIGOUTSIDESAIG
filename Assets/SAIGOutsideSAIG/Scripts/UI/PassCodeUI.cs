using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    public class PassCodeUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private Button _submitButton;
        [SerializeField] private Button __closedButton;
        [SerializeField] private TextMeshProUGUI _responseText;
        [SerializeField] private int _minPasswordLength = 1;
        [SerializeField] private int _maxPasswordLength = 20;

        public event Action<string> OnPasswordSubmitted;
        public event Action OnUIClosed;

        public void Show(bool shouldShow = true)
        {
            _responseText.enabled = false;
            gameObject.SetActive(shouldShow);
        }

        void Start()
        {
            _passwordField.onValueChanged.AddListener(HandlePasswordChanged);
            _submitButton.onClick.AddListener(SubmitPassword);
            __closedButton.onClick.AddListener(ClosePassCodeUI);
            Show(false);
        }

        private void ClosePassCodeUI()
        {
            OnUIClosed?.Invoke();
            Show(false);
        }

        private void HandlePasswordChanged(string arg0)
        {
            int passwordLength = _passwordField.text.Length;
            _submitButton.interactable = passwordLength >= _minPasswordLength &&
                passwordLength <= _maxPasswordLength;
        }

        private void SubmitPassword()
        {
            OnPasswordSubmitted?.Invoke(_passwordField.text);
        }

        public void ShowResult(bool isCorrect)
        {
            _responseText.enabled = true;
            _responseText.text = isCorrect ? "Correct" : "Wrong!";
            _responseText.color = isCorrect ? Color.green : Color.red;
        }
        void OnDestroy()
        {

        }
    }
}