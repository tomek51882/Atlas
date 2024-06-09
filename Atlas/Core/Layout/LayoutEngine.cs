using Atlas.Core.Render;
using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using Atlas.Types.Style;
using System.Runtime.CompilerServices;

namespace Atlas.Core.Layout
{
    internal class LayoutEngine
    {
        internal class NodeCalculations
        {
            internal double x;
            internal double y;
            internal double width;
            internal double height;
        }

        internal void CalculateLayout(RenderTreeNode root)
        {
            var list = TreeToList(root);


            
        }

        internal void CalculateLayout_Old(RenderTreeNode root)
        {
            Queue<RenderTreeNode> queue = new Queue<RenderTreeNode>();
            Queue<RenderTreeNode> reversedQueue = new Queue<RenderTreeNode>();
            Dictionary<IRenderable, NodeCalculations> nodeCalculations = new Dictionary<IRenderable, NodeCalculations>();

            StyleProperty<int> defaultPadding;
            StyleProperty<int> horizontalPadding;
            StyleProperty<int> verticalPadding;


            queue.Enqueue(root);
            reversedQueue.Enqueue(root);
            nodeCalculations.Add(root.Value, new NodeCalculations());
            while (queue.TryDequeue(out RenderTreeNode? node)) //Tree to queue
            {
                if (node is null)
                {
                    throw new Exception("Node empty, this should have never happen");
                }
                if (node.Value is not IPrimitive primitive)
                {
                    continue;
                }
                var style = primitive.StyleProperties;
                defaultPadding = new StyleProperty<int>(style.Padding?.Value ?? 0);
                horizontalPadding = new StyleProperty<int>(style.PaddingHorizontal?.Value ?? defaultPadding.Value);
                verticalPadding = new StyleProperty<int>(style.PaddingVertical?.Value ?? defaultPadding.Value);

                style.PaddingVertical ??= verticalPadding;
                style.PaddingHorizontal ??= horizontalPadding;
                style.PaddingLeft ??= horizontalPadding;
                style.PaddingRight ??= horizontalPadding;
                style.PaddingTop ??= verticalPadding;
                style.PaddingBottom ??= verticalPadding;

                foreach (RenderTreeNode child in node.Children)
                {
                    queue.Enqueue(child);
                    reversedQueue.Enqueue(child);
                    nodeCalculations.Add(child.Value, new NodeCalculations());
                }
            }

            //NOTE: This have to be changed later
            queue = new Queue<RenderTreeNode>(reversedQueue);
            reversedQueue = new Queue<RenderTreeNode>(reversedQueue.Reverse());
            while (reversedQueue.TryDequeue(out RenderTreeNode? node))
            {

                //NOTE: Should be safe to assume that at this point all nodes are IPrimitives
                if (node.Value is not IPrimitive primitive)
                {
                    continue;
                }

                var style = primitive.StyleProperties;
                var elementValue = nodeCalculations[node.Value];

                if (style.Width is not null && style.Width.Value.unit == Unit.Char)
                {
                    elementValue.width = style.Width.Value.value;
                }
                else //if size is not set, check children
                {
                    int relativeChildrenCount = 0;
                    foreach (var child in node.Children)
                    {
                        if (child.Value is IPrimitive childPrimitive
                            && childPrimitive.StyleProperties.Width is not null
                            && childPrimitive.StyleProperties.Width.Value.unit == Unit.Char)
                        {
                            if (style.FlexDirection.Value == FlexDirection.Row
                                && childPrimitive.StyleProperties.Position.Value == Position.Relative)
                            {
                                elementValue.width = childPrimitive.StyleProperties.Width.Value.value;
                            }
                            if (style.FlexDirection.Value == FlexDirection.Column
                                && childPrimitive.StyleProperties.Position.Value == Position.Relative)
                            {
                                elementValue.width = Math.Max(elementValue.width, childPrimitive.StyleProperties.Width.Value.value);
                            }

                            if (childPrimitive.StyleProperties.Position.Value == Position.Relative)
                            {
                                relativeChildrenCount++;
                            }
                        }
                    }

                    elementValue.width += (style.PaddingLeft?.Value ?? 0) +
                        (style.PaddingRight?.Value ?? 0) +
                        (style.FlexDirection.Value == FlexDirection.Row
                            ? ((relativeChildrenCount - 1) * (style.Gap?.Value ?? 0))
                            : 0);
                }

                if (style.Height is not null && style.Height.Value.unit == Unit.Char)
                {
                    elementValue.height = style.Height.Value.value;
                }
                else //if size is not set, check children
                {
                    int relativeChildrenCount = 0;
                    foreach (var child in node.Children)
                    {
                        if (child.Value is IPrimitive childPrimitive
                            && childPrimitive.StyleProperties.Height is not null
                            && childPrimitive.StyleProperties.Height.Value.unit == Unit.Char)
                        {
                            if (style.FlexDirection.Value == FlexDirection.Column
                                && childPrimitive.StyleProperties.Position.Value == Position.Relative)
                            {
                                elementValue.height = childPrimitive.StyleProperties.Height.Value.value;
                            }
                            if (style.FlexDirection.Value == FlexDirection.Row
                                && childPrimitive.StyleProperties.Position.Value == Position.Relative)
                            {
                                elementValue.height = Math.Max(elementValue.height, childPrimitive.StyleProperties.Height.Value.value);
                            }

                            if (childPrimitive.StyleProperties.Position.Value == Position.Relative)
                            {
                                relativeChildrenCount++;
                            }
                        }
                    }

                    elementValue.height += (style.PaddingTop?.Value ?? 0) +
                        (style.PaddingBottom?.Value ?? 0) +
                        (style.FlexDirection.Value == FlexDirection.Column
                            ? ((relativeChildrenCount - 1) * (style.Gap?.Value ?? 0))
                            : 0);
                }
            }

            while (queue.TryDequeue(out RenderTreeNode? node))
            {
                if (node.Parent is null)
                {
                    continue;
                }
                double totalFlex = 0d;
                int childrenCount = 0;

                var parent = node.Parent;
                var parentValue = nodeCalculations[node.Parent.Value];
                var elementValue = nodeCalculations[node.Value];

                if (node.Value is not IPrimitive primitive)
                {
                    continue;
                }

                var currentStyle = primitive.StyleProperties;
                var parentStyle = Unsafe.As<IPrimitive>(parent.Value).StyleProperties;


                if (currentStyle.Width is not null && currentStyle.Width.Value.unit == Unit.Percent)
                {
                    //potential place for padding
                    elementValue.width = ((float)currentStyle.Width.Value.value / 100 * parentValue.width); // - parentStyle.PaddingLeft.Value - parentStyle.PaddingRight.Value;
                }
                if (currentStyle.Left is not null && currentStyle.Right is not null && currentStyle.Width is null)
                {
                    elementValue.x = parentValue.x + currentStyle.Left.Value;
                    elementValue.width = parentValue.width - currentStyle.Left.Value - currentStyle.Right.Value;
                }
                else if (currentStyle.Left is not null)
                {
                    if (currentStyle.Position is not null && currentStyle.Position.Value == Position.Absolute)
                    {
                        elementValue.x = parentValue.x + currentStyle.Left.Value;
                    }
                    else
                    {
                        elementValue.x = currentStyle.Left.Value;
                    }
                }
                else if (currentStyle.Right is not null)
                {

                    if (currentStyle.Position is not null && currentStyle.Position.Value == Position.Absolute)
                    {
                        elementValue.x = parentValue.x + parentValue.width - currentStyle.Right.Value - elementValue.width;
                    }
                    else
                    {
                        elementValue.x = parentValue.x - currentStyle.Right.Value;
                    }
                }
                else if (currentStyle.Position.Value == Position.Absolute)
                {
                    elementValue.x = parentValue.x;
                }

                if (currentStyle.Height is not null && currentStyle.Height.Value.unit == Unit.Percent)
                {
                    elementValue.height = (float)currentStyle.Height.Value.value / 100 * parentValue.height;
                }
                if (currentStyle.Top is not null && currentStyle.Bottom is not null && currentStyle.Height is null)
                {
                    elementValue.y = parentValue.y + currentStyle.Top.Value;
                    elementValue.height = parentValue.height - currentStyle.Top.Value - currentStyle.Bottom.Value;
                }
                else if (currentStyle.Top is not null)
                {
                    if (currentStyle.Position is not null && currentStyle.Position.Value == Position.Absolute)
                    {
                        elementValue.y = parentValue.y + currentStyle.Top.Value;
                    }
                    else
                    {
                        elementValue.y = currentStyle.Top.Value;
                    }
                }
                else if (currentStyle.Bottom is not null)
                {

                    if (currentStyle.Position is not null && currentStyle.Position.Value == Position.Absolute)
                    {
                        elementValue.y = parentValue.y + parentValue.height - currentStyle.Bottom.Value - elementValue.height;
                    }
                    else
                    {
                        elementValue.y = parentValue.y - currentStyle.Bottom.Value;
                    }
                }
                else if (currentStyle.Position.Value == Position.Absolute)
                {
                    elementValue.y = parentValue.y;
                }

                //AlignSelf
                if (currentStyle.Position.Value != Position.Absolute && parent is not null)
                {
                    if (parentStyle.FlexDirection.Value == FlexDirection.Row)
                    {
                        if (currentStyle.AlignSelf?.Value == Align.Center)
                        {
                            elementValue.y = elementValue.y + elementValue.height / 2 - elementValue.height / 2;
                        }
                        if (currentStyle.AlignSelf?.Value == Align.FlexEnd)
                        {
                            elementValue.y = elementValue.y +
                                parentValue.height -
                                elementValue.height -
                                parentStyle.PaddingBottom?.Value ?? 0 -
                                parentStyle.PaddingTop?.Value ?? 0;
                        }
                        if (currentStyle.AlignSelf?.Value == Align.Stretch)
                        {
                            elementValue.height = parentValue.height -
                                parentStyle.PaddingBottom?.Value ?? 0 -
                                parentStyle.PaddingTop?.Value ?? 0;
                        }
                    }

                    if (parentStyle.FlexDirection.Value == FlexDirection.Column)
                    {
                        if (currentStyle.AlignSelf?.Value == Align.Center)
                        {
                            elementValue.x = elementValue.x + elementValue.width / 2 - elementValue.width / 2;
                        }
                        if (currentStyle.AlignSelf?.Value == Align.FlexEnd)
                        {
                            elementValue.x = elementValue.x +
                                parentValue.width -
                                elementValue.width -
                                parentStyle.PaddingLeft?.Value ?? 0 -
                                parentStyle.PaddingRight?.Value ?? 0;
                        }
                        if (currentStyle.AlignSelf?.Value == Align.Stretch)
                        {
                            elementValue.width = parentValue.width -
                                parentStyle.PaddingLeft?.Value ?? 0 -
                                parentStyle.PaddingRight?.Value ?? 0;
                        }
                    }
                }

                //Set percentages
                foreach (var child in node.Children)
                {
                    if (child.Value is not IPrimitive childPrimitive)
                    {
                        continue;
                    }

                    var p = nodeCalculations[childPrimitive];
                    if (childPrimitive.StyleProperties.Width?.Value.unit == Unit.Percent)
                    {
                        //NOTE: Overwrites the correct parent calculation with wrong values
                        p.width = childPrimitive.StyleProperties.Width.Value.value / 100 * elementValue.width;
                        //potential place for padding
                        //p.width -= (currentStyle.PaddingLeft?.Value ?? 0) + (currentStyle.PaddingRight?.Value ?? 0);
                    }
                    if (childPrimitive.StyleProperties.Height?.Value.unit == Unit.Percent)
                    {
                        p.height = childPrimitive.StyleProperties.Height.Value.value / 100 * elementValue.height;
                    }
                    
                }

                currentStyle.ZIndex ??= new StyleProperty<int>(parentStyle.ZIndex?.Value ?? 0);


                //Distribute space
                var availableWidth = elementValue.width;
                var availableHeight = elementValue.height;

                foreach (var child in node.Children)
                {
                    if (child.Value is not IPrimitive childPrimitive)
                    {
                        continue;
                    }

                    var p = nodeCalculations[childPrimitive];
                    if (childPrimitive.StyleProperties.Position.Value == Position.Relative)
                    {
                        childrenCount++;
                    }

                    if (currentStyle.FlexDirection.Value == FlexDirection.Row
                        && childPrimitive.StyleProperties.Flex is null
                        && childPrimitive.StyleProperties.Position.Value == Position.Relative
                        && childPrimitive.StyleProperties.Width?.Value.unit == Unit.Char)
                    {
                        availableWidth -= p.width;
                    }

                    if (currentStyle.FlexDirection.Value == FlexDirection.Column
                        && childPrimitive.StyleProperties.Flex is null
                        && childPrimitive.StyleProperties.Position.Value == Position.Relative
                        && childPrimitive.StyleProperties.Width?.Value.unit == Unit.Char)
                    {
                        availableHeight -= p.height;
                    }

                    if (currentStyle.FlexDirection.Value == FlexDirection.Row && childPrimitive.StyleProperties.Flex is not null)
                    {
                        totalFlex += childPrimitive.StyleProperties.Flex.Value;
                    }

                    if (currentStyle.FlexDirection.Value == FlexDirection.Column && childPrimitive.StyleProperties.Flex is not null)
                    {
                        totalFlex += childPrimitive.StyleProperties.Flex.Value;
                    }
                }

                availableWidth +=
                    currentStyle.PaddingLeft?.Value ?? 0 +
                    currentStyle.PaddingRight?.Value ?? 0 +
                    (currentStyle.FlexDirection.Value == FlexDirection.Row
                        && currentStyle.JustifyContent?.Value != JustifyContent.SpaceBetween
                        && currentStyle.JustifyContent?.Value != JustifyContent.SpaceAround
                        && currentStyle.JustifyContent?.Value != JustifyContent.SpaceEvenly
                            ? (childrenCount - 1) * currentStyle.Gap?.Value ?? 0
                            : 0);
                availableHeight +=
                    currentStyle.PaddingTop?.Value ?? 0 +
                    currentStyle.PaddingBottom?.Value ?? 0 +
                    (currentStyle.FlexDirection.Value == FlexDirection.Column
                        && currentStyle.JustifyContent?.Value != JustifyContent.SpaceBetween
                        && currentStyle.JustifyContent?.Value != JustifyContent.SpaceAround
                        && currentStyle.JustifyContent?.Value != JustifyContent.SpaceEvenly
                            ? (childrenCount - 1) * currentStyle.Gap?.Value ?? 0
                            : 0);

                foreach (var child in node.Children)
                {
                    if (child.Value is not IPrimitive childPrimitive)
                    {
                        continue;
                    }

                    if (currentStyle.FlexDirection.Value == FlexDirection.Row)
                    {
                        if (childPrimitive.StyleProperties.Flex is not null
                            && currentStyle.JustifyContent?.Value != JustifyContent.SpaceBetween
                            && currentStyle.JustifyContent?.Value != JustifyContent.SpaceAround
                            && currentStyle.JustifyContent?.Value != JustifyContent.SpaceEvenly)
                        {
                            nodeCalculations[childPrimitive].width = (childPrimitive.StyleProperties.Flex.Value / totalFlex) * availableWidth;
                        }
                    }
                    if (currentStyle.FlexDirection.Value == FlexDirection.Column)
                    {
                        if (childPrimitive.StyleProperties.Flex is not null
                            && currentStyle.JustifyContent?.Value != JustifyContent.SpaceBetween
                            && currentStyle.JustifyContent?.Value != JustifyContent.SpaceAround
                            && currentStyle.JustifyContent?.Value != JustifyContent.SpaceEvenly)
                        {
                            nodeCalculations[childPrimitive].height = (childPrimitive.StyleProperties.Flex.Value / totalFlex) * availableHeight;
                        }
                    }
                }

                // Determine positions.
                var x = elementValue.x + currentStyle.PaddingLeft?.Value ?? 0;
                var y = elementValue.y + currentStyle.PaddingTop?.Value ?? 0;

                if (currentStyle.FlexDirection.Value == FlexDirection.Row)
                {
                    if (currentStyle.JustifyContent?.Value == JustifyContent.Center)
                    {
                        x += availableWidth / 2;
                    }
                    if (currentStyle.JustifyContent?.Value == JustifyContent.FlexEnd)
                    {
                        x += availableWidth;
                    }
                }
                if (currentStyle.FlexDirection.Value == FlexDirection.Column)
                {
                    if (currentStyle.JustifyContent?.Value == JustifyContent.Center)
                    {
                        y += availableHeight / 2;
                    }
                    if (currentStyle.JustifyContent?.Value == JustifyContent.FlexEnd)
                    {
                        y += availableHeight;
                    }
                }

                if (currentStyle.JustifyContent?.Value == JustifyContent.SpaceBetween
                    || currentStyle.JustifyContent?.Value == JustifyContent.SpaceAround
                    || currentStyle.JustifyContent?.Value == JustifyContent.SpaceEvenly
                    )
                {
                    var count = childrenCount + currentStyle.JustifyContent.Value switch
                    {
                        JustifyContent.SpaceBetween => -1,
                        JustifyContent.SpaceEvenly => 1,
                        _ => 0,
                    };

                    var horizontalGap = availableWidth / count;
                    var verticalGap = availableHeight / count;

                    foreach (var child in node.Children)
                    {
                        if (child.Value is not IPrimitive childPrimitive)
                        {
                            continue;
                        }

                        var childCalculation = nodeCalculations[childPrimitive];
                        childCalculation.x = x + currentStyle.JustifyContent.Value switch
                        {
                            JustifyContent.SpaceAround => horizontalGap / 2,
                            JustifyContent.SpaceEvenly => horizontalGap,
                            _ => 0
                        };
                        childCalculation.y = y + currentStyle.JustifyContent.Value switch
                        {
                            JustifyContent.SpaceAround => verticalGap / 2,
                            JustifyContent.SpaceEvenly => verticalGap,
                            _ => 0
                        };

                        if (currentStyle.FlexDirection.Value == FlexDirection.Row)
                        {
                            x += childCalculation.width + horizontalGap;
                        }
                        if (currentStyle.FlexDirection.Value == FlexDirection.Column)
                        {
                            x += childCalculation.height + verticalGap;
                        }

                    }

                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        if (child.Value is not IPrimitive childPrimitive)
                        {
                            continue;
                        }

                        if (childPrimitive.StyleProperties.Position.Value == Position.Absolute
                            || childPrimitive.StyleProperties.Display?.Value == Display.None)
                        {
                            continue;
                        }

                        var childCalculation = nodeCalculations[childPrimitive];
                        if (currentStyle.FlexDirection.Value == FlexDirection.Row)
                        {
                            childCalculation.x = x;
                            x += childCalculation.width;
                            x += currentStyle.Gap?.Value ?? 0;
                        }
                        else
                        {
                            childCalculation.x = x + childCalculation.x;
                        }
                        if (currentStyle.FlexDirection.Value == FlexDirection.Column)
                        {
                            childCalculation.y = y;
                            y += childCalculation.height;
                            y += currentStyle.Gap?.Value ?? 0;
                        }
                        else
                        {
                            childCalculation.y = y + childCalculation.y;
                        }
                    }
                }

                foreach (var child in node.Children)
                {
                    if (child.Value is not IPrimitive childPrimitive)
                    {
                        continue;
                    }

                    if (childPrimitive.StyleProperties.Position.Value == Position.Absolute)
                    {
                        continue;
                    }

                    var childCalculation = nodeCalculations[childPrimitive];
                    if (currentStyle.FlexDirection.Value == FlexDirection.Row)
                    {
                        if (currentStyle.AlignItems?.Value == Align.Center)
                        {
                            childCalculation.y = elementValue.y + elementValue.height / 2 - childCalculation.height / 2;
                        }
                        if (currentStyle.AlignItems?.Value == Align.FlexEnd)
                        {
                            childCalculation.y =
                                elementValue.y +
                                elementValue.height -
                                childCalculation.height -
                                currentStyle.PaddingBottom?.Value ?? 0;
                        }
                        if (currentStyle.AlignItems?.Value == Align.Stretch && childPrimitive.StyleProperties.Height is null)
                        {
                            childCalculation.height = elementValue.height -
                                currentStyle.PaddingTop?.Value ?? 0 -
                                currentStyle.PaddingBottom?.Value ?? 0;
                        }
                    }
                    if (currentStyle.FlexDirection.Value == FlexDirection.Column)
                    {
                        if (currentStyle.AlignItems?.Value == Align.Center)
                        {
                            childCalculation.x = elementValue.x + elementValue.width / 2 - childCalculation.width / 2;
                        }
                        if (currentStyle.AlignItems?.Value == Align.FlexEnd)
                        {
                            childCalculation.x =
                                elementValue.x +
                                elementValue.width -
                                childCalculation.width -
                                currentStyle.PaddingRight?.Value ?? 0;
                        }
                        if (currentStyle.AlignItems?.Value == Align.Stretch && childPrimitive.StyleProperties.Width is null)
                        {
                            childCalculation.width = elementValue.width -
                                currentStyle.PaddingLeft?.Value ?? 0 -
                                currentStyle.PaddingRight?.Value ?? 0;
                        }
                    }
                }

                node.ComputedRect = new Rect(
                    (int)Math.Floor(elementValue.x),
                    (int)Math.Floor(elementValue.y),
                    (int)Math.Floor(elementValue.width),
                    (int)Math.Floor(elementValue.height)
                    );
            }
        }

