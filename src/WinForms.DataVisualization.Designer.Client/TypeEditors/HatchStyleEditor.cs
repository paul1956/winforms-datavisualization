﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


//
//  Purpose:	Design-time hatch style editor class. 
//


using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.DataVisualization.Charting;

using Microsoft.DotNet.DesignTools.Client;
using Microsoft.DotNet.DesignTools.Client.Proxies;

using WinForms.DataVisualization.Designer.Protocol.Endpoints;

namespace WinForms.DataVisualization.Designer.Client
{
    /// <summary>
    /// AxisName editor for the hatch type.
    /// Paints a rectangle with hatch sample.
    /// </summary>
    internal class HatchStyleEditor : UITypeEditor
    {
        /// <summary>
        /// Override this function to support palette colors drawing
        /// </summary>
        /// <param name="context">Descriptor context.</param>
        /// <returns>Can paint values.</returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }


        /// <summary>
        /// Override this function to support palette colors drawing
        /// </summary>
        /// <param name="e">Paint value event arguments.</param>
        public override void PaintValue(PaintValueEventArgs e)
        {
            ChartHatchStyle chartHatchStyle;
            if (e.Value is not EnumProxy enumProxy || (chartHatchStyle = enumProxy.AsEnumValue<ChartHatchStyle>()) == ChartHatchStyle.None)
                return;

            // Try to get original color from the object
            Color color1 = Color.Black;
            Color color2 = Color.White;
            if (e.Context?.Instance is not null)
            {
                var client = e.Context.GetRequiredService<IDesignToolsClient>();
                var sender = client.Protocol.GetEndpoint<GradientEditorPaintValueEndpoint>().GetSender(client);
                var response = sender.SendRequest(new GradientEditorPaintValueRequest(e.Context.Instance));
                color1 = response.Color1;
                color2 = response.Color2;
            }

            // Check if colors are valid
            if (color1 == Color.Empty)
            {
                color1 = Color.Black;
            }

            if (color2 == Color.Empty)
            {
                color2 = Color.White;
            }

            if (color1 == color2)
            {
                color2 = Color.FromArgb(color1.B, color1.R, color1.G);
            }

            // Draw hatch sample
            using Brush brush = ChartGraphics.GetHatchBrush(chartHatchStyle, color1, color2);
            e.Graphics.FillRectangle(brush, e.Bounds);
        }
    }
}