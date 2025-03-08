using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int colorID; // Blok rengini temsil eden ID
    public int row; // Blok sat�r�
    public int col; // Blok s�tunu
    public SpriteRenderer spriteRenderer; // Blok ikonunu g�stermek i�in SpriteRenderer

    public Sprite defaultIcon; // Varsay�lan ikon
    public Sprite groupAIcon; // A boyutundaki grup i�in ikon
    public Sprite groupBIcon; // B boyutundaki grup i�in ikon
    public Sprite groupCIcon; // C boyutundaki grup i�in ikon

    private bool isMoving = false; // Blok hareket halinde mi?

    private void Awake()
    {
        // SpriteRenderer'� otomatik olarak al
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultIcon;
    }

    private void Update()
    {
        // Bloklar�n g�r�n�rl���n� s�rekli kontrol et
        UpdateBlockVisibility();
    }

    // Bloklar�n g�r�n�rl���n� kontrol et
    private void UpdateBlockVisibility()
    {
        if (GameController.Instance == null || GameController.Instance.invisibleBlock == null)
        {
            return;
        }

        float invisibleBlockY = GameController.Instance.invisibleBlock.transform.position.y;

        if (transform.position.y > invisibleBlockY)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
        }
    }

    // Grup boyutuna g�re blok ikonunu g�ncelle
    public void UpdateIcon(int groupSize)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Sprite newIcon = GetIconForGroupSize(groupSize);
        spriteRenderer.sprite = newIcon;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // Grup boyutuna g�re uygun ikonu al
    private Sprite GetIconForGroupSize(int groupSize)
    {
        if (GameController.Instance == null)
        {
            return defaultIcon;
        }

        if (groupSize >= GameController.Instance.C)
            return groupCIcon;
        else if (groupSize >= GameController.Instance.B)
            return groupBIcon;
        else if (groupSize >= GameController.Instance.A)
            return groupAIcon;
        else
            return defaultIcon;
    }

    // Blok ikonunu varsay�lan haline getir
    public void ResetIcon()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = defaultIcon;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // Bloklar� yava��a hareket ettir (d��me animasyonu ile)
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    // Blok hareket halinde mi?
    public bool IsMoving()
    {
        return isMoving;
    }

    // Blok t�kland���nda �a�r�lacak fonksiyon
    private void OnMouseDown()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.HandleBlockClick(this);
        }
    }
}