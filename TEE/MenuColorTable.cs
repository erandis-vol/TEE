/*
 * Copyright 2014 eien no itari
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at:
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace TEE
{
    /// <summary>
    /// Provides custom colors for a MenuStrip.
    /// </summary>
    public class MenuColorTable : ProfessionalColorTable
    {
        #region Menu

        public override Color MenuStripGradientBegin
        {
            get { return Color.FromArgb(45, 45, 48); }
        }

        public override Color MenuStripGradientEnd
        {
            get { return Color.FromArgb(45, 45, 48); }
        }

        public override Color MenuItemSelected
        {
            get { return Color.FromArgb(51, 51, 52); }
        }

        public override Color MenuItemBorder
        {
            get { return Color.FromArgb(51, 51, 52); }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(62, 62, 64); }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(62, 62, 64); }
        }

        #region SubItems

        /*
        public override Color ToolStripGradientBegin
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color ToolStripGradientMiddle
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color ToolStripGradientEnd
        {
            get { return Color.FromArgb(27, 27, 28); }
        } */

        public override Color ToolStripDropDownBackground
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color ImageMarginGradientMiddle
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color ImageMarginGradientEnd
        {
            get { return Color.FromArgb(27, 27, 28); }
        }

        public override Color MenuBorder
        {
            get { return Color.FromArgb(51, 51, 52); }
        }

        public override Color ToolStripBorder
        {
            get { return Color.FromArgb(51, 51, 52); }
        }

        public override Color SeparatorLight
        {
            get { return Color.FromArgb(51, 51, 52); }
        }

        public override Color SeparatorDark
        {
            get { return Color.FromArgb(51, 51, 52); }
        }

        #endregion

        #endregion
    }
}
