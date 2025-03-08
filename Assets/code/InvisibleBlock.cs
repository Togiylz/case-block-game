using UnityEngine;

public class InvisibleBlock : MonoBehaviour
{
    public int rows; // Grid sat�r say�s�
    public int columns; // Grid s�tun say�s�
    public float blockSize; // Blok boyutu

    private void Start()
    {
        // Ba�lang��ta g�r�nmez blo�u g�ncelle
        UpdateInvisibleBlock(rows, columns);
    }

    // G�r�nmez blo�u g�ncelle
    public void UpdateInvisibleBlock(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;

        // G�r�nmez blo�un boyutlar�n� hesapla
        float width = columns * blockSize;
        float height = blockSize;

        // G�r�nmez blo�un boyutunu ayarla
        transform.localScale = new Vector3(width, height, 1f);

        // G�r�nmez blo�un pozisyonunu ayarla (grid'in en �st�nde)
        float startY = (rows / 2f) * blockSize + (blockSize / 2f);
        transform.position = new Vector3(0, startY, 0);
    }

    // Bloklar�n g�r�n�rl���n� kontrol et
    public bool IsBlockAbove(Vector3 blockPosition)
    {
        // E�er blok, g�r�nmez blo�un �st�ndeyse true d�ner
        return blockPosition.y > transform.position.y;
    }
}