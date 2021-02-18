#if USE_CUSTOM_PAINTER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualPhenix.Painter
{
    public enum PAINT_MODE
    {
        FILL,
        DRAW
    }

    public enum BRUSH_TYPE
    {
        PENCIL,
        BRUSH,
        PURPURIN
    }


    public class VP_PaintManager : VP_SingletonMonobehaviour<VP_PaintManager>
    {
        [Header("Config"),Space]
        [SerializeField] protected PAINT_MODE m_mode;
        [SerializeField] protected BRUSH_TYPE m_brush;
        [SerializeField] protected int m_brushSize;
        [SerializeField] protected bool m_triggerContinuousPaintingEvent = false;
        [SerializeField] protected bool m_setInitialState = true;

        [Header("Colors")]
        public Color m_initColor;
        public Color32 m_fillColor;

        [Header("Sheet Where to Paint"), Space]
        [SerializeField] protected Image m_sheet;
        [SerializeField] protected Texture2D m_sheetTexure;
        public LinkedList<Color32[]> m_sheetVersions = new LinkedList<Color32[]>();
        protected Stack<PAINT_MODE> m_stackModes = new Stack<PAINT_MODE>();

        [Header("Audio"), Space]
        protected VP_AudioItem m_audioItem;

        protected Texture2D m_auxTexture; 
        protected Texture2D m_newSheetTexure; 
        protected Texture2D m_photo; 
        protected bool m_cancelDraw;
        protected bool m_isDrawing;
        protected bool m_pencilMode;
        protected Vector2 m_prevPoint = new Vector2();
        protected Vector2 m_texPoint = new Vector2();
        protected float m_drawStopTimer;
        protected float m_drawingTime;
        protected Color32[] m_sheetColors;
        protected Color32[] m_initColors;
        protected bool m_canInteract;


        /// <summary>
        /// Properties
        /// </summary>
        public PAINT_MODE PaintMode { get { return m_mode; } }
        public BRUSH_TYPE Brush { get { return m_brush; } }
        public int BrushSize { get { return m_brushSize; } }

        public virtual bool IsPencilBrush
        {
            get
            {
                return m_brush == BRUSH_TYPE.PENCIL;
            }
        }

        public virtual bool IsPurpurinBrush
        {
            get
            {
                return m_brush == BRUSH_TYPE.PURPURIN;
            }
        }

        public virtual bool IsBrush
        {
            get
            {
                return m_brush == BRUSH_TYPE.BRUSH;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_drawingTime = Time.time;

            if (m_setInitialState)
                SetInitialState();

            VP_EventManager.TriggerEvent(VP_EventSetup.Painter.ASK_FOR_TEXTURE);
        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();

            VP_EventManager.StartListening<Texture2D>(VP_EventSetup.Painter.GET_PAINT_TEXTURE, SetInitialState);
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

            VP_EventManager.StopListening<Texture2D>(VP_EventSetup.Painter.GET_PAINT_TEXTURE, SetInitialState);
        }

        /// <summary>
        /// selecciona el color
        /// </summary>
        /// <param name="color"></param>
        /// <param name="obj"></param>
        protected virtual void SetFillColor(Color32 color)
        {
            m_fillColor = color;
            VP_EventManager.TriggerEvent(VP_EventSetup.Painter.SET_FILL_COLOR, color);
        }

        // Set the initial state for the app
        protected virtual void SetInitialState(Texture2D _texture = null)
        {
            if (_texture != null)
                m_photo = _texture;

            m_brush = VP_PaintSetup.DEFAULT_BRUSH_TYPE;
            m_brushSize = VP_PaintSetup.DEFAULT_BRUSH_SIZE;
            m_fillColor = m_initColor;

            AdjustCanvas();
            if (m_sheetTexure == null)
                m_sheetTexure = m_sheet.material.mainTexture as Texture2D;

            m_newSheetTexure = new Texture2D(m_sheetTexure.width, m_sheetTexure.height, TextureFormat.RGBA32, false);
            m_sheetColors = m_sheetTexure.GetPixels32();
            m_newSheetTexure.SetPixels32(m_sheetColors);
            m_newSheetTexure.Apply();
            m_sheet.material.mainTexture = m_newSheetTexure;

            m_initColors = new Color32[m_sheetColors.Length];
            m_sheetColors.CopyTo(m_initColors, 0);
            VersionManager(m_initColors);

            m_canInteract = true;

            SetPaintMode(PAINT_MODE.DRAW);

        }

        protected virtual void Update()
        {
            if (m_canInteract)
            {
                if (PaintMode == PAINT_MODE.DRAW)
                {
                    if (Input.touchCount > 0 && Input.touchCount == 1)
                    {
                        Touch lTouch = Input.touches[0];
                        m_cancelDraw = false;
                    }
                    else if (Input.touchCount == 0 && Input.GetMouseButton(0))
                    {
                        m_cancelDraw = false;
                    }
                }
            }
        }
        protected virtual void AdjustCanvas()
        {
            if (m_photo != null)
            {
                m_sheet.rectTransform.sizeDelta = new Vector2(m_photo.width, m_photo.height);
                m_sheet.rectTransform.pivot = new Vector2(0, 0);
                m_sheet.rectTransform.anchoredPosition = new Vector2(-(m_photo.width * 0.5f), -(m_photo.height * 0.5f));

                // TODO: EventManager.TriggerEvent("CheckIOSFrontCamera");

                m_sheet.material.mainTexture = m_photo;
            }
        }

        /// <summary>
        /// Activa el modo Fill
        /// </summary>
        public virtual void SetPaintMode()
        {
            SetPaintMode(PAINT_MODE.FILL);
        }

        /// <summary>
        /// Activa el modo Draw
        /// </summary>
        public void SetDrawMode()
        {
            SetPaintMode(PAINT_MODE.DRAW);
        }

        public void SetPaintMode(PAINT_MODE _paintMode = PAINT_MODE.DRAW)
        {
            m_mode = _paintMode;
        }

        public virtual void CheckDrawingCanvas(Vector2 auxPos)
        {

            Vector2 mouseInScreen = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            Vector2 auxPos2 = new Vector2(mouseInScreen.x, mouseInScreen.y);

            Texture2D tex = m_sheet.material.mainTexture as Texture2D;

            m_audioItem?.PlayIfNotPlaying();

            m_texPoint = auxPos;

            if (Vector2.Distance(m_prevPoint, m_texPoint) == 0f)
            {
                m_drawStopTimer += Time.deltaTime;
                if (m_drawStopTimer > 0.25f)
                    m_audioItem?.Stop();
            }

            m_drawStopTimer = 0;

            if (!m_cancelDraw)
            {
                if (m_isDrawing && !m_pencilMode)
                    if (IsPencilBrush || IsPurpurinBrush)
                        tex.Draw(ref m_sheetColors, m_initColors, (int)m_texPoint.x, (int)m_texPoint.y, m_brushSize, m_fillColor, m_brush, !m_pencilMode, m_prevPoint);
                    else
                        tex.Draw(ref m_sheetColors, m_initColors, (int)m_texPoint.x, (int)m_texPoint.y, m_brushSize, m_fillColor, m_brush, m_pencilMode, m_prevPoint);

                else if (!m_isDrawing)
                {
                    m_isDrawing = true;

                    tex.Draw(ref m_sheetColors, m_initColors, (int)m_texPoint.x, (int)m_texPoint.y, m_brushSize, m_fillColor, m_brush, m_pencilMode);
                    GetComponent<AudioSource>().Play();
                }
            }

            if (m_triggerContinuousPaintingEvent)
                VP_EventManager.TriggerEvent(VP_EventSetup.Painter.PAINTING);

            m_prevPoint = m_texPoint;
        }

        public virtual void CheckFillingCanvas(Vector2 auxPos)
        {
            m_auxTexture = m_sheet.material.mainTexture as Texture2D;
            FillTexture(auxPos);
        }

        protected virtual void FillTexture(Vector2 auxPos)
        {
            Color initColor = m_initColors[(int)auxPos.x + (int)auxPos.y * m_auxTexture.width];
            if (initColor.r < VP_PaintSetup.BLACK_WHITE_POINT) 
                return;

            m_auxTexture.FloodFillArea(ref m_sheetColors, m_initColors, (int)auxPos.x, (int)auxPos.y, m_fillColor);
            Color32[] col = new Color32[m_sheetColors.Length];
            m_sheetColors.CopyTo(col, 0);
            VersionManager(col);
            m_stackModes.Push(PAINT_MODE.FILL);
        }

        protected virtual void VersionManager(Color32[] color)
        {
            m_sheetVersions.AddFirst(color);

            if (m_sheetVersions.Count > 6)
            {
                m_sheetVersions.RemoveLast();
            }
        }


    }
}
#endif