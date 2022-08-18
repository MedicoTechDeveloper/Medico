using Medico.Application.ViewModels.Company;
using Medico.Application.ViewModels.Template;
using System;
using System.Collections.Generic;

namespace Medico.Application.ViewModels
{
    public class TemplateWithTypeNameViewModel : TemplateGridItemVm
    {
        // public string TemplateTypeName { get; set; }
    }

    public class TemplateDuplicateInputModel
    {
        public Guid CopyFrom { get; set; }
        public string NewTitle { get; set; }
        public IEnumerable<TemplateVm> Templates { get; set; }
        public string[] Companies { get; set; }
    }
}