using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX_UI : MonoBehaviour, IPointerClickHandler 
{ 
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance?.PlayButtonClickSFX();
    }

}
