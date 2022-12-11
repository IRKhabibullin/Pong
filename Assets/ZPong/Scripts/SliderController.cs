using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Image _bar;
    public Transform button;
    public float _value = 0;

    private int maxMovementRange = 14;

    public void OnRotationValueChanged(float value)
    {
        float amount = (value / 180f) * 0.5f;
        //_bar.fillAmount = amount;
        float buttonAngle = amount * 360;
        button.localEulerAngles = new Vector3(0, 0, -buttonAngle);
    }

    public void OnMovementValueChanged(float value)
    {
        // button.localEulerAngles = new Vector3(0, -value, 0);
        button.localPosition = new Vector3(value * maxMovementRange, 0, 2.5f);
    }
}
