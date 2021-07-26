using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;

    public LayoutElement layout;

    public int characterWrapLimit;
    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string _content, string _header = "")
    {
        if (string.IsNullOrEmpty(_header))
        {
            header.gameObject.SetActive(false);
        }
        else
        {
            header.gameObject.SetActive(true);
            header.text = _header;
        }

        content.text = _content;

        int headerLength = header.text.Length;
        int contentFieldLength = content.text.Length;

        layout.enabled = (headerLength > characterWrapLimit || contentFieldLength > characterWrapLimit) ? true : false;
    }

    void Update()
    {
        Vector2 mouse = Input.mousePosition;

        float pivotX = mouse.x / Screen.width;
        float pivotY = mouse.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = mouse;


    }
}
