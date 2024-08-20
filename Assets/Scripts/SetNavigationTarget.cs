using UnityEngine;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private SearchWindowManager searchWindowManager;  // �˻� â ���� ��ũ��Ʈ ����

    public void OnOpenSearchWindowButtonClicked()
    {
        if (searchWindowManager != null)
        {
            searchWindowManager.OpenSearchWindow();
        }
        else
        {
            Debug.LogError("SearchWindowManager is not assigned in SetNavigationTarget.");
        }
    }
}