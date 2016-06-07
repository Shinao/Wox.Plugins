namespace Need
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Wox.Plugin;
    using System.Diagnostics;

    public class Need : IPlugin
    {
        private readonly string COMMAND_OPEN = "open";
        private readonly string[] COMMANDS_DELETE = { "delete", "remove" };
        private readonly string[] COMMANDS_RELOAD = { "reload", "refresh" };

        private readonly int MAX_NB_CHAR_BEFORE_ELLIPSIS = 64;
        private readonly int NB_MATCH_BEFORE_ADDING_VALUE_AS_RESULT = 1;

        private Database _db;
        private PluginInitContext _pluginContext;

        public void Init(PluginInitContext context)
        {
            _pluginContext = context;

            _db = new Database(_pluginContext.CurrentPluginMetadata.PluginDirectory + @"\");
            _db.Load();
        }

        public List<Result> Query(Query query)
        {
            var splittedQuery = query.Search.Split(new[] { ' ' }, 2);
            var command = splittedQuery[0].ToLower();

            if (splittedQuery.Length == 1)
                return QueryResultsGetFromKey(command);

            var keyToSearch = splittedQuery[1].ToLower();
            if (COMMANDS_DELETE.Contains(command))
                return QueryResultsDeleteFromKey(keyToSearch);

            return QueryResultsSaveFromKeyAndValue(command, keyToSearch);
        }

        private List<Result> QueryResultsSaveFromKeyAndValue(string key, string value)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Save " + key,
                    SubTitle = "Save " + key + " to " + StringHelper.TruncateWithEllipsis(value, MAX_NB_CHAR_BEFORE_ELLIPSIS),
                    Action = ctx =>
                    {
                        _db.Add(key, value);
                        return true;
                    }
                }
            };
        }

        private List<KeyValuePair<string, string>> GetMatchingItems(string keyToSearch)
        {
            var matchingItems = _db.GetMatchingItemsFromKey(keyToSearch);

            // Add values to the search if did not found enough items with the key
            if (matchingItems.Count <= NB_MATCH_BEFORE_ADDING_VALUE_AS_RESULT)
                matchingItems.AddRange(_db.GetMatchingItemsFromValue(keyToSearch));

            return matchingItems;
        }

        private List<Result> QueryResultsGetFromKey(string keyToSearch)
        {
            var queryResults = new List<Result>();

            var matchingItems = GetMatchingItems(keyToSearch);

            matchingItems.ForEach(item =>
            {
                var result = new Result
                {
                    Title = item.Key,
                    SubTitle = StringHelper.TruncateWithEllipsis(item.Value, MAX_NB_CHAR_BEFORE_ELLIPSIS),
                    Action = ctx =>
                    {
                        Clipboard.SetText(item.Value);
                        return true;
                    }
                };

                queryResults.Add(result);
            });

            TryAddingOpenCommand(queryResults, keyToSearch);
            TryAddingReloadCommand(queryResults, keyToSearch);
            TryAddingDeleteSuggestionCommand(queryResults, keyToSearch);

            return queryResults;
        }

        private List<Result> QueryResultsDeleteFromKey(string splittedQuery)
        {
            var queryResults = new List<Result>();

            var matchingItems = GetMatchingItems(splittedQuery);
            matchingItems.ForEach(item =>
            {
                var result = new Result
                {
                    Title = "Delete " + item.Key,
                    SubTitle = "Delete " + item.Key + " containing " + StringHelper.TruncateWithEllipsis(item.Value, MAX_NB_CHAR_BEFORE_ELLIPSIS),
                    Action = ctx =>
                    {
                        _db.Remove(item.Key);
                        return true;
                    }
                };

                queryResults.Add(result);
            });

            return queryResults;
        }

        private void TryAddingReloadCommand(List<Result> queryResults, string keyToSearch)
        {
            if (keyToSearch == "" || !COMMANDS_RELOAD.Any(cmdReload => cmdReload.Contains(keyToSearch)))
                return;

            queryResults.Insert(0, new Result
            {
                Title = "Reload database",
                SubTitle = "When it has been modified manually",
                Action = ctx =>
                {
                    _db.Load();
                    return true;
                }
            });
        }

        private void TryAddingDeleteSuggestionCommand(List<Result> queryResults, string keyToSearch)
        {
            if (keyToSearch == "" || !COMMANDS_DELETE.Any(cmdDelete => cmdDelete.Contains(keyToSearch)))
                return;

            queryResults.Insert(0, new Result
            {
                Title = "Delete",
                SubTitle = "Delete an item from the database",
                Action = ctx => false
            });
        }

        private void TryAddingOpenCommand(List<Result> queryResults, string keyToSearch)
        {
            if (keyToSearch == "" || !COMMAND_OPEN.Contains(keyToSearch))
                return;

            queryResults.Insert(0, new Result
            {
                Title = "Open key/value database file",
                SubTitle = "Modify, see and delete entries manually",
                Action = ctx =>
                {
                    Process.Start(_db.GetFullPath());
                    return true;
                }
            });
        }
    }
}