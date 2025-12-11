using System.Collections;
using System.IO;
using UnityEngine;
using Vuplex.WebView;

namespace Vuplex.Demos
{

    class SimpleWebViewDemo : MonoBehaviour
    {

        WebViewPrefab webViewPrefab;

        async void Start()
        {
            string portPath = Path.Combine(Application.streamingAssetsPath, "webrtc_test.html");
            Web.SetUserAgent(false);

            webViewPrefab = GameObject.Find("WebViewPrefab").GetComponent<WebViewPrefab>();
            webViewPrefab.InitialUrl = $"file://{portPath}";
            Debug.Log(webViewPrefab.InitialUrl);
            await webViewPrefab.WaitUntilInitialized();

            webViewPrefab.WebView.UrlChanged += (sender, eventArgs) => {
                Debug.Log("[SimpleWebViewDemo] URL changed: " + eventArgs.Url);

                // IP 설정
                SetServerIp("http://192.168.0.199:9000");

                // User Gesture + Connect 실행
                StartCoroutine(AutoConnectWithUserGesture());
            };
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    StartCoroutine(AutoConnectWithUserGesture());
            //}
            //if (Input.GetKeyDown(KeyCode.W))
            //{
            //    ClickDisconnect();
            //}
        }

        // ================================
        //   1) HTML Server IP 입력
        // ================================
        public void SetServerIp(string ip)
        {
            string js = $"document.getElementById('server').value = '{ip}';";
            webViewPrefab.WebView.ExecuteJavaScript(js);
            Debug.Log("Server IP set: " + ip);
        }

        // ================================
        //   2) HTML Connect 버튼 클릭 (JS)
        // ================================
        public void ClickConnect()
        {
            string js = $"document.getElementById('connect').click();";
            webViewPrefab.WebView.ExecuteJavaScript(js);
            Debug.Log("Connect Clicked");
        }

        public void ClickDisconnect()
        {
            string js = $"document.getElementById('disconnect').click();";
            webViewPrefab.WebView.ExecuteJavaScript(js);
            Debug.Log("Disconnect Clicked");
        }

        // ================================================
        // 🚀 핵심: WebView에 실제 "클릭" 이벤트 보내기
        // ================================================
        private IEnumerator AutoConnectWithUserGesture()
        {

            // HTML이 로드되도록 약간 대기
            yield return new WaitForSeconds(1.0f);

            // WebView 중심 좌표 = 가상의 사용자 클릭 지점
            Vector2 clickPoint = new Vector2(0.5f, 0.5f);
            Debug.Log("Sending User Gesture Click at " + clickPoint);

            // 실제 클릭 이벤트 발생 (User Gesture)
            webViewPrefab.WebView.Click(clickPoint);

            // User Gesture 발생 후 약간 대기 → Connect 실행
            yield return new WaitForSeconds(0.2f);

            ClickConnect();

            //// 10초 후 Disconnect → 다시 Connect 테스트용
            //yield return new WaitForSeconds(10);
            //ClickDisconnect();
            //yield return new WaitForSeconds(2);
            //ClickConnect();
        }
    }
}
