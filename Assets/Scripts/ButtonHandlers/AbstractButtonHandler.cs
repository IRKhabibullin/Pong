using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractButtonHandler : MonoBehaviour
{
    private Button button;
    
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(RaiseOnClickEvent);
    }

    protected abstract void RaiseOnClickEvent();
}
