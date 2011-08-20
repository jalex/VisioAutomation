﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;
using VisioAutomation.Extensions;

namespace VisioAutomation.Layout.Pie
{
    public class PieLayout
    {
        private List<PieSlice> _slices;
        public VA.Drawing.Point Center;
        private double _radius;
        public bool DrawBackground { get; set; }
        private IVisio.Shape _backgroundVisioShape;

        public PieLayout()
        {
            this.Center = new VA.Drawing.Point(0,0);
            this._radius = 1.0;
            this._slices = new List<PieSlice>();
        }

        public List<PieSlice> Slices
        {
            get { return _slices; }
        }

        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value <= 0)
                {
                    throw new System.ArgumentOutOfRangeException("value");
                }
                _radius = value;
            }
        }

        public Shape BackgroundCircle
        {
            get { return _backgroundVisioShape; }
        }

        public void Add(PieSlice slice)
        {
            this._slices.Add(slice);
        }

        public PieSlice Add(double value, string text)
        {
            var slice = new PieSlice();
            slice.Value = value;
            slice.Text = text;
            this._slices.Add(slice);
            return slice;
        }

        public void Render(IVisio.Page page)
        {
            var values = this._slices.Select(s=>s.Value).ToArray();
            var shapes = VisioAutomation.Layout.DrawingtHelper.DrawPieSlices(page, this.Center, this._radius, values);
            for (int i = 0; i < shapes.Count; i++)
            {
                var shape = shapes[i];
                var slice = this._slices[i];
                slice.VisioShape = shape;
                if (!string.IsNullOrEmpty(slice.Text))
                {
                    shape.Text = slice.Text;
                }
            }

            if (this.DrawBackground)
            {
                var ll = this.Center.Subtract(this.Radius, this.Radius);
                var ur = this.Center.Add(this.Radius, this.Radius);
                var rect = new VA.Drawing.Rectangle(ll, ur);
                this._backgroundVisioShape = page.DrawOval(rect);
            }
        }
    }
}