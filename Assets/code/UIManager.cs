using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameController gameController; // GameController referansı
    public TMP_InputField rowInput; // Satır sayısı için InputField
    public TMP_InputField columnInput; // Sütun sayısı için InputField
    public TMP_InputField colorCountInput; // Renk sayısı için InputField
    public TMP_InputField aInput; // A değeri için InputField
    public TMP_InputField bInput; // B değeri için InputField
    public TMP_InputField cInput; // C değeri için InputField

    private void Start()
    {
        // Oyun başladığında mevcut değerleri InputField'lere yaz
        UpdateInputFields();
    }

    // InputField'leri güncel değerlerle doldur
    private void UpdateInputFields()
    {
        if (gameController != null)
        {
            rowInput.text = gameController.rows.ToString();
            columnInput.text = gameController.columns.ToString();
            colorCountInput.text = "4";

            aInput.text = gameController.A.ToString();
            bInput.text = gameController.B.ToString();
            cInput.text = gameController.C.ToString();
        }
    }

    // Butona tıklandığında çağrılacak fonksiyon
    public void OnApplyChangesClicked()
    {
        // Satır sayısını ayarla
        if (!string.IsNullOrEmpty(rowInput.text))
        {
            if (int.TryParse(rowInput.text, out int rows))
            {
                // Sütun sayısını ayarla
                if (!string.IsNullOrEmpty(columnInput.text))
                {
                    if (int.TryParse(columnInput.text, out int columns))
                    {
                        gameController.SetGridSize(rows, columns);
                    }
                }
            }
        }

        // Renk sayısını ayarla
        if (!string.IsNullOrEmpty(colorCountInput.text))
        {
            if (int.TryParse(colorCountInput.text, out int colorCount))
            {
                gameController.SetColorCount(colorCount);
            }
        }

        // A değerini ayarla
        if (!string.IsNullOrEmpty(aInput.text))
        {
            if (int.TryParse(aInput.text, out int aValue))
            {
                gameController.SetAValue(aValue);
            }
        }

        // B değerini ayarla
        if (!string.IsNullOrEmpty(bInput.text))
        {
            if (int.TryParse(bInput.text, out int bValue))
            {
                gameController.SetBValue(bValue);
            }
        }

        // C değerini ayarla
        if (!string.IsNullOrEmpty(cInput.text))
        {
            if (int.TryParse(cInput.text, out int cValue))
            {
                gameController.SetCValue(cValue);
            }
        }
    }
}