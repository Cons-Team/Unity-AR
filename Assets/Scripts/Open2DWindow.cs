using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Open2DWindow : MonoBehaviour
{
    public GameObject newWindow;  // ���ο� â���� ����� GameObject (�г�)
    public GameObject newWindow2;
    public Button openButton;     // â�� ���� ��ư
    public Button closeButton;    // â�� �ݴ� ��ư

    void Start()
    {
        // â�� ���� �ݴ� ��ư�� Ŭ�� �̺�Ʈ ������ ���
        openButton.onClick.AddListener(OpenNewWindow);
        closeButton.onClick.AddListener(CloseWindow);

        // ������ �� â�� ��Ȱ��ȭ�� ���·� ����
        newWindow.SetActive(false);
        newWindow2.SetActive(false);
    }

    // â�� ���� �Լ�
    void OpenNewWindow()
    {
        newWindow.SetActive(true);  // â�� Ȱ��ȭ
        newWindow2.SetActive(true);
    }

    // â�� �ݴ� �Լ�
    public void CloseWindow()
    {
        newWindow.SetActive(false); // â�� ��Ȱ��ȭ
        newWindow2.SetActive(false);
    }
}
