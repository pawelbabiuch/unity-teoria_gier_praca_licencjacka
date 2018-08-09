using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject logPrefab;
    [SerializeField]
    private Transform parent;

    public List<Colors> colors = new List<Colors>();

    private int logCounter = 0;
    public static LogsManager ins;

    private void Awake()
    {
        ins = this;
    }

    /// <summary>
    /// Funkcja dodająca do logów treść w odpowiednim kolorze
    /// </summary>
    /// <param name="msg">Wysyłana wiadomość</param>
    /// <param name="eColor">Kolor wiadomości (domyslnie biały)</param>
    public void AddLog(string msg, EColors eColor = EColors.Default)
    {
        Color c = Colors.getColor(eColor);
        string m = string.Format("{0}# {1}", logCounter++, msg);

        GameObject log = Instantiate(logPrefab, parent);
        log.transform.SetSiblingIndex(0);

        log.GetComponentInChildren<Text>().text = m;
        log.GetComponent<Image>().color = c;

        if (!HUDManager2.isAutoMode)
            HUDManager2.SetPause(true);
    }

    public void ClearLogs()
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);

        logCounter = 0;
    }

    public static IEnumerator Wait(int multiply = 1)
    {
        yield return new WaitForSeconds(HUDManager2.gameSpeed * multiply);
    }
}

public enum EColors
{
    Default, Red, Green, Yellow, Blue
}

[System.Serializable]
public class Colors
{
    public EColors colorName;
    public Color colorValue;

    public static Color getColor(EColors colorName)
    {
        return LogsManager.ins.colors.FirstOrDefault(x => x.colorName == colorName).colorValue;
    }
}
