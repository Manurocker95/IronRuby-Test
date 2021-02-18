#if USE_CUSTOM_PAINTER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VirtualPhenix;

namespace VirtualPhenix.Painter
{
    public class VP_CanvasPainter : VP_MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] protected RectTransform m_rectTransform;
        [SerializeField] protected Camera m_camera;

        protected Vector2 m_localCursor;
        protected VP_PaintManager m_paintManager;

        protected override void Initialize()
        {
            base.Initialize();

            m_paintManager = VP_PaintManager.Instance;

            if (!m_camera)
                m_camera = Camera.main;

            if (!m_rectTransform)
                m_rectTransform = transform.GetOrAddComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData ped)
        {
            if (!m_paintManager || !RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectTransform, Input.mousePosition, m_camera, out m_localCursor))
                return;

            if (m_paintManager.PaintMode == PAINT_MODE.DRAW)
                m_paintManager.CheckDrawingCanvas(m_localCursor);
            
            if (m_paintManager.PaintMode == PAINT_MODE.FILL)
                m_paintManager.CheckFillingCanvas(m_localCursor);
        }

        public void OnBeginDrag(PointerEventData ped)
        {
            if (m_paintManager || !RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectTransform, Input.mousePosition, m_camera, out m_localCursor))
                return;

            if (m_paintManager.PaintMode == PAINT_MODE.DRAW)
                m_paintManager.CheckDrawingCanvas(m_localCursor);
        }

        public void OnDrag(PointerEventData ped)
        {
            if (m_paintManager || !RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectTransform, Input.mousePosition, m_camera, out m_localCursor))
                return;

            if (m_paintManager.PaintMode == PAINT_MODE.DRAW)
                m_paintManager.CheckDrawingCanvas(m_localCursor);
        }


    }
}
#endif