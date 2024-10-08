using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Open2DWindow : MonoBehaviour
{
    public GameObject newWindow;  // 새로운 창으로 사용할 GameObject (패널)
    public GameObject newWindow2;
    public Button openButton;     // 창을 여는 버튼
    public Button closeButton;    // 창을 닫는 버튼

    void Start()
    {
        // 창을 열고 닫는 버튼의 클릭 이벤트 리스너 등록
        openButton.onClick.AddListener(OpenNewWindow);
        closeButton.onClick.AddListener(CloseWindow);

        // 시작할 때 창이 비활성화된 상태로 설정
        newWindow.SetActive(false);
        newWindow2.SetActive(false);
    }

    // 창을 여는 함수
    void OpenNewWindow()
    {
        newWindow.SetActive(true);  // 창을 활성화
        newWindow2.SetActive(true);
    }

    // 창을 닫는 함수
    public void CloseWindow()
    {
        newWindow.SetActive(false); // 창을 비활성화
        newWindow2.SetActive(false);
    }
}
