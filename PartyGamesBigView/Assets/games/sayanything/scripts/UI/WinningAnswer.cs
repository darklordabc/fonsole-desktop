﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PPlatform;
using Prime31.ZestKit;

namespace PPlatform.SayAnything.Ui
{
    public class WinningAnswer : UserUi
    {
        public Text _AnswerText;
        public Text _WinnerText;
        public RectTransform _WinnerInfo;
        private RectTransform _rt;

        protected override void Awake()
        {
            base.Awake();
            _rt = GetComponent<RectTransform>();
            _AnswerText.rectTransform.localScale = Vector3.zero;
            _WinnerText.rectTransform.localScale = Vector3.zero;
            _WinnerInfo.localScale = Vector3.zero;
        }

        public void OnEnable()
        {
            /*_rt.ZKanchoredPositionTo(_rt.anchoredPosition, 0.875f)
                .setFrom(new Vector2(_rt.anchoredPosition.x, _rt.anchoredPosition.y + Screen.height * 2f))
                .setEaseType(EaseType.SineOut)
                .start();*/

            var showWinnerDelay = 2f;

            _WinnerText.transform.localScale = Vector3.zero;
            _WinnerText.rectTransform.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .setDelay(showWinnerDelay)
                .start();

            _AnswerText.transform.localScale = Vector3.zero;
            _AnswerText.rectTransform.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .setDelay(showWinnerDelay + 0.5f)
                .start();

            _WinnerInfo.transform.localScale = Vector3.zero;
            _WinnerInfo.ZKlocalScaleTo(Vector3.one, 0.5f)
                .setFrom(Vector3.zero)
                .setEaseType(EaseType.SineOut)
                .setDelay(showWinnerDelay + 0.5f)
                .start();

            AudioManager.Instance.PlayWinnerMusic(1f);

            SharedData data = SayAnythingUi.Instance.CurrentData;
            _WinnerText.color = SayAnythingUi.Instance.GetUserColor(data.judgedAnswerId);
            _AnswerText.text = data.answers[data.judgedAnswerId];
            SetColor(SayAnythingUi.Instance.GetUserColor(data.judgedAnswerId));
            SetUserName(SayAnythingUi.Instance.GetUserName(data.judgedAnswerId));
        }

        void OnDisable()
        {
            _AnswerText.rectTransform.localScale = Vector3.zero;
            _WinnerText.rectTransform.localScale = Vector3.zero;
            _WinnerInfo.localScale = Vector3.zero;
        }
    }
}