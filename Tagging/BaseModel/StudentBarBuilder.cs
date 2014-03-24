﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using K12.Data;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using FISCA.Presentation.Controls;

namespace Tagging.BaseModel
{
    internal class StudentBarBuilder : IDescriptionPaneBulider
    {
        #region IDescriptionPaneBulider 成員

        public DescriptionPane GetContent()
        {
            return new StudentBar();
        }

        #endregion
    }
}
