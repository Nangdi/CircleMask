using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class WebRTCWebViewController : MonoBehaviour
{
    public WebViewPrefab webViewPrefab;  // Hierarchy에 배치된 WebViewPrefab 참조
    public RawImage display;
    public InputField serverInput;
    public Button connectBtn;
    public Button disconnectBtn;

    private IWebView webView;

    async void Start()
    {
        // WebView 객체 준비될 때까지 기다림
        await webViewPrefab.WaitUntilInitialized();

        // WebView 참조 가져오기
        webView = webViewPrefab.WebView;

        // RawImage에 WebView 텍스처 표시
        display.texture = webView.Texture;

        // 로컬 파일 경로
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "webrtc/index.html");
        string url = "file:///" + path.Replace("\\", "/");

        Debug.Log("[WebView] Load URL = " + url);
        webView.LoadUrl(url);

        // 버튼 연결
        connectBtn.onClick.AddListener(OnConnect);
        disconnectBtn.onClick.AddListener(OnDisconnect);
    }

    // HTML connect() 실행
    private void OnConnect()
    {
        string serverUrl = serverInput.text.Trim().Replace("'", "");

        // server input 값을 HTML로 전달
        string jsSetServer = $"document.getElementById('server').value = '{serverUrl}';";
        webView.ExecuteJavaScript(jsSetServer);

        // connect() 실행
        webView.ExecuteJavaScript("connect();");

        Debug.Log($"[WebRTC WebView] Connect() 호출 - {serverUrl}");
    }

    // HTML disconnect() 실행
    private void OnDisconnect()
    {
        webView.ExecuteJavaScript("disconnect();");
        Debug.Log("[WebRTC WebView] Disconnect() 호출");
    }
}
