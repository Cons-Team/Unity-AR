using UnityEngine;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private SearchWindowManager searchWindowManager;  // 검색 창 관리 스크립트 참조

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