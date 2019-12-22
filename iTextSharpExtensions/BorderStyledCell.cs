using iTextSharp.text;
using iTextSharp.text.pdf;

namespace iTextSharpExtensions
{
    public class BorderStyledCell : PdfPCell
    {
        public BorderStyledCell()
        {
            base.Border = NO_BORDER;
            base.CellEvent = new BorderStyledCellEvent(this);
            base.UseVariableBorders = true;
        }

        public BorderStyle BorderStyle { get; set; }

        public new int Border { get; set; }

        public new IPdfPCellEvent CellEvent { get; set; }

        public new bool UseVariableBorders => base.UseVariableBorders;


        // Override all members worked with base.Border

        protected float _borderWidthTop = UNDEFINED;
        public override float BorderWidthTop
        {
            get
            {
                if (HasBorder(TOP_BORDER))
                    return _borderWidthTop != UNDEFINED ? _borderWidthTop : borderWidth;
                return 0;
            }
            set
            {
                _borderWidthTop = value;
                if (_borderWidthTop > 0) EnableBorderSide(TOP_BORDER);
            }
        }


        protected float _borderWidthRight = UNDEFINED;
        public override float BorderWidthRight
        {
            get
            {
                if (HasBorder(RIGHT_BORDER))
                    return _borderWidthRight != UNDEFINED ? _borderWidthRight : borderWidth;
                return 0;
            }
            set
            {
                _borderWidthRight = value;
                if (_borderWidthRight > 0) EnableBorderSide(RIGHT_BORDER);
            }
        }


        protected float _borderWidthBottom = UNDEFINED;
        public override float BorderWidthBottom
        {
            get
            {
                if (HasBorder(BOTTOM_BORDER))
                    return _borderWidthBottom != UNDEFINED ? _borderWidthBottom : borderWidth;
                return 0;
            }
            set
            {
                _borderWidthBottom = value;
                if (_borderWidthBottom > 0) EnableBorderSide(BOTTOM_BORDER);
            }
        }


        protected float _borderWidthLeft = UNDEFINED;
        public override float BorderWidthLeft
        {
            get
            {
                if (HasBorder(LEFT_BORDER))
                    return _borderWidthLeft != UNDEFINED ? _borderWidthLeft : borderWidth;
                return 0;
            }
            set
            {
                _borderWidthLeft = value;
                if (_borderWidthLeft > 0) EnableBorderSide(LEFT_BORDER);
            }
        }


        public override void DisableBorderSide(int side)
        {
            if (Border == UNDEFINED) Border = 0;
            Border &= ~side;
        }

        public override void EnableBorderSide(int side)
        {
            if (Border == UNDEFINED) Border = 0;
            Border |= side;
        }

        public override bool HasBorder(int type)
        {
            return (Border & type) == type;
        }

        public override bool HasBorders()
        {
            return Border > 0 && 
                BorderWidth + BorderWidthTop + BorderWidthRight + BorderWidthBottom + BorderWidthLeft > 0;
        }
    }

    class BorderStyledCellEvent : IPdfPCellEvent
    {
        private readonly BorderStyledCell _caller;

        public BorderStyledCellEvent(BorderStyledCell caller)
        {
            _caller = caller;
        }
        
        public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
        {
            if (_caller.Border <= 0)
            {
                return;
            }

            var canvas = canvases[PdfPTable.LINECANVAS];

            canvas.SaveState();

            if (_caller.BorderWidthTop > 0)
            {
                canvas.SetLineWidth(_caller.BorderWidthTop);

                var color = _caller.BorderColorTop ?? _caller.BorderColor ?? BaseColor.BLACK;
                canvas.SetColorStroke(color);

                if (_caller.BorderStyle == BorderStyle.Dashed)
                {
                    SetCanvasLineDash(canvas, position.Width);
                }

                canvas.MoveTo(position.GetLeft(0), position.GetTop(0));
                canvas.LineTo(position.GetRight(0), position.GetTop(0));

                canvas.Stroke();
            }

            if (_caller.BorderWidthRight > 0)
            {
                canvas.SetLineWidth(_caller.BorderWidthRight);

                var color = _caller.BorderColorRight ?? _caller.BorderColor ?? BaseColor.BLACK;
                canvas.SetColorStroke(color);

                if (_caller.BorderStyle == BorderStyle.Dashed)
                {
                    SetCanvasLineDash(canvas, position.Height);
                }

                canvas.MoveTo(position.GetRight(0), position.GetTop(0));
                canvas.LineTo(position.GetRight(0), position.GetBottom(0));

                canvas.Stroke();
            }

            if (_caller.BorderWidthBottom > 0)
            {
                canvas.SetLineWidth(_caller.BorderWidthBottom);

                var color = _caller.BorderColorBottom ?? _caller.BorderColor ?? BaseColor.BLACK;
                canvas.SetColorStroke(color);

                if (_caller.BorderStyle == BorderStyle.Dashed)
                {
                    SetCanvasLineDash(canvas, position.Width);
                }

                canvas.MoveTo(position.GetRight(0), position.GetBottom(0));
                canvas.LineTo(position.GetLeft(0), position.GetBottom(0));

                canvas.Stroke();
            }

            if (_caller.BorderWidthLeft > 0)
            {
                canvas.SetLineWidth(_caller.BorderWidthLeft);

                var color = _caller.BorderColorLeft ?? _caller.BorderColor ?? BaseColor.BLACK;
                canvas.SetColorStroke(color);

                if (_caller.BorderStyle == BorderStyle.Dashed)
                {
                    SetCanvasLineDash(canvas, position.Height);
                }

                canvas.MoveTo(position.GetLeft(0), position.GetBottom(0));
                canvas.LineTo(position.GetLeft(0), position.GetTop(0));

                canvas.Stroke();
            }

            canvas.RestoreState();

            _caller.CellEvent?.CellLayout(cell, position, canvases);
        }

        private static void SetCanvasLineDash(PdfContentByte canvas, float lineLength)
        {
            const float predictDashWidth = 2.5F;
            int dashesAndGapesCount = (int)(lineLength / predictDashWidth);
            if (dashesAndGapesCount % 2 == 0) dashesAndGapesCount -= 1;
            var dashWidth = lineLength / dashesAndGapesCount;

            canvas.SetLineDash(dashWidth, 0);
        }
    }

    public enum BorderStyle
    {
        Solid,
        Dashed,
    }
}
