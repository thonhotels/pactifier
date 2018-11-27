using System.Collections.Generic;
using System.Linq;

namespace Pactifier.Core.Comparers
{
    public class HeaderComparer
    {
        public bool Execute(string expected, IEnumerable<string> actual)
        {
            return actual                        
                        .All(a => a == expected);
            
        }
    }
}