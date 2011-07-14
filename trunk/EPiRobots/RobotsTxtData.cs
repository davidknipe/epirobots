using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Data.Dynamic;

namespace EPiRobots
{

    // http://sdk.episerver.com/library/cms6.1/html/T_EPiServer_Enterprise_VisitorGroupTransferDataHandler.htm


    [EPiServerDataStore(AutomaticallyCreateStore = true)]
    public class RobotsTxtData
    {
        [EPiServerDataIndex]
        public string Key { get; set; }

        public string RobotsTxtContent { get; set; }
    }
}
