using UnityEngine;

[ExecuteInEditMode]
public class Bar : MonoBehaviour
{
    // Start is called before the first frame update

    public float value
    {
        get { return _value; }
        set
        {
            _value = value;
            Layout();
        }
    }
    [Header("Padding")]
    public float top, left, bottom, right;

    public RectTransform bar;

    private float _value = 1;
    private RectTransform parent;

    void Start()
    {
        parent = GetComponent<RectTransform>();
        Layout();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            Layout();
        }
    }

    void Layout()
    {
        bar.anchoredPosition = new Vector2(left, -top);
        bar.sizeDelta = new Vector2((parent.sizeDelta.x - left - right) * _value, parent.sizeDelta.y - top - bottom);
    }
}