        internal List<RenderTreeNode> TreeToList(RenderTreeNode root)
        {
            Queue<RenderTreeNode> queue = new Queue<RenderTreeNode>();
            List<RenderTreeNode> result = new List<RenderTreeNode>();

            StyleProperty<int> defaultPadding;
            StyleProperty<int> horizontalPadding;
            StyleProperty<int> verticalPadding;

            queue.Enqueue(root);
            result.Add(root);
            while (queue.TryDequeue(out RenderTreeNode? node)) //Tree to queue
            {
                if (node is null)
                {
                    throw new Exception("Node empty, this should have never happen");
                }

                if (node.Value is not IPrimitive primitive)
                {
                    throw new Exception("Node is not primitive");
                }

                var style = primitive.StyleProperties;
                defaultPadding = new StyleProperty<int>(style.Padding?.Value ?? 0);
                horizontalPadding = new StyleProperty<int>(style.PaddingHorizontal?.Value ?? defaultPadding.Value);
                verticalPadding = new StyleProperty<int>(style.PaddingVertical?.Value ?? defaultPadding.Value);

                style.PaddingVertical ??= verticalPadding;
                style.PaddingHorizontal ??= horizontalPadding;
                style.PaddingLeft ??= horizontalPadding;
                style.PaddingRight ??= horizontalPadding;
                style.PaddingTop ??= verticalPadding;
                style.PaddingBottom ??= verticalPadding;
                foreach (RenderTreeNode child in node.Children)
                {
                    queue.Enqueue(child);
                    result.Add(child);
                }
            }

            return result;
        }
    }
}
