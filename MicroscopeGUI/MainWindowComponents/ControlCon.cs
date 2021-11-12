using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using peak;
using peak.core;
using peak.core.nodes;
using std;
using System.Windows.Controls;

namespace MicroscopeGUI
{
    class ControlCon : StackPanel
    {
        readonly static List<(string, NodeType)> controlNodes = new List<(string, NodeType)>()
        {
            ("ExposureTime", NodeType.Float),
            ("AcquisitionFrameRate", NodeType.Float),
            ("Gain", NodeType.Float)
        };

        public ControlCon(StackPanel parent, NodeMap nodeMap)
        {
            parent.Children.Add(this);
            AddControls(nodeMap);
        }

        void AddControls(NodeMap nodeMap)
        {
            int Row = 0;
            foreach ((string, NodeType) element in controlNodes)
            {
                if (element.Item2 == NodeType.Float)
                    AddFloatNodeControl(nodeMap.FindNode<FloatNode>(element.Item1));

                else if (element.Item2 == NodeType.Integer)
                    AddIntNodeControl(nodeMap.FindNode<IntegerNode>(element.Item1));

                else if (element.Item2 == NodeType.Enumeration)
                {

                    Node node = nodeMap.FindNode(element.Item1);

                    AddEnumNodeControl(nodeMap.FindNode<EnumerationNode>(element.Item1));
                }

                Row++;
            }

            Label AddLabel(Node node)
            {
                Label label = new Label()
                {
                    Content = node.DisplayName(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Children.Add(label);
                return label;
            }

            void AddFloatNodeControl(FloatNode node)
            {
                Label label = AddLabel(node);
                label.Content += "\n(" + Math.Round(node.Value(), 2) + ")";

                Slider slider = new Slider()
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Value = node.Value(),
                    Minimum = node.Minimum(),
                    Maximum = node.Maximum()
                };

                slider.ValueChanged += (o, e) =>
                {
                    label.Content = node.DisplayName() + "\n(" + Math.Round(e.NewValue, 2) + ")";
                    node.SetValue(e.NewValue);
                };
                node.ChangedEvent += (o, e) => 
                {
                    slider.Value = node.Value();
                    slider.Minimum = node.Minimum();
                    slider.Maximum = node.Maximum();
                };

                Children.Add(slider);
            }

            void AddIntNodeControl(IntegerNode node)
            {
                Slider slider = new Slider()
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Value = node.Value(),
                    Minimum = node.Minimum(),
                    Maximum = node.Maximum()
                };

                slider.ValueChanged += (o, e) => node.SetValue((int)e.NewValue); 
                node.ChangedEvent += (o, e) =>
                {
                    slider.Value = node.Value();
                    slider.Minimum = node.Minimum();
                    slider.Maximum = node.Maximum();
                };

                AddLabel(node);
                Children.Add(slider);
            }

            void AddEnumNodeControl(EnumerationNode node)
            {
                CheckBox checkBox = new CheckBox()
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    IsChecked = node.CurrentEntry().SymbolicValue() == "Off" ? false : true
                };

                checkBox.Unchecked += (o, e) => node.SetCurrentEntry("Off");
                checkBox.Checked += (o, e) => node.SetCurrentEntry("Continous");

                AddLabel(node);
                Children.Add(checkBox);
            }
        }
    }
}
