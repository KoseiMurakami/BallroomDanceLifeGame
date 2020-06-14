using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class billboard : MonoBehaviour
{
    [SerializeField]
    private Sprite[] info = default;

    private int displayIndex;
    private Image image;

    void Start()
    {
        displayIndex = 0;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            displayIndex = 0;
            gameObject.SetActive(false);
            return;
        }
    }

    public void UpdateDisplay()
    {
        image.sprite = info[displayIndex];
        displayIndex++;

        if (displayIndex >= info.Length)
        {
            displayIndex = 0;
        }
    }
}
