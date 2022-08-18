using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Medico.Application.Interfaces;
using Medico.Application.Services.PatientChart;
using Medico.Application.ViewModels;

namespace Medico.Application.Services
{
    public class PatientChartNodeManagementService : IPatientChartNodeManagementService
    {
        private PatientChartNode _rootNode;

        public IEnumerable<LookupViewModel> GetNodes()
        {
            ThrowNotSetRootNodeExceptionIfNeeded();

            var nodes = new Collection<LookupViewModel>();

            SetNodes(_rootNode, nodes);

            return nodes;
        }

        public string GetFullPathNameToChildNode(Guid patientChartNodeId)
        {
            ThrowNotSetRootNodeExceptionIfNeeded();

            return GetFullPathNameToChildNode(_rootNode, patientChartNodeId);
        }

        public IPatientChartNodeManagementService SetPatientChartRootNode(PatientChartNode rootNode)
        {
            if (rootNode == _rootNode)
                return this;

            _rootNode = rootNode;
            return this;
        }

        public IEnumerable<PatientChartNode> Find(Func<PatientChartNode, bool> filter)
        {
            ThrowNotSetRootNodeExceptionIfNeeded();

            var resultList = new Collection<PatientChartNode>();

            FindRecursively(_rootNode, filter, resultList);

            return resultList;
        }

        private static void SetNodes(PatientChartNode node, ICollection<LookupViewModel> nodes, string path = "")
        {
            var nodeType = node.Type;
            var children = node.Children;

            var doChildrenExist =
                children != null && children.Any();

            if (!doChildrenExist)
            {
                if (nodeType == PatientChartNodeType.SectionNode
                    || nodeType == PatientChartNodeType.DocumentNode
                    || nodeType == PatientChartNodeType.TemplateNode
                    || nodeType == PatientChartNodeType.TemplateListNode)
                    return;

                nodes.Add(new LookupViewModel
                {
                    Id = node.Id,
                    Name = $"{path} / {node.Title}"
                });

                return;
            }

            foreach (var child in children)
            {
                var parentPath = nodeType == PatientChartNodeType.DocumentNode
                    ? node.Title
                    : $"{path} / {node.Title}";

                SetNodes(child, nodes, parentPath);
            }
        }

        private static string GetFullPathNameToChildNode(PatientChartNode node, Guid patientChartNodeId)
        {
            if (node.Id == patientChartNodeId)
                return node.Title;

            var children = node.Children;
            if (children == null || !children.Any())
                return string.Empty;

            foreach (var childPath in children
                .Select(child => GetFullPathNameToChildNode(child, patientChartNodeId))
                .Where(childPath => !string.IsNullOrEmpty(childPath)))
            {
                return $"{node.Title} / {childPath}";
            }

            return string.Empty;
        }

        private void ThrowNotSetRootNodeExceptionIfNeeded()
        {
            if (_rootNode == null)
                throw new NullReferenceException(nameof(_rootNode));
        }

        private static void FindRecursively(PatientChartNode node,
            Func<PatientChartNode, bool> filter, ICollection<PatientChartNode> resultList)
        {
            if (filter(node))
                resultList.Add(node);

            var nodeChildren = node.Children;
            if (nodeChildren == null || !nodeChildren.Any())
                return;

            foreach (var childNode in nodeChildren)
            {
                FindRecursively(childNode, filter, resultList);
            }
        }
    }
}