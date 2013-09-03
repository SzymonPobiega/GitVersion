﻿using System;
using System.Text.RegularExpressions;
using GitFlowVersion;
using LibGit2Sharp;

public class FormatStringTokenResolver
{
    static Regex reEnvironmentToken = new Regex(@"%env\[([^\]]+)]%");

    public string ReplaceTokens(string template, Repository repo, SemanticVersion semanticVersion)
    {
        var branch = repo.Head;

        template = template.Replace("%githash%", branch.Tip.Sha);
        
        template = template.Replace("%branch%", repo.Head.Name);
        
        template = template.Replace("%haschanges%", repo.IsClean() ? "" : "HasChanges");

        template = template.Replace("%user%", FormatUserName());
        template = template.Replace("%machine%", Environment.MachineName);

        template = reEnvironmentToken.Replace(template, FormatEnvironmentVariable);

        return template.Trim();
    }

    string FormatUserName()
    {
        return string.IsNullOrWhiteSpace(Environment.UserDomainName)
                   ? Environment.UserName
                   : string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
    }

    string FormatEnvironmentVariable(Match match)
    {
        return Environment.GetEnvironmentVariable(match.Groups[1].Value);
    }
}