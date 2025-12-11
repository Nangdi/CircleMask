using UnityEngine;
using Vuplex.WebView;
using System.Threading.Tasks;

public class WebRTCWebVie : MonoBehaviour
{

    public WebViewPrefab webViewPrefab;
    public string targetUrl = "file:///C:/Users/humanc_m/Documents/GitHub/CircleMask/RoundScope/Assets/StreamingAssets/webrtc/webrtc_test.html";
    private bool initialized = false;

    async void Start()
    {

        // WebViewPrefab 초기화 대기
        await webViewPrefab.WaitUntilInitialized();
        initialized = true;

        // 웹사이트 URL 로드
        webViewPrefab.WebView.LoadUrl(targetUrl);
        Debug.Log("URL Loaded: " + targetUrl);

        // 웹 렌더 완료 대기
        await Task.Delay(1000);

        // 서버 IP 입력 자동화
        SetServerIp("http://192.168.0.199:9000");
    }

    /// <summary>
    /// HTML input(id="server") 값을 JS로 변경
    /// </summary>
    public void SetServerIp(string ip)
    {

        if (!initialized) return;

        string js = $"document.getElementById('server').value = '{ip}';";
        webViewPrefab.WebView.ExecuteJavaScript(js);

        Debug.Log("Server IP set: " + ip);
    }

    /// <summary>
    /// HTML Connect 버튼 클릭
    /// </summary>
    public void ClickConnect()
    {

        if (!initialized) return;

        string js = $"document.getElementById('connect').click();";
        webViewPrefab.WebView.ExecuteJavaScript(js);

        Debug.Log("Connect Clicked");
    }

    /// <summary>
    /// HTML Disconnect 버튼 클릭
    /// </summary>
    public void ClickDisconnect()
    {

        if (!initialized) return;

        string js = $"document.getElementById('disconnect').click();";
        webViewPrefab.WebView.ExecuteJavaScript(js);

        Debug.Log("Disconnect Clicked");
    }
}
