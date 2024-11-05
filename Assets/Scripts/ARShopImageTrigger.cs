using UnityEngine;

public class ShopInfoDisplay : MonoBehaviour
{
    public GameObject shopImageQuad1; // ù ��° ���� Quad ������Ʈ
    public GameObject shopImageQuad2; // �� ��° ���� Quad ������Ʈ

    private void Start()
    {
        shopImageQuad1.SetActive(false); // ó������ ��Ȱ��ȭ ���·� ����
        shopImageQuad2.SetActive(false); // �� ��° �̹����� ��Ȱ��ȭ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.name == "ShopTrigger1") // ù ��° ���� Collider
            {
                shopImageQuad1.SetActive(true);
            }
            else if (gameObject.name == "ShopTrigger2") // �� ��° ���� Collider
            {
                shopImageQuad2.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.name == "ShopTrigger1") // ù ��° ���� Collider
            {
                shopImageQuad1.SetActive(false);
            }
            else if (gameObject.name == "ShopTrigger2") // �� ��° ���� Collider
            {
                shopImageQuad2.SetActive(false);
            }
        }
    }
}