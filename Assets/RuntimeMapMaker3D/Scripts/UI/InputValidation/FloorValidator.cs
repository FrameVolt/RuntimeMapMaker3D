using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace RMM3D
{
    [CreateAssetMenu(fileName = "FloorValidator", menuName = "TextMeshPro/Input Validators/Floor Validator")]
    public class FloorValidator : TMP_InputValidator
    {
        public GroundGrid groundGrid;

        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (ch < '0' && ch > '9') return (char)0;
            if (!char.IsNumber(ch)) return (char)0;

            string tempText = text + ch;

            var targetValue = System.Convert.ToInt32(tempText);


            var temp = Mathf.Clamp(targetValue, 0, groundGrid.yAmount);

            text = temp.ToString();
            pos++;
            return ch;
        }
    }
}