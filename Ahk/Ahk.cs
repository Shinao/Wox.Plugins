namespace Ahk
{
    using System.Collections.Generic;
    using System.Linq;
    using Wox.Plugin;
    using System.Diagnostics;
    using System.IO;

    public class Ahk : IPlugin
    {
        private const string AHK_FILE_NAME = "AutoHotkey.exe";
        private PluginInitContext _pluginContext;

        public void Init(PluginInitContext context)
        {
            _pluginContext = context;
        }

        public List<Result> Query(Query query)
        {
            var arguments = query.SecondToEndSearch;
            var queryResults = new List<Result>();

            string[] fileEntries = Directory.GetFiles(_pluginContext.CurrentPluginMetadata.PluginDirectory);
            var ahkFiles = fileEntries.Where(filepath => filepath.EndsWith(".ahk") && filepath.Contains(query.FirstSearch)).ToList();

            ahkFiles.ForEach(ahkFile =>
            {
                var filename = Path.GetFileName(ahkFile);
                queryResults.Add(new Result
                {
                    Title = filename,
                    SubTitle = "Launch " + filename + " with arguments [" + arguments + "]",
                    Action = ctx =>
                    {
                        Process.Start(_pluginContext.CurrentPluginMetadata.PluginDirectory + "/" + AHK_FILE_NAME, ahkFile + " " + arguments);
                        return true;
                    }
                });
            });

            if (!ahkFiles.Any() && query.FirstSearch.Any())
            {
                queryResults.Add(new Result
                {
                    Title = "No match for " + query.FirstSearch,
                    SubTitle = "No .ahk file found with name containing " + query.FirstSearch,
                    Action = ctx => false
                });
            }

            return queryResults;
        }
    }
}