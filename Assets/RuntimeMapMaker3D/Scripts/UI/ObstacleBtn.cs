using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace RMM3D
{
    /// <summary>
    /// obstacle UI button on the bottom panel.
    /// this sprite create from ObstacleCreatorWindow, when you build obstacle's assetbundle
    /// </summary>
    public class ObstacleBtn : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {

        private Vector2 lastMousePosition;

        private RectTransform frameRectTrans;
        private LayoutElement layoutElement;

        private int ColumnCount = 5;
        private int ColumnSize = 100; // 40+80*0, 40+80*1 = 120, 40+80*2 

        private GameObject placeholdBtn;

        private void Start()
        {
            frameRectTrans = transform.parent.GetComponent<RectTransform>();
            layoutElement = transform.GetComponent<LayoutElement>();

        }

        /// <summary>
        /// This method will be called on the start of the mouse drag
        /// </summary>
        /// <param name="eventData">mouse pointer event data</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Begin Drag");
            lastMousePosition = eventData.position;
            
            placeholdBtn = Instantiate(this.gameObject, frameRectTrans);
            placeholdBtn.AddComponent<CanvasGroup>().alpha = 0;
            int i = this.transform.GetSiblingIndex();
            placeholdBtn.transform.SetSiblingIndex(i);
            transform.SetAsLastSibling();
            layoutElement.ignoreLayout = true;
        }

        /// <summary>
        /// This method will be called during the mouse drag
        /// </summary>
        /// <param name="eventData">mouse pointer event data</param>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 currentMousePosition = eventData.position;
            Vector2 diff = currentMousePosition - lastMousePosition;
            RectTransform rect = GetComponent<RectTransform>();

            Vector3 newPosition = rect.localPosition + new Vector3(diff.x, diff.y, 0);
            Vector3 oldPos = rect.localPosition;

            rect.localPosition = newPosition;
            ClampPos(rect);
            placeholdBtn.transform.SetSiblingIndex(CalculateCurrentSiblingIndex());


            lastMousePosition = currentMousePosition;
        }

        /// <summary>
        /// This method will be called at the end of mouse drag
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("End Drag");
            //Implement your funtionlity here
            layoutElement.ignoreLayout = false;

            transform.SetSiblingIndex(placeholdBtn.transform.GetSiblingIndex());
            Destroy(placeholdBtn);
        }

        /// <summary>
        /// Clamp button movement
        /// </summary>
        /// <param name="rectTransform"></param>
        private void ClampPos(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            frameRectTrans.GetLocalCorners(corners);

            var pos = rectTransform.localPosition;

            pos.x = Mathf.Clamp(pos.x, corners[0].x, corners[3].x);
            pos.y = Mathf.Clamp(pos.y, corners[0].y, corners[1].y);
            rectTransform.localPosition = pos;
        }
        /// <summary>
        /// Calculate this button covering which place
        /// </summary>
        /// <returns></returns>
        private int CalculateCurrentSiblingIndex()
        {
            int x = Mathf.FloorToInt(transform.localPosition.x / 80f);
            int y = Mathf.FloorToInt(-transform.localPosition.y / 80f);
            //Mathf.FloorToInt(transform.localPosition.x + 400f / 5f);
            //Mathf.FloorToInt(transform.localPosition.y + 400f / 5f);
            int result = x + ColumnCount * y;

            return result;
        }



        public class Factory : PlaceholderFactory<ObstacleBtn>
        {


        }
    }
}