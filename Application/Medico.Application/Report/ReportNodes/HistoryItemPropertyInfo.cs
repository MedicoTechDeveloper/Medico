namespace Medico.Application.Report.ReportNodes
{
    public class HistoryItemPropertyInfo
    {
        public HistoryItemPropertyInfo(string name, bool isFirstItem = false, string dependsOn = null )
        {
            Name = name;
            IsFirstItem = isFirstItem;
            DependsOn = dependsOn;
        }
        
        public string Name { get; }

        public bool IsFirstItem { get; }

        public string DependsOn { get; }
    }
}