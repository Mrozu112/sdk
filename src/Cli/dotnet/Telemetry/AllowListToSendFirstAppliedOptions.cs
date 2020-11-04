// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.CommandLine.Parsing;
using Microsoft.DotNet.Cli.Utils;

namespace Microsoft.DotNet.Cli.Telemetry
{
    internal class AllowListToSendFirstAppliedOptions : IParseResultLogRule
    {
        public AllowListToSendFirstAppliedOptions(
            HashSet<string> topLevelCommandNameAllowList)
        {
            _topLevelCommandNameAllowList = topLevelCommandNameAllowList;
        }

        private HashSet<string> _topLevelCommandNameAllowList { get; }

        public List<ApplicationInsightsEntryFormat> AllowList(ParseResult parseResult)
        {
            var topLevelCommandNameFromParse = parseResult.Tokens.FirstOrDefault(token => token.Type.Equals(TokenType.Command))?.Value ?? string.Empty;
            var result = new List<ApplicationInsightsEntryFormat>();
            if (_topLevelCommandNameAllowList.Contains(topLevelCommandNameFromParse))
            {
                var firstOption = parseResult.RootCommandResult.Children.FirstOrDefault()?.Children.FirstOrDefault(c => c is System.CommandLine.Parsing.CommandResult)?.Symbol.Name ?? null;
                if (firstOption != null)
                {
                    result.Add(new ApplicationInsightsEntryFormat(
                        "sublevelparser/command",
                        new Dictionary<string, string>
                        {
                            { "verb", topLevelCommandNameFromParse},
                            {"argument", firstOption}
                        }));
                }
            }
            return result;
        }
    }
}
