using UnityEngine;

public class InvisibleBlock : MonoBehaviour
{
    public int rows; // Grid satýr sayýsý
    public int columns; // Grid sütun sayýsý
    public float blockSize; // Blok boyutu

    private void Start()
    {
        // Baþlangýçta görünmez bloðu güncelle
        UpdateInvisibleBlock(rows, columns);
    }

    // Görünmez bloðu güncelle
    public void UpdateInvisibleBlock(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;

        // Görünmez bloðun boyutlarýný hesapla
        float width = columns * blockSize;
        float height = blockSize;

        // Görünmez bloðun boyutunu ayarla
        transform.localScale = new Vector3(width, height, 1f);

        // Görünmez bloðun pozisyonunu ayarla (grid'in en üstünde)
        float startY = (rows / 2f) * blockSize + (blockSize / 2f);
        transform.position = new Vector3(0, startY, 0);
    }

    // Bloklarýn görünürlüðünü kontrol et
    public bool IsBlockAbove(Vector3 blockPosition)
    {
        // Eðer blok, görünmez bloðun üstündeyse true döner
        return blockPosition.y > transform.position.y;
    }
}