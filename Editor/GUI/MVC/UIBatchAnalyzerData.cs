using System.Collections.Generic;
using UnityEngine;

namespace SimpleX
{
    public class UIBatchAnalyzerData
    {
        public List<kCanvas> groups { get; private set; }

        public void OnEnable()
        {
            groups = new List<kCanvas>();
        }

        public void OnDisable()
        {
            groups.Clear();
        }
    }
}