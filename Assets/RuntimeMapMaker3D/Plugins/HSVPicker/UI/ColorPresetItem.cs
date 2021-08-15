using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HSVPicker
{
    public class ColorPresetItem : MonoBehaviour, IPointerClickHandler 
    {
        [SerializeField] private Image presetColorImage;
        [SerializeField] private ColorPresets colorPresets;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                colorPresets.PresetSelect(presetColorImage);
            }
            else if (eventData.button == PointerEventData.InputButton.Right) {
                colorPresets.DeletePresetButton(presetColorImage);
            }
        }
    }
}