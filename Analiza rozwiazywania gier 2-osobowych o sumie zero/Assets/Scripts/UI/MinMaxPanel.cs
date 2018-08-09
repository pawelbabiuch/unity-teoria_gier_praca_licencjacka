using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameTheory;

public class MinMaxPanel : MonoBehaviour
{
    public static MinMaxPanel ins;

    [SerializeField]
    private CanvasGroup successPanel, failedPanel;
    private Image blurImage;

    private void Awake()
    {
        ins = this;
    }

    private void Start()
    {
        blurImage = GetComponent<Image>();
    }

    public IEnumerator SetUp(bool succes)
    {
        CanvasGroup panel = (succes) ? successPanel : failedPanel;

        panel.alpha = 1;
        blurImage.enabled = true;
        yield return LogsManager.Wait(3);
        blurImage.enabled = false;
        panel.alpha = 0;
    }
}
