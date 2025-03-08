using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int colorID; // Blok rengini temsil eden ID
    public int row; // Blok satýrý
    public int col; // Blok sütunu
    public SpriteRenderer spriteRenderer; // Blok ikonunu göstermek için SpriteRenderer

    public Sprite defaultIcon; // Varsayýlan ikon
    public Sprite groupAIcon; // A boyutundaki grup için ikon
    public Sprite groupBIcon; // B boyutundaki grup için ikon
    public Sprite groupCIcon; // C boyutundaki grup için ikon

    private bool isMoving = false; // Blok hareket halinde mi?

    private void Awake()
    {
        // SpriteRenderer'ý otomatik olarak al
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultIcon;
    }

    private void Update()
    {
        // Bloklarýn görünürlüðünü sürekli kontrol et
        UpdateBlockVisibility();
    }

    // Bloklarýn görünürlüðünü kontrol et
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

    // Grup boyutuna göre blok ikonunu güncelle
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

    // Grup boyutuna göre uygun ikonu al
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

    // Blok ikonunu varsayýlan haline getir
    public void ResetIcon()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = defaultIcon;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // Bloklarý yavaþça hareket ettir (düþme animasyonu ile)
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

    // Blok týklandýðýnda çaðrýlacak fonksiyon
    private void OnMouseDown()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.HandleBlockClick(this);
        }
    }
}