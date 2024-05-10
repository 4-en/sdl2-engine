using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.UI
{
    /* Parent fields:
      
       public class Drawable : Component
        public AnchorPoint anchorPoint = AnchorPoint.Center;
        ...

       public class DrawableRect : Drawable
        protected Rect rect = new Rect(0, 0, 64, 64);
        public Color color = new Color(255, 255, 255, 255);
        ...
    */

    public enum DisplayType
    {
        Flex,
        Grid,
        Fixed
    }

    public enum JustifyContent
    {
        Start,
        End,
        Center,
        SpaceBetween,
        SpaceAround,
        SpaceEvenly
    }

    public enum AlignItems
    {
        Start,
        End,
        Center,
        Stretch
    }

    public enum FlexDirection
    {
        Row,
        RowReverse,
        Column,
        ColumnReverse
    }

    public struct Sides
    {
        public double top;
        public double right;
        public double bottom;
        public double left;

        public Sides()
        {
            top = 0;
            right = 0;
            bottom = 0;
            left = 0;
        }

        public Sides(int topbottom, int leftright)
        {
            top = topbottom;
            right = leftright;
            bottom = topbottom;
            left = leftright;
        }
        public Sides(int all)
        {
            top = all;
            right = all;
            bottom = all;
            left = all;
        }
        public Sides(int top, int right, int bottom, int left)
        {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
        }
    }

    public class Div : DrawableRect
    {
        // TODO: Implement Div class as base class for UI elements

        // implement useful features from HTML/CSS (not all of those are needed, just some ideas)
        // - position
        // - size
        // - background color
        // - border
        // - border-radius
        // - padding
        // - margin
        // - text
        // - font
        // - text color
        // - text alignment
        // - display: flex, grid, fixed
        // - justify-content
        // - align-items
        // - flex-direction
        // - grid-template-columns
        // - grid-template-rows
        // - grid-gap
        // - grid-column
        // - grid-row
        // - event handling (click, hover, etc.)
        // - transitions (animations)
        // maybe more...
        // sounds like fun :)

        private string text = "";
        private string fontPath = "";
        private int fontSize = 12;
        private Color textColor = new Color(0, 0, 0, 255);
        private Color backgroundColor = new Color(255, 255, 255, 0);
        private AnchorPoint textAlignment = AnchorPoint.TopLeft;
        private bool changed = true;
        private bool dimensionsChanged = true;

        private Rect textSize = new Rect(0, 0);
        private Rect maxSize = new Rect(9999, 9999);
        private Rect minSize = new Rect(0, 0);
        private Rect? preferredSize = null;

        private Sides padding = new();
        private Sides margin = new();

        private DisplayType displayType = DisplayType.Flex;
        private JustifyContent justifyContent = JustifyContent.Start;
        private AlignItems alignItems = AlignItems.Start;
        private FlexDirection flexDirection = FlexDirection.Row;

        public void RecalculateDimensions()
        {
            double totalWidth = preferredSize?.w ?? 0;
            double totalHeight = preferredSize?.h ?? 0;

            if (children.Any())
            {
                ApplyFlexLayout(ref totalWidth, ref totalHeight);
            }

            // Enforce minimum and maximum size constraints.
            totalWidth = Math.Max(minSize.w, Math.Min(totalWidth, maxSize.w));
            totalHeight = Math.Max(minSize.h, Math.Min(totalHeight, maxSize.h));

            // Update the rect size.
            this.rect.w = totalWidth;
            this.rect.h = totalHeight;

            dimensionsChanged = false;
        }

        private void ApplyFlexLayout(ref double totalWidth, ref double totalHeight)
        {
            double contentWidth = padding.left + padding.right;
            double contentHeight = padding.top + padding.bottom;

            // Initial pass to collect flex items size
            foreach (var child in children)
            {
                child.RecalculateDimensions(); // Ensure dimensions are up-to-date
                contentWidth += child.rect.w + child.Margin.left + child.Margin.right;
                contentHeight += child.rect.h + child.Margin.top + child.Margin.bottom;
            }

            double mainAxisSize = flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? contentWidth : contentHeight;
            double crossAxisSize = flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? contentHeight : contentWidth;

            double availableMainSpace = flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? rect.w : rect.h;
            double availableCrossSpace = flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? rect.h : rect.w;

            double spaceBetween = 0;
            if (children.Count > 1)
            {
                spaceBetween = justifyContent switch
                {
                    JustifyContent.SpaceAround => (availableMainSpace - mainAxisSize) / (children.Count * 2),
                    JustifyContent.SpaceBetween => (availableMainSpace - mainAxisSize) / (children.Count - 1),
                    JustifyContent.SpaceEvenly => (availableMainSpace - mainAxisSize) / (children.Count + 1),
                    _ => 0,
                };
            }

            double mainAxisCurrent = justifyContent == JustifyContent.SpaceAround || justifyContent == JustifyContent.SpaceEvenly
                                     ? spaceBetween : 0;

            foreach (var child in children)
            {
                double childMainAxisSize = flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? child.rect.w : child.rect.h;
                double childCrossAxisSize = flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? child.rect.h : child.rect.w;

                // Position each child
                if (flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse)
                {
                    child.SetPosition(mainAxisCurrent + padding.left, CalculateCrossAxisPosition(crossAxisSize, childCrossAxisSize));
                    mainAxisCurrent += childMainAxisSize + spaceBetween;
                }
                else
                {
                    child.SetPosition(CalculateCrossAxisPosition(crossAxisSize, childCrossAxisSize), mainAxisCurrent + padding.top);
                    mainAxisCurrent += childMainAxisSize + spaceBetween;
                }
            }

            totalWidth = Math.Max(totalWidth, contentWidth);
            totalHeight = Math.Max(totalHeight, contentHeight);
        }

        private double CalculateCrossAxisPosition(double totalCrossSize, double childCrossSize)
        {
            return alignItems switch
            {
                AlignItems.Center => (totalCrossSize - childCrossSize) / 2,
                AlignItems.End => totalCrossSize - childCrossSize - (flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? padding.right : padding.bottom),
                AlignItems.Stretch => 0, // Stretch might require updating child's cross-axis size, not just position
                _ => flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse ? padding.top : padding.left,
            };
        }

        public void SetPosition(double x, double y)
        {
            rect.x = x;
            rect.y = y;
        }

        public void RecalculateAll()
        {
            RecalculateDimensions();
            foreach (var child in children)
            {
                child.RecalculateAll();
            }
        }

        public void OnChange(bool dimensionsChanged=false)
        {
            changed = true;
            
            if (dimensionsChanged)
            {
                // notify parent and children
                foreach (var child in children)
                {
                    child.OnChange(true);
                }

                // notify parent
                if (parent != null)
                {
                    parent.OnChange(true);
                }
            }
        }

        public Sides Margin
        {
            get => margin;
            set
            {
                margin = value;
                OnChange(true);
            }
        }

        public Sides Padding
        {
            get => padding;
            set
            {
                padding = value;
                OnChange(true);
            }
        }

        public Rect? PreferredSize
        {
            get => preferredSize;
            set
            {
                preferredSize = value;
                OnChange(true);
            }
        }

        public Rect MaxSize
        {
            get => maxSize;
            set
            {
                maxSize = value;
                OnChange(true);
            }
        }

        public Rect MinSize
        {
            get => minSize;
            set
            {
                minSize = value;
                OnChange(true);
            }
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnChange(true); // true since text size might change
            }
        }

        public string FontPath
        {
            get => fontPath;
            set
            {
                fontPath = value;
                OnChange(true); // true since font might change
            }
        }

        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                OnChange();
            }
        }

        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                OnChange();
            }
        }

        public AnchorPoint TextAlignment
        {
            get => textAlignment;
            set
            {
                textAlignment = value;
                OnChange(); // false since text alignment does not change outer dimensions
            }
        }

        public int FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                OnChange(true); // true since text size might change
            }
        }

        private Div? parent = null;

        public Div? Parent
        {
            get => parent;
            set
            {
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }

                if (value != null)
                {
                    value.Children.Add(this);
                }

                parent = value;
                OnChange(true);
            }
        }

        private List<Div> children = new List<Div>();
        public List<Div> Children
        {
            get => children;
            set
            {
                // remove all children from gameobject
                foreach (var child in gameObject.GetChildren())
                {
                    if (child.drawable is Div)
                    {
                        gameObject.RemoveChild(child);
                    }
                }

                foreach (var child in value)
                {
                    if(child.gameObject == null || child.gameObject == GameObject.Default)
                    {
                        var go = new GameObject(this.gameObject, "DivGameObject");
                        go.AddComponent(child);
                        child.Parent = this;
                    }
                    else
                    {
                        gameObject.AddChild(child.gameObject);
                        child.Parent = this;
                    }
                }

                children = value;
                OnChange(true);
            }
        }

        public void AddChild(Div child)
        {
            if (child.gameObject == null || child.gameObject == GameObject.Default)
            {
                var go = new GameObject(this.gameObject, "DivGameObject");
                go.AddComponent(child);
                child.Parent = this;
            }
            else
            {
                gameObject.AddChild(child.gameObject);
                child.Parent = this;
            }
            children.Add(child);
            OnChange(true);
        }


        public override void Draw(Camera camera)
        {
            if (dimensionsChanged)
            {
                RecalculateDimensions();
            }

            if (changed)
            {
                // redraw textures and stuff
                changed = false;
            }
        }
    }
}
