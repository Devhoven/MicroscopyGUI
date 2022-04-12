using peak.core.nodes;
using std;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MicroscopeGUI.MainWindowComponents.Controls.NodeControls
{
    class EnumNodeControl : ControlBase
    {
        EnumerationNode Node;

        ComboBox EnumComboBox;

        public override bool Enable
        {
            get => EnumComboBox.IsEnabled;
            set
            {
                // If it gets disabled, the foreground color is set to gray
                if (value)
                    Label.Foreground = Brushes.White;
                else
                    Label.Foreground = Brushes.Gray;
                EnumComboBox.IsEnabled = value;
            }
        }

        public EnumNodeControl(EnumerationNode node) : base(node.DisplayName())
        {
            Node = node;

            InitControls();

            //Enable = Node.IsWriteable();

            Node.ChangedEvent += (o, e) 
                => FillComboBox();
        }

        void InitControls()
        {
            EnumComboBox = new ComboBox();

            FillComboBox();

            // If an entry gets selected the CurrentEntry in the node has to be set
            EnumComboBox.SelectionChanged += EnumComboBoxSelectionChanged;

            RowDefinitions.Add(new RowDefinition());
            SetRow(EnumComboBox, 1);
            Children.Add(EnumComboBox);
        }

        void FillComboBox()
        {
            foreach (EnumerationEntryNode node in Node.Entries())
            {
                if (node.IsWriteable() || true)
                {
                    EnumComboBox.Items.Add(new EnumerationEntryNodeItem(node));

                    // If the last added node is the CurrentEntry it is going to be shown as the selected item
                    if (Node.CurrentEntry().Name() == node.Name())
                        EnumComboBox.SelectedIndex = EnumComboBox.Items.Count - 1;
                }
            }
        }

        private void EnumComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
            => CamControl.SetNodeValue(() => Node.SetCurrentEntry(((EnumerationEntryNodeItem)EnumComboBox.SelectedItem).EntryNode));

        // Holds an entry node and returns the display name in the ToString method
        // Necessary for the ComboBox - Items
        struct EnumerationEntryNodeItem
        {
            public EnumerationEntryNode EntryNode;

            public EnumerationEntryNodeItem(EnumerationEntryNode entryNode)
                => EntryNode = entryNode;

            public override string ToString()
                => EntryNode.DisplayName();
        }
    }
}
