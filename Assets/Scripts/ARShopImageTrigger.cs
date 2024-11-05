using UnityEngine;

public class ShopInfoDisplay : MonoBehaviour
{
    public GameObject shopImageQuad1; // 첫 번째 상점 Quad 오브젝트
    public GameObject shopImageQuad2; // 두 번째 상점 Quad 오브젝트

    private void Start()
    {
        shopImageQuad1.SetActive(false); // 처음에는 비활성화 상태로 시작
        shopImageQuad2.SetActive(false); // 두 번째 이미지도 비활성화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.name == "ShopTrigger1") // 첫 번째 상점 Collider
            {
                shopImageQuad1.SetActive(true);
            }
            else if (gameObject.name == "ShopTrigger2") // 두 번째 상점 Collider
            {
                shopImageQuad2.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.name == "ShopTrigger1") // 첫 번째 상점 Collider
            {
                shopImageQuad1.SetActive(false);
            }
            else if (gameObject.name == "ShopTrigger2") // 두 번째 상점 Collider
            {
                shopImageQuad2.SetActive(false);
            }
        }
    }
}