﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroscopeGUI
{
    class Control
    {
        public Label Label;

        protected bool _Enable;

        public virtual bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }

        public Control(string Name)
        {
            Label = new Label();
            Label.Text = Name;
            Label.Anchor = AnchorStyles.Left;
            Label.AutoSize = true;
        }
    }

    class CheckBoxControl : Control
    {
        CheckBox CheckBox;

        public CheckBoxControl(string Name, bool DefaultEnabled, EventHandler BoxClickedEvent,
            TableLayoutControlCollection Control, int Row, int Column) : base(Name)
        {
            CheckBox = new CheckBox();
            CheckBox.Checked = DefaultEnabled;
            CheckBox.Click += BoxClickedEvent;

            Control.Add(CheckBox, Row, Column);
            Control.Add(Label, Row, Column);
        }
    }

    class SliderControl : Control
    {
        string OriginalName;
        public TrackBar Slider;

        public override bool Enable
        {
            get { return _Enable; }
            set
            {
                Slider.Enabled = value;
                _Enable = value;
            }
        }

        public SliderControl(string Name, int Min, int Max, int StartVal, EventHandler ValueChangedEvent, 
            TableLayoutControlCollection Control, int Row, int Column) : base(Name + " (" + StartVal + "):")
        {
            Slider = new TrackBar();
            Slider.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Slider.Minimum = Min;
            Slider.Maximum = Max;
            Slider.Value = StartVal;

            Slider.ValueChanged += ValueChangedEvent;
            Slider.ValueChanged += ChangeLabel;

            OriginalName = Name;

            Control.Add(Slider, Row, Column);
            Control.Add(Label, Row, Column);
        }

        private void ChangeLabel(object sender, EventArgs e)
        {
            Label.Text = OriginalName + " (" + Slider.Value + "): ";
        }
    }
}
