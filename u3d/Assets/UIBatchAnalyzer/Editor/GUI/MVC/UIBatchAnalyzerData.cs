using System.Collections.Generic;

namespace SimpleX
{
    public enum EAnalysisState
    {
        Idle,
        Analyzing,
        Analyzed
    }
    
    public class UIBatchAnalyzerData
    {
        public bool enabled { get; set; } = false;
        public bool dirty { get; set; } = false;
        public EAnalysisState state { get; set; } = EAnalysisState.Idle;
        public List<KCanvas> groups { get; private set; }

        public void OnEnable()
        {
            groups = new List<KCanvas>();
        }

        public void OnDisable()
        {
            groups.Clear();
        }
    }
}