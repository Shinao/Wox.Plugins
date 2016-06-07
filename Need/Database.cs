namespace Need
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    class Database
    {
        private const char ITEM_KEY_VALUE_SEPARATOR = '=';
        private Dictionary<string, string> _keyToValue = new Dictionary<string, string>();

        private string _dbFilePath;
        private string _dbFileName = "db.txt";

        public Database(string filepath = "./")
        {
            _dbFilePath = filepath + _dbFileName;
        }

        public void Load()
        {
            try
            {
                _keyToValue = File
                    .ReadLines(_dbFilePath)
                    .Where(line => line.Length > 3 && line.Contains(ITEM_KEY_VALUE_SEPARATOR))
                    .Select(line =>
                    {
                        var splittedLine = line.Split(new[] { ITEM_KEY_VALUE_SEPARATOR }, 2);
                        return new { Name = splittedLine[0], Value = splittedLine[1] };
                    })
                    .ToDictionary(item => item.Name, item => item.Value);
            }
            catch (FileNotFoundException)
            {
                // Completely normal on first launch
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to parse file " + _dbFilePath + ": " + e.Message);
            }
        }

        public void Save()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in _keyToValue)
            {
                stringBuilder.Append(item.Key)
                    .Append(ITEM_KEY_VALUE_SEPARATOR)
                    .Append(item.Value)
                    .Append(Environment.NewLine);
            }

            File.WriteAllText(_dbFilePath, stringBuilder.ToString());
        }

        public List<KeyValuePair<string, string>> GetMatchingItemsFromKey(string keyToSearch)
        {
            return _keyToValue.Where(item => item.Key.ToLower().Contains(keyToSearch)).ToList();

        }

        public List<KeyValuePair<string, string>> GetMatchingItemsFromValue(string valueToSearch)
        {
            return _keyToValue.Where(item => item.Value.ToLower().Contains(valueToSearch)).ToList();
        }

        public void Add(string key, string value)
        {
            _keyToValue[key] = value;
            Save();
        }

        public void Remove(string key)
        {
            _keyToValue.Remove(key);
            Save();
        }

        public string GetFullPath()
        {
            return _dbFilePath;
        }
    }
}